using Skywalker.Extensions.HtmlAgilityPack.Abstractions;
using Skywalker.Extensions.HtmlAgilityPack.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Extensions.HtmlAgilityPack
{
    public class ResolverFactory: IResolverFactory
    {

        public IResolver CreateResolver(HttpResponseMessage httpResponse)
        {
            throw new NotImplementedException();
        }
    }
}
