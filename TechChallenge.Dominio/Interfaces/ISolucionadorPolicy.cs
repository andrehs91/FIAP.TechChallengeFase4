using TechChallenge.Dominio.Entities;

namespace TechChallenge.Dominio.Interfaces;

public interface ISolucionadorPolicy
{
    Usuario? IdentificarSolucionadorMenosAtarefado(Atividade atividade);
    Usuario? IdentificarSolucionadorMenosAtarefado(int idAtividade);
}
