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

public class DemandaRepository(
    ILogger<DemandaRepository> logger,
    ApplicationDbContext context,
    IRedisCache redisCache) : IDemandaRepository
{
    private readonly ILogger<DemandaRepository> _logger = logger;
    private readonly ApplicationDbContext _context = context;
    private readonly IRedisCache _redisCache = redisCache;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve
    };

    public bool Criar(Demanda demanda)
    {
        try
        {
            _context.Attach(demanda.UsuarioSolicitante);
            _context.Demandas.Add(demanda);
            _context.SaveChanges();

            var db = _redisCache.GetDatabase();
            db.KeyDelete($"DemandasDoSolicitante:{demanda.UsuarioSolicitanteId}");
            db.KeyDelete($"DemandasDoDepartamentoSolicitante:{demanda.DepartamentoSolicitante.Replace(" ", "_")}");
            db.KeyDelete($"DemandasDoDepartamentoSolucionador:{demanda.DepartamentoSolucionador.Replace(" ", "_")}");

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Demanda? BuscarPorId(int id)
    {
        var db = _redisCache.GetDatabase();
        string key = $"Demanda:{id}";
        var cache = db.StringGet(key);
        Demanda? demanda = DesserializarDemanda(cache);
        if (demanda is not null) return demanda;

        demanda = _context.Demandas
            .Include(d => d.Atividade)
            .Include(d => d.EventosRegistrados)
                .ThenInclude(er => er.UsuarioSolucionador)
            .Include(d => d.UsuarioSolicitante)
            .Include(d => d.UsuarioSolucionador)
            .Where(d => d.Id == id)
            .FirstOrDefault();
        if (demanda is not null)
            db.StringSet(key, JsonSerializer.Serialize(demanda, _jsonSerializerOptions));
        return demanda;
    }

    public IList<Demanda> BuscarTodas()
    {
        return _context.Demandas.AsNoTracking().ToList();
    }

    public IList<Demanda> BuscarPorSolicitante(int idSolicitante)
    {
        var db = _redisCache.GetDatabase();
        string key = $"DemandasDoSolicitante:{idSolicitante}";
        var cache = db.StringGet(key);
        List<Demanda>? demandas = DesserializarDemandas(cache);
        if (demandas is not null) return demandas;

        demandas = _context.Demandas
            .Include(d => d.Atividade)
            .Include(d => d.EventosRegistrados)
                .ThenInclude(er => er.UsuarioSolucionador)
            .Include(d => d.UsuarioSolicitante)
            .Include(d => d.UsuarioSolucionador)
            .Where(d => d.UsuarioSolicitanteId == idSolicitante)
            .OrderBy(d => d.Id)
            .AsNoTracking()
            .ToList();
        if (demandas.Count > 0)
            db.StringSet(key, JsonSerializer.Serialize(demandas, _jsonSerializerOptions));
        return demandas;
    }

    public IList<Demanda> BuscarPorDepartamentoSolicitante(string departamento)
    {
        var db = _redisCache.GetDatabase();
        string key = $"DemandasDoDepartamentoSolicitante:{departamento.Replace(" ", "_")}";
        var cache = db.StringGet(key);
        List<Demanda>? demandas = DesserializarDemandas(cache);
        if (demandas is not null) return demandas;

        demandas = _context.Demandas
            .Include(d => d.Atividade)
            .Include(d => d.EventosRegistrados)
                .ThenInclude(er => er.UsuarioSolucionador)
            .Include(d => d.UsuarioSolicitante)
            .Include(d => d.UsuarioSolucionador)
            .Where(d => d.DepartamentoSolicitante == departamento)
            .OrderBy(d => d.Id)
            .AsNoTracking()
            .ToList();
        if (demandas.Count > 0)
            db.StringSet(key, JsonSerializer.Serialize(demandas, _jsonSerializerOptions));
        return demandas;
    }

    public IList<Demanda> BuscarPorSolucionador(int idSolucionador)
    {
        var db = _redisCache.GetDatabase();
        string key = $"DemandasDoSolucionador:{idSolucionador}";
        var cache = db.StringGet(key);
        List<Demanda>? demandas = DesserializarDemandas(cache);
        if (demandas is not null) return demandas;

        demandas = _context.Demandas
            .Include(d => d.Atividade)
            .Include(d => d.EventosRegistrados)
                .ThenInclude(er => er.UsuarioSolucionador)
            .Include(d => d.UsuarioSolicitante)
            .Include(d => d.UsuarioSolucionador)
            .Where(d => d.UsuarioSolucionadorId == idSolucionador)
            .OrderBy(d => d.Atividade.Prioridade)
            .OrderBy(d => d.Prazo)
            .AsNoTracking()
            .ToList();
        if (demandas.Count > 0)
            db.StringSet(key, JsonSerializer.Serialize(demandas, _jsonSerializerOptions));
        return demandas;
    }

    public IList<Demanda> BuscarPorDepartamentoSolucionador(string departamento)
    {
        var db = _redisCache.GetDatabase();
        string key = $"DemandasDoDepartamentoSolucionador:{departamento.Replace(" ", "_")}";
        var cache = db.StringGet(key);
        List<Demanda>? demandas = DesserializarDemandas(cache);
        if (demandas is not null) return demandas;

        demandas = _context.Demandas
            .Include(d => d.Atividade)
            .Include(d => d.EventosRegistrados)
                .ThenInclude(er => er.UsuarioSolucionador)
            .Include(d => d.UsuarioSolicitante)
            .Include(d => d.UsuarioSolucionador)
            .Where(d => d.DepartamentoSolucionador == departamento)
            .OrderBy(d => d.Atividade.Prioridade)
            .OrderBy(d => d.Prazo)
            .AsNoTracking()
            .ToList();
        if (demandas.Count > 0)
            db.StringSet(key, JsonSerializer.Serialize(demandas, _jsonSerializerOptions));
        return demandas;
    }

    public bool Editar(Demanda demanda)
    {
        try
        {
            _context.Demandas.Update(demanda);
            _context.SaveChanges();

            var db = _redisCache.GetDatabase();
            db.KeyDelete($"Demanda:{demanda.Id}");
            db.KeyDelete($"DemandasDoSolicitante:{demanda.UsuarioSolicitanteId}");
            db.KeyDelete($"DemandasDoDepartamentoSolicitante:{demanda.DepartamentoSolicitante.Replace(" ", "_")}");
            if (demanda.UsuarioSolucionadorId is not null)
                db.KeyDelete($"DemandasDoSolucionador:{demanda.UsuarioSolucionadorId}");
            db.KeyDelete($"DemandasDoDepartamentoSolucionador:{demanda.DepartamentoSolucionador.Replace(" ", "_")}");

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private Demanda? DesserializarDemanda(RedisValue cache)
    {
        Demanda? demanda = null;
        if (cache.HasValue)
        {
            try
            {
                demanda = JsonSerializer.Deserialize<Demanda>(cache.ToString(), _jsonSerializerOptions);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao desserializar demanda.");
            }
        }
        return demanda;
    }

    private List<Demanda>? DesserializarDemandas(RedisValue cache)
    {
        List<Demanda>? demandas = null;
        if (cache.HasValue)
        {
            try
            {
                demandas = JsonSerializer.Deserialize<List<Demanda>>(cache.ToString(), _jsonSerializerOptions);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao desserializar lista de demandas.");
            }
        }
        return demandas;
    }
}
