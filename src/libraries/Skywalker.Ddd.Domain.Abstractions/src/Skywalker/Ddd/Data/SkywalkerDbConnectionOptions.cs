namespace Skywalker.Ddd.Data;

/// <summary>
/// Todo: services.Configure<SkywalkerDbConnectionOptions>(Configuration);
/// </summary>
public class SkywalkerDbConnectionOptions
{
    public ConnectionStrings ConnectionStrings { get; set; }

    public SkywalkerDbConnectionOptions()
    {
        ConnectionStrings = new ConnectionStrings();
    }
}
