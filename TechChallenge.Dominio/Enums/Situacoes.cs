using System.ComponentModel;

namespace TechChallenge.Dominio.Enums;

public enum Situacoes
{
    [Description("Aguardando Distribuição")]
    AguardandoDistribuicao,

    [Description("Em Atendimento")]
    EmAtendimento,

    [Description("Encaminhada pelo Gestor")]
    EncaminhadaPeloGestor,

    [Description("Encaminhada pelo Solucionador")]
    EncaminhadaPeloSolucionador,

    [Description("Capturada")]
    Capturada,

    [Description("Rejeitada")]
    Rejeitada,

    [Description("Respondida")]
    Respondida,

    [Description("Cancelada pelo Gestor")]
    CanceladaPeloGestor,

    [Description("Cancelada pelo Solucionador")]
    CanceladaPeloSolucionador,

    [Description("Cancelada pelo Solicitante")]
    CanceladaPeloSolicitante,
}
