using Nivelatorio_BilleteraVirtual.features.common;

namespace Nivelatorio_BilleteraVirtual.features.transacciones.model;

public record TransactionEntity
{
    public int Id { get; init; }
    public Guid PublicId { get; init; }
    public int AccountId { get; init; }
    public int CategoryId { get; init; }
    public decimal Amount { get; init; }
    public MovementType Type { get; init; }
    public DateTime Date { get; init; }
}