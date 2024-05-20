using TechChallenge.Dominio.Entities;

namespace TechChallenge.Aplicacao.DTO;

public class UsuarioDTO(Usuario usuario)
{
    public int Id { get; set; } = usuario.Id;
    public string? Matricula { get; set; } = usuario.Matricula;
    public string? Nome { get; set; } = usuario.Nome;
    public string Departamento { get; set; } = usuario.Departamento;
    public bool EhGestor { get; set; } = usuario.EhGestor;
}
