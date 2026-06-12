using Nivelatorio_BilleteraVirtual.features.cuentas.model;

namespace Nivelatorio_BilleteraVirtual.features.cuentas;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/accounts");

        group.MapPost("/", (NewAccountDto newAccount, AccountService service) => service.Save(newAccount));
        
        group.MapGet("/{accountNumber}/history", async (string accountNumber, AccountService service) => 
        {
            var history = await service.GetHistoryAsync(accountNumber);
            return history is not null ? Results.Ok(history) : Results.NotFound();
        });

        return group;
    }
    
}