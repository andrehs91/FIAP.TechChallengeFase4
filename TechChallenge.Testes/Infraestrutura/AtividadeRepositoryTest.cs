using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Moq;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using TechChallenge.Aplicacao.Commands;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;
using TechChallenge.Dominio.Exceptions;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.DTO;
using TechChallenge.Infraestrutura.Cache;
using TechChallenge.Infraestrutura.Data;
using TechChallenge.Infraestrutura.Repositories;
using TechChallenge.Infraestrutura.Settings;
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

    private readonly Usuario _usuario = new()
    {
        Id = 1,
        Matricula = "1000",
        Nome = "Nome do Usuário",
        Departamento = "Departamento",
        EhGestor = false,
    };
    private readonly List<Atividade> _atividades = [
        new()
        {
            Id = 1,
            Nome = "Nome da Primeira Atividade",
            Descricao = "Descrição da Primeira Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        },
        new()
        {
            Id = 2,
            Nome = "Nome da Segunda Atividade",
            Descricao = "Descrição da Segunda Atividade",
            EstahAtiva = false,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Manual,
            Prioridade = Prioridades.Baixa,
            PrazoEstimado = 120,
        },
        new()
        {
            Id = 3,
            Nome = "Nome da Terceira Atividade",
            Descricao = "Descrição da Terceira Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Outro Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Manual,
            Prioridade = Prioridades.Alta,
            PrazoEstimado = 40,
        },
    ];

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
            Nome = "Nome Atividade",
            Descricao = "Descrição Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        });
        AtividadeRepository atividadeRepository = new(_logger, context, _redisCache);

        // Act
        Atividade? atividade = atividadeRepository.BuscarPorId(1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(atividade);
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



    //IList<Atividade> BuscarTodas();
    //IList<Atividade> BuscarAtivas();
    //IList<Atividade> BuscarPorDepartamentoSolucionador(string departamento);
    //void Editar(Atividade atividade);
    //Usuario? IdentificarSolucionadorMenosAtarefado(int atividadeId);






























    //[Theory]
    //[InlineData(TiposDeDistribuicao.Automatica, Prioridades.Critica)]
    //public void CriarAtividade(TiposDeDistribuicao tiposDeDistribuicao, Prioridades prioridade)
    //{
    //    // Arrange
    //    var mockAtividadeRepository = new Mock<IAtividadeRepository>();
    //    mockAtividadeRepository.Setup(m => m.Criar(It.IsAny<Atividade>()));
    //    var mockUsuarioRepository = new Mock<IUsuarioRepository>();
    //    AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);
    //    Usuario usuario = new()
    //    {
    //        Id = 1,
    //        Matricula = "1000",
    //        Nome = "Nome do Usuário",
    //        Departamento = "Departamento",
    //        EhGestor = true,
    //    };
    //    Random random = new();
    //    uint prazoEstimado = (uint)random.Next(1, 259200);
    //    AtividadeDTO atividadeDTO = new()
    //    {
    //        Nome = "Nome da Atividade",
    //        Descricao = "Descrição da Atividade",
    //        EstahAtiva = true,
    //        DepartamentoSolucionador = usuario.Departamento,
    //        TipoDeDistribuicao = tiposDeDistribuicao,
    //        Prioridade = prioridade,
    //        PrazoEstimado = prazoEstimado,
    //    };

    //    // Act
    //    Atividade atividade = atividadeCommand.CriarAtividade(usuario, atividadeDTO);
    //    _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

    //    // Assert
    //    Assert.Equal(atividadeDTO.Nome, atividade.Nome);
    //    Assert.Equal(atividadeDTO.Descricao, atividade.Descricao);
    //    Assert.Equal(atividadeDTO.EstahAtiva, atividade.EstahAtiva);
    //    Assert.Equal(atividadeDTO.DepartamentoSolucionador, atividade.DepartamentoSolucionador);
    //    Assert.Equal(atividadeDTO.TipoDeDistribuicao, atividade.TipoDeDistribuicao);
    //    Assert.Equal(atividadeDTO.Prioridade, atividade.Prioridade);
    //    Assert.Equal(atividadeDTO.PrazoEstimado, atividade.PrazoEstimado);
    //}

    //[Theory]
    //[InlineData(TiposDeDistribuicao.Automatica, Prioridades.Critica)]
    //public void CriarAtividadeDeveLancarExcecaoPorUsuarioNaoAutorizado(
    //    TiposDeDistribuicao tiposDeDistribuicao,
    //    Prioridades prioridade)
    //{
    //    // Arrange
    //    var mockAtividadeRepository = new Mock<IAtividadeRepository>();
    //    mockAtividadeRepository.Setup(m => m.Criar(It.IsAny<Atividade>()));
    //    var mockUsuarioRepository = new Mock<IUsuarioRepository>();
    //    AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);
    //    Random random = new();
    //    uint prazoEstimado = (uint)random.Next(1, 259200);
    //    AtividadeDTO atividadeDTO = new()
    //    {
    //        Nome = "Nome da Atividade",
    //        Descricao = "Descrição da Atividade",
    //        EstahAtiva = true,
    //        DepartamentoSolucionador = _usuario.Departamento,
    //        TipoDeDistribuicao = tiposDeDistribuicao,
    //        Prioridade = prioridade,
    //        PrazoEstimado = prazoEstimado,
    //    };
    //    void act() => atividadeCommand.CriarAtividade(_usuario, atividadeDTO);

    //    // Act and Assert
    //    Exception exception = Assert.Throws<AcaoNaoAutorizadaException>(act);
    //    _testOutputHelper.WriteLine(exception.Message);
    //}
}
