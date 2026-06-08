using Nivelatorio_BilleteraVirtual.features.common;

namespace Nivelatorio_BilleteraVirtual.features.transacciones.model;

public record TransactionEntity(
    int Id, 
    int AccountId, 
    int CategoryId, 
    decimal Amount, 
    MovementType Type, 
    DateTime Date
);