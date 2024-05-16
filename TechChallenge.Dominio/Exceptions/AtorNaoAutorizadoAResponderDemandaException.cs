namespace TechChallenge.Dominio.Exceptions;

public class AtorNaoAutorizadoAResponderDemandaException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "Apenas o solucionador da demanda pode respondê-la.";

    public AtorNaoAutorizadoAResponderDemandaException()
        : base(defaultMessage)
    {
    }

    public AtorNaoAutorizadoAResponderDemandaException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public AtorNaoAutorizadoAResponderDemandaException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
