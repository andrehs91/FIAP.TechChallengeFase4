namespace TechChallenge.Dominio.Exceptions;

public class AtorNaoAutorizadoARejeitarDemandaException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "A rejeição de demandas é restrita ao solucionador da demanda.";

    public AtorNaoAutorizadoARejeitarDemandaException()
        : base(defaultMessage)
    {
    }

    public AtorNaoAutorizadoARejeitarDemandaException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public AtorNaoAutorizadoARejeitarDemandaException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
