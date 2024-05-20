using System.Text.Json.Serialization;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;

namespace TechChallenge.Aplicacao.DTO;

public class AtividadeComSolucionadoresDTO
{

    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool EstahAtiva { get; set; } = true;
    public string DepartamentoSolucionador { get; set; } = string.Empty;
    [JsonConverter(typeof(JsonStringEnumConverter))] public TiposDeDistribuicao TipoDeDistribuicao { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))] public Prioridades Prioridade { get; set; }
    public uint PrazoEstimado { get; set; }
    public List<SolucionadorDTO> Solucionadores { get; set; } = [];

    public AtividadeComSolucionadoresDTO() { }

    public AtividadeComSolucionadoresDTO(Atividade atividade)
    {
        Id = atividade.Id;
        Nome = atividade.Nome;
        Descricao = atividade.Descricao;
        EstahAtiva = atividade.EstahAtiva;
        DepartamentoSolucionador = atividade.DepartamentoSolucionador;
        TipoDeDistribuicao = atividade.TipoDeDistribuicao;
        Prioridade = atividade.Prioridade;
        PrazoEstimado = atividade.PrazoEstimado;

        if (atividade.Solucionadores.Any())
        {
            foreach (var solucionador in atividade.Solucionadores)
                Solucionadores.Add(new SolucionadorDTO(solucionador));
        }
    }
}
