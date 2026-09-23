namespace Catarinense.Application.Exceptions;

/// <summary>
/// Lançada quando um recurso solicitado (ônibus, usuário, registro de limpeza, etc.)
/// não é encontrado. A API layer deve traduzir isso para HTTP 404.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
