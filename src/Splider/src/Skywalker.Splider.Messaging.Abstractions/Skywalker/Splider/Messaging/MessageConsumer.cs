using System;
using System.Threading.Tasks;

namespace Skywalker.Splider.Messaging;

public class MessageConsumer<TMessage>
{
    public bool Registered { get; private set; }

    public string Queue { get; }

    public event MessageHandler<TMessage> Received;

    public event Action<MessageConsumer<TMessage>> OnClosing;

    public MessageConsumer(string queue)
    {
        if (queue.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(queue));
        }

        Queue = queue;
    }

    public void Register()
    {
        Registered = true;
    }

    public async Task InvokeAsync(TMessage message)
    {
        if (Received == null)
        {
            throw new ArgumentException("Received delegate is null");
        }

        await Received(message);
    }

    public virtual void Close()
    {
        OnClosing?.Invoke(this);
    }
}

public class ObjectMessageConsumer : MessageConsumer<object>
{
    public ObjectMessageConsumer(string queue) : base(queue)
    {
    }
}