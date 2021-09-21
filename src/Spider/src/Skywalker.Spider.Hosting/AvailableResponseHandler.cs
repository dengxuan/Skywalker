using MySql.Data.MySqlClient;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TiTools.Spider.Hosting
{
    internal class AvailableResponseHandler : IResponseHandler
    {

        public static async Task UpdateDatabaseAsync(string goodsNumber, short statusCode, short isAvailable, CancellationToken cancellationToken)
        {
            using MySqlConnection connection = new("Server=127.0.0.1; Database=TiTools; Uid=root; Password=1234qwer;");
            await connection.OpenAsync(cancellationToken);
            string cmdText = "UPDATE `Goods` SET `IsAvailable`=@IsAvailable, `StatusCode`=@StatusCode ,`PackingQuantity`=0  WHERE `GoodsNumber`=@GoodsNumber;";
            MySqlCommand command = new(cmdText, connection);
            command.Parameters.Add(new MySqlParameter { ParameterName = "@GoodsNumber", MySqlDbType = MySqlDbType.VarChar, Value = goodsNumber });
            command.Parameters.Add(new MySqlParameter { ParameterName = "@IsAvailable", MySqlDbType = MySqlDbType.Int16, Value = isAvailable });
            command.Parameters.Add(new MySqlParameter { ParameterName = "@StatusCode", MySqlDbType = MySqlDbType.Int16, Value = statusCode });
            await command.ExecuteNonQueryAsync(cancellationToken);
            connection.Close();
        }

        public async Task HandleAsync(Request request, Response response, CancellationToken cancellationToken)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.OK || response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                return;
            }
            string jsonContext = Encoding.UTF8.GetString(response.Content!.Bytes);
            request.Properties.TryGetValue("Id", out object? goodsNumber);
            short statusCode = (short)response.StatusCode;
            short isAvailable = 0;
            JsonDocument json = JsonDocument.Parse(jsonContext);
            if (json.RootElement.TryGetProperty("availableForPurchaseFlag", out JsonElement value))
            {
                if (value.GetString()?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true)
                {
                    isAvailable = 1;
                }
            }
            await UpdateDatabaseAsync(goodsNumber!.ToString()!, statusCode, isAvailable, cancellationToken);
        }
    }
}
