using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Aplicacao.Commands;
using TechChallenge.Dominio.Exceptions;
using TechChallenge.Aplicacao.DTO;

namespace TechChallenge.Aplicacao.Controllers;

[ApiController]
[Route("[controller]")]
public class DemandaController(DemandaCommand comandos) : BaseController
{
    private readonly DemandaCommand _comandos = comandos;
    private readonly string _detalhesInvalidos = "O preenchimento dos detalhes é obrigatório e seu conteúdo deve possuir ao menos 3 caracteres.";
    private readonly string _mensagemInvalida = "O preenchimento da mensagem é obrigatório e seu conteúdo deve possuir ao menos 3 caracteres.";

    /// <summary>
    /// Modelo de Leitura: Lista de Demandas do Solicitante
    /// </summary>
    /// <remarks>
    /// Lista as demandas que o usuário autenticado abriu.
    /// </remarks>
    [HttpGet("do-solicitante")]
    [Authorize]
    [ProducesResponseType(typeof(IList<DemandaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    public ActionResult<IList<DemandaDTO>> BuscarDemandasDoSolicitante()
    {
        return Ok(_comandos.ListarDemandasDoSolicitante(ObterUsuarioAutenticado()).Select(demanda => new DemandaDTO(demanda)));
    }

    /// <summary>
    /// Modelo de Leitura: Lista de Demandas do Departamento Solicitante
    /// </summary>
    /// <remarks>
    /// Lista as demandas que os usuários do departamento do usuário autenticado abriram.
    /// </remarks>
    [HttpGet("do-departamento-solicitante")]
    [Authorize]
    [ProducesResponseType(typeof(IList<DemandaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    public ActionResult<IList<DemandaDTO>> BuscarDemandasDoDepartamentoSolicitante()
    {
        return Ok(_comandos.ListarDemandasDoDepartamentoSolicitante(ObterUsuarioAutenticado()).Select(demanda => new DemandaDTO(demanda)));
    }

    /// <summary>
    /// Modelo de Leitura: Lista de Demandas do Solucionador
    /// </summary>
    /// <remarks>
    /// Lista as demandas pelas quais o usuário autenticado é responsável.
    /// </remarks>
    [HttpGet("do-solucionador")]
    [Authorize]
    [ProducesResponseType(typeof(IList<DemandaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    public ActionResult<IList<DemandaDTO>> BuscarDemandasDoSolucionador()
    {
        return Ok(_comandos.ListarDemandasDoSolucionador(ObterUsuarioAutenticado()).Select(demanda => new DemandaDTO(demanda)));
    }

    /// <summary>
    /// Modelo de Leitura: Lista de Demandas do Departamento Solucionador
    /// </summary>
    /// <remarks>
    /// Lista as demandas pelas quais o departamento do usuário autenticado é responsável.
    /// </remarks>
    [HttpGet("do-departamento-solucionador")]
    [Authorize]
    [ProducesResponseType(typeof(IList<DemandaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    public ActionResult<IList<DemandaDTO>> BuscarDemandasDoDepartamentoSolucionador()
    {
        return Ok(_comandos.ListarDemandasDoDepartamentoSolucionador(ObterUsuarioAutenticado()).Select(demanda => new DemandaDTO(demanda)));
    }

    /// <summary>
    /// Comando: Consultar Demanda
    /// </summary>
    /// <remarks>
    /// Exibe os dados de uma demanda específica.
    /// </remarks>
    /// <param name="id">Identificador da demanda</param>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(DemandaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult<DemandaDTO> BuscarDemanda(int id)
    {
        return Ok(new DemandaDTO(_comandos.ConsultarDemanda(id)));
    }

    /// <summary>
    /// Comando: Abrir Demanda
    /// </summary>
    /// <remarks>
    /// Abre uma demanda com base nos dados enviados e nos dados do usuário autenticado.
    /// </remarks>
    /// <param name="id">Identificador da atividade</param>
    /// <param name="detalhes">Descrição detalhada da demanda</param>
    [HttpPost("abrir")]
    [Authorize]
    [ProducesResponseType(typeof(DemandaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult<DemandaDTO> AbrirDemanda([FromQuery] int id, [FromBody] TextoDTO detalhes)
    {
        if (!ModelState.IsValid) throw new DadosInvalidosException(_detalhesInvalidos);
        var demanda = _comandos.AbrirDemanda(ObterUsuarioAutenticado(), id, detalhes.Conteudo);
        return Ok(new DemandaDTO(demanda));
    }

    /// <summary>
    /// Comando: Encaminhar Demanda
    /// </summary>
    /// <remarks>
    /// Encaminha uma demanda que esteja na situação "Aguardando Distribuição" ou "Em Atendimento".
    /// O novo solucionador deverá pertencer ao departamento solucionador e o ator deverá ser o
    /// solucionador atual da demanda ou um gestor do departamento solucionador.
    /// </remarks>
    /// <param name="id">Identificador da demanda</param>
    /// <param name="idNovoSolucionador">Identificador do novo solucionador</param>
    /// <param name="mensagem">Justificativa para o encaminhamento</param>
    [HttpPut("{id}/encaminhar")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult EncaminharDemanda(int id, [FromQuery] int idNovoSolucionador, [FromBody] TextoDTO mensagem)
    {
        if (!ModelState.IsValid) throw new DadosInvalidosException(_mensagemInvalida);
        _comandos.EncaminharDemanda(ObterUsuarioAutenticado(), id, idNovoSolucionador, mensagem.Conteudo);
        return NoContent();
    }

    /// <summary>
    /// Comando: Capturar Demanda
    /// </summary>
    /// <remarks>
    /// Captura uma demanda que esteja esteja na situação "Aguardando Distribuição" ou "Em Atendimento".
    /// O ator deverá pertencer ao departamento solucionador e não poderá ser o solucionador atual da demanda.
    /// </remarks>
    /// <param name="id">Identificador da demanda</param>
    [HttpPut("{id}/capturar")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult CapturarDemanda(int id)
    {
        _comandos.CapturarDemanda(ObterUsuarioAutenticado(), id);
        return NoContent();
    }

    /// <summary>
    /// Comando: Rejeitar Demanda
    /// </summary>
    /// <remarks>
    /// Rejeita uma demanda que esteja na situação "Em Atendimento" e a altera para a situação "Aguardando Distribuição".
    /// O ator deverá ser o solucionador da demanda.
    /// </remarks>
    /// <param name="id">Identificador da demanda</param>
    /// <param name="mensagem">Justificativa para a rejeição</param>
    [HttpPut("{id}/rejeitar")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult RejeitarDemanda(int id, [FromBody] TextoDTO mensagem)
    {
        if (!ModelState.IsValid) throw new DadosInvalidosException(_mensagemInvalida);
        _comandos.RejeitarDemanda(ObterUsuarioAutenticado(), id, mensagem.Conteudo);
        return NoContent();
    }

    /// <summary>
    /// Comando: Responder Demanda
    /// </summary>
    /// <remarks>
    /// Responde uma demanda que esteja na situação "Em Atendimento" e a altera para a situação "Respondida".
    /// O ator deverá ser o solucionador da demanda.
    /// </remarks>
    /// <param name="id">Identificador da demanda</param>
    /// <param name="mensagem">Resposta</param>
    [HttpPut("{id}/responder")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult ResponderDemanda(int id, [FromBody] TextoDTO mensagem)
    {
        if (!ModelState.IsValid) throw new DadosInvalidosException(_mensagemInvalida);
        _comandos.ResponderDemanda(ObterUsuarioAutenticado(), id, mensagem.Conteudo);
        return NoContent();
    }

    /// <summary>
    /// Comando: Cancelar Demanda
    /// </summary>
    /// <remarks>
    /// Cancela uma demanda que esteja na situação "Aguardando Distribuição" ou "Em Atendimento".
    /// O ator deverá ser o solicitante da demanda ou o solucionador da demanda ou um gestor do departamento solucionador.
    /// </remarks>
    /// <param name="id">Identificador da demanda</param>
    /// <param name="mensagem">Justificativa para o cancelamento</param>
    [HttpPut("{id}/cancelar")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult CancelarDemanda(int id, [FromBody] TextoDTO mensagem)
    {
        if (!ModelState.IsValid) throw new DadosInvalidosException(_mensagemInvalida);
        _comandos.CancelarDemanda(ObterUsuarioAutenticado(), id, mensagem.Conteudo);
        return NoContent();
    }

    /// <summary>
    /// Comando: Reabrir Demanda
    /// </summary>
    /// <remarks>
    /// Reabre uma demanda que esteja na situação "Respondida" ou "Cancelada pelo Solucionador" ou "Cancelada pelo Gestor".
    /// O ator deverá pertencer ao departamento solicitante da demanda.
    /// </remarks>
    /// <param name="id">Identificador da demanda</param>
    /// <param name="mensagem">Justificativa para a reabertura</param>
    [HttpPut("{id}/reabrir")]
    [Authorize]
    [ProducesResponseType(typeof(DemandaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult<DemandaDTO> ReabrirDemanda(int id, [FromBody] TextoDTO mensagem)
    {
        if (!ModelState.IsValid) throw new DadosInvalidosException(_mensagemInvalida);
        var demanda = _comandos.ReabrirDemanda(ObterUsuarioAutenticado(), id, mensagem.Conteudo);
        return Ok(new DemandaDTO(demanda));
    }

    /// <summary>
    /// Comando: Reativar Demanda
    /// </summary>
    /// <remarks>
    /// Reativa uma demanda que esteja na situação "Respondida" ou "Cancelada pelo Solucionador" ou "Cancelada pelo Gestor".
    /// O ator deverá ser o solucionador atual da demanda ou um gestor do departamento solucionador.
    /// </remarks>
    /// <param name="id">Identificador da demanda</param>
    /// <param name="mensagem">Justificativa para a reabertura</param>
    [HttpPut("{id}/reativar")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult<DemandaDTO> ReativarDemanda(int id, [FromBody] TextoDTO mensagem)
    {
        if (!ModelState.IsValid) throw new DadosInvalidosException(_mensagemInvalida);
        _comandos.ReativarDemanda(ObterUsuarioAutenticado(), id, mensagem.Conteudo);
        return NoContent();
    }
}
