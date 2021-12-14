﻿using System.Collections.Generic;

namespace Skywalker.Extensions.HtmlAgilityPack.Abstractions;

/// <summary>
/// 查询器
/// </summary>
public interface ISelector
{
    /// <summary>
    /// 从文本中查询单个结果
    /// 如果符合条件的结果有多个, 仅返回第一个
    /// </summary>
    /// <param name="text">需要查询的文本</param>
    /// <returns>查询结果</returns>
    IResolver? Select(string text);

    /// <summary>
    /// 从文本中查询所有结果
    /// </summary>
    /// <param name="text">需要查询的文本</param>
    /// <returns>查询结果</returns>
    IEnumerable<IResolver> SelectList(string text);
}
