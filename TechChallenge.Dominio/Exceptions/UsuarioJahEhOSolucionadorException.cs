namespace TechChallenge.Dominio.Exceptions;

public class UsuarioJahEhOSolucionadorException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "O usuário já é o solucionador desta demanda.";

    public UsuarioJahEhOSolucionadorException()
        : base(defaultMessage)
    {
    }

    public UsuarioJahEhOSolucionadorException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public UsuarioJahEhOSolucionadorException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
