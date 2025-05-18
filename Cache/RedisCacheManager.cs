// <copyright file="RedisCacheManager.cs" project="Platform.MicroFrame.Caching">
// 
//    Copyright (c) MicroFrame Solutions. All rights reserved.
//    Author:    Berk Kulaksız
//    Created:   15.01.2022
//    Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// 
// </copyright>

namespace Cache;

public class RedisCacheManager<T> : ICacheManager<T>
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheManager<T>> _logger;
    private readonly bool _enabled;
    private bool _available;

    public RedisCacheManager(IRedisDatabaseFactory redisDatabaseFactory, IRedisSettings settings, ILogger<RedisCacheManager<T>> logger)
    {
        var redisDatabase = redisDatabaseFactory.GetDatabase(settings.Database);
        _database = redisDatabase.Database;
        _enabled = settings.Enabled;
        _available = _database != null && settings.Enabled;
        _logger = logger;
    }

    public IEnumerable<RedisKey> GetRedisKeys(string keyPattern)
    {
        var connectionMultiplexer = _database.Multiplexer;
        var endPoint = connectionMultiplexer.GetEndPoints().First();
        var server = connectionMultiplexer.GetServer(endPoint);
        var keys = server.Keys(pattern: $"{keyPattern}:*").AsEnumerable();
        return keys;
    }

    public IEnumerable<RedisKey> GetRedisKeys()
    {
        var listKeys = new List<RedisKey>();
        var connectionMultiplexer = _database.Multiplexer;
        var endPoints = connectionMultiplexer.GetEndPoints();
        foreach (var endPoint in endPoints)
        {
            var server = connectionMultiplexer.GetServer(endPoint);
            var keys = server.Keys(pattern: "*").AsEnumerable();
            listKeys.AddRange(keys);
        }
            
        return listKeys;
    }

    public T Add(string key, T value)
    {
        TryExecute(database => database.StringSet(GetKey(key), ToJson(value)));
        return value;
    }

    public T Add(string key, T value, TimeSpan? expireTime)
    {
        TryExecute(database => database.StringSet(GetKey(key), ToJson(value), expireTime));
        return value;
    }

    public string Add(string key, string value)
    {
        TryExecute(database => database.StringSet(GetKey(key), value));
        return value;
    }

    public string Add(string key, string value, TimeSpan? expireTime)
    {
        TryExecute(database => database.StringSet(GetKey(key), value, expireTime));
        return value;
    }

    public IEnumerable<T> AddAll(string key, IEnumerable<T> values)
    {
        TryExecute(database => database.StringSet(GetKey(key), ToJson(values)));
        return values;
    }

    public IEnumerable<string> AddAll(string key, IEnumerable<string> values)
    {
        TryExecute(database => database.StringSet(GetKey(key), values.ToString()));
        return values;
    }

    public IEnumerable<T> AddAll(string key, IEnumerable<T> values, TimeSpan? expireTime)
    {
        TryExecute(database => database.StringSet(GetKey(key), ToJson(values), expireTime));
        return values;
    }

    public IEnumerable<string> AddAll(string key, IEnumerable<string> values, TimeSpan? expireTime)
    {
        TryExecute(database => database.StringSet(GetKey(key), values.ToString(), expireTime));
        return values;
    }

    public async Task<bool> AddAllAsync(string key, IEnumerable<T> values)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(GetKey(key), ToJson(values)))
            .ConfigureAwait(false);
    }

    public async Task<bool> AddAllAsync(string key, IEnumerable<T> values, TimeSpan? expireTime)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(GetKey(key), ToJson(values), expireTime))
            .ConfigureAwait(false);
    }

    public async Task<bool> AddAllAsync(string key, IEnumerable<string> values)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(key, values.ToString()))
            .ConfigureAwait(false);
    }

    public async Task<bool> AddAllAsync(string key, IEnumerable<string> values, TimeSpan? expireTime)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(GetKey(key), values.ToString(), expireTime))
            .ConfigureAwait(false);
    }

    public async Task<bool> AddAsync(string key, T value)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(GetKey(key), ToJson(value)))
            .ConfigureAwait(false);
    }

    public async Task<bool> AddAsync(string key, T value, TimeSpan? expireTime)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(GetKey(key), ToJson(value), expireTime))
            .ConfigureAwait(false);
    }

    public async Task<bool> AddAsync(string key, string value)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(GetKey(key), value))
            .ConfigureAwait(false);
    }

    public async Task<bool> AddAsync(string key, string value, TimeSpan? expireTime)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(GetKey(key), value, expireTime))
            .ConfigureAwait(false);
    }

    public bool Delete(string key)
    {
        return TryExecute(database => database.KeyDelete(GetKey(key)));
    }

    public long DeleteAll(IEnumerable<string> keys)
    {
        return TryExecute(database =>
            database.KeyDelete(keys.Select(key => (RedisKey) GetKey(key)).ToArray()));
    }

    public long FlushAllDatabases()
    {
        var deletedCount = 0;
        var connectionMultiplexer = _database.Multiplexer;
        var endPoints = connectionMultiplexer.GetEndPoints();
        foreach (var endPoint in endPoints)
        {
            var server = connectionMultiplexer.GetServer(endPoint);
            server.FlushAllDatabases();
            deletedCount++;
        }

        return deletedCount;
    }

    public async Task<long> DeleteAllAsync(IEnumerable<string> keys)
    {
        return await TryExecuteAsync(database =>
                database.KeyDeleteAsync(keys.Select(key => (RedisKey) GetKey(key)).ToArray()))
            .ConfigureAwait(false);
    }

    public async Task<long> FlushAllDatabasesAsync()
    {
        var deletedCount = 0;
        var connectionMultiplexer = _database.Multiplexer;
        var endPoints = connectionMultiplexer.GetEndPoints();
        foreach (var endPoint in endPoints)
        {
            var server = connectionMultiplexer.GetServer(endPoint);
            await server.FlushAllDatabasesAsync();
            deletedCount++;
        }

        return deletedCount;
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await TryExecuteAsync(database => database.KeyDeleteAsync(GetKey(key))).ConfigureAwait(false);
    }

    public T Get(string key)
    {
        var redisKey = GetKey(key);
        var redisValue = TryExecute(database => database.StringGet(redisKey));
            
        if (redisValue.IsNullOrEmpty)
        {
            return default;
        }
            
        var jsonValue = Encoding.UTF8.GetString(redisValue);
        var value = FromJson(jsonValue);
        return value;
    }

    public async Task<T> GetAsync(string key)
    {
        var redisKey = GetKey(key);
        var redisValue = await TryExecuteAsync(database => database.StringGetAsync(redisKey)).ConfigureAwait(false);
            
        if (redisValue.IsNullOrEmpty)
        {
            return default;
        }
            
        var jsonValue = Encoding.UTF8.GetString(redisValue);
        var value = FromJson(jsonValue);
        return value;
    }

    public IQueryable<T> GetAll(IEnumerable<string> keys)
    {
        if (keys == null || !keys.Any())
        {
            return null;
        }

        var result = TryExecute(database =>
            database.StringGet(keys.Select(key => (RedisKey) key).ToArray()).AsQueryable());

        return (IQueryable<T>) result;
    }

    public async Task<IEnumerable<T>> GetAllAsync(IEnumerable<string> keys)
    {
        if (keys == null || !keys.Any())
        {
            return Enumerable.Empty<T>();
        }

        var results = await TryExecuteAsync(database =>
            database.StringGetAsync(keys.Select(x => (RedisKey) GetKey(x)).ToArray())).ConfigureAwait(false);

        return results?.Select(result => FromJson(result));
    }

    public long PublishMessage(string channel, object message)
    {
        return TryExecute(database => database.Publish(channel, message.ToString()));
    }

    public async Task<long> PublishMessageAsync(string channel, object message)
    {
        return await TryExecuteAsync(database => database.PublishAsync(channel, message.ToString()))
            .ConfigureAwait(false);
    }

    public bool HashSet(string key, string hashName, object hashValue)
    {
        return TryExecute(database => database.HashSet(GetKey(key), hashName, ToJson(hashValue)));
    }

    public T HashGet(string key, string hashName)
    {
        return FromJson(TryExecute(database => database.HashGet(GetKey(key), hashName)));
    }

    public IDictionary<string, T> HashGetAll(string key)
    {
        var hashes = TryExecute(database => database.HashGetAll(GetKey(key)));

        IDictionary<string, T> result = new Dictionary<string, T>();

        foreach (var hash in hashes)
        {
            result.Add(hash.Name, FromJson(hash.Value));
        }

        return result;
    }

    public async Task<bool> HashSetAsync(string key, string hashName, object hashValue)
    {
        return await TryExecuteAsync(database => database.HashSetAsync(GetKey(key), hashName, ToJson(hashValue)))
            .ConfigureAwait(false);
    }

    public async Task<T> HashGetAsync(string key, string hashName)
    {
        return FromJson(await TryExecuteAsync(database => database.HashGetAsync(GetKey(key), hashName))
            .ConfigureAwait(false));
    }

    public async Task<IDictionary<string, T>> HashGetAllAsync(string key)
    {
        var hashes = await TryExecuteAsync(database => database.HashGetAllAsync(GetKey(key)))
            .ConfigureAwait(false);

        IDictionary<string, T> result = new Dictionary<string, T>();

        foreach (var hash in hashes)
        {
            result.Add(hash.Name, FromJson(hash.Value));
        }

        return result;
    }

    private FType TryExecute<FType>(Func<IDatabase, FType> database)
    {
        if (!_enabled)
        {
            return default(FType);
        }

        try
        {
            var result = database(_database);
            if (_available)
            {
                return result;
            }

            _available = true;
            _logger.LogInformation("Redis became available");
            return result;
        }
        catch (Exception ex)
        {
            if (!_available)
            {
                return default(FType);
            }

            _available = false;
            _logger.LogError(ex, "Redis became unavailable");
        }

        return default;
    }

    private async Task<FType> TryExecuteAsync<FType>(Func<IDatabase, Task<FType>> database)
    {
        if (!_enabled)
        {
            return default(FType);
        }

        try
        {
            var result = await database(_database).ConfigureAwait(false);
            if (_available)
            {
                return result;
            }

            _available = true;
            _logger.LogInformation("Redis became available");

            return result;
        }
        catch (Exception ex)
        {
            if (!_available)
            {
                return default;
            }

            _available = false;
            _logger.LogError(ex, "Redis became unavailable");
        }

        return default;
    }

    private static string GetKey(string key)
    {
        return key.ToLowerInvariant();
    }

    private static string ToJson(object value)
    {
        return value != null ? JsonConvert.SerializeObject(value) : default;
    }

    private static T FromJson(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return default;
        }
        value = value.Trim();
        return JsonConvert.DeserializeObject<T>(value);
    }
}