using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;

namespace TechChallenge.Aplicacao.DTO;

public class EventoRegistradoDTO
{
    public long Id { get; set; }
    public string? MatriculaUsuarioSolucionador { get; set; }
    public string? NomeUsuarioSolucionador { get; set; }
    public Situacoes Situacao { get; set; }
    public DateTime MomentoInicial { get; set; }
    public DateTime? MomentoFinal { get; set; }
    public string? Mensagem { get; set; }

    public EventoRegistradoDTO(EventoRegistrado eventoRegistrado)
    {
        Id = eventoRegistrado.Id;
        MatriculaUsuarioSolucionador = eventoRegistrado.UsuarioSolucionador?.Matricula;
        NomeUsuarioSolucionador = eventoRegistrado.UsuarioSolucionador?.Nome;
        Situacao = eventoRegistrado.Situacao;
        MomentoInicial = eventoRegistrado.MomentoInicial;
        MomentoFinal = eventoRegistrado.MomentoFinal;
        Mensagem = eventoRegistrado.Mensagem;
    }
}
