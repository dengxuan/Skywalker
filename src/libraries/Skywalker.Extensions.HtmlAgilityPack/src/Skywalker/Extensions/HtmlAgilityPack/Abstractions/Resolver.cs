using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Skywalker.Extensions.HtmlAgilityPack.Abstractions;

/// <summary>
/// 查询接口
/// </summary>
public abstract class Resolver : IResolver
{
    /// <summary>
    /// 查找所有的链接
    /// </summary>
    /// <returns>查询接口</returns>
    public abstract IEnumerable<string> Links();

    /// <summary>
    /// 通过XPath查找结果
    /// </summary>
    /// <param name="xpath">XPath 表达式</param>
    /// <returns>查询接口</returns>
    public virtual IResolver? XPath(string xpath)
    {
        return Select(SelectorFactory.XPath(xpath));
    }

    /// <summary>
    /// 通过Css 选择器查找元素, 并取得属性的值
    /// </summary>
    /// <param name="css">Css 选择器</param>
    /// <param name="attrName">查询到的元素的属性</param>
    /// <returns>查询接口</returns>
    public IResolver? Css(string css, string? attrName)
    {
        return Select(SelectorFactory.Css(css, attrName));
    }

    /// <summary>
    /// 通过JsonPath查找结果
    /// </summary>
    /// <param name="jsonPath">JsonPath 表达式</param>
    /// <returns>查询接口</returns>
    public virtual IResolver? JsonPath(string jsonPath)
    {
        return Select(SelectorFactory.JsonPath(jsonPath));
    }

    /// <summary>
    /// 通过正则表达式查找结果
    /// </summary>
    /// <param name="pattern">正则表达式</param>
    /// <param name="options"></param>
    /// <param name="replacement"></param>
    /// <returns>查询接口</returns>
    public virtual IResolver? Regex(string pattern, RegexOptions options = RegexOptions.None, string replacement = "$0")
    {
        return Select(SelectorFactory.Regex(pattern, options, replacement));
    }

    public abstract IEnumerable<IResolver> Nodes();

    public abstract string? Value { get; }

    /// <summary>
    /// 通过查询器查找结果
    /// </summary>
    /// <param name="selector">查询器</param>
    /// <returns>查询接口</returns>
    public abstract IResolver? Select(ISelector selector);

    /// <summary>
    /// 通过查询器查找结果
    /// </summary>
    /// <param name="selector">查询器</param>
    /// <returns>查询接口</returns>
    public abstract IEnumerable<IResolver> SelectList(ISelector selector);
}
