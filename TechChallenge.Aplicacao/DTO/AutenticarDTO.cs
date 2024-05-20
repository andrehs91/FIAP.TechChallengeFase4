using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TechChallenge.Aplicacao.DTO;

public class AutenticarDTO
{
    [Required(ErrorMessage = "O campo 'Matricula' é obrigatório")]
    public string Matricula { get; set; } = null!;

    [Required(ErrorMessage = "O campo 'Senha' é obrigatório")]
    [DefaultValue("senha")]
    public string Senha { get; set; } = null!;
}
