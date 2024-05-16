using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechChallenge.Dominio.Entities;

namespace TechChallenge.Aplicacao.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(Usuario usuario)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Role, usuario.EhGestor ? "Gestor" : "Usuario"),
            new Claim("Id", usuario.Id.ToString()),
            new Claim("Matricula", usuario.Matricula),
            new Claim("Nome", usuario.Nome),
            new Claim("Departamento", usuario.Departamento.ToString()),
            new Claim("EhGestor", usuario.EhGestor ? "Sim" : "Não")
        ];

        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = _configuration.GetValue<string>("Secret");
        var key = Encoding.ASCII.GetBytes(secret!);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
