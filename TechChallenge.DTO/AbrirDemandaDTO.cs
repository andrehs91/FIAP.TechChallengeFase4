using System.ComponentModel.DataAnnotations;

namespace TechChallenge.DTO;

public class AbrirDemandaDTO
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Detalhes { get; set; } = null!;
}
