using System.Data;
using Nivelatorio_BilleteraVirtual.features.cuentas.model;

namespace Nivelatorio_BilleteraVirtual.features.cuentas;

public interface IAccountRepository
{
    public Task<AccountEntity?> save(AccountEntity c);
    public Task<AccountEntity?> GetByIdAsync(int id, IDbTransaction? transaction = null);

    public Task<bool> UpdateBalanceAsync(int id, decimal newBalance, IDbTransaction? transaction = null);
    public Task<AccountEntity?> GetByAccountNumberAsync(string accountNumber);
}