using Nivelatorio_BilleteraVirtual.features.common;

namespace Nivelatorio_BilleteraVirtual.features.transacciones.model;

public record NewTransactionDto(
    string AccountNumber, 
    string CategoryName, 
    decimal Amount, 
    MovementType Type
);