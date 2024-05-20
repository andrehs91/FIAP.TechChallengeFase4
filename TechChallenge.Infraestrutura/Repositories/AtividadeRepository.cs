using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.Infraestrutura.Cache;
using TechChallenge.Infraestrutura.Data;

namespace TechChallenge.Infraestrutura.Repositories;

public class AtividadeRepository(
    ILogger<AtividadeRepository> logger,
    ApplicationDbContext context,
    IRedisCache redisCache) : IAtividadeRepository
{
    private readonly ILogger<AtividadeRepository> _logger = logger;
    private readonly ApplicationDbContext _context = context;
    private readonly IRedisCache _redisCache = redisCache;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve
    };

    public bool Criar(Atividade atividade)
    {
        try
        {
            _context.Atividades.Add(atividade);
            _context.SaveChanges();

            var db = _redisCache.GetDatabase();
            db.KeyDelete("TodasAtividades");
            db.KeyDelete("AtividadesAtivas");

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Atividade? BuscarPorId(int id)
    {
        var db = _redisCache.GetDatabase();
        string key = $"Atividade:{id}";
        var cache = db.StringGet(key);
        Atividade? atividade = DesserializarAtividade(cache);
        if (atividade is not null) return atividade;

        atividade = _context.Atividades.Find(id);
        if (atividade is not null)
            db.StringSet(key, JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        return atividade;
    }

    public Atividade? BuscarPorIdComSolucionadores(int id)
    {
        var db = _redisCache.GetDatabase();
        string key = $"AtividadeComSolucionadores:{id}";
        var cache = db.StringGet(key);
        Atividade? atividade = DesserializarAtividade(cache);
        if (atividade is not null) return atividade;

        atividade = _context.Atividades
            .Include(a => a.Solucionadores)
            .FirstOrDefault(a => a.Id == id);
        if (atividade is not null)
            db.StringSet(key,
                JsonSerializer.Serialize(atividade, _jsonSerializerOptions));

        return atividade;
    }

    public IList<Atividade> BuscarTodas()
    {
        var db = _redisCache.GetDatabase();
        string key = "TodasAtividades";
        var cache = db.StringGet(key);
        List<Atividade>? atividades = DesserializarAtividades(cache);
        if (atividades is not null) return atividades;

        atividades = _context.Atividades.ToList();
        if (atividades.Count > 0)
            db.StringSet(key, JsonSerializer.Serialize(atividades, _jsonSerializerOptions));

        return atividades;
    }

    public IList<Atividade> BuscarAtivas()
    {
        var db = _redisCache.GetDatabase();
        string key = "AtividadesAtivas";
        var cache = db.StringGet(key);
        List<Atividade>? atividades = DesserializarAtividades(cache);
        if (atividades is not null) return atividades;

        atividades = _context.Atividades.Where(a => a.EstahAtiva).ToList();
        if (atividades.Count > 0)
            db.StringSet(key, JsonSerializer.Serialize(atividades, _jsonSerializerOptions));

        return atividades;
    }

    public IList<Atividade> BuscarPorDepartamentoSolucionador(string departamento)
    {
        return _context.Atividades.Where(a => a.DepartamentoSolucionador == departamento).ToList();
    }

    public bool Editar(Atividade atividade)
    {
        try
        {
            _context.Atividades.Update(atividade);
            _context.SaveChanges();

            int id = atividade.Id;
            var db = _redisCache.GetDatabase();
            db.KeyDelete($"Atividade:{id}");
            db.KeyDelete($"AtividadeComSolucionadores:{id}");
            db.KeyDelete("TodasAtividades");
            db.KeyDelete("AtividadesAtivas");

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Usuario? IdentificarSolucionadorMenosAtarefado(int atividadeId)
    {
        var atividade = BuscarPorIdComSolucionadores(atividadeId);
        if (atividade is null) return null;

        var solucionadores = atividade.Solucionadores;
        if (!solucionadores.Any()) return null;

        var demandas = _context.Demandas
            .Where(d => solucionadores.Select(s => s.Id).Contains(d.UsuarioSolucionadorId ?? -1))
            .Select(d => new { d.UsuarioSolucionadorId, PrazoEstimado = (int)d.Prazo.Subtract(d.MomentoDeAbertura).TotalMinutes })
            .ToList();

        List<ItemOrdenacao> listaDesordenada = [];
        foreach (var solucionador in solucionadores)
        {
            listaDesordenada.Add(new()
            {
                Solucionador = solucionador,
                PrazoEstimadoTotal = demandas.Where(d => d.UsuarioSolucionadorId == solucionador.Id).Sum(d => d.PrazoEstimado),
                QuantidadeDeDemandas = demandas.Count(d => d.UsuarioSolucionadorId == solucionador.Id),
            });
        }

        return listaDesordenada
            .OrderBy(r => r.PrazoEstimadoTotal)
            .ThenBy(r => r.QuantidadeDeDemandas)
            .First()
            .Solucionador;
    }

    private Atividade? DesserializarAtividade(RedisValue cache)
    {
        Atividade? atividade = null;
        if (cache.HasValue)
        {
            try
            {
                atividade = JsonSerializer.Deserialize<Atividade>(cache.ToString(), _jsonSerializerOptions);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao desserializar atividade.");
            }
        }
        return atividade;
    }

    private List<Atividade>? DesserializarAtividades(RedisValue cache)
    {
        List<Atividade>? atividades = null;
        if (cache.HasValue)
        {
            try
            {
                atividades = JsonSerializer.Deserialize<List<Atividade>>(cache.ToString(), _jsonSerializerOptions);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao desserializar lista de atividades.");
            }
        }
        return atividades;
    }
}
