using Nivelatorio_BilleteraVirtual.features.categorias;
using Nivelatorio_BilleteraVirtual.features.cuentas.model;
using Nivelatorio_BilleteraVirtual.features.transacciones;
using Nivelatorio_BilleteraVirtual.features.transacciones.model;
using System.Data;

namespace Nivelatorio_BilleteraVirtual.features.cuentas;

public class AccountService(
    IAccountRepository repository,
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository,
    IDbConnection connection)
{
    public async Task<AccountDto?> Save(NewAccountDto dto)
    {
        var saved = await repository.save(AccountEntity.create(dto.HolderName));

        return saved != null ? new AccountDto(saved.HolderName, saved.AccountNumber, saved.Balance) : null;
    }

    public async Task<AccountHistoryDto?> GetHistoryAsync(string accountNumber)
    {
        var account = await repository.GetByAccountNumberAsync(accountNumber);
        if (account == null) return null;

        var transactions = await transactionRepository.GetByAccountIdAsync(account.Id);
        var categories = await categoryRepository.GetAllAsync();

        var historyDto = new AccountHistoryDto
        {
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            HolderName = account.HolderName,
            Transactions = transactions.Select(t => new TransactionDto(
                t.PublicId,
                account.AccountNumber,
                categories.FirstOrDefault(c => c.Id == t.CategoryId)?.Name ?? "Unknown",
                t.Amount,
                t.Type,
                t.Date
            )).ToList()
        };

        return historyDto;
    }

    public async Task<bool> TransferAsync(TransferDto dto)
    {
        var origin = await repository.GetByAccountNumberAsync(dto.OriginAccountNumber);
        var target = await repository.GetByAccountNumberAsync(dto.TargetAccountNumber);

        if (origin == null || target == null)
            throw new Exception("Una o ambas cuentas no existen");

        if (origin.Balance < dto.Amount)
            throw new Exception("Saldo insuficiente en la cuenta origen");

        var categories = (await categoryRepository.GetAllAsync()).ToList();
        var catIngreso = categories.FirstOrDefault(c => c.Type == Nivelatorio_BilleteraVirtual.features.common.MovementType.INGRESO);
        if (catIngreso == null)
        {
            await categoryRepository.save(new Nivelatorio_BilleteraVirtual.features.categorias.model.CategoryEntity(0, "Transferencia Ingreso", Nivelatorio_BilleteraVirtual.features.common.MovementType.INGRESO));
            catIngreso = (await categoryRepository.GetAllAsync()).First(c => c.Type == Nivelatorio_BilleteraVirtual.features.common.MovementType.INGRESO);
        }

        var catEgreso = categories.FirstOrDefault(c => c.Type == Nivelatorio_BilleteraVirtual.features.common.MovementType.EGRESO);
        if (catEgreso == null)
        {
            await categoryRepository.save(new Nivelatorio_BilleteraVirtual.features.categorias.model.CategoryEntity(0, "Transferencia Egreso", Nivelatorio_BilleteraVirtual.features.common.MovementType.EGRESO));
            catEgreso = (await categoryRepository.GetAllAsync()).First(c => c.Type == Nivelatorio_BilleteraVirtual.features.common.MovementType.EGRESO);
        }

        if (connection.State != ConnectionState.Open)
            connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            await transactionRepository.save(dto.OriginAccountNumber, catEgreso.Name, dto.Amount, "EGRESO", transaction);
            await transactionRepository.save(dto.TargetAccountNumber, catIngreso.Name, dto.Amount, "INGRESO", transaction);
            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}