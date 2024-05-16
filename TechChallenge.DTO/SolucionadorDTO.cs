using TechChallenge.Dominio.Entities;

namespace TechChallenge.DTO;

public class SolucionadorDTO(Usuario usuario)
{
    public int Id { get; set; } = usuario.Id;
    public string Matricula { get; set; } = usuario.Matricula;
    public string Nome { get; set; } = usuario.Nome;
}
