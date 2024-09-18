using BookProject.Interface;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BookProject.Service
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly ILogger<RedisCacheService> _logger;
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        public RedisCacheService(IConfiguration configuration, ILogger<RedisCacheService> logger)
        {
            string connectionString = configuration.GetValue<string>("RedisServer:ConnectionString");
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
            _logger = logger;
        }

        public ConnectionMultiplexer Connection => _lazyConnection.Value;

        private IDatabase RedisDb => Connection.GetDatabase();
        private List<IServer> GetRedisServers() => Connection.GetEndPoints().Select(endpoint => Connection.GetServer(endpoint)).ToList();

        public async Task SaveAsync(string key, object value)
        {
            try
            {
                string stringValue = JsonConvert.SerializeObject(value);
                await RedisDb.StringSetAsync(key, stringValue);
                _logger.LogInformation("Save Async to redis success.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return;
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                RedisValue stringValue = await RedisDb.StringGetAsync(key);
                if (string.IsNullOrEmpty(stringValue))
                {
                    return default(T);
                }
                else
                {
                    T objectValue = JsonConvert.DeserializeObject<T>(stringValue);
                    _logger.LogInformation("Get Async to redis success.");
                    return objectValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return default(T);
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                bool stringvalue = await RedisDb.KeyDeleteAsync(key);
                _logger.LogInformation("Delete Async from redis success.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }
    }
}
