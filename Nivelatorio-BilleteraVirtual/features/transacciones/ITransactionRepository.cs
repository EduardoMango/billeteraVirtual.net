using System.Data;
using Nivelatorio_BilleteraVirtual.features.transacciones.model;

namespace Nivelatorio_BilleteraVirtual.features.transacciones;

public interface ITransactionRepository
{
    Task<TransactionEntity?> save(string accountNumber, 
        string categoryName, 
        decimal amount, 
        string type);

    public Task<bool> DeleteAsync(Guid publicId, IDbTransaction? transaction = null);

    public Task<TransactionEntity?> GetByPublicIdAsync(Guid publicId, IDbTransaction? transaction = null);

    public Task<IEnumerable<TransactionEntity>> GetByAccountIdAsync(int accountId);

}