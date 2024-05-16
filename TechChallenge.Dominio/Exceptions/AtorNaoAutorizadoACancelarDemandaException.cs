namespace TechChallenge.Dominio.Exceptions;

public class AtorNaoAutorizadoACancelarDemandaException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "O cancelamento de demandas é restrito ao solicitante da demanda, ao solucionador da demanda e aos gestores do departamento solucionador.";

    public AtorNaoAutorizadoACancelarDemandaException()
        : base(defaultMessage)
    {
    }

    public AtorNaoAutorizadoACancelarDemandaException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public AtorNaoAutorizadoACancelarDemandaException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
