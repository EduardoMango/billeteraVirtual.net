namespace Nivelatorio_BilleteraVirtual.features.cuentas.model;

public record TransferDto(
    string OriginAccountNumber,
    string TargetAccountNumber,
    decimal Amount
);
