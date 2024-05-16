using TechChallenge.Dominio.Entities;

namespace TechChallenge.Dominio.Interfaces;

public interface IDemandaRepository
{
    void Criar(Demanda demanda);
    Demanda? BuscarPorId(int id);
    IList<Demanda> BuscarTodas();
    IList<Demanda> BuscarPorSolicitante(int idSolicitante);
    IList<Demanda> BuscarPorDepartamentoSolicitante(string departamento);
    IList<Demanda> BuscarPorSolucionador(int idSolucionador);
    IList<Demanda> BuscarPorDepartamentoSolucionador(string departamento);
    void Editar(Demanda demanda);
    void Apagar(Demanda demanda);
}
