using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Skywalker.Spider.Proxies.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.Spider.Proxies
{
    internal class MysqlProxyStorage : IProxyStorage
    {
        private readonly ProxyOptions _proxyOptions;
        private readonly ILogger<MysqlProxyStorage> _logger;

        public MysqlProxyStorage(IOptions<ProxyOptions> options, ILogger<MysqlProxyStorage> logger)
        {
            _proxyOptions = options.Value;
            _logger = logger;
        }

        public async Task CreateAsync(ProxyEntry proxy)
        {
            try
            {
                using MySqlConnection connection = new(_proxyOptions.ConnectionString);
                string cmdText = "INSERT INTO `SpiderProxies`(`Scheme`, `Host`, `Prot`, `ExpireTime`, `CreationTime`) VALUES(@Scheme, @Host, @Prot, @ExpireTime, @CreationTime);";
                await connection.OpenAsync();
                using var cmd = new MySqlCommand(cmdText, connection);
                cmd.Parameters.AddWithValue("@Scheme", proxy.Uri.Scheme);
                cmd.Parameters.AddWithValue("@Host", proxy.Uri.Host);
                cmd.Parameters.AddWithValue("@Prot", proxy.Uri.Port);
                cmd.Parameters.AddWithValue("@ExpireTime", proxy.ExpireTime);
                cmd.Parameters.AddWithValue("@CreationTime", DateTime.Now);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create Proxy failed! {0}", ex.Message);
            }
        }

        public async Task<IEnumerable<ProxyEntry>> GetAvailablesAsync()
        {
            try
            {
                using MySqlConnection connection = new(_proxyOptions.ConnectionString);
                var cmdText = "SELECT `Scheme`, `Host`, `Prot`, `ExpireTime`, `CreationTime` FROM `SpiderProxies` WHERE `ExpireTime` < @NOW;";
                await connection.OpenAsync();
                using var cmd = new MySqlCommand(cmdText, connection);
                using var reader = await cmd.ExecuteReaderAsync();
                var entries = new List<ProxyEntry>();
                while (reader.Read())
                {
                    var scheme = reader.GetString("Scheme");
                    var ipAddress = reader.GetString("Host");
                    var port = reader.GetInt32("Port");
                    DateTime expireTime = reader.GetDateTime("ExpireTime");
                    UriBuilder builder = new(scheme, ipAddress, port);
                    entries.Add(new ProxyEntry(builder.Uri, expireTime));
                }
                return entries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get avaliable proxy failed! {0}", ex.Message);
            }
            return Enumerable.Empty<ProxyEntry>();
        }

        public Task UpdateAsync(ProxyEntry proxy)
        {
            return Task.CompletedTask;
        }
    }
}
