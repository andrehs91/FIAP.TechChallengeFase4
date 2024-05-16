using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Aplicacao.Commands;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Exceptions;
using TechChallenge.DTO;

namespace TechChallenge.Aplicacao.Controllers;

[ApiController]
[Route("[controller]")]
public class AtividadeController(AtividadeCommand comandos) : BaseController
{
    private readonly AtividadeCommand _comandos = comandos;

    /// <summary>
    /// Modelo de Leitura: Lista de Atividades Ativas
    /// </summary>
    /// <remarks>
    /// Lista as atividades que podem ser abertas.
    /// </remarks>
    [HttpGet("ativas")]
    [Authorize]
    [ProducesResponseType(typeof(IList<AtividadeDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    public ActionResult<IList<AtividadeDTO>> ListarAtividadesAtivas()
    {
        var listaDeAtividadesDTO = _comandos.ListarAtividadesAtivas()
            .Select(atividade => new AtividadeDTO(atividade))
            .ToList();
        return Ok(listaDeAtividadesDTO);
    }

    /// <summary>
    /// Modelo de Leitura: Lista de Atividades do Departamento
    /// </summary>
    /// <remarks>
    /// Lista as atividades, do departamento do usuário autenticado, que podem ser editadas pelo gestor e podem ter seus solucionadores definidos.
    /// </remarks>
    [HttpGet("do-departamento")]
    [Authorize]
    [ProducesResponseType(typeof(IList<AtividadeDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    public ActionResult<IList<AtividadeDTO>> ListarAtividadesDoDepartamento()
    {
        var usuario = ObterUsuarioAutenticado();
        var listaDeAtividadesDTO = _comandos.ListarAtividadesPorDepartamentoSolucionador(usuario)
            .Select(atividade => new AtividadeDTO(atividade))
            .ToList();
        return Ok(listaDeAtividadesDTO);
    }

    /// <summary>
    /// Comando: Consultar uma atividade
    /// </summary>
    /// <remarks>
    /// Consulta uma atividade específica.
    /// </remarks>
    /// <param name="id">Identificador da atividade</param>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(AtividadeComSolucionadoresDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult<AtividadeDTO> ConsultarAtividade(int id)
    {
        return _comandos.ConsultarAtividade(id) is Atividade atividade
            ? Ok(new AtividadeComSolucionadoresDTO(atividade))
            : NotFound(new RespostaDTO(RespostaDTO.TiposDeResposta.Aviso, "Atividade não encontrada."));
    }

    /// <summary>
    /// Comando: Criar Atividade
    /// </summary>
    /// <remarks>
    /// Cria uma atividade com base nos dados enviados e nos dados do usuário autenticado.
    /// Acesso restrito aos usuários com a função "Gestor".
    /// </remarks>
    [HttpPost("criar/")]
    [Authorize(Roles = "Gestor")]
    [ProducesResponseType(typeof(AtividadeDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    public ActionResult<AtividadeDTO> CriarAtividade([FromBody] CriarAtividadeDTO criarAtividadeDTO)
    {
        if (!ModelState.IsValid) throw new DadosInvalidosException(ObterErrosDeValidacao());

        var usuario = ObterUsuarioAutenticado();
        var atividadeDTO = criarAtividadeDTO.ConverterParaAtividadeDTO(usuario.Departamento);

        Atividade atividade = _comandos.CriarAtividade(usuario, atividadeDTO);
        return CreatedAtAction(nameof(ConsultarAtividade), new { id = atividade.Id }, new AtividadeDTO(atividade));
    }

    /// <summary>
    /// Comando: Editar Atividade
    /// </summary>
    /// <remarks>
    /// Edita uma atividade específica com base nos dados enviados e nos dados do usuário autenticado.
    /// Acesso restrito aos usuários com a função "Gestor".
    /// </remarks>
    /// <param name="id">Identificador da atividade</param>
    /// <param name="editarAtividadeDTO">Objeto contendo os novos dados da atividade editada.</param>
    [HttpPut("editar/{id}")]
    [Authorize(Roles = "Gestor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public IActionResult EditarAtividade(int id, [FromBody] EditarAtividadeDTO editarAtividadeDTO)
    {
        if (!ModelState.IsValid) throw new DadosInvalidosException(ObterErrosDeValidacao());

        var usuario = ObterUsuarioAutenticado();
        var atividadeDTO = editarAtividadeDTO.ConverterParaAtividadeDTO(id, usuario.Departamento);

        return _comandos.EditarAtividade(usuario, atividadeDTO)
            ? NoContent()
            : NotFound();
    }

    /// <summary>
    /// Comando: Apagar Atividade
    /// </summary>
    /// <remarks>
    /// Apaga uma atividade específica. Ação não mapeada no Event Storming.
    /// Acesso restrito aos usuários com a função "Gestor".
    /// </remarks>
    /// <param name="id">Identificador da atividade</param>
    [HttpDelete("apagar/{id}")]
    [Authorize(Roles = "Gestor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public IActionResult ApagarAtividade(int id)
    {
        return _comandos.ApagarAtividade(ObterUsuarioAutenticado(), id)
            ? NoContent()
            : NotFound();
    }

    /// <summary>
    /// Comando: Definir Solucionador
    /// </summary>
    /// <remarks>
    /// Recebe um objeto com uma lista de ids de usuários que devem ser promovidos a solucionador e outra lista de ids de usuários
    /// que devem ser demovidos da função. É possível enviar apenas uma das listas por requisição.
    /// Acesso restrito aos usuários com a função "Gestor".
    /// </remarks>
    /// <param name="id">Identificador da atividade</param>
    /// <param name="idsDosUsuariosDTO">Listas de ids dos usuários a serem promovidos e demovidos</param>
    [HttpPost("{id}/definir-solucionadores")]
    [Authorize(Roles = "Gestor")]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult<RespostaDTO> DefinirSolucionadores(int id, [FromBody] IdsDosUsuariosDTO idsDosUsuariosDTO)
    {
        RespostaDTO resposta = _comandos.DefinirSolucionadores(ObterUsuarioAutenticado(), idsDosUsuariosDTO, id);
        if (resposta.Tipo == RespostaDTO.TiposDeResposta.Erro) return BadRequest(resposta);
        if (resposta.Tipo == RespostaDTO.TiposDeResposta.Aviso) return NotFound(resposta);
        return Ok(resposta);
    }
}
