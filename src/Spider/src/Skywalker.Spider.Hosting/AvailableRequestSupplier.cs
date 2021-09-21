using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TiTools.Spider.Hosting
{
    public class AvailableRequestSupplier : IRequestSupplier
    {
        private readonly ILogger<AvailableRequestSupplier> _logger;

        public AvailableRequestSupplier(ILogger<AvailableRequestSupplier> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Request>> GetAllListAsync(CancellationToken cancellationToken)
        {
            var requests = new List<Request>();
            try
            {
                using MySqlConnection connection = new("Server=127.0.0.1; Database=TiTools; Uid=root; Password=1234qwer;");
                await connection.OpenAsync(cancellationToken);
                string cmdText = "SELECT `Id`, `GoodsNumber`, `OrderablePartNumber` FROM `Goods` WHERE `IsEnabled` = TRUE AND `StatusCode`=@StatusCode;";
                MySqlCommand command = new(cmdText, connection);
                command.Parameters.AddWithValue("@StatusCode", HttpStatusCode.OK);
                using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    string goodsNumber = reader.GetString("GoodsNumber");
                    string orderablePartNumber = reader.GetString("OrderablePartNumber");
                    var properties = new Dictionary<string, object>
                    {
                        { "Id", goodsNumber },
                        { "OrderablePartNumber", orderablePartNumber }
                    };
                    requests.Add(new Request($"https://www.ti.com.cn/productmodel/{orderablePartNumber}/orderables?locale=zh-CN&orderable={goodsNumber}", properties) { Downloader="Http" });

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Available RequestSupplier Error, {0}", ex.Message);
            }
            return requests;
        }
    }
}
