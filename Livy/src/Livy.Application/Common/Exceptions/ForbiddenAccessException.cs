// Application/Common/Exceptions/ForbiddenAccessException.cs
namespace Livy.Application.Common.Exceptions;

// El usuario está autenticado (token válido) pero no tiene permiso sobre el recurso → 403.
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
        : base("No tenés permiso para acceder a este recurso.") { }

    public ForbiddenAccessException(string message) : base(message) { }
}
