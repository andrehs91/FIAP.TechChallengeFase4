using RabbitMQ.Client;
using System.Text.Json;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;
using TechChallenge.Dominio.Exceptions;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.Infraestrutura.Exceptions;
using TechChallenge.Infraestrutura.Settings;

namespace TechChallenge.Aplicacao.Commands;

public class DemandaCommand(
    IAppSettings appSettings,
    IAtividadeRepository atividadeRepository,
    IDemandaRepository demandaRepository,
    IUsuarioRepository usuarioRepository)
{
    private readonly IAppSettings _appSettings = appSettings;
    private readonly IAtividadeRepository _atividadeRepository = atividadeRepository;
    private readonly IDemandaRepository _demandaRepository = demandaRepository;
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

    public Demanda AbrirDemanda(Usuario solicitante, int idAtividade, string detalhes)
    {
        var atividade = _atividadeRepository.BuscarPorId(idAtividade)
            ?? throw new EntidadeNaoEncontradaException("Atividade não encontrada.");

        if (!atividade.EstahAtiva)
            throw new AcaoNaoAutorizadaException("A atividade não está ativa.");

        Demanda demanda = Demanda.Abrir(atividade, solicitante, detalhes);
        bool sucesso = _demandaRepository.Criar(demanda);
        if (sucesso)
        {
            if (atividade.TipoDeDistribuicao == TiposDeDistribuicao.Automatica)
                EnviarDemandaParaFilaDefinirSolucionador(demanda.Id);
            return demanda;
        }
        else
        {
            throw new ErroDeInfraestruturaException("Não foi possível abrir a demanda.");
        }
    }

    public Demanda ConsultarDemanda(int idDemanda)
    {
        return _demandaRepository.BuscarPorId(idDemanda)
            ?? throw new EntidadeNaoEncontradaException("Demanda não encontrada.");
    }

    public IList<Demanda> ListarDemandasDoSolicitante(Usuario usuario)
    {
        return _demandaRepository.BuscarPorSolicitante(usuario.Id);
    }

    public IList<Demanda> ListarDemandasDoDepartamentoSolicitante(Usuario usuario)
    {
        return _demandaRepository.BuscarPorDepartamentoSolicitante(usuario.Departamento);
    }

    public IList<Demanda> ListarDemandasDoSolucionador(Usuario usuario)
    {
        return _demandaRepository.BuscarPorSolucionador(usuario.Id);
    }

    public IList<Demanda> ListarDemandasDoDepartamentoSolucionador(Usuario usuario)
    {
        return _demandaRepository.BuscarPorDepartamentoSolucionador(usuario.Departamento);
    }

    public void EncaminharDemanda(Usuario ator, int idDemanda, int idNovoSolucionador, string mensagem)
    {
        var novoSolucionador = _usuarioRepository.BuscarPorId(idNovoSolucionador)
            ?? throw new EntidadeNaoEncontradaException("Usuário não encontrado.");
        var demanda = ConsultarDemanda(idDemanda);
        demanda.Encaminhar(ator, novoSolucionador, mensagem);
        bool sucesso = _demandaRepository.Editar(demanda);
        if (!sucesso) throw new ErroDeInfraestruturaException("Não foi possível encaminhar a demanda.");
    }

    public void CapturarDemanda(Usuario novoSolucionador, int idDemanda)
    {
        novoSolucionador = _usuarioRepository.BuscarPorId(novoSolucionador.Id)
            ?? throw new EntidadeNaoEncontradaException("Usuário não encontrado.");
        var demanda = ConsultarDemanda(idDemanda);
        demanda.Capturar(novoSolucionador);
        bool sucesso = _demandaRepository.Editar(demanda);
        if (!sucesso) throw new ErroDeInfraestruturaException("Não foi possível capturar a demanda.");
    }

    public void RejeitarDemanda(Usuario ator, int idDemanda, string mensagem)
    {
        var demanda = ConsultarDemanda(idDemanda);
        demanda.Rejeitar(ator, mensagem);
        bool sucesso = _demandaRepository.Editar(demanda);
        if (!sucesso) throw new ErroDeInfraestruturaException("Não foi possível rejeitar a demanda.");

        if (demanda.Atividade.TipoDeDistribuicao == TiposDeDistribuicao.Automatica)
            EnviarDemandaParaFilaDefinirSolucionador(demanda.Id);
    }

    public void ResponderDemanda(Usuario ator, int idDemanda, string mensagem)
    {
        var demanda = ConsultarDemanda(idDemanda);
        demanda.Responder(ator, mensagem);
        bool sucesso = _demandaRepository.Editar(demanda);
        if (!sucesso) throw new ErroDeInfraestruturaException("Não foi possível responder a demanda.");
    }

    public void CancelarDemanda(Usuario ator, int idDemanda, string mensagem)
    {
        var demanda = ConsultarDemanda(idDemanda);
        demanda.Cancelar(ator, mensagem);
        bool sucesso = _demandaRepository.Editar(demanda);
        if (!sucesso) throw new ErroDeInfraestruturaException("Não foi possível cancelar a demanda.");
    }

    public Demanda ReabrirDemanda(Usuario solicitante, int idDemanda, string mensagem)
    {
        var demanda = ConsultarDemanda(idDemanda);
        Demanda novaDemanda = demanda.Reabrir(solicitante, mensagem);
        _demandaRepository.Criar(novaDemanda);
        return novaDemanda;
    }

    public void ReativarDemanda(Usuario ator, int idDemanda, string mensagem)
    {
        var demanda = ConsultarDemanda(idDemanda);
        demanda.Reativar(ator, mensagem);
        bool sucesso = _demandaRepository.Editar(demanda);
        if (!sucesso) throw new ErroDeInfraestruturaException("Não foi possível reativar a demanda.");
    }

    private void EnviarDemandaParaFilaDefinirSolucionador(int idDemanda)
    {
        ConnectionFactory factory = new()
        {
            HostName = _appSettings.GetValue("RabbitMQ:HostName"),
            UserName = _appSettings.GetValue("RabbitMQ:UserName"),
            Password = _appSettings.GetValue("RabbitMQ:Password"),
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var basicProperties = channel.CreateBasicProperties();
        basicProperties.Persistent = true;

        channel.BasicPublish(exchange: "fiap.techchallenge",
                             routingKey: "demanda",
                             basicProperties: basicProperties,
                             body: JsonSerializer.SerializeToUtf8Bytes(idDemanda));
    }
}
