using Nivelatorio_BilleteraVirtual.features.categorias;
using Nivelatorio_BilleteraVirtual.features.cuentas.model;
using Nivelatorio_BilleteraVirtual.features.transacciones;
using Nivelatorio_BilleteraVirtual.features.transacciones.model;

namespace Nivelatorio_BilleteraVirtual.features.cuentas;

public class AccountService(
    IAccountRepository repository,
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository)
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
}