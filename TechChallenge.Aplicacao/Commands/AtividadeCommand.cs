using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Exceptions;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.DTO;

namespace TechChallenge.Aplicacao.Commands;

public class AtividadeCommand
{
    private readonly IAtividadeRepository _repositorioDeAtividades;
    private readonly IUsuarioRepository _repositorioDeUsuarios;

    public AtividadeCommand(IAtividadeRepository repositorioDeAtividades, IUsuarioRepository repositorioDeUsuarios)
    {
        _repositorioDeAtividades = repositorioDeAtividades;
        _repositorioDeUsuarios = repositorioDeUsuarios;
    }

    private void VerificarSeUsuarioEstahAutorizado(Usuario usuario, Atividade atividade)
    {
        if (!usuario.EhGestor ||
            usuario.Departamento != atividade.DepartamentoSolucionador)
            throw new AcaoNaoAutorizadaException("Usuário não autorizado.");
    }

    public Atividade CriarAtividade(Usuario usuario, AtividadeDTO atividadeDTO)
    {
        Atividade atividade = new(
            atividadeDTO.Nome,
            atividadeDTO.Descricao,
            atividadeDTO.EstahAtiva,
            atividadeDTO.DepartamentoSolucionador,
            atividadeDTO.TipoDeDistribuicao,
            atividadeDTO.Prioridade,
            atividadeDTO.PrazoEstimado
        );

        VerificarSeUsuarioEstahAutorizado(usuario, atividade);

        _repositorioDeAtividades.Criar(atividade);
        return atividade;
    }

    public IList<Atividade> ListarAtividades()
    {
        return _repositorioDeAtividades.BuscarTodas();
    }

    public IList<Atividade> ListarAtividadesAtivas()
    {
        return _repositorioDeAtividades.BuscarAtivas();
    }

    public IList<Atividade> ListarAtividadesPorDepartamentoSolucionador(Usuario usuario)
    {
        return _repositorioDeAtividades.BuscarPorDepartamentoSolucionador(usuario.Departamento);
    }

    public Atividade? ConsultarAtividade(int id)
    {
        return _repositorioDeAtividades.BuscarPorIdComSolucionadores(id);
    }

    public bool EditarAtividade(Usuario usuario, AtividadeDTO atividadeDTO)
    {
        var atividade = _repositorioDeAtividades.BuscarPorId(atividadeDTO.Id);

        if (atividade is null) return false;

        VerificarSeUsuarioEstahAutorizado(usuario, atividade);

        atividade.Nome = atividadeDTO.Nome;
        atividade.Descricao = atividadeDTO.Descricao;
        atividade.EstahAtiva = atividadeDTO.EstahAtiva;
        atividade.DepartamentoSolucionador = atividadeDTO.DepartamentoSolucionador;
        atividade.TipoDeDistribuicao = atividadeDTO.TipoDeDistribuicao;
        atividade.Prioridade = atividadeDTO.Prioridade;
        atividade.PrazoEstimado = atividadeDTO.PrazoEstimado;

        _repositorioDeAtividades.Editar(atividade);
        return true;
    }

    public bool ApagarAtividade(Usuario usuario, int id)
    {
        var atividade = _repositorioDeAtividades.BuscarPorId(id);

        if (atividade is null) return false;

        VerificarSeUsuarioEstahAutorizado(usuario, atividade);

        _repositorioDeAtividades.Apagar(atividade);
        return true;
    }

    public RespostaDTO DefinirSolucionadores(Usuario usuario, IdsDosUsuariosDTO idsDosUsuariosDTO, int id)
    {
        var atividade = _repositorioDeAtividades.BuscarPorIdComSolucionadores(id);

        if (atividade is null) return new RespostaDTO(RespostaDTO.TiposDeResposta.Aviso, "Atividade não encontrada.");
        if (atividade.DepartamentoSolucionador != usuario.Departamento) return new RespostaDTO(RespostaDTO.TiposDeResposta.Erro, "A atividade não é de responsabilidade do teu departamento.");

        var usuariosPromovidos = _repositorioDeUsuarios.BuscarPorIds(idsDosUsuariosDTO.IdsDosUsuariosASeremPromovidos);
        var usuariosDemovivos = _repositorioDeUsuarios.BuscarPorIds(idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos);

        int quantidadeDeIds = idsDosUsuariosDTO.IdsDosUsuariosASeremPromovidos.Count() + idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos.Count();
        int quantidadeDeUsuarios = usuariosPromovidos.Count() + usuariosDemovivos.Count();

        if (quantidadeDeUsuarios == 0)
            return new RespostaDTO(RespostaDTO.TiposDeResposta.Aviso, "Nenhum usuário foi encontrado.");
        if (quantidadeDeIds != quantidadeDeUsuarios)
            return new RespostaDTO(RespostaDTO.TiposDeResposta.Aviso, "Um ou mais usuários não foram encontrados.");
        if (usuariosPromovidos.Concat(usuariosDemovivos).Any(u => u.Departamento != usuario.Departamento))
            return new RespostaDTO(RespostaDTO.TiposDeResposta.Erro, "Um ou mais usuários não fazem parte do teu departamento.");

        foreach (Usuario usuarioPromovido in usuariosPromovidos)
        {
            if (!atividade.Solucionadores.Contains(usuarioPromovido))
                atividade.Solucionadores.Add(usuarioPromovido);
        }
        foreach (Usuario usuarioDemovivo in usuariosDemovivos)
        {
            if (atividade.Solucionadores.Contains(usuarioDemovivo))
                atividade.Solucionadores.Remove(usuarioDemovivo);
        }

        _repositorioDeAtividades.Editar(atividade);
        return new RespostaDTO(RespostaDTO.TiposDeResposta.Sucesso, "Solucionador(es) definido(s) com sucesso.");
    }
}
