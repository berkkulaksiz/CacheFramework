// <copyright file="RedisSettings.cs" project="Sample.Cache.Api">
// 
//    Copyright (c) MicroFrame Solutions. All rights reserved.
//    Author:    berkkulaksiz
//    CreatedAt:   18.05.2025
//    UpdatedAt: 18.05.2025
// 
//    Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// 
// </copyright>

namespace Sample.Cache.Api;

public class RedisSettings : IRedisSettings
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; }
    public bool AllowAdmin { get; set; }
    public int Database { get; set; }
}