using System.Threading.Tasks;

namespace Skywalker.Splider.Messaging;

public delegate Task MessageHandler<in TMessage>(TMessage message);
