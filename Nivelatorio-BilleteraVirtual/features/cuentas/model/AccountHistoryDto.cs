using Nivelatorio_BilleteraVirtual.features.transacciones.model;

namespace Nivelatorio_BilleteraVirtual.features.cuentas.model;

public class AccountHistoryDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string HolderName { get; set; } = string.Empty;
    public List<TransactionDto> Transactions { get; set; } = new();
}
