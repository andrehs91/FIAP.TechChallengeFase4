using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Exceptions;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.DTO;
using TechChallenge.Infraestrutura.Exceptions;

namespace TechChallenge.Aplicacao.Commands;

public class AtividadeCommand(
    IAtividadeRepository atividadeRepository,
    IUsuarioRepository usuarioRepository)
{
    private readonly IAtividadeRepository _atividadeRepository = atividadeRepository;
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

    private static void VerificarSeUsuarioEstahAutorizado(Usuario usuario, Atividade atividade)
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

        bool sucesso = _atividadeRepository.Criar(atividade);
        if (sucesso) return atividade;
        else throw new ErroDeInfraestruturaException("Não foi possível criar a atividade.");
    }

    public IList<Atividade> ListarAtividades()
    {
        return _atividadeRepository.BuscarTodas();
    }

    public IList<Atividade> ListarAtividadesAtivas()
    {
        return _atividadeRepository.BuscarAtivas();
    }

    public IList<Atividade> ListarAtividadesPorDepartamentoSolucionador(Usuario usuario)
    {
        return _atividadeRepository.BuscarPorDepartamentoSolucionador(usuario.Departamento);
    }

    public Atividade? ConsultarAtividade(int id)
    {
        return _atividadeRepository.BuscarPorIdComSolucionadores(id);
    }

    public bool EditarAtividade(Usuario usuario, AtividadeDTO atividadeDTO)
    {
        var atividade = _atividadeRepository.BuscarPorId(atividadeDTO.Id);

        if (atividade is null) return false;

        VerificarSeUsuarioEstahAutorizado(usuario, atividade);

        atividade.Nome = atividadeDTO.Nome;
        atividade.Descricao = atividadeDTO.Descricao;
        atividade.EstahAtiva = atividadeDTO.EstahAtiva;
        atividade.DepartamentoSolucionador = atividadeDTO.DepartamentoSolucionador;
        atividade.TipoDeDistribuicao = atividadeDTO.TipoDeDistribuicao;
        atividade.Prioridade = atividadeDTO.Prioridade;
        atividade.PrazoEstimado = atividadeDTO.PrazoEstimado;

        bool sucesso = _atividadeRepository.Editar(atividade);
        if (sucesso) return true;
        else throw new ErroDeInfraestruturaException("Não foi possível editar a atividade.");
    }

    public RespostaDTO DefinirSolucionadores(Usuario usuario, IdsDosUsuariosDTO idsDosUsuariosDTO, int id)
    {
        if (!usuario.EhGestor)
            return new RespostaDTO(RespostaDTO.TiposDeResposta.Erro, "Somente gestores podem definir solucionadores.");

        int quantidadeDeIds =
            idsDosUsuariosDTO.IdsDosUsuariosASeremPromovidos.Count +
            idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos.Count;
        if (quantidadeDeIds == 0)
            return new RespostaDTO(RespostaDTO.TiposDeResposta.Erro, "Nenhum usuário foi informado.");

        var atividade = _atividadeRepository.BuscarPorIdComSolucionadores(id);
        if (atividade is null)
            return new RespostaDTO(RespostaDTO.TiposDeResposta.Aviso, "Atividade não encontrada.");
        if (atividade.DepartamentoSolucionador != usuario.Departamento)
            return new RespostaDTO(RespostaDTO.TiposDeResposta.Erro, "A atividade não é de responsabilidade do teu departamento.");

        int quantidadeUsuariosASeremPromovidos = idsDosUsuariosDTO.IdsDosUsuariosASeremPromovidos.Count;
        if (quantidadeUsuariosASeremPromovidos > 0)
        {
            IList<Usuario> usuariosPromovidos = _usuarioRepository.BuscarPorIds(idsDosUsuariosDTO.IdsDosUsuariosASeremPromovidos);
            if (quantidadeUsuariosASeremPromovidos != usuariosPromovidos.Count)
                return new RespostaDTO(RespostaDTO.TiposDeResposta.Aviso, "Um ou mais usuários a serem promovidos não foram encontrados.");
            if (usuariosPromovidos.Any(u => u.Departamento != usuario.Departamento))
                return new RespostaDTO(RespostaDTO.TiposDeResposta.Erro, "Um ou mais usuários a ser(em) promovido(s) não faz(em) parte do teu departamento.");
            foreach (Usuario usuarioPromovido in usuariosPromovidos)
            {
                if (!atividade.Solucionadores.Contains(usuarioPromovido))
                    atividade.Solucionadores.Add(usuarioPromovido);
            }
        }

        int quantidadeUsuariosASeremDemovidos = idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos.Count;
        if (quantidadeUsuariosASeremDemovidos > 0)
        {
            IList<Usuario> usuariosDemovidos = _usuarioRepository.BuscarPorIds(idsDosUsuariosDTO.IdsDosUsuariosASeremDemovidos);
            if (quantidadeUsuariosASeremDemovidos != usuariosDemovidos.Count)
                return new RespostaDTO(RespostaDTO.TiposDeResposta.Aviso, "Um ou mais usuários a serem demovidos não foram encontrados.");
            if (usuariosDemovidos.Any(u => u.Departamento != usuario.Departamento))
                return new RespostaDTO(RespostaDTO.TiposDeResposta.Erro, "Um ou mais usuários a ser(em) demovido(s) não faz(em) parte do teu departamento.");
            foreach (var usuarioDemovido in from Usuario usuarioDemovido in usuariosDemovidos
                                            where atividade.Solucionadores.Contains(usuarioDemovido)
                                            select usuarioDemovido)
            {
                atividade.Solucionadores.Remove(usuarioDemovido);
            }
        }

        _atividadeRepository.Editar(atividade);
        return new RespostaDTO(RespostaDTO.TiposDeResposta.Sucesso, "Solucionador(es) definido(s) com sucesso.");
    }
}
