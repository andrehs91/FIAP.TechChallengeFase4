using Moq;
using System.Text.Json;
using System.Text.Json.Serialization;
using TechChallenge.Aplicacao.Commands;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;
using TechChallenge.Dominio.Exceptions;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.Aplicacao.DTO;
using TechChallenge.Infraestrutura.Exceptions;
using Xunit.Abstractions;

namespace TechChallenge.Testes.Aplicacao;

public class AtividadeCommandTest(ITestOutputHelper testOutputHelper)
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    private readonly Usuario _usuario = new()
    {
        Id = 1,
        Matricula = "1000",
        Nome = "Nome do Usuário",
        Departamento = "Departamento",
        EhGestor = false,
    };
    private readonly Usuario _gestor = new()
    {
        Id = 2,
        Matricula = "2000",
        Nome = "Nome do Gestor",
        Departamento = "Departamento",
        EhGestor = true,
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
    private readonly AtividadeDTO _atividadeEditadaDTO = new()
    {
        Nome = "Nome da Atividade Editada",
        Descricao = "Descrição da Atividade Editada",
        EstahAtiva = true,
        DepartamentoSolucionador = "Departamento",
        TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
        Prioridade = Prioridades.Media,
        PrazoEstimado = 60,
    };

    [Theory]
    [InlineData(TiposDeDistribuicao.Automatica, Prioridades.Critica)]
    [InlineData(TiposDeDistribuicao.Automatica, Prioridades.MuitoAlta)]
    [InlineData(TiposDeDistribuicao.Automatica, Prioridades.Alta)]
    [InlineData(TiposDeDistribuicao.Automatica, Prioridades.Media)]
    [InlineData(TiposDeDistribuicao.Automatica, Prioridades.Baixa)]
    [InlineData(TiposDeDistribuicao.Manual, Prioridades.Critica)]
    [InlineData(TiposDeDistribuicao.Manual, Prioridades.MuitoAlta)]
    [InlineData(TiposDeDistribuicao.Manual, Prioridades.Alta)]
    [InlineData(TiposDeDistribuicao.Manual, Prioridades.Media)]
    [InlineData(TiposDeDistribuicao.Manual, Prioridades.Baixa)]
    public void CriarAtividade(TiposDeDistribuicao tiposDeDistribuicao, Prioridades prioridade)
    {
        // Arrange
        Random random = new();
        uint prazoEstimado = (uint)random.Next(1, 259200);
        AtividadeDTO atividadeDTO = new()
        {
            Nome = "Nome da Atividade",
            Descricao = "Descrição da Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = _gestor.Departamento,
            TipoDeDistribuicao = tiposDeDistribuicao,
            Prioridade = prioridade,
            PrazoEstimado = prazoEstimado,
        };

        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.Criar(It.IsAny<Atividade>())).Returns(true);
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act
        Atividade atividade = atividadeCommand.CriarAtividade(_gestor, atividadeDTO);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.Equal(atividadeDTO.Nome, atividade.Nome);
        Assert.Equal(atividadeDTO.Descricao, atividade.Descricao);
        Assert.Equal(atividadeDTO.EstahAtiva, atividade.EstahAtiva);
        Assert.Equal(atividadeDTO.DepartamentoSolucionador, atividade.DepartamentoSolucionador);
        Assert.Equal(atividadeDTO.TipoDeDistribuicao, atividade.TipoDeDistribuicao);
        Assert.Equal(atividadeDTO.Prioridade, atividade.Prioridade);
        Assert.Equal(atividadeDTO.PrazoEstimado, atividade.PrazoEstimado);
    }

    [Theory]
    [InlineData(TiposDeDistribuicao.Automatica, Prioridades.Critica)]
    [InlineData(TiposDeDistribuicao.Automatica, Prioridades.MuitoAlta)]
    [InlineData(TiposDeDistribuicao.Automatica, Prioridades.Alta)]
    [InlineData(TiposDeDistribuicao.Automatica, Prioridades.Media)]
    [InlineData(TiposDeDistribuicao.Automatica, Prioridades.Baixa)]
    [InlineData(TiposDeDistribuicao.Manual, Prioridades.Critica)]
    [InlineData(TiposDeDistribuicao.Manual, Prioridades.MuitoAlta)]
    [InlineData(TiposDeDistribuicao.Manual, Prioridades.Alta)]
    [InlineData(TiposDeDistribuicao.Manual, Prioridades.Media)]
    [InlineData(TiposDeDistribuicao.Manual, Prioridades.Baixa)]
    public void CriarAtividadeDeveLancarExcecaoPorUsuarioNaoAutorizado(
        TiposDeDistribuicao tiposDeDistribuicao,
        Prioridades prioridade)
    {
        // Arrange
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);
        Random random = new();
        uint prazoEstimado = (uint)random.Next(1, 259200);
        AtividadeDTO atividadeDTO = new()
        {
            Nome = "Nome da Atividade",
            Descricao = "Descrição da Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = _usuario.Departamento,
            TipoDeDistribuicao = tiposDeDistribuicao,
            Prioridade = prioridade,
            PrazoEstimado = prazoEstimado,
        };
        void act() => atividadeCommand.CriarAtividade(_usuario, atividadeDTO);

        // Act and Assert
        Exception exception = Assert.Throws<AcaoNaoAutorizadaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Fact]
    public void ListarAtividades()
    {
        // Arrange
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarTodas()).Returns(_atividades);
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act
        IList<Atividade> atividades = atividadeCommand.ListarAtividades();
        foreach (var atividade in atividades)
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.Equal(3, atividades.Count);
    }

    [Fact]
    public void ListarAtividadesDeveRetornarListaVazia()
    {
        // Arrange
        List<Atividade> listaVazia = [];
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarTodas()).Returns(listaVazia);
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act
        IList<Atividade> atividades = atividadeCommand.ListarAtividades();
        foreach (var atividade in atividades)
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.Empty(atividades);
    }

    [Fact]
    public void ListarAtividadesAtivas()
    {
        // Arrange
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarAtivas()).Returns(_atividades.FindAll(a => a.EstahAtiva));
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act
        IList<Atividade> atividades = atividadeCommand.ListarAtividadesAtivas();
        foreach (var atividade in atividades)
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.Equal(2, atividades.Count);
    }

    [Fact]
    public void ListarAtividadesAtivasDeveRetornarListaVazia()
    {
        // Arrange
        List<Atividade> listaVazia = [];
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarAtivas()).Returns(listaVazia);
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act
        IList<Atividade> atividades = atividadeCommand.ListarAtividadesAtivas();
        foreach (var atividade in atividades)
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.Empty(atividades);
    }

    [Theory]
    [InlineData("Departamento")]
    [InlineData("Outro Departamento")]
    public void ListarAtividadesPorDepartamentoSolucionador(string departamento)
    {
        // Arrange
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorDepartamentoSolucionador(It.Is<string>(d => d == "Departamento")))
            .Returns(_atividades.FindAll(a => a.DepartamentoSolucionador == "Departamento"));
        mockAtividadeRepository.Setup(m => m.BuscarPorDepartamentoSolucionador(It.Is<string>(d => d == "Outro Departamento")))
            .Returns(_atividades.FindAll(a => a.DepartamentoSolucionador == "Outro Departamento"));
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);
        Usuario usuario = new()
        {
            Id = 1,
            Matricula = "1000",
            Nome = "Nome do Usuário",
            Departamento = departamento,
            EhGestor = false,
        };

        // Act
        IList<Atividade> atividades = atividadeCommand.ListarAtividadesPorDepartamentoSolucionador(usuario);
        foreach (var atividade in atividades)
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        if (departamento == "Departamento") Assert.Equal(2, atividades.Count);
        if (departamento == "Outro Departamento") Assert.Single(atividades);
    }

    [Fact]
    public void ListarAtividadesPorDepartamentoSolucionadorDeveRetornarListaVazia()
    {
        // Arrange
        List<Atividade> listaVazia = [];
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorDepartamentoSolucionador(It.IsAny<string>())).Returns(listaVazia);
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act
        IList<Atividade> atividades = atividadeCommand.ListarAtividadesPorDepartamentoSolucionador(_usuario);
        foreach (var atividade in atividades)
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.Empty(atividades);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ConsultarAtividade(int idAtividade)
    {
        // Arrange
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorIdComSolucionadores(It.IsAny<int>()))
            .Returns(_atividades.Find(a => a.Id == idAtividade));
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act
        Atividade? atividade = atividadeCommand.ConsultarAtividade(idAtividade);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.NotNull(atividade);
        Assert.Equal(idAtividade, atividade.Id);
    }

    [Fact]
    public void ConsultarAtividadeDeveRetornarNulo()
    {
        // Arrange
        int idAtividade = 0;
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorIdComSolucionadores(It.IsAny<int>()))
            .Returns(_atividades.Find(a => a.Id == idAtividade));
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act
        Atividade? atividade = atividadeCommand.ConsultarAtividade(idAtividade);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        // Assert
        Assert.Null(atividade);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void EditarAtividadeDeveRetornarVerdadeiro(int idAtividade)
    {
        // Arrange
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorId(It.IsAny<int>()))
            .Returns(_atividades.Find(a => a.Id == idAtividade));
        mockAtividadeRepository.Setup(m => m.Editar(It.IsAny<Atividade>())).Returns(true);
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act
        bool resultado = atividadeCommand.EditarAtividade(_gestor, _atividadeEditadaDTO);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void EditarAtividadeDeveRetornarFalsoPorAtividadeNaoEncontrada()
    {
        // Arrange
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorId(It.IsAny<int>())).Returns(() => null);
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act
        bool resultado = atividadeCommand.EditarAtividade(_gestor, _atividadeEditadaDTO);

        // Assert
        Assert.False(resultado);
    }

    [Theory]
    [InlineData("Departamento", false)]
    [InlineData("Outro Departamento", false)]
    [InlineData("Outro Departamento", true)]
    public void EditarAtividadeDeveLancarExcecaoPorUsuarioNaoAutorizado(
        string departamento,
        bool ehGestor)
    {
        // Arrange
        int idAtividade = 1;
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorId(It.IsAny<int>()))
            .Returns(_atividades.Find(a => a.Id == idAtividade));
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);
        Usuario usuario = new()
        {
            Id = 1,
            Matricula = "1000",
            Nome = "Nome do Usuário",
            Departamento = departamento,
            EhGestor = ehGestor,
        };
        void act() => atividadeCommand.EditarAtividade(usuario, _atividadeEditadaDTO);

        // Act and Assert
        Exception exception = Assert.Throws<AcaoNaoAutorizadaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Fact]
    public void EditarAtividadeDeveLancarExcecaoPorErroDeInfraestrutura()
    {
        // Arrange
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorId(It.IsAny<int>()))
            .Returns(_atividades.Find(a => a.Id == 1));
        mockAtividadeRepository.Setup(m => m.Editar(It.IsAny<Atividade>())).Returns(false);
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);
        void act() => atividadeCommand.EditarAtividade(_gestor, _atividadeEditadaDTO);

        // Act and Assert
        Exception exception = Assert.Throws<ErroDeInfraestruturaException>(act);
        _testOutputHelper.WriteLine(exception.Message);
    }

    [Fact]
    public void DefinirSolucionadores()
    {
        // Arrange
        Dictionary<int, Usuario> usuarios = new()
        {
            {
                1,
                new()
                {
                    Id = 1,
                    Matricula = "1000",
                    Nome = "Nome do Gestor",
                    Departamento = "Departamento",
                    EhGestor = true,
                }
            },
            {
                2,
                new()
                {
                    Id = 2,
                    Matricula = "2000",
                    Nome = "Nome do Solucionador",
                    Departamento = "Departamento",
                    EhGestor = false,
                }
            },
            {
                3,
                new()
                {
                    Id = 3,
                    Matricula = "3000",
                    Nome = "Nome do Outro Solucionador",
                    Departamento = "Departamento",
                    EhGestor = false,
                }
            }
        };

        Atividade atividade = new()
        {
            Id = 1,
            Nome = "Nome da Primeira Atividade",
            Descricao = "Descrição da Primeira Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        };
        atividade.Solucionadores.Add(usuarios[2]);
        atividade.Solucionadores.Add(usuarios[3]);
        List<Atividade> atividades = [atividade];

        int idAtividade = 1;
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorIdComSolucionadores(It.Is<int>(id => id == idAtividade)))
            .Returns(atividades.Find(a => a.Id == idAtividade));
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        mockUsuarioRepository.Setup(m => m.BuscarPorIds(It.Is<IList<int>>(ids => ids.Count == 1 && ids.Contains(1))))
            .Returns([usuarios[1]]);
        mockUsuarioRepository.Setup(m => m.BuscarPorIds(It.Is<IList<int>>(ids => ids.Count == 1 && ids.Contains(2))))
            .Returns([usuarios[2]]);
        mockUsuarioRepository.Setup(m => m.BuscarPorIds(It.Is<IList<int>>(ids => ids.Count == 1 && ids.Contains(3))))
            .Returns([usuarios[3]]);
        mockUsuarioRepository
            .Setup(m => m.BuscarPorIds(It.Is<IList<int>>(ids => ids.Count == 2 && ids.Contains(2) && ids.Contains(3))))
            .Returns([usuarios[2], usuarios[3]]);
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act and Assert 1
        IdsDosUsuariosDTO idsDosUsuariosDTO1 = new();
        idsDosUsuariosDTO1.IdsDosUsuariosASeremDemovidos.Add(2);
        idsDosUsuariosDTO1.IdsDosUsuariosASeremDemovidos.Add(3);
        idsDosUsuariosDTO1.IdsDosUsuariosASeremPromovidos.Add(1);
        RespostaDTO respostaDTO1 = atividadeCommand.DefinirSolucionadores(usuarios[1], idsDosUsuariosDTO1, idAtividade);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO1));
        Assert.Equal(RespostaDTO.TiposDeResposta.Sucesso, respostaDTO1.Tipo);
        Assert.Equal("Solucionador(es) definido(s) com sucesso.", respostaDTO1.Mensagem);
        Assert.Single(atividade.Solucionadores);

        // Act and Assert 2
        IdsDosUsuariosDTO idsDosUsuariosDTO2 = new();
        idsDosUsuariosDTO2.IdsDosUsuariosASeremPromovidos.Add(2);
        idsDosUsuariosDTO2.IdsDosUsuariosASeremPromovidos.Add(3);
        idsDosUsuariosDTO2.IdsDosUsuariosASeremDemovidos.Add(1);
        RespostaDTO respostaDTO2 = atividadeCommand.DefinirSolucionadores(usuarios[1], idsDosUsuariosDTO2, idAtividade);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO2));
        Assert.Equal(RespostaDTO.TiposDeResposta.Sucesso, respostaDTO2.Tipo);
        Assert.Equal("Solucionador(es) definido(s) com sucesso.", respostaDTO2.Mensagem);
        Assert.Equal(2, atividade.Solucionadores.Count);

        // Act and Assert 3
        IdsDosUsuariosDTO idsDosUsuariosDTO3 = new();
        idsDosUsuariosDTO3.IdsDosUsuariosASeremPromovidos.Add(1);
        RespostaDTO respostaDTO3 = atividadeCommand.DefinirSolucionadores(usuarios[1], idsDosUsuariosDTO3, idAtividade);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO3));
        Assert.Equal(RespostaDTO.TiposDeResposta.Sucesso, respostaDTO3.Tipo);
        Assert.Equal("Solucionador(es) definido(s) com sucesso.", respostaDTO3.Mensagem);
        Assert.Equal(3, atividade.Solucionadores.Count);

        // Act and Assert 4
        IdsDosUsuariosDTO idsDosUsuariosDTO4 = new();
        idsDosUsuariosDTO4.IdsDosUsuariosASeremPromovidos.Add(1);
        idsDosUsuariosDTO4.IdsDosUsuariosASeremDemovidos.Add(1);
        RespostaDTO respostaDTO4 = atividadeCommand.DefinirSolucionadores(usuarios[1], idsDosUsuariosDTO4, idAtividade);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO4));
        Assert.Equal(RespostaDTO.TiposDeResposta.Sucesso, respostaDTO4.Tipo);
        Assert.Equal("Solucionador(es) definido(s) com sucesso.", respostaDTO4.Mensagem);
        Assert.Equal(2, atividade.Solucionadores.Count);
    }

    [Fact]
    public void DefinirSolucionadoresDeveRetornarErroPorUsuarioNaoAutorizado()
    {
        // Arrange
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act
        IdsDosUsuariosDTO idsDosUsuariosDTO = new();
        idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos.Add(2);
        idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos.Add(3);
        idsDosUsuariosDTO.IdsDosUsuariosASeremPromovidos.Add(1);
        RespostaDTO respostaDTO = atividadeCommand.DefinirSolucionadores(_usuario, idsDosUsuariosDTO, 1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO));

        // Assert
        Assert.Equal(RespostaDTO.TiposDeResposta.Erro, respostaDTO.Tipo);
        Assert.Equal("Somente gestores podem definir solucionadores.", respostaDTO.Mensagem);
    }

    [Fact]
    public void DefinirSolucionadoresDeveRetornarErroPorNenhumUsuarioTerSidoInformado()
    {
        // Arrange
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);
        Usuario usuario = new()
        {
            Id = 1,
            Matricula = "1000",
            Nome = "Nome do Gestor",
            Departamento = "Departamento",
            EhGestor = true,
        };

        // Act
        IdsDosUsuariosDTO idsDosUsuariosDTO = new();
        RespostaDTO respostaDTO = atividadeCommand.DefinirSolucionadores(usuario, idsDosUsuariosDTO, 1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO));

        // Assert
        Assert.Equal(RespostaDTO.TiposDeResposta.Erro, respostaDTO.Tipo);
        Assert.Equal("Nenhum usuário foi informado.", respostaDTO.Mensagem);
    }

    [Fact]
    public void DefinirSolucionadoresDeveRetornarAvisoPorAtividadeNaoEncontrada()
    {
        // Arrange
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorIdComSolucionadores(It.IsAny<int>())).Returns(() => null);
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);
        Usuario usuario = new()
        {
            Id = 1,
            Matricula = "1000",
            Nome = "Nome do Gestor",
            Departamento = "Departamento",
            EhGestor = true,
        };

        // Act
        IdsDosUsuariosDTO idsDosUsuariosDTO = new();
        idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos.Add(2);
        idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos.Add(3);
        idsDosUsuariosDTO.IdsDosUsuariosASeremPromovidos.Add(1);
        RespostaDTO respostaDTO = atividadeCommand.DefinirSolucionadores(usuario, idsDosUsuariosDTO, 1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO));

        // Assert
        Assert.Equal(RespostaDTO.TiposDeResposta.Aviso, respostaDTO.Tipo);
        Assert.Equal("Atividade não encontrada.", respostaDTO.Mensagem);
    }

    [Fact]
    public void DefinirSolucionadoresDeveRetornarErroPorAtividadeNaoSerDoDepartamentoDoUsuario()
    {
        // Arrange
        Atividade atividade = new()
        {
            Id = 1,
            Nome = "Nome da Primeira Atividade",
            Descricao = "Descrição da Primeira Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Outro Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        };
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorIdComSolucionadores(It.IsAny<int>())).Returns(atividade);
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);
        Usuario usuario = new()
        {
            Id = 1,
            Matricula = "1000",
            Nome = "Nome do Gestor",
            Departamento = "Departamento",
            EhGestor = true,
        };

        // Act
        IdsDosUsuariosDTO idsDosUsuariosDTO = new();
        idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos.Add(2);
        idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos.Add(3);
        idsDosUsuariosDTO.IdsDosUsuariosASeremPromovidos.Add(1);
        RespostaDTO respostaDTO = atividadeCommand.DefinirSolucionadores(usuario, idsDosUsuariosDTO, 1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO));

        // Assert
        Assert.Equal(RespostaDTO.TiposDeResposta.Erro, respostaDTO.Tipo);
        Assert.Equal("A atividade não é de responsabilidade do teu departamento.", respostaDTO.Mensagem);
    }

    [Fact]
    public void DefinirSolucionadoresDeveRetornarErroPorAlgumUsuarioNaoTerSidoEncontrado()
    {
        // Arrange
        Atividade atividade = new()
        {
            Id = 1,
            Nome = "Nome da Primeira Atividade",
            Descricao = "Descrição da Primeira Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        };
        Usuario usuario1 = new()
        {
            Id = 1,
            Matricula = "1000",
            Nome = "Nome do Gestor",
            Departamento = "Departamento",
            EhGestor = true,
        };
        Usuario usuario2 = new()
        {
            Id = 2,
            Matricula = "2000",
            Nome = "Nome do Solucionador",
            Departamento = "Departamento",
            EhGestor = false,
        };
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorIdComSolucionadores(It.IsAny<int>())).Returns(atividade);
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        mockUsuarioRepository.Setup(m => m.BuscarPorIds(It.Is<IList<int>>(ids => ids.Count == 1 && ids.Contains(1))))
            .Returns([usuario1]);
        mockUsuarioRepository.Setup(m => m.BuscarPorIds(It.Is<IList<int>>(ids => ids.Count != 1 || !ids.Contains(1))))
            .Returns([usuario2]);
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act and Assert 1
        IdsDosUsuariosDTO idsDosUsuariosDTO1 = new();
        idsDosUsuariosDTO1.IdsDosUsuariosASeremDemovidos.Add(2);
        idsDosUsuariosDTO1.IdsDosUsuariosASeremDemovidos.Add(3);
        idsDosUsuariosDTO1.IdsDosUsuariosASeremPromovidos.Add(1);
        RespostaDTO respostaDTO1 = atividadeCommand.DefinirSolucionadores(usuario1, idsDosUsuariosDTO1, 1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO1));
        Assert.Equal(RespostaDTO.TiposDeResposta.Aviso, respostaDTO1.Tipo);
        Assert.Equal("Um ou mais usuários a serem demovidos não foram encontrados.", respostaDTO1.Mensagem);

        // Act and Assert 2
        IdsDosUsuariosDTO idsDosUsuariosDTO2 = new();
        idsDosUsuariosDTO2.IdsDosUsuariosASeremPromovidos.Add(2);
        idsDosUsuariosDTO2.IdsDosUsuariosASeremPromovidos.Add(3);
        idsDosUsuariosDTO2.IdsDosUsuariosASeremDemovidos.Add(1);
        RespostaDTO respostaDTO2 = atividadeCommand.DefinirSolucionadores(usuario1, idsDosUsuariosDTO2, 1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO2));
        Assert.Equal(RespostaDTO.TiposDeResposta.Aviso, respostaDTO2.Tipo);
        Assert.Equal("Um ou mais usuários a serem promovidos não foram encontrados.", respostaDTO2.Mensagem);
    }

    [Fact]
    public void DefinirSolucionadoresDeveRetornarErroPorAlgumUsuarioNaoFazerParteDoDepartamentoSolucionador()
    {
        // Arrange
        Dictionary<int, Usuario> usuarios = new()
        {
            {
                1,
                new()
                {
                    Id = 1,
                    Matricula = "1000",
                    Nome = "Nome do Gestor",
                    Departamento = "Departamento",
                    EhGestor = true,
                }
            },
            {
                2,
                new()
                {
                    Id = 2,
                    Matricula = "2000",
                    Nome = "Nome do Solucionador",
                    Departamento = "Departamento",
                    EhGestor = false,
                }
            },
            {
                3,
                new()
                {
                    Id = 3,
                    Matricula = "3000",
                    Nome = "Nome do Outro Solucionador",
                    Departamento = "Outro Departamento",
                    EhGestor = false,
                }
            }
        };

        Atividade atividade = new()
        {
            Id = 1,
            Nome = "Nome da Primeira Atividade",
            Descricao = "Descrição da Primeira Atividade",
            EstahAtiva = true,
            DepartamentoSolucionador = "Departamento",
            TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
            Prioridade = Prioridades.Media,
            PrazoEstimado = 60,
        };
        atividade.Solucionadores.Add(usuarios[2]);
        atividade.Solucionadores.Add(usuarios[3]);
        List<Atividade> atividades = [atividade];

        int idAtividade = 1;
        var mockAtividadeRepository = new Mock<IAtividadeRepository>();
        mockAtividadeRepository.Setup(m => m.BuscarPorIdComSolucionadores(It.Is<int>(id => id == idAtividade)))
            .Returns(atividades.Find(a => a.Id == idAtividade));
        var mockUsuarioRepository = new Mock<IUsuarioRepository>();
        mockUsuarioRepository.Setup(m => m.BuscarPorIds(It.Is<IList<int>>(ids => ids.Count == 1 && ids.Contains(1))))
            .Returns([usuarios[1]]);
        mockUsuarioRepository.Setup(m => m.BuscarPorIds(It.Is<IList<int>>(ids => ids.Count == 1 && ids.Contains(2))))
            .Returns([usuarios[2]]);
        mockUsuarioRepository.Setup(m => m.BuscarPorIds(It.Is<IList<int>>(ids => ids.Count == 1 && ids.Contains(3))))
            .Returns([usuarios[3]]);
        mockUsuarioRepository
            .Setup(m => m.BuscarPorIds(It.Is<IList<int>>(ids => ids.Count == 2 && ids.Contains(2) && ids.Contains(3))))
            .Returns([usuarios[2], usuarios[3]]);
        AtividadeCommand atividadeCommand = new(mockAtividadeRepository.Object, mockUsuarioRepository.Object);

        // Act and Assert 1
        IdsDosUsuariosDTO idsDosUsuariosDTO1 = new();
        idsDosUsuariosDTO1.IdsDosUsuariosASeremDemovidos.Add(2);
        idsDosUsuariosDTO1.IdsDosUsuariosASeremDemovidos.Add(3);
        idsDosUsuariosDTO1.IdsDosUsuariosASeremPromovidos.Add(1);
        RespostaDTO respostaDTO1 = atividadeCommand.DefinirSolucionadores(usuarios[1], idsDosUsuariosDTO1, 1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO1));
        Assert.Equal(RespostaDTO.TiposDeResposta.Erro, respostaDTO1.Tipo);
        Assert.Equal("Um ou mais usuários a ser(em) demovido(s) não faz(em) parte do teu departamento.", respostaDTO1.Mensagem);

        // Act and Assert 2
        IdsDosUsuariosDTO idsDosUsuariosDTO2 = new();
        idsDosUsuariosDTO2.IdsDosUsuariosASeremPromovidos.Add(2);
        idsDosUsuariosDTO2.IdsDosUsuariosASeremPromovidos.Add(3);
        idsDosUsuariosDTO2.IdsDosUsuariosASeremDemovidos.Add(1);
        RespostaDTO respostaDTO2 = atividadeCommand.DefinirSolucionadores(usuarios[1], idsDosUsuariosDTO2, 1);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(respostaDTO2));
        Assert.Equal(RespostaDTO.TiposDeResposta.Erro, respostaDTO2.Tipo);
        Assert.Equal("Um ou mais usuários a ser(em) promovido(s) não faz(em) parte do teu departamento.", respostaDTO2.Mensagem);
    }
}
