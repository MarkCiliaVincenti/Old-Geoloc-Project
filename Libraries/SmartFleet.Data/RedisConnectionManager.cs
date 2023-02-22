using System;
using StackExchange.Redis;

namespace SmartFleet.Data
{
    public class RedisConnectionManager : IRedisConnectionManager
    {
        private static string _serverAddress;
        static string  _password;
        public RedisConnectionManager(string serverAddress, string password)
        {
            _serverAddress = serverAddress;
            _password = password;
        }

        private static readonly Lazy<ConfigurationOptions> ConfigOptions = new Lazy<ConfigurationOptions>(() =>
        {
            var configOptions = new ConfigurationOptions();
            configOptions.EndPoints.Add(_serverAddress);
            configOptions.ClientName = "RedisConnection";
            configOptions.ConnectTimeout = 100000;
            configOptions.SyncTimeout = 100000;
            configOptions.AbortOnConnectFail = false;
            configOptions.Password = _password;
            return configOptions;
        });

        private static readonly Lazy<ConnectionMultiplexer> Conn = new Lazy<ConnectionMultiplexer>(
            () => ConnectionMultiplexer.Connect(ConfigOptions.Value));

        public IDatabase RedisServer => Conn.Value.GetDatabase(0);
    }
}
