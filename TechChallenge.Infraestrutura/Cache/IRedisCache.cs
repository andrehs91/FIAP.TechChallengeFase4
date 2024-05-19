using StackExchange.Redis;

namespace TechChallenge.Infraestrutura.Cache;

public interface IRedisCache
{
    public IDatabase GetDatabase();
}