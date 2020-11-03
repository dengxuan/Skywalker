using System.Collections.Generic;

namespace Skywalker.Lightning.Messaging
{
    public class LightningMessage<TBody>
    {
        public string Id { get; set; }

        public TBody Body { get; set; }

        public LightningMessage(string id, TBody body)
        {
            Id = id;
            Body = body;
        }
    }
}
