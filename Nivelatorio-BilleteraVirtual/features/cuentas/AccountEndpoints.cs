using Nivelatorio_BilleteraVirtual.features.cuentas.model;

namespace Nivelatorio_BilleteraVirtual.features.cuentas;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapCuentaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/accounts");

        group.MapPost("/", (NewAccountDto newAccount, AccountService service) => service.Save(newAccount));
        

        return group;
    }
    
}