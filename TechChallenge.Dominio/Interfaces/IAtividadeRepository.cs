using TechChallenge.Dominio.Entities;

namespace TechChallenge.Dominio.Interfaces;

public interface IAtividadeRepository
{
    bool Criar(Atividade atividade);
    Atividade? BuscarPorId(int id);
    Atividade? BuscarPorIdComSolucionadores(int id);
    IList<Atividade> BuscarTodas();
    IList<Atividade> BuscarAtivas();
    IList<Atividade> BuscarPorDepartamentoSolucionador(string departamento);
    bool Editar(Atividade atividade);
    Usuario? IdentificarSolucionadorMenosAtarefado(int atividadeId);
}
