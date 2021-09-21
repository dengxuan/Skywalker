using MySql.Data.MySqlClient;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TiTools.Spider.Hosting;

internal class GenericPartNumberResponseHandler : IResponseHandler
{

    public static async Task UpdateDatabaseAsync(string goodsNumber, string orderablePartNumber, short statusCode, CancellationToken cancellationToken)
    {
        using MySqlConnection connection = new("Server=127.0.0.1; Database=TiTools; Uid=root; Password=1234qwer;");
        await connection.OpenAsync(cancellationToken);
        string cmdText = "UPDATE `Goods` SET `OrderablePartNumber`=@OrderablePartNumber, `StatusCode`=@StatusCode WHERE `GoodsNumber`=@GoodsNumber;";
        MySqlCommand command = new(cmdText, connection);
        command.Parameters.Add(new MySqlParameter { ParameterName = "@GoodsNumber", MySqlDbType = MySqlDbType.VarChar, Value = goodsNumber });
        command.Parameters.Add(new MySqlParameter { ParameterName = "@OrderablePartNumber", MySqlDbType = MySqlDbType.VarChar, Value = orderablePartNumber });
        command.Parameters.Add(new MySqlParameter { ParameterName = "@StatusCode", MySqlDbType = MySqlDbType.Int32, Value = statusCode });
        await command.ExecuteNonQueryAsync(cancellationToken);
        await connection.CloseAsync(cancellationToken);
    }

    public async Task HandleAsync(Request request, Response response, CancellationToken cancellationToken)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return;
        }
        string jsonContent = Encoding.UTF8.GetString(response.Content!.Bytes);
        JsonDocument json = JsonDocument.Parse(jsonContent);
        if (json.RootElement.TryGetProperty("genericPartNumber", out JsonElement value))
        {
            string genericPartNumber = value.GetString()!;
            if (request.Properties.TryGetValue("GoodsNumber", out object? goodsNumber))
            {
                await UpdateDatabaseAsync(goodsNumber!.ToString()!, genericPartNumber.ToString(), (short)response.StatusCode, cancellationToken);
            }
        }
    }
}
