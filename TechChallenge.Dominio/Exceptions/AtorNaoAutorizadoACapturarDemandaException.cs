namespace TechChallenge.Dominio.Exceptions;

public class AtorNaoAutorizadoACapturarDemandaException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "A captura de demandas é restrita aos integrantes do departamento solucionador.";

    public AtorNaoAutorizadoACapturarDemandaException()
        : base(defaultMessage)
    {
    }

    public AtorNaoAutorizadoACapturarDemandaException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public AtorNaoAutorizadoACapturarDemandaException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
