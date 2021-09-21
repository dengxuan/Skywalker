using MySql.Data.MySqlClient;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace TiTools.Spider.Hosting
{
    internal class InventoryRequestSupplier : IRequestSupplier
    {
        public async Task<IEnumerable<Request>> GetAllListAsync(CancellationToken cancellationToken)
        {
            var requests = new List<Request>();
            using MySqlConnection connection = new("Server=127.0.0.1; Database=TiTools; Uid=root; Password=1234qwer;");
            await connection.OpenAsync(cancellationToken);
            string cmdText = "SELECT `Id`, `GoodsNumber`, `OrderablePartNumber` FROM `Goods` WHERE `IsEnabled` = TRUE AND `IsAvailable` = TRUE AND `StatusCode`=200;";
            MySqlCommand command = new(cmdText, connection);
            using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken);
            while (reader.Read())
            {
                string id = reader.GetString("GoodsNumber");
                string orderablePartNumber = reader.GetString("OrderablePartNumber");
                var properties = new Dictionary<string, object>
                {
                    { "Id", id },
                    { "OrderablePartNumber", orderablePartNumber }
                };
                requests.Add(new Request($"https://www.ti.com/storeservices/cart/opninventory?opn={id}", properties) { Downloader = "Http" });
            }
            return requests;
        }
    }
}
