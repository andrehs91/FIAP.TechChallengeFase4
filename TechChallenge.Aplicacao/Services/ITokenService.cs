using TechChallenge.Dominio.Entities;

namespace TechChallenge.Aplicacao.Services;

public interface ITokenService
{
    string GenerateToken(Usuario usuario);
}
