namespace TechChallenge.Dominio.Exceptions;

public class AcaoNaoAutorizadaException : Exception
{
    public AcaoNaoAutorizadaException()
    {
    }

    public AcaoNaoAutorizadaException(string? message)
        : base(message)
    {
    }

    public AcaoNaoAutorizadaException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
