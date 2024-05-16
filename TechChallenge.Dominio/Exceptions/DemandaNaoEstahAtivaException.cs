namespace TechChallenge.Dominio.Exceptions;

public class DemandaNaoEstahAtivaException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "Esta demanda não está na situação 'Aguardando Distribuição' e nem na situação 'Em Atendimento'.";

    public DemandaNaoEstahAtivaException()
        : base(defaultMessage)
    {
    }

    public DemandaNaoEstahAtivaException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public DemandaNaoEstahAtivaException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
