using Nivelatorio_BilleteraVirtual.features.transacciones.model;

namespace Nivelatorio_BilleteraVirtual.features.transacciones;

public static class TransactionEndpoints
{
    public static IEndpointRouteBuilder MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions");

        group.MapPost("/", (NewTransactionDto newTransaction, TransactionService service) => service.Save(newTransaction));
        
        group.MapDelete("/{publicId:guid}", async (Guid publicId, TransactionService service) =>
        {
            await service.RevertirYEliminarTransaccionAsync(publicId);
            return Results.NoContent(); // 204 si sale bien
        });

        return group;
    }
}