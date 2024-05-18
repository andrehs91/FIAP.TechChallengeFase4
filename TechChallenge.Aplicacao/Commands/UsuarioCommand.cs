using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.DTO;

namespace TechChallenge.Aplicacao.Commands;

public class UsuarioCommand(IUsuarioRepository usuarioRepository)
{
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

    public Usuario? Autenticar(AutenticarDTO autenticarDTO)
    {
        if (autenticarDTO.Senha != "senha") return null;
        return _usuarioRepository.BuscarPorMatricula(autenticarDTO.Matricula);
    }

    public RespostaDTO DefinirGestoresDoDepartamento(Usuario usuario, IdsDosUsuariosDTO idsDosUsuariosDTO)
    {
        var usuariosPromovidos = _usuarioRepository.BuscarPorIds(idsDosUsuariosDTO.IdsDosUsuariosASeremPromovidos);
        var usuariosDemovivos = _usuarioRepository.BuscarPorIds(idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos);

        int quantidadeDeIds = idsDosUsuariosDTO.IdsDosUsuariosASeremPromovidos.Count + idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos.Count;
        int quantidadeDeUsuarios = usuariosPromovidos.Count + usuariosDemovivos.Count;

        if (quantidadeDeUsuarios == 0)
            return new RespostaDTO(RespostaDTO.TiposDeResposta.Aviso, "Nenhum usuário foi encontrado.");
        if (quantidadeDeIds != quantidadeDeUsuarios)
            return new RespostaDTO(RespostaDTO.TiposDeResposta.Aviso, "Um ou mais usuários não foram encontrados.");
        if (usuariosPromovidos.Concat(usuariosDemovivos).Any(u => u.Departamento != usuario.Departamento))
            return new RespostaDTO(RespostaDTO.TiposDeResposta.Erro, "Um ou mais usuários não fazem parte do teu departamento.");

        foreach (Usuario usuarioPromovido in usuariosPromovidos)
            usuarioPromovido.EhGestor = true;
        foreach (Usuario usuarioDemovivo in usuariosDemovivos)
            usuarioDemovivo.EhGestor = false;
        var usuarios = usuariosPromovidos.Concat(usuariosDemovivos).ToList();

        _usuarioRepository.EditarVarios(usuarios);
        return new RespostaDTO(RespostaDTO.TiposDeResposta.Sucesso, "Gestor(es) definido(s) com sucesso.");
    }

    public IList<Usuario> ListarUsuariosDoDepartamento(Usuario usuario)
    {
        return _usuarioRepository.BuscarPorDepartamento(usuario.Departamento);
    }
}
