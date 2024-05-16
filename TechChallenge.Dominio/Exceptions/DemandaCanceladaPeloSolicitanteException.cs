namespace TechChallenge.Dominio.Exceptions;

public class DemandaCanceladaPeloSolicitanteException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "Esta demanda foi cancelada pelo solicitante, portanto não pode ser reaberta.";

    public DemandaCanceladaPeloSolicitanteException()
        : base(defaultMessage)
    {
    }

    public DemandaCanceladaPeloSolicitanteException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public DemandaCanceladaPeloSolicitanteException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
