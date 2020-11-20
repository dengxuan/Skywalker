using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Skywalker.IdentityServer.Extensions
{
    internal static class StringExtensions
    {

        [DebuggerStepThrough]
        public static NameValueCollection ReadQueryStringAsNameValueCollection(this string url)
        {
            if (url != null)
            {
                var idx = url.IndexOf('?');
                if (idx >= 0)
                {
                    url = url[(idx + 1)..];
                }
                var query = QueryHelpers.ParseNullableQuery(url);
                if (query != null)
                {
                    return query.AsNameValueCollection();
                }
            }

            return new NameValueCollection();
        }
    }
}
