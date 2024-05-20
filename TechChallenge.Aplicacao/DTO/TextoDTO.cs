using System.ComponentModel.DataAnnotations;

namespace TechChallenge.Aplicacao.DTO;

public class TextoDTO
{
    [Required(ErrorMessage = "O campo 'Conteudo' é obrigatório")]
    [MinLength(3)]
    public string Conteudo { get; set; } = null!;
}
