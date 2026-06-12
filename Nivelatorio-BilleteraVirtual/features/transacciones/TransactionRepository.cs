using System.Data;
using Dapper;
using Nivelatorio_BilleteraVirtual.features.transacciones.model;
using Npgsql;

namespace Nivelatorio_BilleteraVirtual.features.transacciones;

public class TransactionRepository(IDbConnection connection) : ITransactionRepository
{
    public async Task<TransactionEntity?> save(string accountNumber, 
        string categoryName, 
        decimal amount, 
        string type,
        IDbTransaction? transaction = null)
    {
        const string sql = @"
    SELECT 
        id AS Id, 
        publicid AS PublicId,
        cuentaid AS AccountId, 
        categoriaid AS CategoryId, 
        monto AS Amount, 
        tipo AS Type, 
        fecha AS Date
    FROM wallet.registrar_transaccion(
        @AccountNumber, 
        @CategoryName, 
        @Amount, 
        @Type
    );";

        // Pasamos un objeto anónimo con las propiedades exactas que lee la consulta sql
        return await connection.QueryFirstOrDefaultAsync<TransactionEntity>(sql, new {
            AccountNumber = accountNumber,
            CategoryName = categoryName,
            Amount = amount,
            Type = type.ToString()
        }, transaction);
    }
    
    
    public async Task<TransactionEntity?> GetByPublicIdAsync(Guid publicId, IDbTransaction? transaction = null)
    {
        const string sql = @"
        SELECT 
            id AS Id, 
            publicid AS PublicId, 
            cuentaid AS AccountId,      -- Postgres 'cuentaid' -> C# 'AccountId'
            categoriaid AS CategoryId,  -- Postgres 'categoriaid' -> C# 'CategoryId'
            monto AS Amount, 
            tipo AS Type, 
            fecha AS Date
        FROM wallet.transacciones 
        WHERE publicid = @PublicId;";
        
        // Le pasamos la transacción a Dapper
        return await connection.QueryFirstOrDefaultAsync<TransactionEntity>(sql, new { PublicId = publicId }, transaction);
    }

    public async Task<bool> DeleteAsync(Guid publicId, IDbTransaction? transaction = null)
    {
        const string sql = "DELETE FROM wallet.transacciones WHERE publicid = @PublicId;";
        var affectedRows = await connection.ExecuteAsync(sql, new { PublicId = publicId }, transaction);
        return affectedRows > 0;
    }

    public async Task<IEnumerable<TransactionEntity>> GetByAccountIdAsync(int accountId)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                publicid AS PublicId, 
                cuentaid AS AccountId,
                categoriaid AS CategoryId,
                monto AS Amount, 
                tipo AS Type, 
                fecha AS Date
            FROM wallet.transacciones 
            WHERE cuentaid = @AccountId;";
        return await connection.QueryAsync<TransactionEntity>(sql, new { AccountId = accountId });
    }
}