using TechChallenge.Dominio.Enums;

namespace TechChallenge.Dominio.Entities;

public class Atividade
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool EstahAtiva { get; set; }
    public string DepartamentoSolucionador { get; set; } = string.Empty;
    public TiposDeDistribuicao TipoDeDistribuicao { get; set; }
    public Prioridades Prioridade { get; set; }
    public uint PrazoEstimado { get; set; }
    public virtual List<Usuario> Solucionadores { get; } = [];
    public virtual List<Demanda> Demandas { get; set; } = [];

    public Atividade() { }

    public Atividade(
        string nome,
        string descricao,
        bool estahAtiva,
        string departamentoSolucionador,
        TiposDeDistribuicao tipoDeDistribuicao,
        Prioridades prioridade,
        uint prazoEstimado)
    {
        Nome = ValidarNome(nome);
        Descricao = ValidarDescricao(descricao);
        EstahAtiva = estahAtiva;
        DepartamentoSolucionador = ValidarDepartamentoSolucionador(departamentoSolucionador);
        TipoDeDistribuicao = tipoDeDistribuicao;
        Prioridade = prioridade;
        PrazoEstimado = ValidarPrazoEstimado(prazoEstimado);
    }

    private static string ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentNullException(nameof(nome), "A propriedade Atividade.Nome não pode ser nula, vazia ou composta apenas por espaços.");
        if (nome.Length < 5 || nome.Length > 50)
            throw new ArgumentException("A propriedade Atividade.Nome deve conter entre 5 e 50 caracteres.", nameof(nome));
        return nome;
    }

    private static string ValidarDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentNullException(nameof(descricao), "A propriedade Atividade.Descricao não pode ser nula, vazia ou composta apenas por espaços.");
        if (descricao.Length < 5 || descricao.Length > 500)
            throw new ArgumentException("A propriedade Atividade.Descricao deve conter entre 5 e 500 caracteres.", nameof(descricao));
        return descricao;
    }

    private static string ValidarDepartamentoSolucionador(string departamentoSolucionador)
    {
        if (string.IsNullOrWhiteSpace(departamentoSolucionador))
            throw new ArgumentNullException(nameof(departamentoSolucionador), "A propriedade Atividade.DepartamentoSolucionador deve ser informada.");
        return departamentoSolucionador;
    }

    private static uint ValidarPrazoEstimado(uint prazoEstimado)
    {
        if (prazoEstimado == 0)
            throw new ArgumentException("A propriedade Atividade.PrazoEstimado deve ser maior do que 0.", nameof(prazoEstimado));
        if (prazoEstimado > 259200)
            throw new ArgumentException("A propriedade Atividade.PrazoEstimado deve ser menor do que 259.200 minutos (180 dias).", nameof(prazoEstimado));
        return prazoEstimado;
    }
}
