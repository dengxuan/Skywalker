// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Skywalker.Ddd.EntityFrameworkCore;

public static class SkywalkerDbContextOptionsMySqlExtensions
{

    public static void UseMySql(this SkywalkerDbContextOptions options, Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null)
    {
        options.Configure(context =>
        {
            context.UseMySql(mySQLOptionsAction);
        });
    }
}
