// <copyright file="RedisDatabaseFactory.cs" project="Platform.MicroFrame.Caching">
// 
//    Copyright (c) MicroFrame Solutions. All rights reserved.
//    Author:    Berk Kulaksız
//    Created:   15.01.2022
//    Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// 
// </copyright>

namespace Cache;

public class RedisDatabaseFactory : IRedisDatabaseFactory
{
    private readonly IRedisSettings _redisSettings;
    private ConnectionMultiplexer _connectionMultiplexer;

    public RedisDatabaseFactory(IRedisSettings redisSettings)
    {
        _redisSettings = redisSettings;
        TryConnect();
    }

    private void TryConnect()
    {
        if (!_redisSettings.Enabled)
        {
            return;
        }

        var configurationOptions = ConfigurationOptions.Parse(_redisSettings.ConnectionString);
        configurationOptions.AllowAdmin = _redisSettings.AllowAdmin;
        _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
    }

    public RedisDatabase GetDatabase(int id = -1)
    {
        var database = _connectionMultiplexer?.GetDatabase(id);

        if (database == null)
        {
            TryConnect();
        }

        return database == null ? null : new RedisDatabase(database);
    }
}