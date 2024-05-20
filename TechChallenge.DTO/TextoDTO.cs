using System.ComponentModel.DataAnnotations;

namespace TechChallenge.DTO;

public class TextoDTO
{
    [Required(ErrorMessage = "O campo 'Conteudo' é obrigatório")]
    [MinLength(3)]
    public string Conteudo { get; set; } = null!;
}
