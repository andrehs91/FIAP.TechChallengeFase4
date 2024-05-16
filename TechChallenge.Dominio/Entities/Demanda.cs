using TechChallenge.Dominio.Enums;
using TechChallenge.Dominio.Exceptions;

namespace TechChallenge.Dominio.Entities;

public class Demanda
{
    public int AtividadeId { get; set; }
    public Atividade Atividade { get; set; } = null!;
    public int Id { get; set; }
    public virtual List<EventoRegistrado> EventosRegistrados { get; set; } = [];
    public int? IdDaDemandaReaberta { get; set; }
    public DateTime MomentoDeAbertura { get; set; }
    public DateTime? MomentoDeFechamento { get; set; } = null;
    public DateTime Prazo { get; set; }
    public Situacoes Situacao { get; set; }
    public string DepartamentoSolicitante { get; set; } = null!;
    public int UsuarioSolicitanteId { get; set; }
    public Usuario UsuarioSolicitante { get; set; } = null!;
    public string DepartamentoSolucionador { get; set; } = null!;
    public int? UsuarioSolucionadorId { get; set; }
    public Usuario? UsuarioSolucionador { get; set; }
    public string Detalhes { get; set; } = string.Empty;

    public Demanda() { }

    private Demanda(
        Atividade atividade,
        Usuario solicitante,
        string detalhes,
        int? idDaDemandaReaberta = null)
    {
        Atividade = atividade;
        IdDaDemandaReaberta = idDaDemandaReaberta;
        MomentoDeAbertura = DateTime.Now;
        Prazo = DateTime.Now.AddMinutes(atividade.PrazoEstimado);
        Situacao = Situacoes.AguardandoDistribuicao;
        DepartamentoSolicitante = solicitante.Departamento;
        UsuarioSolicitanteId = solicitante.Id;
        DepartamentoSolucionador = atividade.DepartamentoSolucionador;
        Detalhes = detalhes;
        RegistrarEvento();
    }

    private void RegistrarEvento()
    {
        EventosRegistrados.Add(new EventoRegistrado()
        {
            Demanda = this,
            UsuarioSolucionador = UsuarioSolucionador,
            Situacao = Situacao,
            MomentoInicial = DateTime.Now
        });
    }

    private void AtualizarEventoRegistrado(string mensagem)
    {
        EventoRegistrado ultimo = EventosRegistrados
            .OrderBy(er => er.MomentoInicial)
            .Last();
        ultimo.Situacao = Situacao;
        ultimo.MomentoFinal = DateTime.Now;
        ultimo.Mensagem = mensagem;
    }

    private void VerificarSeADemandaEstahAtiva()
    {
        if (Situacao != Situacoes.AguardandoDistribuicao && Situacao != Situacoes.EmAtendimento)
            throw new DemandaNaoEstahAtivaException();
    }

    public static Demanda Abrir(
        Atividade atividade,
        Usuario solicitante,
        string detalhes,
        int? idDaDemandaReaberta = null)
    {
        return new Demanda(atividade, solicitante, detalhes, idDaDemandaReaberta);
    }

    public bool DefinirSolucionador(Usuario solucionador)
    {
        if (Situacao != Situacoes.AguardandoDistribuicao) return false;
        AtualizarEventoRegistrado($"Solucionador definido: ({solucionador.Matricula}) {solucionador.Nome}.");
        UsuarioSolucionadorId = solucionador.Id;
        UsuarioSolucionador = solucionador;
        Situacao = Situacoes.EmAtendimento;
        RegistrarEvento();
        return true;
    }

    public void Encaminhar(Usuario ator, Usuario novoSolucionador, string mensagem)
    {
        VerificarSeADemandaEstahAtiva();

        bool atorEhOSolucionadorDaDemanda = ator.Id == UsuarioSolucionadorId;
        bool atorEhGestorDoDepartamentoSolucionador = ator.EhGestor && ator.Departamento == DepartamentoSolucionador;
        if (!atorEhOSolucionadorDaDemanda && !atorEhGestorDoDepartamentoSolucionador)
            throw new AtorNaoAutorizadoAEncaminharDemandaException();
        if (novoSolucionador.Id == UsuarioSolucionadorId)
            throw new UsuarioJahEhOSolucionadorException();

        Situacao = Situacoes.EncaminhadaPeloSolucionador;
        string complemento = $"Demanda encaminhada para ({novoSolucionador.Matricula}) {novoSolucionador.Nome}.";
        if (atorEhGestorDoDepartamentoSolucionador && !atorEhOSolucionadorDaDemanda)
        {
            Situacao = Situacoes.EncaminhadaPeloGestor;
            complemento += $" Gestor responsável pela ação: ({ator.Matricula}) {ator.Nome}. ";
        }
        AtualizarEventoRegistrado(complemento + mensagem);

        UsuarioSolucionadorId = novoSolucionador.Id;
        UsuarioSolucionador = novoSolucionador;
        Situacao = Situacoes.EmAtendimento;
        RegistrarEvento();
    }

    public void Capturar(Usuario novoSolucionador)
    {
        VerificarSeADemandaEstahAtiva();

        if (novoSolucionador.Id == UsuarioSolucionadorId)
            throw new UsuarioJahEhOSolucionadorException();
        if (novoSolucionador.Departamento != DepartamentoSolucionador)
            throw new AtorNaoAutorizadoACapturarDemandaException();

        Situacao = Situacoes.Capturada;
        AtualizarEventoRegistrado($"Demanda capturada por ({novoSolucionador.Matricula}) {novoSolucionador.Nome}.");

        UsuarioSolucionadorId = novoSolucionador.Id;
        UsuarioSolucionador = novoSolucionador;
        Situacao = Situacoes.EmAtendimento;
        RegistrarEvento();
    }

    public void Rejeitar(Usuario ator, string mensagem)
    {
        VerificarSeADemandaEstahAtiva();

        if (ator.Id != UsuarioSolucionadorId)
            throw new AtorNaoAutorizadoARejeitarDemandaException();

        Situacao = Situacoes.Rejeitada;
        AtualizarEventoRegistrado(mensagem);

        UsuarioSolucionadorId = null;
        UsuarioSolucionador = null;
        Situacao = Situacoes.AguardandoDistribuicao;
        RegistrarEvento();
    }

    public void Responder(Usuario ator, string mensagem)
    {
        if (Situacao != Situacoes.EmAtendimento)
            throw new DemandaNaoEstahEmAtendimentoException();
        if (ator.Id != UsuarioSolucionadorId)
            throw new AtorNaoAutorizadoAResponderDemandaException();

        MomentoDeFechamento = DateTime.Now;
        Situacao = Situacoes.Respondida;
        AtualizarEventoRegistrado(mensagem);
    }

    public void Cancelar(Usuario ator, string mensagem)
    {
        VerificarSeADemandaEstahAtiva();

        if (ator.Id == UsuarioSolicitanteId)
            Situacao = Situacoes.CanceladaPeloSolicitante;
        else if (ator.Id == UsuarioSolucionadorId)
            Situacao = Situacoes.CanceladaPeloSolucionador;
        else if (ator.EhGestor && ator.Departamento == DepartamentoSolucionador)
            Situacao = Situacoes.CanceladaPeloGestor;
        else
            throw new AtorNaoAutorizadoACancelarDemandaException();

        MomentoDeFechamento = DateTime.Now;
        AtualizarEventoRegistrado(mensagem);
    }

    public Demanda Reabrir(Usuario solicitante, string mensagem)
    {
        if (Situacao == Situacoes.CanceladaPeloSolicitante)
            throw new DemandaCanceladaPeloSolicitanteException();
        if (Situacao != Situacoes.Respondida &&
            Situacao != Situacoes.CanceladaPeloSolucionador &&
            Situacao != Situacoes.CanceladaPeloGestor)
            throw new DemandaNaoEstahRespondidaOuCanceladaException();
        if (DepartamentoSolicitante != solicitante.Departamento)
            throw new AtorNaoAutorizadoAReabrirDemandaException();

        string detalhes = $"Esta demanda é a reabertura da demanda {Id}. Motivo da reabertura: {mensagem}. Detalhes da demanda reaberta: {Detalhes}.";
        return new Demanda(Atividade, solicitante, detalhes, Id);
    }

    public void Reativar(Usuario ator, string mensagem)
    {
        if (Situacao == Situacoes.CanceladaPeloSolicitante)
            throw new DemandaCanceladaPeloSolicitanteException();
        if (Situacao != Situacoes.Respondida &&
            Situacao != Situacoes.CanceladaPeloSolucionador &&
            Situacao != Situacoes.CanceladaPeloGestor)
            throw new DemandaNaoEstahRespondidaOuCanceladaException();

        bool atorEhOSolucionadorDaDemanda = ator.Id == UsuarioSolucionadorId;
        bool atorEhGestorDoDepartamentoSolucionador = ator.EhGestor && ator.Departamento == DepartamentoSolucionador;
        if (!atorEhOSolucionadorDaDemanda && !atorEhGestorDoDepartamentoSolucionador)
            throw new AtorNaoAutorizadoAReativarDemandaException();

        string acao = Situacao == Situacoes.Respondida ? "Resposta desfeita." : "Cancelamento desfeito.";
        string complemento = $"{acao} Responsável pela ação: ({ator.Matricula}) {ator.Nome}. ";
        AtualizarEventoRegistrado(complemento + mensagem);

        MomentoDeFechamento = null;
        Situacao = UsuarioSolucionadorId is not null ? Situacoes.EmAtendimento : Situacoes.AguardandoDistribuicao;
        RegistrarEvento();
    }
}
