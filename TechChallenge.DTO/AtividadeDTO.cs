using System.Text.Json.Serialization;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;

namespace TechChallenge.DTO;

public class AtividadeDTO
{

    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool EstahAtiva { get; set; } = true;
    public string DepartamentoSolucionador { get; set; } = string.Empty;
    [JsonConverter(typeof(JsonStringEnumConverter))] public TiposDeDistribuicao TipoDeDistribuicao { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))] public Prioridades Prioridade { get; set; }
    public uint PrazoEstimado { get; set; }

    public AtividadeDTO() { }

    public AtividadeDTO(Atividade atividade)
    {
        Id = atividade.Id;
        Nome = atividade.Nome;
        Descricao = atividade.Descricao;
        EstahAtiva = atividade.EstahAtiva;
        DepartamentoSolucionador = atividade.DepartamentoSolucionador;
        TipoDeDistribuicao = atividade.TipoDeDistribuicao;
        Prioridade = atividade.Prioridade;
        PrazoEstimado = atividade.PrazoEstimado;
    }

    public Atividade ConverterParaEntidade()
    {
        return new Atividade()
        {
            Id = Id,
            Nome = Nome,
            Descricao = Descricao,
            EstahAtiva = EstahAtiva,
            DepartamentoSolucionador = DepartamentoSolucionador,
            TipoDeDistribuicao = TipoDeDistribuicao,
            Prioridade = Prioridade,
            PrazoEstimado = PrazoEstimado,
        };
    }
}
