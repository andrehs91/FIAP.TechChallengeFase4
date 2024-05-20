using System.Text.Json.Serialization;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;

namespace TechChallenge.Aplicacao.DTO;

public class DemandaDTO
{
    public int Id { get; set; }
    public AtividadeDTO Atividade { get; set; }
    public List<EventoRegistradoDTO> EventosRegistrados { get; set; } = [];
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public int? IdDaDemandaReaberta { get; set; }
    public DateTime MomentoDeAbertura { get; set; }
    public DateTime? MomentoDeFechamento { get; set; }
    public DateTime Prazo { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))] public Situacoes Situacao { get; set; }
    public string DepartamentoSolicitante { get; set; }
    public string MatriculaUsuarioSolicitante { get; set; }
    public string NomeUsuarioSolicitante { get; set; }
    public string DepartamentoSolucionador { get; set; }
    public string? MatriculaUsuarioSolucionador { get; set; }
    public string? NomeUsuarioSolucionador { get; set; }
    public string Detalhes { get; set; }

    public DemandaDTO(Demanda demanda)
    {
        Id = demanda.Id;
        Atividade = new(demanda.Atividade);
        IdDaDemandaReaberta = demanda.IdDaDemandaReaberta;
        MomentoDeAbertura = demanda.MomentoDeAbertura;
        MomentoDeFechamento = demanda.MomentoDeFechamento;
        Prazo = demanda.Prazo;
        Situacao = demanda.Situacao;
        DepartamentoSolicitante = demanda.DepartamentoSolicitante;
        MatriculaUsuarioSolicitante = demanda.UsuarioSolicitante.Matricula;
        NomeUsuarioSolicitante = demanda.UsuarioSolicitante.Nome;
        DepartamentoSolucionador = demanda.DepartamentoSolucionador;
        MatriculaUsuarioSolucionador = demanda.UsuarioSolucionador?.Matricula;
        NomeUsuarioSolucionador = demanda.UsuarioSolucionador?.Nome;
        Detalhes = demanda.Detalhes;

        foreach (var eventoRegistrado in demanda.EventosRegistrados)
            EventosRegistrados.Add(new EventoRegistradoDTO(eventoRegistrado));
    }
}
