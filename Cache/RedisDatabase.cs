// <copyright file="RedisDatabase.cs" project="Platform.MicroFrame.Caching">
// 
//    Copyright (c) MicroFrame Solutions. All rights reserved.
//    Author:    Berk Kulaksız
//    Created:   15.01.2022
//    Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// 
// </copyright>

namespace Cache;

public class RedisDatabase(IDatabase database) : IRedisDatabase
{
    public IDatabase Database { get; set; } = database;
}