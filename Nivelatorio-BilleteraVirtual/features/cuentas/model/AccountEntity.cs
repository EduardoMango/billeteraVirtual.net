namespace Nivelatorio_BilleteraVirtual.features.cuentas.model;

public record AccountEntity
{
    public int Id { get; init; }
    public string HolderName { get; init; } = string.Empty;
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Balance { get; init; }

    // El método Factory se adapta usando llaves de inicialización
    public static AccountEntity create(string holderName)
    {
        var accountNumber = Guid.NewGuid().ToString("N").Substring(0, 20).ToUpper(); 
        
        return new AccountEntity
        {
            Id = 0,
            HolderName = holderName,
            AccountNumber = accountNumber,
            Balance = 0.00m
        };
    }
}