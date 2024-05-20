using System.ComponentModel.DataAnnotations;

namespace TechChallenge.Aplicacao.DTO;

public class AbrirDemandaDTO
{
    [Required(ErrorMessage = "O campo 'Id' é obrigatório")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O campo 'Detalhes' é obrigatório")]
    public string Detalhes { get; set; } = null!;
}
