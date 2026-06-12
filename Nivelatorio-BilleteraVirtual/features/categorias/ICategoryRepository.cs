using Nivelatorio_BilleteraVirtual.features.categorias.model;

namespace Nivelatorio_BilleteraVirtual.features.categorias;

public interface ICategoryRepository
{
    Task<CategoryEntity?> save(CategoryEntity category);
    Task<IEnumerable<CategoryEntity>> GetAllAsync();
}