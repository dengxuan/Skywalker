using Skywalker.Extensions.HtmlAgilityPack.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skywalker.Extensions.HtmlAgilityPack.Resolvers;

/// <summary>
/// 查询接口
/// </summary>
public class TextResolver : Resolver
{
    private readonly string _text;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="text">内容</param>
    public TextResolver(string text)
    {
        _text = text;
    }

    /// <summary>
    /// 取得查询器里所有的结果
    /// </summary>
    /// <returns>查询接口</returns>
    public override IEnumerable<IResolver> Nodes()
    {
        return new[] { new TextResolver(_text) };
    }

    /// <summary>
    /// 查找所有的链接
    /// </summary>
    /// <returns>查询接口</returns>
    public override IEnumerable<string> Links()
    {
        // todo: re-impl with regex
        var links = SelectList(SelectorFactory.XPath("./descendant-or-self::*/@href")).Select(x => x.Value);
        var sourceLinks = SelectList(SelectorFactory.XPath("./descendant-or-self::*/@src"))
            .Select(x => x.Value);

        var results = new HashSet<string>();
        foreach (var link in links)
        {
            if (Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out _))
            {
                results.Add(link!);
            }
        }

        foreach (var link in sourceLinks)
        {
            if (Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out _))
            {
                results.Add(link!);
            }
        }

        return results;
    }


    public override string Value => _text;

    /// <summary>
    /// 通过查询器查找结果
    /// </summary>
    /// <param name="selector">查询器</param>
    /// <returns>查询接口</returns>
    public override IResolver? Select(ISelector selector)
    {
        selector.NotNull(nameof(selector));
        return selector.Select(_text);
    }

    /// <summary>
    /// 通过查询器查找结果
    /// </summary>
    /// <param name="selector">查询器</param>
    /// <returns>查询接口</returns>
    public override IEnumerable<IResolver> SelectList(ISelector selector)
    {
        selector.NotNull(nameof(selector));
        return selector.SelectList(_text);
    }
}
