namespace Nivelatorio_BilleteraVirtual.features.cuentas.model;

public record AccountEntity(
    int Id,
    string HolderName,
    string AccountNumber,
    decimal Balance
)
{
    public static AccountEntity create(string holderName)
    {
        var accountNumber = Guid.NewGuid().ToString("N").Substring(0, 20).ToUpper(); 
        
        return new AccountEntity(
            Id: 0,
            HolderName: holderName,
            AccountNumber: accountNumber,
            Balance: 0.00m
        );
    }
}