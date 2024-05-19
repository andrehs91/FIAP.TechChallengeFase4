using Moq;
using System.Text.Json.Serialization;
using System.Text.Json;
using TechChallenge.Aplicacao.Commands;
using TechChallenge.Aplicacao.Configurations;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;
using TechChallenge.Dominio.Interfaces;
using Xunit.Abstractions;
using TechChallenge.Dominio.Exceptions;

namespace TechChallenge.Testes.Aplicacao;

public class CommandsDemandaCommandTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    private readonly Dictionary<int, Demanda> _demandas;
    private readonly Dictionary<int, Usuario> _usuarios;
    private readonly DemandaCommand _demandaCommand;

    public CommandsDemandaCommandTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        // Default Arrange
        var mockAppSettings = new Mock<IAppSettings>();

        Atividade atividade = new()
        {
            Id = 1,
            Nome = "Nome da Atividade",
            Descricao = "Descrição da Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Manual,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        };
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorId(It.Is<int>(id => id == 1))).Returns(atividade);
        mockAtividadeRepository.Setup(m => m.BuscarPorId(It.Is<int>(id => id != 1))).Returns(() => null);

        _usuarios = [];
        _usuarios.Add(1, new()
        {
            Id = 1,
            Matricula = "1000",
            Nome = "Nome do Gestor",
            Departamento = "Departamento",
            EhGestor = true,
        });
        _usuarios.Add(2, new()
        {
            Id = 2,
            Matricula = "2000",
            Nome = "Nome do Usuário",
            Departamento = "Departamento",
            EhGestor = false,
        });
        _usuarios.Add(3, new()
        {
            Id = 3,
            Matricula = "3000",
            Nome = "Nome do Usuário do Outro Departamento",
            Departamento = "Outro Departamento",
            EhGestor = false,
        });

        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        mockUsuarioRepository.Setup(m => m.BuscarPorId(It.Is<int>(id => !_usuarios.Keys.Contains(id))))
            .Returns(() => null);
        mockUsuarioRepository.Setup(m => m.BuscarPorId(It.Is<int>(id => id == 1))).Returns(_usuarios[1]);
        mockUsuarioRepository.Setup(m => m.BuscarPorId(It.Is<int>(id => id == 2))).Returns(_usuarios[2]);
        mockUsuarioRepository.Setup(m => m.BuscarPorId(It.Is<int>(id => id == 3))).Returns(_usuarios[3]);

        _demandas = [];
        Demanda demanda1 = Demanda.Abrir(atividade, _usuarios[1], "Solicitante do mesmo departamento; Aguardando Distribuição.");
        demanda1.Id = 1;
        _demandas.Add(1, demanda1);
        Demanda demanda2 = Demanda.Abrir(atividade, _usuarios[3], "Solicitante de outro departamento; Em Atendimento.");
        demanda2.Id = 2;
        demanda2.DefinirSolucionador(_usuarios[2]);
        _demandas.Add(2, demanda2);
        Demanda demanda3 = Demanda.Abrir(atividade, _usuarios[3], "Solicitante de outro departamento; Aguardando Distribuição.");
        demanda3.Id = 3;
        _demandas.Add(3, demanda3);
        Demanda demanda4 = Demanda.Abrir(atividade, _usuarios[3], "Solicitante de outro departamento; Aguardando Distribuição.");
        demanda4.Id = 4;
        _demandas.Add(4, demanda4);

        var mockDemandaRepository = new Mock<IDemandaRepository>();
        mockDemandaRepository.Setup(m => m.Criar(It.IsAny<Demanda>()))
            .Returns(Demanda.Abrir(atividade, _usuarios[2], "Detalhes"));
        mockDemandaRepository.Setup(m => m.BuscarPorId(It.Is<int>(id => !_demandas.Keys.Contains(id))))
            .Returns(() => null);
        mockDemandaRepository.Setup(m => m.BuscarPorId(It.Is<int>(id => id == 1))).Returns(_demandas[1]);
        mockDemandaRepository.Setup(m => m.BuscarPorId(It.Is<int>(id => id == 2))).Returns(_demandas[2]);
        mockDemandaRepository.Setup(m => m.BuscarPorId(It.Is<int>(id => id == 3))).Returns(_demandas[3]);
        mockDemandaRepository.Setup(m => m.BuscarPorId(It.Is<int>(id => id == 4))).Returns(_demandas[4]);
        mockDemandaRepository.Setup(m => m.BuscarPorSolicitante(It.Is<int>(id => id == 1)))
            .Returns(_demandas.Values.Where(d => d.UsuarioSolicitanteId == 1).ToList());
        mockDemandaRepository.Setup(m => m.BuscarPorSolicitante(It.Is<int>(id => id == 2)))
            .Returns(_demandas.Values.Where(d => d.UsuarioSolicitanteId == 2).ToList());
        mockDemandaRepository.Setup(m => m.BuscarPorDepartamentoSolicitante(It.Is<string>(d => d == "Departamento")))
            .Returns(_demandas.Values.Where(d => d.DepartamentoSolicitante == "Departamento").ToList());
        mockDemandaRepository.Setup(m => m.BuscarPorDepartamentoSolicitante(It.Is<string>(d => d == "Outro Departamento")))
            .Returns(_demandas.Values.Where(d => d.DepartamentoSolicitante == "Outro Departamento").ToList());
        mockDemandaRepository.Setup(m => m.BuscarPorSolucionador(It.Is<int>(id => id == 1)))
            .Returns(_demandas.Values.Where(d => d.UsuarioSolucionadorId == 1).ToList());
        mockDemandaRepository.Setup(m => m.BuscarPorSolucionador(It.Is<int>(id => id == 2)))
            .Returns(_demandas.Values.Where(d => d.UsuarioSolucionadorId == 2).ToList());
        mockDemandaRepository.Setup(m => m.BuscarPorDepartamentoSolucionador(It.Is<string>(d => d == "Departamento")))
            .Returns(_demandas.Values.Where(d => d.DepartamentoSolucionador == "Departamento").ToList());
        mockDemandaRepository.Setup(m => m.BuscarPorDepartamentoSolucionador(It.Is<string>(d => d == "Outro Departamento")))
            .Returns(_demandas.Values.Where(d => d.DepartamentoSolucionador == "Outro Departamento").ToList());
        mockDemandaRepository.Setup(m => m.Editar(It.IsAny<Demanda>()));

        _demandaCommand = new(
            mockAppSettings.Object,
            mockAtividadeRepository.Object,
            mockDemandaRepository.Object,
            mockUsuarioRepository.Object);
    }

    [Fact]
    public void AbrirDemanda()
    {
        // Act
        Demanda demanda = _demandaCommand.AbrirDemanda(_usuarios[2], 1, "Detalhes...");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demanda, _jsonSerializerOptions));

        // Assert
        Assert.Equal(Situacoes.AguardandoDistribuicao, demanda.Situacao);
    }

    [Fact]
    public void AbrirDemandaDeveLancarExcecaoPorAtividadeNaoEncontrada()
    {
        // Arrange
        void act() => _demandaCommand.AbrirDemanda(_usuarios[2], 0, "Detalhes...");

        // Act and Assert
        Exception exception = Assert.Throws<EntidadeNaoEncontradaException>(act);
        Assert.Equal("Atividade não encontrada.", exception.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void ConsultarDemanda(int idDemanda)
    {
        // Act
        Demanda demanda = _demandaCommand.ConsultarDemanda(idDemanda);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demanda, _jsonSerializerOptions));

        // Assert
        Assert.Equal(_demandas[idDemanda].Id, demanda.Id);
    }

    [Fact]
    public void ConsultarDemandaDeveLancarExcecaoPorDemandaNaoEncontrada()
    {
        // Arrange
        void act() => _demandaCommand.ConsultarDemanda(0);

        // Act and Assert
        Exception exception = Assert.Throws<EntidadeNaoEncontradaException>(act);
        Assert.Equal("Demanda não encontrada.", exception.Message);
    }

    [Fact]
    public void ListarDemandasDoSolicitante()
    {
        // Act
        IList<Demanda> demandas = _demandaCommand.ListarDemandasDoSolicitante(_usuarios[1]);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandas, _jsonSerializerOptions));

        // Assert
        Assert.Single(demandas);
    }

    [Fact]
    public void ListarDemandasDoSolicitanteDeveRetornarListaVazia()
    {
        // Act
        IList<Demanda> demandas = _demandaCommand.ListarDemandasDoSolicitante(_usuarios[2]);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandas, _jsonSerializerOptions));

        // Assert
        Assert.Empty(demandas);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(3, 3)]
    public void ListarDemandasDoDepartamentoSolicitante(int idUsuario, int qunatidadeDemandas)
    {
        // Act
        IList<Demanda> demandas = _demandaCommand.ListarDemandasDoDepartamentoSolicitante(_usuarios[idUsuario]);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandas, _jsonSerializerOptions));

        // Assert
        Assert.Equal(qunatidadeDemandas, demandas.Count);
    }

    [Fact]
    public void ListarDemandasDoSolucionador()
    {
        // Act
        IList<Demanda> demandas = _demandaCommand.ListarDemandasDoSolucionador(_usuarios[2]);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandas, _jsonSerializerOptions));

        // Assert
        Assert.Single(demandas);
    }

    [Fact]
    public void ListarDemandasDoSolucionadorDeveRetornarListaVazia()
    {
        // Act
        IList<Demanda> demandas = _demandaCommand.ListarDemandasDoSolucionador(_usuarios[1]);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandas, _jsonSerializerOptions));

        // Assert
        Assert.Empty(demandas);
    }

    [Theory]
    [InlineData(1, 4)]
    [InlineData(3, 0)]
    public void ListarDemandasDoDepartamentoSolucionador(int idUsuario, int qunatidadeDemandas)
    {
        // Act
        IList<Demanda> demandas = _demandaCommand.ListarDemandasDoDepartamentoSolucionador(_usuarios[idUsuario]);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandas, _jsonSerializerOptions));

        // Assert
        Assert.Equal(qunatidadeDemandas, demandas.Count);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(3, 2)]
    public void EncaminharDemanda(int idDemanda, int quantidadeDeEventosRegistrados)
    {
        // Act
        _demandaCommand.EncaminharDemanda(_usuarios[1], idDemanda, 1, "EncaminharDemanda.");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(_demandas[idDemanda], _jsonSerializerOptions));

        // Assert
        Assert.Equal(Situacoes.EmAtendimento, _demandas[idDemanda].Situacao);
        Assert.Equal(1, _demandas[idDemanda].UsuarioSolucionadorId);
        Assert.Equal(quantidadeDeEventosRegistrados, _demandas[idDemanda].EventosRegistrados.Count);
    }

    [Fact]
    public void EncaminharDemandaDeveLancarExcecaoPorUsuarioNaoEncontrado()
    {
        // Arrange
        void act() => _demandaCommand.EncaminharDemanda(_usuarios[1], 1, 0, "EncaminharDemanda.");

        // Act and Assert
        Exception exception = Assert.Throws<EntidadeNaoEncontradaException>(act);
        Assert.Equal("Usuário não encontrado.", exception.Message);
    }

    [Fact]
    public void EncaminharDemandaDeveLancarExcecaoPorDemandaNaoEncontrada()
    {
        // Arrange
        void act() => _demandaCommand.EncaminharDemanda(_usuarios[1], 0, 1, "EncaminharDemanda.");

        // Act and Assert
        Exception exception = Assert.Throws<EntidadeNaoEncontradaException>(act);
        Assert.Equal("Demanda não encontrada.", exception.Message);
    }

    [Fact]
    public void CapturarDemanda()
    {
        // Act
        _demandaCommand.CapturarDemanda(_usuarios[1], 3);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(_demandas[3], _jsonSerializerOptions));

        // Assert
        Assert.Equal(Situacoes.EmAtendimento, _demandas[3].Situacao);
        Assert.Equal(1, _demandas[3].UsuarioSolucionadorId);
        Assert.Equal(2, _demandas[3].EventosRegistrados.Count);
    }

    [Fact]
    public void CapturarDemandaDeveLancarExcecaoPorUsuarioNaoEncontrado()
    {
        // Arrange
        Usuario usuario = new()
        {
            Id = 4,
            Matricula = "4000",
            Nome = "Nome do Usuário",
            Departamento = "Departamento",
            EhGestor = false,
        };
        void act() => _demandaCommand.CapturarDemanda(usuario, 1);

        // Act and Assert
        Exception exception = Assert.Throws<EntidadeNaoEncontradaException>(act);
        Assert.Equal("Usuário não encontrado.", exception.Message);
    }

    [Fact]
    public void CapturarDemandaDeveLancarExcecaoPorDemandaNaoEncontrada()
    {
        // Arrange
        void act() => _demandaCommand.CapturarDemanda(_usuarios[1], 0);

        // Act and Assert
        Exception exception = Assert.Throws<EntidadeNaoEncontradaException>(act);
        Assert.Equal("Demanda não encontrada.", exception.Message);
    }

    [Fact]
    public void RejeitarDemanda()
    {
        // Act
        _demandaCommand.CapturarDemanda(_usuarios[1], 3);
        _demandaCommand.RejeitarDemanda(_usuarios[1], 3, "RejeitarDemanda.");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(_demandas[3], _jsonSerializerOptions));

        // Assert
        Assert.Equal(Situacoes.AguardandoDistribuicao, _demandas[3].Situacao);
        Assert.Null(_demandas[3].UsuarioSolucionadorId);
        Assert.Equal(3, _demandas[3].EventosRegistrados.Count);
    }

    [Fact]
    public void RejeitarDemandaDeveLancarExcecaoPorDemandaNaoEncontrada()
    {
        // Arrange
        void act() => _demandaCommand.RejeitarDemanda(_usuarios[1], 0, "RejeitarDemanda.");

        // Act and Assert
        Exception exception = Assert.Throws<EntidadeNaoEncontradaException>(act);
        Assert.Equal("Demanda não encontrada.", exception.Message);
    }

    [Fact]
    public void ResponderDemanda()
    {
        // Act
        _demandaCommand.ResponderDemanda(_usuarios[2], 2, "ResponderDemanda.");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(_demandas[2], _jsonSerializerOptions));

        // Assert
        Assert.Equal(Situacoes.Respondida, _demandas[2].Situacao);
        Assert.Equal(2, _demandas[2].EventosRegistrados.Count);
    }

    [Fact]
    public void ResponderDemandaDeveLancarExcecaoPorDemandaNaoEncontrada()
    {
        // Arrange
        void act() => _demandaCommand.ResponderDemanda(_usuarios[1], 0, "ResponderDemanda.");

        // Act and Assert
        Exception exception = Assert.Throws<EntidadeNaoEncontradaException>(act);
        Assert.Equal("Demanda não encontrada.", exception.Message);
    }

    [Theory]
    [InlineData(1, 4, Situacoes.CanceladaPeloGestor)]
    [InlineData(2, 2, Situacoes.CanceladaPeloSolucionador)]
    [InlineData(3, 3, Situacoes.CanceladaPeloSolicitante)]
    public void CancelarDemanda(int idUsuario, int idDemanda, Situacoes situacao)
    {
        // Act
        _demandaCommand.CancelarDemanda(_usuarios[idUsuario], idDemanda, "CancelarDemanda.");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(_demandas[idDemanda], _jsonSerializerOptions));

        // Assert
        Assert.Equal(situacao, _demandas[idDemanda].Situacao);
    }

    [Fact]
    public void CancelarDemandaDeveLancarExcecaoPorDemandaNaoEncontrada()
    {
        // Arrange
        void act() => _demandaCommand.CancelarDemanda(_usuarios[1], 0, "CancelarDemanda.");

        // Act and Assert
        Exception exception = Assert.Throws<EntidadeNaoEncontradaException>(act);
        Assert.Equal("Demanda não encontrada.", exception.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void ReabrirDemanda(int idUsuario)
    {
        // Act
        _demandaCommand.CapturarDemanda(_usuarios[2], 1);
        _demandaCommand.CancelarDemanda(_usuarios[2], 1, "CancelarDemanda.");
        Demanda demanda = _demandaCommand.ReabrirDemanda(_usuarios[idUsuario], 1, "ReabrirDemanda.");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(_demandas[1], _jsonSerializerOptions));
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demanda, _jsonSerializerOptions));

        // Assert
        Assert.Equal(1, demanda.IdDaDemandaReaberta);
    }

    [Fact]
    public void ReabrirDemandaDeveLancarExcecaoPorDemandaNaoEncontrada()
    {
        // Arrange
        void act() => _demandaCommand.ReabrirDemanda(_usuarios[1], 0, "ReabrirDemanda.");

        // Act and Assert
        Exception exception = Assert.Throws<EntidadeNaoEncontradaException>(act);
        Assert.Equal("Demanda não encontrada.", exception.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void ReativarDemanda(int idUsuario)
    {
        // Act
        _demandaCommand.CapturarDemanda(_usuarios[2], 1);
        _demandaCommand.CancelarDemanda(_usuarios[2], 1, "CancelarDemanda.");
        _demandaCommand.ReativarDemanda(_usuarios[idUsuario], 1, "ReativarDemanda.");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(_demandas[1], _jsonSerializerOptions));

        // Assert
        Assert.Equal(Situacoes.EmAtendimento, _demandas[1].Situacao);
        Assert.Equal(2, _demandas[1].UsuarioSolucionadorId);
    }

    [Fact]
    public void ReativarDemandaDeveLancarExcecaoPorDemandaNaoEncontrada()
    {
        // Arrange
        void act() => _demandaCommand.ReativarDemanda(_usuarios[1], 0, "ReativarDemanda.");

        // Act and Assert
        Exception exception = Assert.Throws<EntidadeNaoEncontradaException>(act);
        Assert.Equal("Demanda não encontrada.", exception.Message);
    }
}
