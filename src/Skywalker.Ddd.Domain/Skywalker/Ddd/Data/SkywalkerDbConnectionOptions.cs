namespace Skywalker.Ddd.Data;

/// <summary>
/// <![CDATA[Todo: services.Configure<SkywalkerDbConnectionOptions>(Configuration)]]>
/// </summary>
public class SkywalkerDbConnectionOptions
{
    /// <summary>
    /// 
    /// </summary>
    public ConnectionStrings ConnectionStrings { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public SkywalkerDbConnectionOptions()
    {
        ConnectionStrings = new ConnectionStrings();
    }
}
