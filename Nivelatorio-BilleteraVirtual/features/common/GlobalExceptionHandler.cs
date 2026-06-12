using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Nivelatorio_BilleteraVirtual.features.common;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Ocurrió una excepción no controlada: {Message}", exception.Message);

        // Mapeamos la excepción al código de estado correspondiente
        var (statusCode, title, detail) = exception switch
        {
            KeyNotFoundException => 
                (StatusCodes.Status404NotFound, "Recurso no encontrado", exception.Message),
                
            // CASO ESPECIAL: Capturamos el error de negocio enviado desde el Stored Procedure/Función
            PostgresException { SqlState: "P0001" } pgEx => 
                (StatusCodes.Status400BadRequest, "Error de validación en Billetera", pgEx.MessageText),

            // Capturamos cualquier otro error de Postgres (ej: caída del servidor, error de sintaxis)
            PostgresException pgEx => 
                (StatusCodes.Status500InternalServerError, "Error crítico de Base de Datos", $"Código de estado SQL: {pgEx.SqlState}"),

            InvalidOperationException => 
                (StatusCodes.Status400BadRequest, "Operación inválida", exception.Message),
            
            _ => 
                (StatusCodes.Status500InternalServerError, "Error interno del servidor", "Ocurrió un error inesperado.")
        };

        // Creamos la estructura estándar de respuesta (Problem Details)
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        // Escribimos la respuesta en formato JSON directamente en el cuerpo HTTP
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Retornamos true para decirle a .NET que ya manejamos la excepción 
        // y que no debe seguir propagándose
        return true; 
    }
    
}