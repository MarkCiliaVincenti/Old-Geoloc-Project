using System;
using System.Threading.Tasks;

namespace SmartFleet.Data
{
    public interface IRedisCache
    {
        Task SetAsync<T>(string key, T value);
        T Get<T>(string keyName);
        T Get<T>(string keyName, Func<T> queryFunction);
        T Get<T>(string keyName, int expireTimeInMinutes, Func<T> queryFunction);
        void Expire(string keyName);
        double GetTimeToLive(string keyName);
	}
}