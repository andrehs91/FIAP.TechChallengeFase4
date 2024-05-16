namespace TechChallenge.Dominio.Exceptions;

public class AtorNaoAutorizadoAReativarDemandaException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "A reativação de demandas é restrita ao solucionador da demanda e aos gestores do departamento solucionador.";

    public AtorNaoAutorizadoAReativarDemandaException()
        : base(defaultMessage)
    {
    }

    public AtorNaoAutorizadoAReativarDemandaException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public AtorNaoAutorizadoAReativarDemandaException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
