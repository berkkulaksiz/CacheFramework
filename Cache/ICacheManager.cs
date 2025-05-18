// <copyright file="ICacheManager.cs" project="Cache">
// 
//    Copyright (c) MicroFrame Solutions. All rights reserved.
//    Author:    berkkulaksiz
//    CreatedAt:   18.05.2025
//    UpdatedAt: 18.05.2025
// 
//    Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// 
// </copyright>

namespace Cache;

public interface ICacheManager<T>
{
    T Add(string key, T value);

    T Add(string key, T value, TimeSpan? expireTime);

    string Add(string key, string value);

    string Add(string key, string value, TimeSpan? expireTime);

    IEnumerable<T> AddAll(string key, IEnumerable<T> values);

    IEnumerable<string> AddAll(string key, IEnumerable<string> values);

    IEnumerable<T> AddAll(string key, IEnumerable<T> values, TimeSpan? expireTime);

    IEnumerable<string> AddAll(string key, IEnumerable<string> values, TimeSpan? expireTime);

    bool HashSet(string key, string hashName, object hashValue);

    T HashGet(string key, string hashName);

    IDictionary<string, T> HashGetAll(string key);

    T Get(string key);

    IQueryable<T> GetAll(IEnumerable<string> keys);

    Task<IEnumerable<T>> GetAllAsync(IEnumerable<string> keys);

    bool Delete(string key);

    long DeleteAll(IEnumerable<string> keys);

    long FlushAllDatabases();

    long PublishMessage(string channel, object message);

    Task<bool> AddAsync(string key, T value);

    Task<bool> AddAsync(string key, T value, TimeSpan? expireTime);

    Task<bool> AddAsync(string key, string value);

    Task<bool> AddAsync(string key, string value, TimeSpan? expireTime);

    Task<bool> AddAllAsync(string key, IEnumerable<T> values);

    Task<bool> AddAllAsync(string key, IEnumerable<T> values, TimeSpan? expireTime);

    Task<bool> AddAllAsync(string key, IEnumerable<string> values);

    Task<bool> AddAllAsync(string key, IEnumerable<string> values, TimeSpan? expireTime);

    Task<bool> HashSetAsync(string key, string hashName, object hashValue);

    Task<T> HashGetAsync(string key, string hashName);

    Task<IDictionary<string, T>> HashGetAllAsync(string key);

    Task<T> GetAsync(string key);

    IEnumerable<RedisKey> GetRedisKeys(string keyPattern);

    IEnumerable<RedisKey> GetRedisKeys();

    Task<bool> DeleteAsync(string key);

    Task<long> DeleteAllAsync(IEnumerable<string> keys);

    Task<long> FlushAllDatabasesAsync();

    Task<long> PublishMessageAsync(string channel, object message);
}