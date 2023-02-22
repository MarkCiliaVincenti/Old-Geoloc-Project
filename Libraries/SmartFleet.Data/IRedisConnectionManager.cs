using StackExchange.Redis;

namespace SmartFleet.Data
{
    public interface IRedisConnectionManager
    {
        IDatabase RedisServer { get; }
    }
}