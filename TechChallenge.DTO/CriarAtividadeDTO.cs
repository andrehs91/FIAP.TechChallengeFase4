using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TechChallenge.Dominio.Enums;

namespace TechChallenge.DTO;

public class CriarAtividadeDTO
{
    [Required(ErrorMessage = "O campo 'Nome' é obrigatório")]
    public string Nome { get; set; } = null!;

    [Required(ErrorMessage = "O campo 'Descricao' é obrigatório")]
    public string Descricao { get; set; } = null!;

    [Required(ErrorMessage = "O campo 'TipoDeDistribuicao' é obrigatório")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TiposDeDistribuicao TipoDeDistribuicao { get; set; }

    [Required(ErrorMessage = "O campo 'Prioridade' é obrigatório")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Prioridades Prioridade { get; set; }

    [Required(ErrorMessage = "O campo 'PriorPrazoEstimadoidade' é obrigatório")]
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
