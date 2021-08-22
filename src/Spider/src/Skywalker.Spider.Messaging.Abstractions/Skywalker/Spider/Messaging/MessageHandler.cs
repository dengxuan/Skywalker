using System.Threading.Tasks;

namespace Skywalker.Spider.Messaging;

public delegate Task MessageHandler<in TMessage>(TMessage message);
