using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TechChallenge.Dominio.Enums;

namespace TechChallenge.DTO;

public class EditarAtividadeDTO
{
    [Required(ErrorMessage = "O campo 'Nome' é obrigatório")]
    public string Nome { get; set; } = null!;

    [Required(ErrorMessage = "O campo 'Descricao' é obrigatório")]
    public string Descricao { get; set; } = null!;

    [Required(ErrorMessage = "O campo 'EstahAtiva' é obrigatório")]
    public bool EstahAtiva { get; set; }

    [Required(ErrorMessage = "O campo 'TipoDeDistribuicao' é obrigatório")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TiposDeDistribuicao TipoDeDistribuicao { get; set; }

    [Required(ErrorMessage = "O campo 'Prioridade' é obrigatório")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Prioridades Prioridade { get; set; }

    [Required(ErrorMessage = "O campo 'PrazoEstimado' é obrigatório")]
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
