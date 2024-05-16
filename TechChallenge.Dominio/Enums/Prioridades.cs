using System.ComponentModel;

namespace TechChallenge.Dominio.Enums;

public enum Prioridades
{
    [Description("Crítica")]
    Critica,

    [Description("Muito Alta")]
    MuitoAlta,

    [Description("Alta")]
    Alta,

    [Description("Média")]
    Media,

    [Description("Baixa")]
    Baixa,
}
