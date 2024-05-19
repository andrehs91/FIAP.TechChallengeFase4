using TechChallenge.Dominio.Entities;

namespace TechChallenge.Dominio.Interfaces;

public interface IAtividadeRepository
{
    Atividade Criar(Atividade atividade);
    Atividade? BuscarPorId(int id);
    Atividade? BuscarPorIdComSolucionadores(int id);
    IList<Atividade> BuscarTodas();
    IList<Atividade> BuscarAtivas();
    IList<Atividade> BuscarPorDepartamentoSolucionador(string departamento);
    void Editar(Atividade atividade);
    Usuario? IdentificarSolucionadorMenosAtarefado(int atividadeId);
}
