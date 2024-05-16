using Microsoft.EntityFrameworkCore;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.Infraestrutura.Data;

namespace TechChallenge.Infraestrutura.Repositories;

public class DemandaRepository(ApplicationDbContext context) : IDemandaRepository
{
    private readonly ApplicationDbContext _context = context;

    public void Criar(Demanda demanda)
    {
        _context.Demandas.Add(demanda);
        _context.SaveChanges();
    }

    public Demanda? BuscarPorId(int id)
    {
        return _context.Demandas
            .Include(d => d.Atividade)
            .Include(d => d.EventosRegistrados)
                .ThenInclude(er => er.UsuarioSolucionador)
            .Include(d => d.UsuarioSolicitante)
            .Include(d => d.UsuarioSolucionador)
            .Where(d => d.Id == id)
            .FirstOrDefault();
    }

    public IList<Demanda> BuscarTodas()
    {
        return _context.Demandas.AsNoTracking().ToList();
    }

    public IList<Demanda> BuscarPorSolicitante(int idSolicitante)
    {
        return _context.Demandas
            .Include(d => d.Atividade)
            .Include(d => d.EventosRegistrados)
                .ThenInclude(er => er.UsuarioSolucionador)
            .Include(d => d.UsuarioSolicitante)
            .Include(d => d.UsuarioSolucionador)
            .Where(d => d.UsuarioSolicitanteId == idSolicitante)
            .OrderBy(d => d.Id)
            .AsNoTracking()
            .ToList();
    }

    public IList<Demanda> BuscarPorDepartamentoSolicitante(string departamento)
    {
        return _context.Demandas
            .Include(d => d.Atividade)
            .Include(d => d.EventosRegistrados)
                .ThenInclude(er => er.UsuarioSolucionador)
            .Include(d => d.UsuarioSolicitante)
            .Include(d => d.UsuarioSolucionador)
            .Where(d => d.DepartamentoSolicitante == departamento)
            .OrderBy(d => d.Id)
            .AsNoTracking()
            .ToList();
    }

    public IList<Demanda> BuscarPorSolucionador(int idSolucionador)
    {
        return _context.Demandas
            .Include(d => d.Atividade)
            .Include(d => d.EventosRegistrados)
                .ThenInclude(er => er.UsuarioSolucionador)
            .Include(d => d.UsuarioSolicitante)
            .Include(d => d.UsuarioSolucionador)
            .Where(d => d.UsuarioSolucionadorId == idSolucionador)
            .OrderBy(d => d.Atividade.Prioridade)
            .OrderBy(d => d.Prazo)
            .AsNoTracking()
            .ToList();
    }

    public IList<Demanda> BuscarPorDepartamentoSolucionador(string departamento)
    {
        return _context.Demandas
            .Include(d => d.Atividade)
            .Include(d => d.EventosRegistrados)
                .ThenInclude(er => er.UsuarioSolucionador)
            .Include(d => d.UsuarioSolicitante)
            .Include(d => d.UsuarioSolucionador)
            .Where(d => d.DepartamentoSolucionador == departamento)
            .OrderBy(d => d.Atividade.Prioridade)
            .OrderBy(d => d.Prazo)
            .AsNoTracking()
            .ToList();
    }

    public void Editar(Demanda demanda)
    {
        _context.Demandas.Update(demanda);
        _context.SaveChanges();
    }

    public void Apagar(Demanda demanda)
    {
        _context.Demandas.Remove(demanda);
        _context.SaveChanges();
    }
}
