using HtmlAgilityPack;
using Skywalker.Extensions.HtmlAgilityPack.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skywalker.Extensions.HtmlAgilityPack.Resolvers;

public class HtmlResolver : Resolver
{
    private readonly HtmlNode _node;

    public HtmlResolver(HtmlNode node)
    {
        _node = node;
    }

    public HtmlResolver(string html, string? relativeUri = null, bool removeOutboundLinks = true)
    {
        var document = new HtmlDocument { OptionAutoCloseOnEnd = true };
        document.LoadHtml(html);

        if (!string.IsNullOrWhiteSpace(relativeUri))
        {
            HtmlUtilities.FixAllRelativeHref(document, relativeUri);
            if (removeOutboundLinks)
            {
                var host = new Uri(relativeUri).Host;
                var parts = host.Split('.');
                var domainPattern = string.Join("\\.", parts);
                HtmlUtilities.RemoveOutboundLinks(document, domainPattern);
            }
        }

        _node = document.DocumentNode;
    }

    public override IEnumerable<string> Links()
    {
        var links = SelectList(SelectorFactory.XPath("./descendant-or-self::*/@href"))?.Select(x => x.Value);
        var sourceLinks = SelectList(SelectorFactory.XPath("./descendant-or-self::*/@src"))
            ?.Select(x => x.Value);

        var results = new HashSet<string>();
        if (links != null)
        {
            foreach (var link in links)
            {
                if (Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out _))
                {
                    results.Add(link);
                }
            }
        }

        if (sourceLinks != null)
        {
            foreach (var link in sourceLinks)
            {
                if (Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out _))
                {
                    results.Add(link);
                }
            }
        }

        return results;
    }

    public override IEnumerable<IResolver> Nodes()
    {
        return _node?.ChildNodes.Select(x => new HtmlResolver(x));
    }

    public override string Value => _node?.InnerText;

    public string InnerHtml => _node?.InnerHtml;

    public string OuterHtml => _node?.OuterHtml;

    /// <summary>
    /// 通过查询器查找结果
    /// </summary>
    /// <param name="selector">查询器</param>
    /// <returns>查询接口</returns>
    public override IResolver Select(ISelector selector)
    {
        selector.NotNull(nameof(selector));
        return selector.Select(_node.OuterHtml);
    }

    /// <summary>
    /// 通过查询器查找结果
    /// </summary>
    /// <param name="selector">查询器</param>
    /// <returns>查询接口</returns>
    public override IEnumerable<IResolver> SelectList(ISelector selector)
    {
        selector.NotNull(nameof(selector));
        return selector.SelectList(_node.OuterHtml);
    }
}
