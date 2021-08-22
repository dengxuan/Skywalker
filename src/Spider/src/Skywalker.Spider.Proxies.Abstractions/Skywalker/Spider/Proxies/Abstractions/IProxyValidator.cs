using System;
using System.Threading.Tasks;

namespace Skywalker.Spider.Proxies.Abstractions;

public interface IProxyValidator
{
    Task<bool> IsAvailable(Uri proxy);
}
