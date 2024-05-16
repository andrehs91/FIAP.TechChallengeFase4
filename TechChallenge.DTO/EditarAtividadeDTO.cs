using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TechChallenge.Dominio.Enums;

namespace TechChallenge.DTO;

public class EditarAtividadeDTO
{
    [Required]
    public string Nome { get; set; } = null!;

    [Required]
    public string Descricao { get; set; } = null!;

    [Required]
    public bool EstahAtiva { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TiposDeDistribuicao TipoDeDistribuicao { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Prioridades Prioridade { get; set; }

    [Required]
    public uint PrazoEstimado { get; set; }

    public AtividadeDTO ConverterParaAtividadeDTO(int id, string departamentoSolucionador)
    {
        return new AtividadeDTO()
        {
            Id = id,
            Nome = Nome,
            Descricao = Descricao,
            EstahAtiva = EstahAtiva,
            DepartamentoSolucionador = departamentoSolucionador,
            TipoDeDistribuicao = TipoDeDistribuicao,
            Prioridade = Prioridade,
            PrazoEstimado = PrazoEstimado
        };
    }
}
