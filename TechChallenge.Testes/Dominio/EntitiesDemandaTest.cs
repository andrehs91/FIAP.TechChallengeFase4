using System.Text.Json;
using System.Text.Json.Serialization;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;
using TechChallenge.Dominio.Exceptions;
using Xunit.Abstractions;

namespace TechChallenge.Testes.Dominio;

public class EntitiesDemandaTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    private readonly Atividade _atividade;
    private readonly Usuario _gestor;
    private readonly Usuario _solicitante;
    private readonly Usuario _solucionador;

    private void LoggarEventosRegistrados(Demanda demanda)
    {
        _testOutputHelper.WriteLine(
            JsonSerializer.Serialize(
                demanda.EventosRegistrados.Select(er => new
                {
                    Situacao = er.Situacao.ToString(),
                    er.MomentoInicial,
                    er.MomentoFinal,
                    er.Mensagem,
                    Solucionador = er.UsuarioSolucionador?.Id ?? null,
                }).ToList(),
                _jsonSerializerOptions
            )
        );
    }

    public EntitiesDemandaTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _atividade = new Atividade(
            "Nome da atividade",
            "Descrição da atividade.",
            true,
            "Departamento Solucionador",
            TiposDeDistribuicao.Automatica,
            Prioridades.MuitoAlta,
            1);
        _gestor = new Usuario()
        {
            Id = 10,
            Matricula = "1000",
            Nome = "Nome do Gestor",
            Departamento = "Departamento Solucionador",
            EhGestor = true,
        };
        _solicitante = new Usuario()
        {
            Id = 20,
            Matricula = "2000",
            Nome = "Nome do Solicitante",
            Departamento = "Departamento Solicitante",
            EhGestor = false,
        };
        _solucionador = new Usuario()
        {
            Id = 30,
            Matricula = "3000",
            Nome = "Nome do Solucionador",
            Departamento = "Departamento Solucionador",
            EhGestor = false,
        };
    }

    [Theory]
    [Trait("Expectativa", "Sucesso")]
    [InlineData(false, Situacoes.AguardandoDistribuicao)]
    [InlineData(true, Situacoes.EmAtendimento)]
    public void AbrirDemandaEDefinirSolucionador(bool solucionadorDefinido, Situacoes situacao)
    {
        // Arrange and Act
        string detalhes = "Detalhes...";
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            detalhes
        );
        if (solucionadorDefinido)
            demanda.DefinirSolucionador(_solucionador);
        LoggarEventosRegistrados(demanda);

        // Assert
        Assert.Equal(_atividade.Id, demanda.AtividadeId);
        Assert.Null(demanda.IdDaDemandaReaberta);
        Assert.Null(demanda.MomentoDeFechamento);
        Assert.Equal(situacao, demanda.Situacao);
        Assert.Equal(_solicitante.Departamento, demanda.DepartamentoSolicitante);
        Assert.Equal(_solicitante.Id, demanda.UsuarioSolicitanteId);
        Assert.Equal(_atividade.DepartamentoSolucionador, demanda.DepartamentoSolucionador);
        Assert.Equal(detalhes, demanda.Detalhes);
        if (solucionadorDefinido)
        {
            Assert.Equal(2, demanda.EventosRegistrados.Count);
            Assert.Equal(situacao, demanda.EventosRegistrados[1].Situacao);
            Assert.Equal(_solucionador, demanda.EventosRegistrados[1].UsuarioSolucionador);
        }
        else
        {
            Assert.Single(demanda.EventosRegistrados);
            Assert.Equal(situacao, demanda.EventosRegistrados[0].Situacao);
        }
    }

    [Theory]
    [Trait("Expectativa", "Sucesso")]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void DefinirSolucionadorDeveRetornarValorEsperado(bool solucionadorDefinido, bool valorEsperado)
    {
        // Arrange and Act
        string detalhes = "Detalhes...";
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            detalhes
        );
        if (solucionadorDefinido)
            demanda.DefinirSolucionador(_solucionador);
        bool retorno = demanda.DefinirSolucionador(_solucionador);
        LoggarEventosRegistrados(demanda);

        // Assert
        Assert.Equal(valorEsperado, retorno);
    }

    [Theory]
    [Trait("Expectativa", "Sucesso")]
    [InlineData(false, "gestor")]
    [InlineData(true, "gestor")]
    [InlineData(true, "solucionador")]
    public void EncaminharDemanda(bool solucionadorDefinido, string usuario)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        if (solucionadorDefinido)
            demanda.DefinirSolucionador(_solucionador);
        Usuario ator = null!;
        if (usuario == "gestor") ator = _gestor;
        if (usuario == "solucionador") ator = _solucionador;
        int novoSolucionadorId = 40;
        Usuario novoSolucionador = new()
        {
            Id = novoSolucionadorId,
            Matricula = "4000",
            Nome = "Nome do Solucionador",
            Departamento = "Departamento Solucionador",
            EhGestor = false,
        };

        // Act
        demanda.Encaminhar(ator, novoSolucionador, "Mensagem...");
        LoggarEventosRegistrados(demanda);

        // Assert
        Assert.Equal(Situacoes.EmAtendimento, demanda.Situacao);
        Assert.Equal(novoSolucionadorId, demanda.UsuarioSolucionadorId);
        if (solucionadorDefinido) Assert.Equal(3, demanda.EventosRegistrados.Count);
        else Assert.Equal(2, demanda.EventosRegistrados.Count);
    }

    [Fact]
    [Trait("Expectativa", "Exceção")]
    public void EncaminharDemandaDeveLancarExcecaoPorDemandaNaoEstarAtiva()
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        demanda.Situacao = Situacoes.Respondida;
        void act() => demanda.Encaminhar(_gestor, _solucionador, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<DemandaNaoEstahAtivaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Theory]
    [Trait("Expectativa", "Exceção")]
    [InlineData(false, 40, "4000", "Nome do Gestor", "Outro Departamento", true)]               // Gestor de outro departamento
    [InlineData(false, 50, "5000", "Nome do Solucionador", "Outro Departamento", false)]        // Solucionador de outro departamento
    [InlineData(false, 60, "6000", "Nome do Solucionador", "Departamento Solucionador", false)] // Solucionador da demanda
    [InlineData(true, 40, "4000", "Nome do Gestor", "Outro Departamento", true)]                // Gestor de outro departamento
    [InlineData(true, 50, "5000", "Nome do Solucionador", "Outro Departamento", false)]         // Solucionador de outro departamento
    [InlineData(true, 60, "6000", "Nome do Solucionador", "Departamento Solucionador", false)]  // Solucionador da demanda
    public void EncaminharDemandaDeveLancarExcecaoPorAtorNaoAutorizado(
        bool solucionadorDefinido,
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        if (solucionadorDefinido)
            demanda.DefinirSolucionador(_solucionador);
        Usuario ator = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };
        void act() => demanda.Encaminhar(ator, _solucionador, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<AtorNaoAutorizadoAEncaminharDemandaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Fact]
    [Trait("Expectativa", "Exceção")]
    public void EncaminharDemandaDeveLancarExcecaoPorNovoSolucionadorJahSerOSolucionador()
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        void act() => demanda.Encaminhar(_solucionador, _solucionador, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<UsuarioJahEhOSolucionadorException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Theory]
    [Trait("Expectativa", "Sucesso")]
    [InlineData(false, 40, "4000", "Nome do Novo Solucionador", "Departamento Solucionador", false)]
    [InlineData(true, 40, "4000", "Nome do Novo Solucionador", "Departamento Solucionador", false)]
    public void CapturarDemanda(
        bool solucionadorDefinido,
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        if (solucionadorDefinido)
            demanda.DefinirSolucionador(_solucionador);
        Usuario novoSolucionador = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };

        // Act
        demanda.Capturar(novoSolucionador);
        LoggarEventosRegistrados(demanda);

        // Assert
        Assert.Equal(Situacoes.EmAtendimento, demanda.Situacao);
        Assert.Equal(id, demanda.UsuarioSolucionadorId);
        if (solucionadorDefinido) Assert.Equal(3, demanda.EventosRegistrados.Count);
        else Assert.Equal(2, demanda.EventosRegistrados.Count);
    }

    [Fact]
    [Trait("Expectativa", "Exceção")]
    public void CapturarDemandaDeveLancarExcecaoPorNovoSolucionadorJahSerOSolucionador()
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        void act() => demanda.Capturar(_solucionador);

        // Act and Assert
        Exception exception = Assert.Throws<UsuarioJahEhOSolucionadorException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Theory]
    [Trait("Expectativa", "Exceção")]
    [InlineData(false)]
    [InlineData(true)]
    public void CapturarDemandaDeveLancarExcecaoPorAtorNaoAutorizado(bool solucionadorDefinido)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        if (solucionadorDefinido)
            demanda.DefinirSolucionador(_solucionador);
        Usuario novoSolucionador = new()
        {
            Id = 40,
            Matricula = "4000",
            Nome = "Nome do Solucionador de Outro Departamento",
            Departamento = "Outro Departamento",
            EhGestor = false,
        };
        void act() => demanda.Capturar(novoSolucionador);

        // Act and Assert
        Exception exception = Assert.Throws<AtorNaoAutorizadoACapturarDemandaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Fact]
    [Trait("Expectativa", "Sucesso")]
    public void RejeitarDemanda()
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);

        // Act
        demanda.Rejeitar(_solucionador, "Mensagem...");
        LoggarEventosRegistrados(demanda);

        // Assert
        Assert.Equal(Situacoes.AguardandoDistribuicao, demanda.Situacao);
        Assert.Null(demanda.UsuarioSolucionadorId);
        Assert.Equal(3, demanda.EventosRegistrados.Count);
    }

    [Theory]
    [Trait("Expectativa", "Exceção")]
    [InlineData(40, "4000", "Nome do Gestor", "Departamento Solucionador", false)]       // Gestor do departamento solucionador
    [InlineData(50, "5000", "Nome do Solucionador", "Departamento Solucionador", false)] // Solucionador da demanda
    [InlineData(60, "6000", "Nome do Gestor", "Outro Departamento", false)]              // Gestor de outro departamento
    [InlineData(70, "7000", "Nome do Solucionador", "Outro Departamento", false)]        // Solucionador de outro departamento
    public void RejeitarDemandaDeveLancarExcecaoPorAtorNaoSerOSolucionador(
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        Usuario ator = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };
        void act() => demanda.Rejeitar(ator, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<AtorNaoAutorizadoARejeitarDemandaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Fact]
    [Trait("Expectativa", "Sucesso")]
    public void ResponderDemanda()
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);

        // Act
        demanda.Responder(_solucionador, "Mensagem...");
        LoggarEventosRegistrados(demanda);

        // Assert
        Assert.Equal(Situacoes.Respondida, demanda.Situacao);
        Assert.Equal(2, demanda.EventosRegistrados.Count);
    }

    [Fact]
    [Trait("Expectativa", "Exceção")]
    public void ResponderDemandaDeveLancarExcecaoPorDemandaNaoEstarEmAtendimento()
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        demanda.Responder(_solucionador, "Mensagem...");
        void act() => demanda.Responder(_solucionador, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<DemandaNaoEstahEmAtendimentoException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Theory]
    [Trait("Expectativa", "Exceção")]
    [InlineData(40, "4000", "Nome do Gestor", "Departamento Solucionador", false)]       // Gestor do departamento solucionador
    [InlineData(50, "5000", "Nome do Solucionador", "Departamento Solucionador", false)] // Solucionador da demanda
    [InlineData(60, "6000", "Nome do Gestor", "Outro Departamento", false)]              // Gestor de outro departamento
    [InlineData(70, "7000", "Nome do Solucionador", "Outro Departamento", false)]        // Solucionador de outro departamento
    public void ResponderDemandaDeveLancarExcecaoPorAtorNaoSerOSolucionador(
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        Usuario ator = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };
        void act() => demanda.Responder(ator, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<AtorNaoAutorizadoAResponderDemandaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Theory]
    [Trait("Expectativa", "Sucesso")]
    [InlineData("gestor")]
    [InlineData("solicitante")]
    [InlineData("solucionador")]
    public void CancelarDemanda(string usuario)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        Usuario ator = _solicitante;
        if (usuario == "solucionador") ator = _solucionador;
        if (usuario == "gestor") ator = _gestor;

        // Act
        demanda.Cancelar(ator, "Mensagem...");
        LoggarEventosRegistrados(demanda);

        // Assert
        if (usuario == "solicitante")
            Assert.Equal(Situacoes.CanceladaPeloSolicitante, demanda.Situacao);
        if (usuario == "solucionador")
            Assert.Equal(Situacoes.CanceladaPeloSolucionador, demanda.Situacao);
        if (usuario == "gestor")
            Assert.Equal(Situacoes.CanceladaPeloGestor, demanda.Situacao);
        Assert.Equal(2, demanda.EventosRegistrados.Count);
    }

    [Theory]
    [Trait("Expectativa", "Exceção")]
    [InlineData(40, "4000", "Nome do Solucionador", "Departamento Solucionador", false)] // Solucionador da demanda
    [InlineData(50, "5000", "Nome do Gestor", "Outro Departamento", false)]              // Gestor de outro departamento
    [InlineData(60, "6000", "Nome do Solucionador", "Outro Departamento", false)]        // Solucionador de outro departamento
    public void CancelarDemandaDeveLancarExcecaoPorAtorNaoEstarAutorizado(
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        Usuario ator = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };
        void act() => demanda.Cancelar(ator, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<AtorNaoAutorizadoACancelarDemandaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Theory]
    [Trait("Expectativa", "Sucesso")]
    [InlineData(false, 20, "2000", "Nome do Gestor", "Departamento Solicitante", true)]
    [InlineData(false, 20, "2000", "Nome do Solicitante", "Departamento Solicitante", false)]
    [InlineData(false, 40, "4000", "Outro Solicitante", "Departamento Solicitante", false)]
    [InlineData(true, 20, "2000", "Nome do Gestor", "Departamento Solicitante", true)]
    [InlineData(true, 20, "2000", "Nome do Solicitante", "Departamento Solicitante", false)]
    [InlineData(true, 40, "4000", "Outro Solicitante", "Departamento Solicitante", false)]
    public void ReabrirDemanda(
        bool solucionadorDefinido,
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        demanda.Responder(_solucionador, "Mensagem...");
        Usuario ator = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };

        // Act
        Demanda novaDemanda = demanda.Reabrir(ator, "Mensagem...");
        if (solucionadorDefinido)
            novaDemanda.DefinirSolucionador(_solucionador);
        LoggarEventosRegistrados(novaDemanda);

        // Assert
        Assert.Equal(demanda.Id, novaDemanda.IdDaDemandaReaberta);
        Assert.Equal(demanda.AtividadeId, novaDemanda.AtividadeId);
        Assert.Null(novaDemanda.MomentoDeFechamento);
        Assert.Equal(demanda.DepartamentoSolicitante, novaDemanda.DepartamentoSolicitante);
        Assert.Equal(ator.Id, novaDemanda.UsuarioSolicitanteId);
        Assert.Equal(demanda.DepartamentoSolucionador, novaDemanda.DepartamentoSolucionador);
        Assert.NotEqual("Detalhes...", novaDemanda.Detalhes);
        Assert.NotEqual("Mensagem...", novaDemanda.Detalhes);
        Assert.Contains("Mensagem...", novaDemanda.Detalhes);
        if (solucionadorDefinido) Assert.Equal(2, novaDemanda.EventosRegistrados.Count);
        else Assert.Single(novaDemanda.EventosRegistrados);
    }

    [Theory]
    [Trait("Expectativa", "Exceção")]
    [InlineData(false, 20, "2000", "Nome do Gestor", "Departamento Solicitante", true)]
    [InlineData(false, 20, "2000", "Nome do Solicitante", "Departamento Solicitante", false)]
    [InlineData(false, 40, "4000", "Outro Solicitante", "Departamento Solicitante", false)]
    [InlineData(true, 20, "2000", "Nome do Gestor", "Departamento Solicitante", true)]
    [InlineData(true, 20, "2000", "Nome do Solicitante", "Departamento Solicitante", false)]
    [InlineData(true, 40, "4000", "Outro Solicitante", "Departamento Solicitante", false)]
    public void ReabrirDemandaDeveLancarExcecaoPorDemandaCanceladaPeloSolicitante(
        bool solucionadorDefinido,
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        if (solucionadorDefinido)
            demanda.DefinirSolucionador(_solucionador);
        demanda.Cancelar(_solicitante, "Mensagem...");
        Usuario ator = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };
        void act() => demanda.Reabrir(ator, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<DemandaCanceladaPeloSolicitanteException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Theory]
    [Trait("Expectativa", "Exceção")]
    [InlineData(false, 20, "2000", "Nome do Gestor", "Departamento Solicitante", true)]
    [InlineData(false, 20, "2000", "Nome do Solicitante", "Departamento Solicitante", false)]
    [InlineData(false, 40, "4000", "Outro Solicitante", "Departamento Solicitante", false)]
    [InlineData(true, 20, "2000", "Nome do Gestor", "Departamento Solicitante", true)]
    [InlineData(true, 20, "2000", "Nome do Solicitante", "Departamento Solicitante", false)]
    [InlineData(true, 40, "4000", "Outro Solicitante", "Departamento Solicitante", false)]
    public void ReabrirDemandaDeveLancarExcecaoPorDemandaNaoEstarRespondidaOuCancelada(
        bool solucionadorDefinido,
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        if (solucionadorDefinido)
            demanda.DefinirSolucionador(_solucionador);
        Usuario ator = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };
        void act() => demanda.Reabrir(ator, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<DemandaNaoEstahRespondidaOuCanceladaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Theory]
    [Trait("Expectativa", "Exceção")]
    [InlineData(20, "2000", "Nome do Gestor", "Outro Departamento", true)]
    [InlineData(20, "2000", "Nome do Solicitante", "Outro Departamento", false)]
    [InlineData(40, "4000", "Outro Solicitante", "Outro Departamento", false)]
    public void ReabrirDemandaDeveLancarExcecaoPorAtorNaoSerDoDepartamentoSolicitante(
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        demanda.Responder(_solucionador, "Mensagem...");
        Usuario ator = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };
        void act() => demanda.Reabrir(ator, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<AtorNaoAutorizadoAReabrirDemandaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Theory]
    [Trait("Expectativa", "Sucesso")]
    [InlineData(false, Situacoes.CanceladaPeloGestor, "gestor")]
    [InlineData(true, Situacoes.CanceladaPeloGestor, "gestor")]
    [InlineData(true, Situacoes.CanceladaPeloGestor, "solucionador")]
    [InlineData(true, Situacoes.CanceladaPeloSolucionador, "gestor")]
    [InlineData(true, Situacoes.CanceladaPeloSolucionador, "solucionador")]
    [InlineData(true, Situacoes.Respondida, "gestor")]
    [InlineData(true, Situacoes.Respondida, "solucionador")]
    public void ReativarDemanda(
        bool solucionadorDefinido,
        Situacoes situacaoAtual,
        string usuario)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        if (solucionadorDefinido)
            demanda.DefinirSolucionador(_solucionador);
        if (situacaoAtual == Situacoes.Respondida)
            demanda.Responder(_solucionador, "Mensagem...");
        if (situacaoAtual == Situacoes.CanceladaPeloSolucionador)
            demanda.Cancelar(_solucionador, "Mensagem...");
        if (situacaoAtual == Situacoes.CanceladaPeloGestor)
            demanda.Cancelar(_gestor, "Mensagem...");

        Usuario ator = null!;
        if (usuario == "gestor") ator = _gestor;
        if (usuario == "solucionador") ator = _solucionador;

        // Act
        demanda.Reativar(ator, "Mensagem...");
        LoggarEventosRegistrados(demanda);

        // Assert
        Assert.Null(demanda.MomentoDeFechamento);
        if (solucionadorDefinido) Assert.Equal(3, demanda.EventosRegistrados.Count);
        else Assert.Equal(2, demanda.EventosRegistrados.Count);
    }

    [Theory]
    [Trait("Expectativa", "Exceção")]
    [InlineData(20, "2000", "Nome do Gestor", "Departamento Solicitante", true)]
    [InlineData(20, "2000", "Nome do Solicitante", "Departamento Solicitante", false)]
    [InlineData(40, "4000", "Outro Solicitante", "Departamento Solicitante", false)]
    public void ReativarDemandaDeveLancarExcecaoPorDemandaCanceladaPeloSolicitante(
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        demanda.Cancelar(_solicitante, "Mensagem...");
        Usuario ator = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };
        void act() => demanda.Reativar(ator, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<DemandaCanceladaPeloSolicitanteException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Theory]
    [Trait("Expectativa", "Exceção")]
    [InlineData(10, "1000", "Nome do Gestor", "Departamento Solucionador", true)]        // Gestor do departamento solucionador
    [InlineData(30, "3000", "Nome do Solucionador", "Departamento Solucionador", false)] // Solucionador da demanda
    [InlineData(40, "4000", "Nome do Gestor", "Outro Departamento", true)]               // Gestor de outro departamento
    [InlineData(50, "5000", "Nome do Solucionador", "Outro Departamento", false)]        // Solucionador de outro departamento
    public void ReativarDemandaDeveLancarExcecaoPorDemandaNaoEstarRespondidaOuCancelada(
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        Usuario ator = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };
        void act() => demanda.Reativar(ator, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<DemandaNaoEstahRespondidaOuCanceladaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Theory]
    [Trait("Expectativa", "Exceção")]
    [InlineData(40, "4000", "Nome do Gestor", "Outro Departamento", true)]               // Gestor de outro departamento
    [InlineData(50, "5000", "Nome do Solucionador", "Outro Departamento", false)]        // Solucionador de outro departamento
    [InlineData(60, "6000", "Nome do Solucionador", "Departamento Solucionador", false)] // Solucionador da demanda
    public void ReativarDemandaDeveLancarExcecaoPorAtorNaoAutorizado(
        int id,
        string matricula,
        string nome,
        string departamento,
        bool ehGestor)
    {
        // Arrange
        Demanda demanda = Demanda.Abrir(
            _atividade,
            _solicitante,
            "Detalhes..."
        );
        demanda.DefinirSolucionador(_solucionador);
        demanda.Responder(_solucionador, "Mensagem...");
        Usuario ator = new()
        {
            Id = id,
            Matricula = matricula,
            Nome = nome,
            Departamento = departamento,
            EhGestor = ehGestor,
        };
        void act() => demanda.Reativar(ator, "Mensagem...");

        // Act and Assert
        Exception exception = Assert.Throws<AtorNaoAutorizadoAReativarDemandaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }
}
