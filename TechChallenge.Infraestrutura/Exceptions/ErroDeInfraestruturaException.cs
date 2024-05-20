namespace TechChallenge.Infraestrutura.Exceptions;

public class ErroDeInfraestruturaException : Exception
{
    public ErroDeInfraestruturaException()
    {
    }

    public ErroDeInfraestruturaException(string? message)
        : base(message)
    {
    }

    public ErroDeInfraestruturaException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
