using HtmlAgilityPack;
using Skywalker.Extensions.HtmlAgilityPack.Abstractions;
using Skywalker.Extensions.HtmlAgilityPack.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skywalker.Extensions.HtmlAgilityPack.Selectors;

/// <summary>
/// CSS 选择器
/// </summary>
public class CssSelector : ISelector
{
    private readonly string _selector;
    private readonly string? _attrName;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="selector">Css 选择器</param>
    public CssSelector(string selector)
    {
        _selector = selector;
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="selector">Css 选择器</param>
    /// <param name="attr">属性名称</param>
    public CssSelector(string selector, string? attr)
    {
        _selector = selector;
        _attrName = attr;
    }

    /// <summary>
    /// 对节点进行查询, 查询结果为第一个符合查询条件的元素
    /// </summary>
    /// <param name="text">HTML</param>
    /// <returns>查询结果</returns>
    public IResolver? Select(string text)
    {
        if (text.IsNullOrWhiteSpace())
        {
            return null;
        }

        var document = new HtmlDocument { OptionAutoCloseOnEnd = true };
        document.LoadHtml(text);
        var node = document.DocumentNode.QuerySelector(_selector);

        if (HasAttribute)
        {
            return new TextResolver(node!.Attributes[_attrName].Value.Trim());
        }
        else
        {
            return new HtmlResolver(node!);
        }
    }

    /// <summary>
    /// 对节点进行查询, 查询结果为所有符合查询条件的元素
    /// </summary>
    /// <param name="text">HTML</param>
    /// <returns>查询结果</returns>
    public IEnumerable<IResolver> SelectList(string text)
    {
        if (text.IsNullOrWhiteSpace())
        {
            return Enumerable.Empty<IResolver>();
        }
        var document = new HtmlDocument { OptionAutoCloseOnEnd = true };
        document.LoadHtml(text);
        var nodes = document.DocumentNode.QuerySelectorAll(_selector);
        if (HasAttribute)
        {
            return nodes?.Select(x => new TextResolver(x.Attributes[_attrName].Value.Trim()))!;
        }
        else
        {
            return nodes?.Select(node => new HtmlResolver(node))!;
        }
    }

    /// <summary>
    /// 判断查询是否包含属性
    /// </summary>
    /// <returns>如果返回 True, 则说明是查询元素的属性值</returns>
    public bool HasAttribute => !_attrName.IsNullOrWhiteSpace();
}
