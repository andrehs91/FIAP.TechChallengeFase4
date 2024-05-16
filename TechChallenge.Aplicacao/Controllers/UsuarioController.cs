using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Aplicacao.Commands;
using TechChallenge.Aplicacao.Services;
using TechChallenge.Dominio.Exceptions;
using TechChallenge.DTO;

namespace TechChallenge.Aplicacao.Controllers;

[ApiController]
[Route("[controller]")]
public class UsuarioController(UsuarioCommand comandos, ITokenService tokenService) : BaseController
{
    private readonly UsuarioCommand _comandos = comandos;
    private readonly ITokenService _tokenService = tokenService;

    /// <summary>
    /// Ponto de entrada para autenticação
    /// </summary>
    /// <remarks>
    /// Para utilizar o sistema o usuário deverá estar autenticado.
    /// </remarks>
    [HttpPost("autenticar")]
    [ProducesResponseType(typeof(UsuarioAutenticadoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public IActionResult Autenticar([FromBody] AutenticarDTO autenticarDTO)
    {
        if (!ModelState.IsValid) throw new DadosInvalidosException(ObterErrosDeValidacao());

        var usuario = _comandos.Autenticar(autenticarDTO);

        if (usuario is null) return NotFound(new RespostaDTO(RespostaDTO.TiposDeResposta.Aviso, "Matrícula e/ou senha inválidos."));

        return Ok(new UsuarioAutenticadoDTO(new UsuarioDTO(usuario), _tokenService.GenerateToken(usuario)));
    }

    /// <summary>
    /// Modelo de Leitura: Lista de Usuários
    /// </summary>
    /// <remarks>
    /// Lista os usuários do departamento do usuário autenticado. Acesso restrito aos usuários com a função "Gestor".
    /// </remarks>
    [HttpGet("listar-usuarios-do-departamento")]
    [Authorize(Roles = "Gestor")]
    [ProducesResponseType(typeof(List<UsuarioDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    public ActionResult<List<UsuarioDTO>> ListarUsuariosDoDepartamento()
    {
        return Ok(_comandos.ListarUsuariosDoDepartamento(ObterUsuarioAutenticado()).Select(u => new UsuarioDTO(u)));
    }

    /// <summary>
    /// Comando: Definir Gestor
    /// </summary>
    /// <remarks>
    /// Recebe um objeto com uma lista de ids de usuários que devem ser promovidos a gestor e outra lista de ids de 
    /// usuários que devem ser demovidos da função. É possível enviar apenas uma das listas por requisição.
    /// Acesso restrito aos usuários com a função "Gestor".
    /// </remarks>
    [HttpPost("definir-gestores-do-departamento")]
    [Authorize(Roles = "Gestor")]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status404NotFound)]
    public ActionResult<RespostaDTO> DefinirGestoresDoDepartamento([FromBody] IdsDosUsuariosDTO idsDosUsuariosDTO)
    {
        if (!ModelState.IsValid) throw new DadosInvalidosException(ObterErrosDeValidacao());

        RespostaDTO resposta = _comandos.DefinirGestoresDoDepartamento(ObterUsuarioAutenticado(), idsDosUsuariosDTO);
        if (resposta.Tipo == RespostaDTO.TiposDeResposta.Erro) return BadRequest(resposta);
        if (resposta.Tipo == RespostaDTO.TiposDeResposta.Aviso) return NotFound(resposta);
        return Ok(resposta);
    }
}
