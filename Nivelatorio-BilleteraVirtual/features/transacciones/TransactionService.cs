using System.Data;
using Nivelatorio_BilleteraVirtual.features.common;
using Nivelatorio_BilleteraVirtual.features.cuentas;
using Nivelatorio_BilleteraVirtual.features.transacciones.model;

namespace Nivelatorio_BilleteraVirtual.features.transacciones;

public class TransactionService(IAccountRepository cuentaRepo, ITransactionRepository transaccionRepo, IDbConnection connection)
{
    

    
    public async Task<TransactionDto?> Save(NewTransactionDto dto)
    {
        // Enviamos los datos directamente al nuevo método del repositorio
        var saved = await transaccionRepo.save(
            dto.AccountNumber,
            dto.CategoryName,
            dto.Amount,
            dto.Type.ToString() // Convierte el enum a "INGRESO" o "EGRESO"
        );

        return saved != null
            ? new TransactionDto(
                saved.PublicId,
                dto.AccountNumber, 
                dto.CategoryName, 
                saved.Amount, 
                dto.Type,
                saved.Date)
            : null;
    }
    public async Task RevertirYEliminarTransaccionAsync(Guid publicId)
    {
        // 1. Abrimos la conexión si no está abierta (es Scoped, se comparte)
        if (connection.State == ConnectionState.Closed) 
            connection.Open();

        // 2. Iniciamos la transacción explícita de C#
        using var dbTransaction = connection.BeginTransaction();

        try
        {
            // LÓGICA 1: Validar existencia de la transacción
            var transaccion = await transaccionRepo.GetByPublicIdAsync(publicId, dbTransaction);
            if (transaccion == null)
                throw new KeyNotFoundException($"La transacción con ID {publicId} no existe.");

            // LÓGICA 2: Obtener la cuenta asociada
            var cuenta = await cuentaRepo.GetByIdAsync(transaccion.AccountId, dbTransaction);
            if (cuenta == null)
                throw new InvalidOperationException("La cuenta asociada a la transacción ya no existe.");

            // LÓGICA 3: Calcular el nuevo saldo aplicando la reversa
            // Si era INGRESO, al eliminarlo hay que RESTAR el monto.
            // Si era EGRESO, al eliminarlo hay que SUMAR el monto de vuelta.
            decimal nuevoSaldo = transaccion.Type.Equals(MovementType.EGRESO) 
                ? cuenta.Balance - transaccion.Amount 
                : cuenta.Balance + transaccion.Amount;

            // LÓGICA 4: Validar impacto (No permitir que el saldo quede en negativo tras la reversa)
            if (nuevoSaldo < 0)
                throw new InvalidOperationException("No se puede eliminar la transacción: El saldo de la cuenta quedaría en negativo.");

            // 3. Persistir los cambios en la Base de Datos
            // Actualizar el saldo de la cuenta
            await cuentaRepo.UpdateBalanceAsync(cuenta.Id, nuevoSaldo, dbTransaction);

            // Eliminar la transacción física
            await transaccionRepo.DeleteAsync(publicId, dbTransaction);

            // 4. Si todo salió bien, confirmamos los cambios en la BD
            dbTransaction.Commit();
        }
        catch
        {
            // Si algo falló en cualquier punto, deshacemos todo lo que se haya hecho en este bloque
            dbTransaction.Rollback();
            throw; // Re-lanzamos la excepción para que la capa superior la maneje
        }
    }
    
}