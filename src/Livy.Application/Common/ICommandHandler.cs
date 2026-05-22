// Application/Common/ICommandHandler.cs
namespace Livy.Application.Common;
 
/// <summary>
/// Contrato base para todos los Command Handlers.
/// Un Command cambia el estado del sistema (crear, actualizar, eliminar).
/// </summary>
public interface ICommandHandler<TCommand, TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}
 
/// <summary>
/// Contrato base para todos los Query Handlers.
/// Una Query solo lee datos, nunca modifica el estado.
/// </summary>
public interface IQueryHandler<TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}
 