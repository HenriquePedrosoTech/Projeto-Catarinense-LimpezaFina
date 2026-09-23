namespace Catarinense.Application.Exceptions;

/// <summary>
/// Lançada quando o usuário autenticado não tem permissão para executar a ação
/// (ex.: operador tentando aprovar uma limpeza). A API layer deve traduzir para HTTP 403.
/// </summary>
public class NaoAutorizadoException : Exception
{
    public NaoAutorizadoException(string message) : base(message) { }
}
