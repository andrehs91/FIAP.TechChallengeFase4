using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TechChallenge.DTO;

public class AutenticarDTO
{
    [Required]
    public string Matricula { get; set; } = null!;

    [Required]
    [DefaultValue("senha")]
    public string Senha { get; set; } = null!;
}
