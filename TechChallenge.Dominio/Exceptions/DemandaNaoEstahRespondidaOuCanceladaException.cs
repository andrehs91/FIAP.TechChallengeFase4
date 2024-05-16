namespace TechChallenge.Dominio.Exceptions;

public class DemandaNaoEstahRespondidaOuCanceladaException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "Esta demanda ainda não foi respondida ou cancelada.";

    public DemandaNaoEstahRespondidaOuCanceladaException()
        : base(defaultMessage)
    {
    }

    public DemandaNaoEstahRespondidaOuCanceladaException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public DemandaNaoEstahRespondidaOuCanceladaException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
