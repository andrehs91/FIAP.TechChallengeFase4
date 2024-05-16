namespace TechChallenge.Dominio.Exceptions;

public class AtorNaoAutorizadoAReabrirDemandaException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "A reabertura de demandas é restrita aos usuários do departamento solicitante.";

    public AtorNaoAutorizadoAReabrirDemandaException()
        : base(defaultMessage)
    {
    }

    public AtorNaoAutorizadoAReabrirDemandaException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public AtorNaoAutorizadoAReabrirDemandaException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
