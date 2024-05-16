using TechChallenge.Dominio.Entities;

namespace TechChallenge.Testes.Dominio;

public class EntitiesUsuarioTest
{
    [Theory]
    [InlineData(1, "1000", "Nome do Usuário", "Departamento", false, true)]
    [InlineData(1, "2000", "Usuário", "Nome do Departamento", true, true)]
    [InlineData(2, "1000", "Nome do Usuário", "Departamento", false, false)]
    [InlineData(2, "2000", "Usuário", "Nome do Departamento", true, false)]
    public void ComparacaoEntreUsuarios(
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor,
        bool ehIgual)
    {
        // Arrange
        Usuario usuarioA = new()
        {
            Id = 1,
            Matricula = "1000",
            Nome = "Nome do Usuário",
            Departamento = "Departamento",
            EhGestor = true,
        };
        Usuario usuarioB = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };

        // Act and Assert
        Assert.Equal(ehIgual, usuarioA.Equals(usuarioB));
    }
}
