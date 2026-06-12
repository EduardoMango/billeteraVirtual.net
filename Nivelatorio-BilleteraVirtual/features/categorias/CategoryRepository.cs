using System.Data;
using Dapper;
using Nivelatorio_BilleteraVirtual.features.categorias.model;

namespace Nivelatorio_BilleteraVirtual.features.categorias;

public class CategoryRepository(IDbConnection connection) : ICategoryRepository
{
    public async Task<CategoryEntity?> save(CategoryEntity category)
    {
        var sql = """
                  INSERT INTO wallet.categorias (Nombre, Tipo)
                  VALUES (@Name, @Type)
                  RETURNING Id, Nombre AS Name, Tipo AS Type; 
                  """;

        // Pasamos un objeto anónimo mapeando Type como String
        return await connection.QueryFirstOrDefaultAsync<CategoryEntity>(sql, new {
            category.Name,
            Type = category.Type.ToString() // Esto enviará "INGRESO" en lugar de 0
        });
    }

    public async Task<IEnumerable<CategoryEntity>> GetAllAsync()
    {
        const string sql = "SELECT id AS Id, nombre AS Name, tipo AS Type FROM wallet.categorias;";
        return await connection.QueryAsync<CategoryEntity>(sql);
    }
}