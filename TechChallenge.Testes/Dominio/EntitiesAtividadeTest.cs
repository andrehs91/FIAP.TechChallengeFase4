using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;
using Xunit.Abstractions;

namespace TechChallenge.Testes.Dominio;

public class EntitiesAtividadeTest(ITestOutputHelper testOutputHelper)
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

    [Theory]
    [InlineData("", "Descrição da atividade.", true, "Departamento", TiposDeDistribuicao.Automatica, Prioridades.Critica, 1, typeof(ArgumentNullException))]
    [InlineData("Nome da atividade", "...", true, "Departamento", TiposDeDistribuicao.Automatica, Prioridades.Media, 1, typeof(ArgumentException))]
    [InlineData("Nome da atividade", "", true, "Departamento", TiposDeDistribuicao.Automatica, Prioridades.Alta, 1, typeof(ArgumentNullException))]
    [InlineData("Nome da atividade", "Descrição da atividade.", true, "", TiposDeDistribuicao.Automatica, Prioridades.Baixa, 1, typeof(ArgumentNullException))]
    [InlineData("Nome da atividade", "Descrição da atividade.", true, "Departamento", TiposDeDistribuicao.Automatica, Prioridades.Media, 0, typeof(ArgumentException))]
    [InlineData("Nome da atividade", "Descrição da atividade.", true, "Departamento", TiposDeDistribuicao.Manual, Prioridades.Media, 259201, typeof(ArgumentException))]
    [InlineData("Nome", "Descrição da atividade.", true, "Departamento", TiposDeDistribuicao.Automatica, Prioridades.MuitoAlta, 1, typeof(ArgumentException))]
    public void InstanciarAtividadeDeveLancarExcecao(
        string nome,
        string descricao,
        bool estahAtiva,
        string departamentoSolucionador,
        TiposDeDistribuicao tipoDeDistribuicao,
        Prioridades prioridade,
        uint prazoEstimado,
        Type exceptionType)
    {
        // Arrange
        void act() => new Atividade(
            nome,
            descricao,
            estahAtiva,
            departamentoSolucionador,
            tipoDeDistribuicao,
            prioridade,
            prazoEstimado);

        // Act and Assert
        Exception exception = Assert.Throws(exceptionType, act);
        _testOutputHelper.WriteLine(exception.Message);
    }
}
