using Nivelatorio_BilleteraVirtual.features.categorias.model;

namespace Nivelatorio_BilleteraVirtual.features.categorias;

public class CategoryService(ICategoryRepository repository)
{
    public async Task<CategoryDto?> Save(NewCategoryDto dto)
    {
        var toSave = new CategoryEntity(0, dto.Name, dto.Type);
        
        Console.WriteLine(toSave);
        var saved = await repository.save(toSave);

        return saved != null ? new CategoryDto(saved.Name, saved.Type) : null;
    }
}