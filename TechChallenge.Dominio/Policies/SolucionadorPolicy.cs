using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;
using TechChallenge.Dominio.Interfaces;

namespace TechChallenge.Dominio.Policies;

public class SolucionadorPolicy(IAtividadeRepository repositorioDeAtividades) : ISolucionadorPolicy
{
    private readonly IAtividadeRepository _repositorioDeAtividades = repositorioDeAtividades;

    public Usuario? IdentificarSolucionadorMenosAtarefado(Atividade atividade)
    {
        if (atividade.TipoDeDistribuicao == TiposDeDistribuicao.Manual) return null;

        return _repositorioDeAtividades.IdentificarSolucionadorMenosAtarefado(atividade.Id);
    }

    public Usuario? IdentificarSolucionadorMenosAtarefado(int idAtividade)
    {
        return _repositorioDeAtividades.IdentificarSolucionadorMenosAtarefado(idAtividade);
    }
}
