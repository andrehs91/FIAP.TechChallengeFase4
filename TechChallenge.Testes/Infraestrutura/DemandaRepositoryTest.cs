using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;
using TechChallenge.Infraestrutura.Cache;
using TechChallenge.Infraestrutura.Data;
using TechChallenge.Infraestrutura.Repositories;
using Xunit.Abstractions;

namespace TechChallenge.Testes.Infraestrutura;

public class DemandaRepositoryTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ILogger<DemandaRepository> _logger;
    private readonly IRedisCache _redisCache;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    private readonly Atividade _atividade = new()
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
    private readonly Usuario _usuario = new()
    {
        Id = 1,
        Matricula = "1000",
        Nome = "Nome do Gestor",
        Departamento = "Departamento",
        EhGestor = true,
    };

    public DemandaRepositoryTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        var loggerFactory = new LoggerFactory();
        _logger = loggerFactory.CreateLogger<DemandaRepository>();

        var mockDatabase = new Mock<IDatabase>();
        mockDatabase.Setup(m => m.KeyDelete(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(true);
        mockDatabase.Setup(m => m.StringGet(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(new RedisValue());
        mockDatabase.Setup(m => m.StringSet(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Returns(true);
        var mockRedisCache = new Mock<IRedisCache>();
        mockRedisCache.Setup(m => m.GetDatabase()).Returns(mockDatabase.Object);
        _redisCache = mockRedisCache.Object;
    }

    [Fact]
    public void Criar()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaCriar");
        ApplicationDbContext context = new(optionsBuilder.Options);
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);
        Demanda demanda = Demanda.Abrir(_atividade, _usuario, "Detalhes...");

        // Act
        bool sucesso = demandaRepository.Criar(demanda);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(sucesso, _jsonSerializerOptions));

        // Assert
        Assert.True(sucesso);
        Assert.Single(context.Atividades);
        var demandaSalva = context.Atividades.First();
        Assert.NotNull(demandaSalva);
        Assert.Equal(_atividade.Id, demanda.AtividadeId);
        Assert.Null(demanda.IdDaDemandaReaberta);
        Assert.Null(demanda.MomentoDeFechamento);
        Assert.Equal(Situacoes.AguardandoDistribuicao, demanda.Situacao);
        Assert.Equal("Departamento", demanda.DepartamentoSolicitante);
        Assert.Equal(_usuario.Id, demanda.UsuarioSolicitanteId);
        Assert.Equal(_atividade.DepartamentoSolucionador, demanda.DepartamentoSolucionador);
        Assert.Equal("Detalhes...", demanda.Detalhes);
    }

    [Fact]
    public void BuscarPorId()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarPorId");
        ApplicationDbContext context = new(optionsBuilder.Options);
        Demanda demanda = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda.Id = 1;
        context.Demandas.Add(demanda);
        context.SaveChanges();
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        Demanda? demandaSalva = demandaRepository.BuscarPorId(1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandaSalva, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(demandaSalva);
        Assert.Equal(_atividade.Id, demandaSalva.AtividadeId);
        Assert.Null(demandaSalva.IdDaDemandaReaberta);
        Assert.Null(demandaSalva.MomentoDeFechamento);
        Assert.Equal(Situacoes.AguardandoDistribuicao, demandaSalva.Situacao);
        Assert.Equal("Departamento", demandaSalva.DepartamentoSolicitante);
        Assert.Equal(_usuario.Id, demandaSalva.UsuarioSolicitanteId);
        Assert.Equal(_atividade.DepartamentoSolucionador, demandaSalva.DepartamentoSolucionador);
        Assert.Equal("Detalhes...", demandaSalva.Detalhes);
    }

    [Fact]
    public void BuscarPorIdDeveRetornarVazio()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarPorIdDeveRetornarVazio");
        ApplicationDbContext context = new(optionsBuilder.Options);
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        Demanda? demandaSalva = demandaRepository.BuscarPorId(1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandaSalva, _jsonSerializerOptions));

        // Assert
        Assert.Null(demandaSalva);
    }

    [Fact]
    public void BuscarTodas()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarTodas");
        ApplicationDbContext context = new(optionsBuilder.Options);
        Demanda demanda1 = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda1.Id = 1;
        context.Demandas.Add(demanda1);
        Demanda demanda2 = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda2.Id = 2;
        context.Demandas.Add(demanda2);
        Demanda demanda3 = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda3.Id = 3;
        context.Demandas.Add(demanda3);
        context.SaveChanges();
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        IList<Demanda>? demandasSalvas = demandaRepository.BuscarTodas();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandasSalvas, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(demandasSalvas);
        Assert.Equal(3, demandasSalvas.Count);
    }

    [Fact]
    public void BuscarTodasDeveRetornarListaVazia()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarTodasDeveRetornarListaVazia");
        ApplicationDbContext context = new(optionsBuilder.Options);
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        IList<Demanda>? demandasSalvas = demandaRepository.BuscarTodas();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandasSalvas, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(demandasSalvas);
        Assert.Empty(demandasSalvas);
    }

    [Fact]
    public void BuscarPorSolicitante()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarPorSolicitante");
        ApplicationDbContext context = new(optionsBuilder.Options);
        Demanda demanda1 = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda1.Id = 1;
        context.Demandas.Add(demanda1);
        Demanda demanda2 = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda2.Id = 2;
        context.Demandas.Add(demanda2);
        Demanda demanda3 = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda3.Id = 3;
        context.Demandas.Add(demanda3);
        context.SaveChanges();
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        IList<Demanda>? demandasSalvas = demandaRepository.BuscarPorSolicitante(_usuario.Id);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandasSalvas, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(demandasSalvas);
        Assert.Equal(3, demandasSalvas.Count);
    }

    [Fact]
    public void BuscarPorSolicitanteDeveRetornarListaVazia()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarPorSolicitanteDeveRetornarListaVazia");
        ApplicationDbContext context = new(optionsBuilder.Options);
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        IList<Demanda>? demandasSalvas = demandaRepository.BuscarPorSolicitante(_usuario.Id);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandasSalvas, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(demandasSalvas);
        Assert.Empty(demandasSalvas);
    }

    [Fact]
    public void BuscarPorDepartamentoSolicitante()
    {
        // Arrange
        Usuario solicitante = new()
        {
            Id = 2,
            Matricula = "2000",
            Nome = "Nome do Solicitante",
            Departamento = "Outro Departamento",
            EhGestor = false,
        };
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarPorDepartamentoSolicitante");
        ApplicationDbContext context = new(optionsBuilder.Options);
        Demanda demanda1 = Demanda.Abrir(_atividade, solicitante, "Detalhes...");
        demanda1.Id = 1;
        context.Demandas.Add(demanda1);
        Demanda demanda2 = Demanda.Abrir(_atividade, solicitante, "Detalhes...");
        demanda2.Id = 2;
        context.Demandas.Add(demanda2);
        Demanda demanda3 = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda3.Id = 3;
        context.Demandas.Add(demanda3);
        context.SaveChanges();
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        IList<Demanda>? demandasSalvas = demandaRepository.BuscarPorDepartamentoSolicitante(solicitante.Departamento);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandasSalvas, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(demandasSalvas);
        Assert.Equal(2, demandasSalvas.Count);
    }

    [Fact]
    public void BuscarPorDepartamentoSolicitanteDeveRetornarListaVazia()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarPorDepartamentoSolicitanteDeveRetornarListaVazia");
        ApplicationDbContext context = new(optionsBuilder.Options);
        Demanda demanda = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda.Id = 1;
        context.Demandas.Add(demanda);
        context.SaveChanges();
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        IList<Demanda>? demandasSalvas = demandaRepository.BuscarPorDepartamentoSolicitante("");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandasSalvas, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(demandasSalvas);
        Assert.Empty(demandasSalvas);
    }

    [Fact]
    public void BuscarPorSolucionador()
    {
        // Arrange
        Usuario solucionador = new()
        {
            Id = 2,
            Matricula = "2000",
            Nome = "Nome do Solucionador",
            Departamento = "Departamento",
            EhGestor = false,
        };
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarPorSolucionador");
        ApplicationDbContext context = new(optionsBuilder.Options);
        Demanda demanda1 = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda1.Id = 1;
        demanda1.DefinirSolucionador(solucionador);
        context.Demandas.Add(demanda1);
        Demanda demanda2 = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda2.Id = 2;
        demanda2.DefinirSolucionador(solucionador);
        context.Demandas.Add(demanda2);
        Demanda demanda3 = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda3.Id = 3;
        context.Demandas.Add(demanda3);
        context.SaveChanges();
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        IList<Demanda>? demandasSalvas = demandaRepository.BuscarPorSolucionador(solucionador.Id);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandasSalvas, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(demandasSalvas);
        Assert.Equal(2, demandasSalvas.Count);
    }

    [Fact]
    public void BuscarPorSolucionadorDeveRetornarListaVazia()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarPorSolucionadorDeveRetornarListaVazia");
        ApplicationDbContext context = new(optionsBuilder.Options);
        Demanda demanda = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda.Id = 1;
        context.Demandas.Add(demanda);
        context.SaveChanges();
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        IList<Demanda>? demandasSalvas = demandaRepository.BuscarPorSolucionador(0);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandasSalvas, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(demandasSalvas);
        Assert.Empty(demandasSalvas);
    }

    [Fact]
    public void BuscarPorDepartamentoSolucionador()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarPorDepartamentoSolucionador");
        ApplicationDbContext context = new(optionsBuilder.Options);
        Demanda demanda = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda.Id = 1;
        context.Demandas.Add(demanda);
        context.SaveChanges();
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        IList<Demanda>? demandasSalvas = demandaRepository.BuscarPorDepartamentoSolucionador("Departamento");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandasSalvas, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(demandasSalvas);
        Assert.Single(demandasSalvas);
    }

    [Fact]
    public void BuscarPorDepartamentoSolucionadorDeveRetornarListaVazia()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaBuscarPorDepartamentoSolucionadorDeveRetornarListaVazia");
        ApplicationDbContext context = new(optionsBuilder.Options);
        Demanda demanda = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda.Id = 1;
        context.Demandas.Add(demanda);
        context.SaveChanges();
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);

        // Act
        IList<Demanda>? demandasSalvas = demandaRepository.BuscarPorDepartamentoSolicitante("");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandasSalvas, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(demandasSalvas);
        Assert.Empty(demandasSalvas);
    }

    [Fact]
    public void Editar()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DemandaEditar");
        ApplicationDbContext context = new(optionsBuilder.Options);
        Demanda demanda = Demanda.Abrir(_atividade, _usuario, "Detalhes...");
        demanda.Id = 1;
        context.Demandas.Add(demanda);
        context.SaveChanges();
        DemandaRepository demandaRepository = new(_logger, context, _redisCache);
        Demanda? demandaSalva = demandaRepository.BuscarPorId(1);
        Assert.NotNull(demandaSalva);
        demandaSalva.Situacao = Situacoes.EmAtendimento;
        demandaSalva.Detalhes = "...";

        // Act
        bool sucesso = demandaRepository.Editar(demandaSalva);
        Demanda? demandaEditada = demandaRepository.BuscarPorId(1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(demandaEditada, _jsonSerializerOptions));

        // Assert
        Assert.True(sucesso);
        Assert.NotNull(demandaEditada);
        Assert.Null(demandaEditada.IdDaDemandaReaberta);
        Assert.Null(demandaEditada.MomentoDeFechamento);
        Assert.Equal(Situacoes.EmAtendimento, demandaEditada.Situacao);
        Assert.Equal("Departamento", demandaEditada.DepartamentoSolicitante);
        Assert.Equal(_usuario.Id, demandaEditada.UsuarioSolicitanteId);
        Assert.Equal(_atividade.DepartamentoSolucionador, demandaEditada.DepartamentoSolucionador);
        Assert.Equal("...", demandaEditada.Detalhes);
    }
}
