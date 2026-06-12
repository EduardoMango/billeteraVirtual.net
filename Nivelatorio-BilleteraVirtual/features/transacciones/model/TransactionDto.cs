using Nivelatorio_BilleteraVirtual.features.common;

namespace Nivelatorio_BilleteraVirtual.features.transacciones.model;

public record TransactionDto(
    Guid PublicId,
    string AccountNumber, 
    string CategoryName, 
    decimal Amount, 
    MovementType Type, 
    DateTime Date
);