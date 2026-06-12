using System.Data;
using System.Data.Common;
using Dapper;
using Nivelatorio_BilleteraVirtual.features.cuentas.model;

namespace Nivelatorio_BilleteraVirtual.features.cuentas;

public class AccountRepository(IDbConnection connection) : IAccountRepository
{
    public async Task<AccountEntity?> save(AccountEntity c)
    {
        const string sql = """
                           INSERT INTO wallet.cuentas (nombretitular, numerocuenta, saldo)
                           VALUES (@HolderName, @AccountNumber, @Balance)
                           RETURNING 
                               id AS Id, 
                               nombretitular AS HolderName, 
                               numerocuenta AS AccountNumber, 
                               saldo AS Balance;
                           """;

        return await connection.QueryFirstOrDefaultAsync<AccountEntity>(sql, c);
    }
    

    public async Task<AccountEntity?> GetByIdAsync(int id, IDbTransaction? transaction = null)
    {
        const string sql = "SELECT * FROM wallet.cuentas WHERE Id = @Id;";
        return await connection.QueryFirstOrDefaultAsync<AccountEntity>(sql, new { Id = id }, transaction);
    }

    public async Task<bool> UpdateBalanceAsync(int id, decimal newBalance, IDbTransaction? transaction = null)
    {
        const string sql = "UPDATE wallet.cuentas SET Saldo = @NewBalance WHERE Id = @Id;";
        int affectedRows = await connection.ExecuteAsync(sql, new { Id = id, NewBalance = newBalance }, transaction);
        return affectedRows > 0;
    }
    public async Task<AccountEntity?> GetByAccountNumberAsync(string accountNumber)
    {
        const string sql = """
                           SELECT 
                               id AS Id,
                               numerocuenta AS AccountNumber, 
                               saldo AS Balance, 
                               nombretitular AS HolderName
                           FROM wallet.cuentas
                           WHERE numerocuenta = @AccountNumber;
                           """;
        return await connection.QueryFirstOrDefaultAsync<AccountEntity>(sql, new { AccountNumber = accountNumber });
    }
    
    
}