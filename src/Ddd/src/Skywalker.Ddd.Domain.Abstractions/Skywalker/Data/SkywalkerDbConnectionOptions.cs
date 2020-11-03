namespace Skywalker.Data
{
    public class SkywalkerDbConnectionOptions
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public SkywalkerDbConnectionOptions()
        {
            ConnectionStrings = new ConnectionStrings();
        }
    }
}
