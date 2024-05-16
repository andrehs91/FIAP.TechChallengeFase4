using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.Infraestrutura.Data;

namespace TechChallenge.Infraestrutura.Repositories;

public class UsuarioRepository(ApplicationDbContext context) : IUsuarioRepository
{
    private readonly ApplicationDbContext _context = context;

    public void Criar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
    }

    public Usuario? BuscarPorId(int id)
    {
        return _context.Usuarios.FirstOrDefault(u => u.Id == id);
    }

    public Usuario? BuscarPorMatricula(string matricula)
    {
        matricula = matricula.ToLower();
        return _context.Usuarios.FirstOrDefault(u => u.Matricula.ToLower() == matricula);
    }

    public IList<Usuario> BuscarTodos()
    {
        return _context.Usuarios.ToList();
    }

    public IList<Usuario> BuscarPorIds(IList<int> ids)
    {
        return _context.Usuarios.Where(u => ids.Contains(u.Id)).ToList();
    }

    public IList<Usuario> BuscarPorMatriculas(IList<string> matriculas)
    {
        matriculas = matriculas.Select(m => m.ToLower()).ToList();
        return _context.Usuarios.Where(u => matriculas.Contains(u.Matricula.ToLower())).ToList();
    }

    public IList<Usuario> BuscarPorDepartamento(string departamento)
    {
        return _context.Usuarios.Where(u => u.Departamento == departamento).ToList();
    }

    public void Editar(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        _context.SaveChanges();
    }

    public void EditarVarios(IList<Usuario> usuarios)
    {
        _context.Usuarios.UpdateRange(usuarios);
        _context.SaveChanges();
    }

    public void Apagar(int id)
    {
        var usuario = BuscarPorId(id);
        if (usuario is not null)
        {
            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();
        }
    }

    public void ApagarVarios(IList<int> ids)
    {
        var usuarios = BuscarPorIds(ids);
        if (usuarios.Any())
        {
            _context.Usuarios.RemoveRange(usuarios);
            _context.SaveChanges();
        }
    }
}
