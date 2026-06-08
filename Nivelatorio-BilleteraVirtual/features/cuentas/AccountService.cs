using Nivelatorio_BilleteraVirtual.features.cuentas.model;

namespace Nivelatorio_BilleteraVirtual.features.cuentas;

public class AccountService(IAccountRepository repository)
{
    public async Task<AccountDto?> Save(NewAccountDto dto)
    {
        var saved = await repository.save(AccountEntity.create(dto.NombreTitular));

        return saved != null ? new AccountDto(saved.HolderName, saved.AccountNumber, saved.Balance) : null;
    }
    
    
}