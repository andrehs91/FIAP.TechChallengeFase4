namespace TechChallenge.Dominio.Exceptions;

public class AtorNaoAutorizadoAEncaminharDemandaException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "O encaminhamento de demandas é restrito ao solucionador da demanda e aos gestores do departamento solucionador.";

    public AtorNaoAutorizadoAEncaminharDemandaException()
        : base(defaultMessage)
    {
    }

    public AtorNaoAutorizadoAEncaminharDemandaException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public AtorNaoAutorizadoAEncaminharDemandaException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
