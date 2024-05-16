using System.Text.Json.Serialization;

namespace TechChallenge.DTO;

public class RespostaDTO(RespostaDTO.TiposDeResposta tipo, string mensagem)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TiposDeResposta Tipo { get; set; } = tipo;

    public string Mensagem { get; set; } = mensagem;

    public enum TiposDeResposta
    {
        Erro, Aviso, Sucesso
    }
}
