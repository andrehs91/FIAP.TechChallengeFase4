using Microsoft.AspNetCore.Mvc;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;
using TechChallenge.DTO;
using TechChallenge.Infraestrutura.Data;

namespace TechChallenge.Aplicacao.Controllers;

[ApiController]
[Route("[controller]")]
public class CargaDeDadosController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// Configuração Inicial
    /// </summary>
    /// <remarks>
    /// Inclui dados para testes.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public ActionResult<IList<AtividadeDTO>> CargaDeDados()
    {
        if (!_context.Usuarios.Any())
        {
            Usuario[] entities = [
                new Usuario() {
                    Id = 1,
                    Matricula = "1062",
                    Nome = "Pedro",
                    Departamento = "Suporte Tecnológico",
                    EhGestor = true,
                },
                new Usuario() {
                    Id = 2,
                    Matricula = "1123",
                    Nome = "Fernanda",
                    Departamento = "Suporte Tecnológico",
                    EhGestor = false,
                },
                new Usuario() {
                    Id = 3,
                    Matricula = "1099",
                    Nome = "Tiago",
                    Departamento = "Suporte Tecnológico",
                    EhGestor = false,
                },
                new Usuario() {
                    Id = 4,
                    Matricula = "1255",
                    Nome = "Felipe",
                    Departamento = "Suporte Tecnológico",
                    EhGestor = false,
                },
                new Usuario() {
                    Id = 5,
                    Matricula = "1012",
                    Nome = "Helena",
                    Departamento = "Financeiro",
                    EhGestor = true,
                },
                new Usuario() {
                    Id = 6,
                    Matricula = "1294",
                    Nome = "Rafael",
                    Departamento = "Financeiro",
                    EhGestor = false,
                },
                new Usuario() {
                    Id = 7,
                    Matricula = "1004",
                    Nome = "Lucas",
                    Departamento = "Jurídico",
                    EhGestor = true,
                },
                new Usuario() {
                    Id = 8,
                    Matricula = "1344",
                    Nome = "Isabel",
                    Departamento = "Jurídico",
                    EhGestor = false,
                },
            ];
            _context.Usuarios.AddRange(entities);
            _context.SaveChanges();
        }

        if (!_context.Atividades.Any())
        {
            Atividade[] entities = [
                new Atividade() {
                    Id = 1,
                    Nome = "Remanejar Equipamento",
                    Descricao = "Remanejar fisicamente um equipamento de informática.",
                    EstahAtiva = true,
                    DepartamentoSolucionador = "Suporte Tecnológico",
                    TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
                    Prioridade = Prioridades.Media,
                    PrazoEstimado = 60,
                },
                new Atividade() {
                    Id = 2,
                    Nome = "Instalar Software",
                    Descricao = "Instalar um software em determinado computador.",
                    EstahAtiva = true,
                    DepartamentoSolucionador = "Suporte Tecnológico",
                    TipoDeDistribuicao = TiposDeDistribuicao.Manual,
                    Prioridade = Prioridades.Critica,
                    PrazoEstimado = 120,
                },
                new Atividade() {
                    Id = 3,
                    Nome = "Analisar Minuta de Contrato",
                    Descricao = "Analisar uma minuta proposta para um contrato.",
                    EstahAtiva = true,
                    DepartamentoSolucionador = "Jurídico",
                    TipoDeDistribuicao = TiposDeDistribuicao.Automatica,
                    Prioridade = Prioridades.MuitoAlta,
                    PrazoEstimado = 240,
                },
            ];
            _context.Atividades.AddRange(entities);
            _context.SaveChanges();

            var atividade1 = _context.Atividades.First(e => e.Id == 1);
            var atividade2 = _context.Atividades.First(e => e.Id == 2);
            var solucionador2 = _context.Usuarios.First(e => e.Id == 2);
            var solucionador3 = _context.Usuarios.First(e => e.Id == 3);
            var solucionador4 = _context.Usuarios.First(e => e.Id == 4);
            atividade1.Solucionadores.Add(solucionador2);
            atividade1.Solucionadores.Add(solucionador3);
            atividade2.Solucionadores.Add(solucionador2);
            atividade2.Solucionadores.Add(solucionador4);
            _context.SaveChanges();
        }

        return Ok("Carga de dados efetuada com sucesso.");
    }
}
