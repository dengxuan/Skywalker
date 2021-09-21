using MySql.Data.MySqlClient;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TiTools.Spider.Hosting
{
    internal class InventoryResponseHandler : IResponseHandler
    {

        public static async Task UpdateDatabase(string id, int inventory, CancellationToken cancellationToken)
        {
            using MySqlConnection connection = new("Server=127.0.0.1; Database=TiTools; Uid=root; Password=1234qwer;");
            await connection.OpenAsync(cancellationToken);
            string cmdText = "INSERT INTO `ScanLogs`(`GoodsId`, `Inventory`, `CreationTime`) VALUES (@GoodsId, @Inventory, CURRENT_TIMESTAMP); UPDATE `Goods` SET `PackingQuantity`=@Inventory WHERE `GoodsNumber`=@GoodsId;";
            MySqlCommand command = new(cmdText, connection);
            command.Parameters.Add(new MySqlParameter { ParameterName = "@GoodsId", MySqlDbType = MySqlDbType.VarChar, Value = id });
            command.Parameters.Add(new MySqlParameter { ParameterName = "@Inventory", MySqlDbType = MySqlDbType.Int32, Value = inventory });
            await command.ExecuteNonQueryAsync(cancellationToken);
            await connection.CloseAsync(cancellationToken);
        }

        public async Task HandleAsync(Request request, Response response, CancellationToken cancellationToken)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            string jsonContext = Encoding.UTF8.GetString(response.Content!.Bytes);
            request.Properties.TryGetValue("Id", out object? id);
            JsonDocument json = JsonDocument.Parse(jsonContext);
            if (json.RootElement.TryGetProperty("inventory", out JsonElement value))
            {
                int inventory = value.GetInt32();
                await UpdateDatabase(id!.ToString()!, inventory, cancellationToken);
            }
        }
    }
}
