using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TechChallenge.Dominio.Enums;

namespace TechChallenge.DTO;

public class CriarAtividadeDTO
{
    [Required]
    public string Nome { get; set; } = null!;

    [Required]
    public string Descricao { get; set; } = null!;

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TiposDeDistribuicao TipoDeDistribuicao { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Prioridades Prioridade { get; set; }

    [Required]
    public uint PrazoEstimado { get; set; }

    public AtividadeDTO ConverterParaAtividadeDTO(string departamentoSolucionador)
    {
        return new AtividadeDTO()
        {
            Nome = Nome,
            Descricao = Descricao,
            EstahAtiva = true,
            DepartamentoSolucionador = departamentoSolucionador,
            TipoDeDistribuicao = TipoDeDistribuicao,
            Prioridade = Prioridade,
            PrazoEstimado = PrazoEstimado
        };
    }
}
