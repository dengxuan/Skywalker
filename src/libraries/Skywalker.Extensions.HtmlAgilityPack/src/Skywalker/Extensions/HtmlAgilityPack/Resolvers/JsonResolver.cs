using Newtonsoft.Json.Linq;
using Skywalker.Extensions.HtmlAgilityPack.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skywalker.Extensions.HtmlAgilityPack.Resolvers;

public class JsonResolver : Resolver
{
    private readonly JToken _token;

    public JsonResolver(JToken token)
    {
        _token = token;
    }

    public override IEnumerable<string> Links()
    {
        throw new NotSupportedException();
    }

    public override IEnumerable<IResolver> Nodes()
    {
        return _token.Children().Select(x => new JsonResolver(x));
    }

    public override string? Value => _token?.ToString();

    /// <summary>
    /// 通过查询器查找结果
    /// </summary>
    /// <param name="selector">查询器</param>
    /// <returns>查询接口</returns>
    public override IResolver? Select(ISelector selector)
    {
        selector.NotNull(nameof(selector));
        return selector.Select(_token.ToString());
    }

    /// <summary>
    /// 通过查询器查找结果
    /// </summary>
    /// <param name="selector">查询器</param>
    /// <returns>查询接口</returns>
    public override IEnumerable<IResolver> SelectList(ISelector selector)
    {
        selector.NotNull(nameof(selector));
        return selector.SelectList(_token.ToString());
    }
}
