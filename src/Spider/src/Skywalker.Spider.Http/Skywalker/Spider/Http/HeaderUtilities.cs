using System.Collections.Generic;
using System.Text;

namespace Skywalker.Spider.Http
{
    public class HeaderUtilities
    {
        internal static void DumpHeaders(StringBuilder sb, params Dictionary<string, dynamic>[] headers)
        {
            if (headers.IsNullOrEmpty())
            {
                return;
            }
            sb.AppendLine("{");
            foreach (var t in headers!)
            {
                if (t == null)
                {
                    continue;
                }

                foreach (var keyValuePair in t)
                {
                    sb.Append("  ");
                    sb.Append(keyValuePair.Key);
                    sb.Append(": ");
                    foreach (var item in keyValuePair.Value)
                    {
                        sb.Append(item);
                    }
                    sb.AppendLine();
                }
            }

            sb.Append('}');
        }
    }
}
