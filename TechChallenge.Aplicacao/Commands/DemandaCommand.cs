using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Exceptions;
using TechChallenge.Dominio.Interfaces;

namespace TechChallenge.Aplicacao.Commands;

public class DemandaCommand(
    IAtividadeRepository repositorioDeAtividades,
    IDemandaRepository repositorioDeDemandas,
    IUsuarioRepository repositorioDeUsuarios)
{
    private readonly IAtividadeRepository _repositorioDeAtividades = repositorioDeAtividades;
    private readonly IDemandaRepository _repositorioDeDemandas = repositorioDeDemandas;
    private readonly IUsuarioRepository _repositorioDeUsuarios = repositorioDeUsuarios;

    public Demanda AbrirDemanda(Usuario solicitante, int id, string detalhes)
    {
        var atividade = _repositorioDeAtividades.BuscarPorId(id)
            ?? throw new EntidadeNaoEncontradaException("Atividade não encontrada.");
        Demanda demanda = Demanda.Abrir(atividade, solicitante, detalhes);
        _repositorioDeDemandas.Criar(demanda);
        return demanda;
    }

    public Demanda ConsultarDemanda(int id)
    {
        return _repositorioDeDemandas.BuscarPorId(id)
            ?? throw new EntidadeNaoEncontradaException("Demanda não encontrada.");
    }

    internal IList<Demanda> ListarDemandasDoSolicitante(Usuario usuario)
    {
        return _repositorioDeDemandas.BuscarPorSolicitante(usuario.Id);
    }

    internal IList<Demanda> ListarDemandasDoDepartamentoSolicitante(Usuario usuario)
    {
        return _repositorioDeDemandas.BuscarPorDepartamentoSolicitante(usuario.Departamento);
    }

    internal IList<Demanda> ListarDemandasDoSolucionador(Usuario usuario)
    {
        return _repositorioDeDemandas.BuscarPorSolucionador(usuario.Id);
    }

    internal IList<Demanda> ListarDemandasDoDepartamentoSolucionador(Usuario usuario)
    {
        return _repositorioDeDemandas.BuscarPorDepartamentoSolucionador(usuario.Departamento);
    }

    public void EncaminharDemanda(Usuario ator, int id, int idNovoSolucionador, string mensagem)
    {
        var novoSolucionador = _repositorioDeUsuarios.BuscarPorId(idNovoSolucionador)
            ?? throw new EntidadeNaoEncontradaException("Usuário não encontrado.");
        var demanda = ConsultarDemanda(id);
        demanda.Encaminhar(ator, novoSolucionador, mensagem);
        _repositorioDeDemandas.Editar(demanda);
    }

    public void CapturarDemanda(Usuario novoSolucionador, int id)
    {
        novoSolucionador = _repositorioDeUsuarios.BuscarPorId(novoSolucionador.Id)
            ?? throw new EntidadeNaoEncontradaException("Usuário não encontrado.");
        var demanda = ConsultarDemanda(id);
        demanda.Capturar(novoSolucionador);
        _repositorioDeDemandas.Editar(demanda);
    }

    public void RejeitarDemanda(Usuario ator, int id, string mensagem)
    {
        var demanda = ConsultarDemanda(id);
        demanda.Rejeitar(ator, mensagem);
        _repositorioDeDemandas.Editar(demanda);
    }

    public void ResponderDemanda(Usuario ator, int id, string mensagem)
    {
        var demanda = ConsultarDemanda(id);
        demanda.Responder(ator, mensagem);
        _repositorioDeDemandas.Editar(demanda);
    }

    public void CancelarDemanda(Usuario ator, int id, string mensagem)
    {
        var demanda = ConsultarDemanda(id);
        demanda.Cancelar(ator, mensagem);
        _repositorioDeDemandas.Editar(demanda);
    }

    public Demanda ReabrirDemanda(Usuario solicitante, int id, string mensagem)
    {
        var demanda = ConsultarDemanda(id);
        Demanda novaDemanda = demanda.Reabrir(solicitante, mensagem);
        _repositorioDeDemandas.Criar(novaDemanda);
        return novaDemanda;
    }

    public void ReativarDemanda(Usuario ator, int id, string mensagem)
    {
        var demanda = ConsultarDemanda(id);
        demanda.Reativar(ator, mensagem);
        _repositorioDeDemandas.Editar(demanda);
    }
}
