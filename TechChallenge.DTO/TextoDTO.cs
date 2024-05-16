using System.ComponentModel.DataAnnotations;

namespace TechChallenge.DTO;

public class TextoDTO
{
    [Required]
    [MinLength(3)]
    public string Conteudo { get; set; } = null!;
}
