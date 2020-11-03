namespace Skywalker.Lightning.Messaging
{
    public class LightningResponse
    {
        public int? State { get; set; }

        public object? Response { get; set; }

        public LightningResponse() { }

        public LightningResponse(object result)
        {
            Response = result!;
        }
    }
}
