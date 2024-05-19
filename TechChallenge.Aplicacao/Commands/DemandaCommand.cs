using RabbitMQ.Client;
using System.Text.Json;
using TechChallenge.Aplicacao.Configurations;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;
using TechChallenge.Dominio.Exceptions;
using TechChallenge.Dominio.Interfaces;

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
        Demanda demanda = _demandaRepository.Criar(Demanda.Abrir(atividade, solicitante, detalhes));
        if (atividade.TipoDeDistribuicao == TiposDeDistribuicao.Automatica)
            EnviarDemandaParaFilaDefinirSolucionador(demanda.Id);
        return demanda;
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
        _demandaRepository.Editar(demanda);
    }

    public void CapturarDemanda(Usuario novoSolucionador, int idDemanda)
    {
        novoSolucionador = _usuarioRepository.BuscarPorId(novoSolucionador.Id)
            ?? throw new EntidadeNaoEncontradaException("Usuário não encontrado.");
        var demanda = ConsultarDemanda(idDemanda);
        demanda.Capturar(novoSolucionador);
        _demandaRepository.Editar(demanda);
    }

    public void RejeitarDemanda(Usuario ator, int idDemanda, string mensagem)
    {
        var demanda = ConsultarDemanda(idDemanda);
        demanda.Rejeitar(ator, mensagem);
        _demandaRepository.Editar(demanda);
    }

    public void ResponderDemanda(Usuario ator, int idDemanda, string mensagem)
    {
        var demanda = ConsultarDemanda(idDemanda);
        demanda.Responder(ator, mensagem);
        _demandaRepository.Editar(demanda);
    }

    public void CancelarDemanda(Usuario ator, int idDemanda, string mensagem)
    {
        var demanda = ConsultarDemanda(idDemanda);
        demanda.Cancelar(ator, mensagem);
        _demandaRepository.Editar(demanda);
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
        _demandaRepository.Editar(demanda);
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
