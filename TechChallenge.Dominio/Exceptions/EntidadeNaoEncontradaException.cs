namespace TechChallenge.Dominio.Exceptions;

public class EntidadeNaoEncontradaException : Exception
{
    public EntidadeNaoEncontradaException()
    {
    }

    public EntidadeNaoEncontradaException(string? message)
        : base(message)
    {
    }

    public EntidadeNaoEncontradaException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
