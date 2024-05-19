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

public class AtividadeRepositoryTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ILogger<AtividadeRepository> _logger;
    private readonly IRedisCache _redisCache;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };

    public AtividadeRepositoryTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        var loggerFactory = new LoggerFactory();
        _logger = loggerFactory.CreateLogger<AtividadeRepository>();

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
        optionsBuilder.UseInMemoryDatabase("Criar");
        ApplicationDbContext context = new(optionsBuilder.Options);
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        Atividade atividade = atividadeRepository.Criar(new()
        {
            Id = 1,
            Nome = "Nome da Atividade",
            Descricao = "Descrição da Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        });
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.Single(context.Atividades);
        var atividadeSalva = context.Atividades.First();
        Assert.NotNull(atividadeSalva);
        Assert.Equal(atividade.Id, atividadeSalva.Id);
        Assert.Equal(atividade.Nome, atividadeSalva.Nome);
        Assert.Equal(atividade.Descricao, atividadeSalva.Descricao);
        Assert.Equal(atividade.EstahAtiva, atividadeSalva.EstahAtiva);
        Assert.Equal(atividade.DepartamentoSolucionador, atividadeSalva.DepartamentoSolucionador);
        Assert.Equal(atividade.TipoDeDistribuicao, atividadeSalva.TipoDeDistribuicao);
        Assert.Equal(atividade.Prioridade, atividadeSalva.Prioridade);
        Assert.Equal(atividade.PrazoEstimado, atividadeSalva.PrazoEstimado);
    }

    [Fact]
    public void BuscarPorId()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("BuscarPorId");
        ApplicationDbContext context = new(optionsBuilder.Options);
        context.Atividades.Add(new()
        {
            Id = 1,
            Nome = "Nome da Atividade",
            Descricao = "Descrição da Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        });
        context.SaveChanges();
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        Atividade? atividade = atividadeRepository.BuscarPorId(1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(atividade);
        Assert.Equal(1, atividade.Id);
        Assert.Equal("Nome da Atividade", atividade.Nome);
        Assert.Equal("Descrição da Atividade", atividade.Descricao);
        Assert.True(atividade.EstahAtiva);
        Assert.Equal("Departamento", atividade.DepartamentoSolucionador);
        Assert.Equal(TiposDeDistribuicao.Automatica, atividade.TipoDeDistribuicao);
        Assert.Equal(Prioridades.Media, atividade.Prioridade);
        Assert.Equal((uint)60, atividade.PrazoEstimado);
    }

    [Fact]
    public void BuscarPorIdDeveRetornarVazio()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("BuscarPorIdDeveRetornarVazio");
        ApplicationDbContext context = new(optionsBuilder.Options);
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        Atividade? atividade = atividadeRepository.BuscarPorId(1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.Null(atividade);
    }

    [Fact]
    public void BuscarPorIdComSolucionadores()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("BuscarPorIdComSolucionadores");
        ApplicationDbContext context = new(optionsBuilder.Options);
        Atividade atividade = new()
        {
            Id = 1,
            Nome = "Nome da Atividade",
            Descricao = "Descrição da Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        };
        atividade.Solucionadores.Add(new()
        {
            Id = 1,
            Matricula = "1000",
            Nome = "Nome do Usuário",
            Departamento = "Departamento",
            EhGestor = false,
        });
        atividade.Solucionadores.Add(new()
        {
            Id = 2,
            Matricula = "2000",
            Nome = "Nome do Outro Usuário",
            Departamento = "Departamento",
            EhGestor = false,
        });
        context.Atividades.Add(atividade);
        context.SaveChanges();
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        Atividade? atividadeSalva = atividadeRepository.BuscarPorIdComSolucionadores(1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividadeSalva, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(atividadeSalva);
        Assert.Equal(1, atividadeSalva.Id);
        Assert.Equal("Nome da Atividade", atividadeSalva.Nome);
        Assert.Equal("Descrição da Atividade", atividadeSalva.Descricao);
        Assert.True(atividadeSalva.EstahAtiva);
        Assert.Equal("Departamento", atividadeSalva.DepartamentoSolucionador);
        Assert.Equal(TiposDeDistribuicao.Automatica, atividadeSalva.TipoDeDistribuicao);
        Assert.Equal(Prioridades.Media, atividadeSalva.Prioridade);
        Assert.Equal((uint)60, atividadeSalva.PrazoEstimado);
        Assert.Equal(2, atividadeSalva.Solucionadores.Count);
    }

    [Fact]
    public void BuscarPorIdComSolucionadoresDeveRetornarVazio()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("BuscarPorIdComSolucionadoresDeveRetornarVazio");
        ApplicationDbContext context = new(optionsBuilder.Options);
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        Atividade? atividade = atividadeRepository.BuscarPorIdComSolucionadores(1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.Null(atividade);
    }

    [Fact]
    public void BuscarTodas()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("BuscarTodas");
        ApplicationDbContext context = new(optionsBuilder.Options);
        context.Atividades.Add(new()
        {
            Id = 1,
            Nome = "Nome da Atividade",
            Descricao = "Descrição da Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        });
        context.Atividades.Add(new()
        {
            Id = 2,
            Nome = "Nome da Outra Atividade",
            Descricao = "Descrição da Outra Atividade",
            EstahAtiva = false,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Manual,
            Prioridade = Prioridades.Baixa,
            PrazoEstimado = 120,
        });
        context.SaveChanges();
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        IList<Atividade>? atividades = atividadeRepository.BuscarTodas();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividades, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(atividades);
        Assert.Equal(2, atividades.Count);
    }

    [Fact]
    public void BuscarTodasDeveRetornarListaVazia()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("BuscarTodasDeveRetornarListaVazia");
        ApplicationDbContext context = new(optionsBuilder.Options);
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        IList<Atividade>? atividades = atividadeRepository.BuscarTodas();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividades, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(atividades);
        Assert.Empty(atividades);
    }

    [Fact]
    public void BuscarAtivas()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("BuscarAtivas");
        ApplicationDbContext context = new(optionsBuilder.Options);
        context.Atividades.Add(new()
        {
            Id = 1,
            Nome = "Nome da Atividade",
            Descricao = "Descrição da Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        });
        context.Atividades.Add(new()
        {
            Id = 2,
            Nome = "Nome da Outra Atividade",
            Descricao = "Descrição da Outra Atividade",
            EstahAtiva = false,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Manual,
            Prioridade = Prioridades.Baixa,
            PrazoEstimado = 120,
        });
        context.SaveChanges();
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        IList<Atividade>? atividades = atividadeRepository.BuscarAtivas();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividades, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(atividades);
        Assert.Single(atividades);
    }

    [Fact]
    public void BuscarAtivasDeveRetornarListaVazia()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("BuscarAtivasDeveRetornarListaVazia");
        ApplicationDbContext context = new(optionsBuilder.Options);
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        IList<Atividade>? atividades = atividadeRepository.BuscarAtivas();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividades, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(atividades);
        Assert.Empty(atividades);
    }

    [Fact]
    public void BuscarPorDepartamentoSolucionador()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("BuscarPorDepartamentoSolucionador");
        ApplicationDbContext context = new(optionsBuilder.Options);
        context.Atividades.Add(new()
        {
            Id = 1,
            Nome = "Nome da Atividade",
            Descricao = "Descrição da Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        });
        context.Atividades.Add(new()
        {
            Id = 2,
            Nome = "Nome da Outra Atividade",
            Descricao = "Descrição da Outra Atividade",
            EstahAtiva = false,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Manual,
            Prioridade = Prioridades.Baixa,
            PrazoEstimado = 120,
        });
        context.Atividades.Add(new()
        {
            Id = 3,
            Nome = "Nome de mais uma Atividade",
            Descricao = "Descrição de mais uma Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento Qualquer",
            TipoDeDistribuicao = TiposDeDistribuicao.Manual,
            Prioridade = Prioridades.Critica,
            PrazoEstimado = 240,
        });
        context.SaveChanges();
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        IList<Atividade>? atividades = atividadeRepository.BuscarPorDepartamentoSolucionador("Departamento");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividades, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(atividades);
        Assert.Equal(2, atividades.Count);
    }

    [Fact]
    public void BuscarPorDepartamentoSolucionadorDeveRetornarListaVazia()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("BuscarPorDepartamentoSolucionadorDeveRetornarListaVazia");
        ApplicationDbContext context = new(optionsBuilder.Options);
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        IList<Atividade>? atividades = atividadeRepository.BuscarPorDepartamentoSolucionador("Departamento");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividades, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(atividades);
        Assert.Empty(atividades);
    }

    [Fact]
    public void Editar()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("BuscarPorDepartamentoSolucionador");
        ApplicationDbContext context = new(optionsBuilder.Options);
        context.Atividades.Add(new()
        {
            Id = 1,
            Nome = "Nome da Atividade",
            Descricao = "Descrição da Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        });
        context.SaveChanges();
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        Atividade? atividade = atividadeRepository.BuscarPorId(1);
        Assert.NotNull(atividade);
        atividade.Nome = "Novo Nome da Atividade";
        atividade.Descricao = "Nova Descrição da Atividade";
        atividade.EstahAtiva = true;
        atividade.DepartamentoSolucionador = "Departamento";
        atividade.TipoDeDistribuicao = TiposDeDistribuicao.Manual;
        atividade.Prioridade = Prioridades.Critica;
        atividade.PrazoEstimado = 120;
        atividadeRepository.Editar(atividade);
        Atividade? atividadeEditada = atividadeRepository.BuscarPorId(1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividadeEditada, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(atividadeEditada);
        Assert.Equal(atividade.Id, atividadeEditada.Id);
        Assert.Equal(atividade.Nome, atividadeEditada.Nome);
        Assert.Equal(atividade.Descricao, atividadeEditada.Descricao);
        Assert.Equal(atividade.EstahAtiva, atividadeEditada.EstahAtiva);
        Assert.Equal(atividade.DepartamentoSolucionador, atividadeEditada.DepartamentoSolucionador);
        Assert.Equal(atividade.TipoDeDistribuicao, atividadeEditada.TipoDeDistribuicao);
        Assert.Equal(atividade.Prioridade, atividadeEditada.Prioridade);
        Assert.Equal(atividade.PrazoEstimado, atividadeEditada.PrazoEstimado);
    }
}
