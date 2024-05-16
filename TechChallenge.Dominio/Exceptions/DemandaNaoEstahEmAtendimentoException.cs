namespace TechChallenge.Dominio.Exceptions;

public class DemandaNaoEstahEmAtendimentoException : AcaoNaoAutorizadaException
{
    private static readonly string defaultMessage = "Esta demanda não está na situação 'Em Atendimento'.";

    public DemandaNaoEstahEmAtendimentoException()
        : base(defaultMessage)
    {
    }

    public DemandaNaoEstahEmAtendimentoException(string? message)
        : base(message ?? defaultMessage)
    {
    }

    public DemandaNaoEstahEmAtendimentoException(string? message, Exception? innerException)
        : base(message ?? defaultMessage, innerException)
    {
    }
}
