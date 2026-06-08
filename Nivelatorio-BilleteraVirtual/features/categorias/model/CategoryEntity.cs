using Nivelatorio_BilleteraVirtual.features.common;

namespace Nivelatorio_BilleteraVirtual.features.categorias.model;

public record CategoryEntity(
    int Id, 
    string Name, 
    MovementType Type
);