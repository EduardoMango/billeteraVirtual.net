using Nivelatorio_BilleteraVirtual.features.categorias.model;

namespace Nivelatorio_BilleteraVirtual.features.categorias;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoriaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categorias");

        group.MapPost("/", (NewCategoryDto newCategory, CategoryService service) => service.Save(newCategory));

        return group;
    }
}