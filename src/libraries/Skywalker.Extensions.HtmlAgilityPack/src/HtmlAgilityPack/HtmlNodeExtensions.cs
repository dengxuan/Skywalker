using System.Diagnostics;
using System.Text;
using Skywalker;
using Skywalker.Extensions.HtmlAgilityPack;

namespace HtmlAgilityPack;

/// <summary>
/// Selector API for <see cref="HtmlNode"/>.
/// </summary>
/// <remarks>
/// For more information, see <a href="http://www.w3.org/TR/selectors-api/">Selectors API</a>.
/// </remarks>
public static class HtmlNodeSelection
{
    private static readonly HtmlNodeOps s_ops = new();

    /// <summary>
    /// Similar to <see cref="QuerySelectorAll(HtmlNode,string)" /> 
    /// except it returns only the first element matching the supplied 
    /// selector strings.
    /// </summary>
    public static HtmlNode? QuerySelector(this HtmlNode node, string selector)
    {
        return node.NotNull(nameof(node)).QuerySelectorAll(selector)?.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves all element nodes from descendants of the starting 
    /// element node that match any selector within the supplied 
    /// selector strings. 
    /// </summary>
    public static IEnumerable<HtmlNode>? QuerySelectorAll(this HtmlNode node, string selector)
    {
        return QuerySelectorAll(node, selector, null);
    }

    /// <summary>
    /// Retrieves all element nodes from descendants of the starting 
    /// element node that match any selector within the supplied 
    /// selector strings. An additional parameter specifies a 
    /// particular compiler to use for parsing and compiling the 
    /// selector.
    /// </summary>
    /// <remarks>
    /// The <paramref name="compiler"/> can be <c>null</c>, in which
    /// case a default compiler is used. If the selector is to be used
    /// often, it is recommended to use a caching compiler such as the
    /// one supplied by <see cref="CreateCachingCompiler()"/>.
    /// </remarks>
    public static IEnumerable<HtmlNode>? QuerySelectorAll(this HtmlNode node, string selector, Func<string, Func<HtmlNode, IEnumerable<HtmlNode>?>>? compiler)
    {
        return (compiler ?? CachableCompile)(selector)(node);
    }

    /// <summary>
    /// Gets or sets the maximum number of compiled selectors that will be kept in cache.
    /// </summary>
    public static int CacheSize
    {
        get => s_compilerCache.NotNull(nameof(s_compilerCache)).Capacity;
        set => s_compilerCache.NotNull(nameof(s_compilerCache)).Capacity = value;
    }

    /// <summary>
    /// Parses and compiles CSS selector text into run-time function.
    /// </summary>
    /// <remarks>
    /// Use this method to compile and reuse frequently used CSS selectors
    /// without parsing them each time.
    /// </remarks>
    public static Func<HtmlNode, IEnumerable<HtmlNode>?> Compile(string selector)
    {
        var compiled = Parser.Parse(selector, new SelectorGenerator<HtmlNode>(s_ops)).Selector;
        return node => compiled?.Invoke(Enumerable.Repeat(node, 1));
    }

    //
    // Caching
    //

    private const int DefaultCacheSize = 60;

    private static readonly LruCache<string, Func<HtmlNode, IEnumerable<HtmlNode>?>> s_compilerCache = new(Compile, DefaultCacheSize);
    private static readonly Func<string, Func<HtmlNode, IEnumerable<HtmlNode>?>> s_defaultCachingCompiler = s_compilerCache.NotNull(nameof(s_compilerCache)).GetValue;

    /// <summary>
    /// Compiles a selector. If the selector has been previously 
    /// compiled then this method returns it rather than parsing
    /// and compiling the selector on each invocation.
    /// </summary>
    public static Func<HtmlNode, IEnumerable<HtmlNode>?> CachableCompile(string selector)
    {
        return s_defaultCachingCompiler(selector);
    }

    /// <summary>
    /// Determines whether this node is an element or not.
    /// </summary>
    public static bool IsElement(this HtmlNode node)
    {
        return node.NotNull(nameof(node)).NodeType == HtmlNodeType.Element;
    }

    /// <summary>
    /// Returns a collection of elements from this collection.
    /// </summary>
    public static IEnumerable<HtmlNode> Elements(this IEnumerable<HtmlNode> nodes)
    {
        return nodes.NotNull(nameof(nodes)).Where(n => n.NotNull(nameof(n)).IsElement());
    }

    /// <summary>
    /// Returns a collection of child nodes of this node.
    /// </summary>
    public static IEnumerable<HtmlNode> Children(this HtmlNode node)
    {
        return node.NotNull(nameof(node)).ChildNodes;
    }

    /// <summary>
    /// Returns a collection of child elements of this node.
    /// </summary>
    public static IEnumerable<HtmlNode> Elements(this HtmlNode node)
    {
        return node.NotNull(nameof(node)).Children().Elements();
    }

    /// <summary>
    /// Returns a collection of the sibling elements after this node.
    /// </summary>
    public static IEnumerable<HtmlNode> ElementsAfterSelf(this HtmlNode node)
    {
        return node.NotNull(nameof(node)).NodesAfterSelf().Elements();
    }

    /// <summary>
    /// Returns a collection of the sibling nodes after this node.
    /// </summary>
    public static IEnumerable<HtmlNode> NodesAfterSelf(this HtmlNode node)
    {
        return NodesAfterSelfImpl(node);
    }

    private static IEnumerable<HtmlNode> NodesAfterSelfImpl(HtmlNode node)
    {
        while ((node = node.NotNull(nameof(node)).NextSibling) != null)
        {
            yield return node;
        }
    }

    /// <summary>
    /// Returns a collection of the sibling elements before this node.
    /// </summary>
    public static IEnumerable<HtmlNode> ElementsBeforeSelf(this HtmlNode node)
    {
        return node.NotNull(nameof(node)).NodesBeforeSelf().Elements();
    }

    /// <summary>
    /// Returns a collection of the sibling nodes before this node.
    /// </summary>
    public static IEnumerable<HtmlNode> NodesBeforeSelf(this HtmlNode node)
    {
        return NodesBeforeSelfImpl(node.NotNull(nameof(node)));
    }

    private static IEnumerable<HtmlNode> NodesBeforeSelfImpl(HtmlNode node)
    {
        while ((node = node.NotNull(nameof(node)).PreviousSibling) != null)
        {
            yield return node;
        }
    }

    /// <summary>
    /// Returns a collection of nodes that contains this element 
    /// followed by all descendant nodes of this element.
    /// </summary>
    public static IEnumerable<HtmlNode> DescendantsAndSelf(this HtmlNode node)
    {
        node.NotNull(nameof(node));
        return Enumerable.Repeat(node, 1).Concat(node.Descendants());
    }

    /// <summary>
    /// Returns a collection of all descendant nodes of this element.
    /// </summary>
    public static IEnumerable<HtmlNode> Descendants(this HtmlNode node)
    {
        return DescendantsImpl(node.NotNull(nameof(node)));
    }

    private static IEnumerable<HtmlNode> DescendantsImpl(HtmlNode node)
    {
        Debug.Assert(node != null);
        foreach (var child in node!.ChildNodes)
        {
            yield return child;
            foreach (var descendant in child.NotNull(nameof(child)).Descendants())
            {
                yield return descendant;
            }
        }
    }

    /// <summary>
    /// Returns a begin tag, including attributes, string 
    /// representation of this element.
    /// </summary>
    public static string GetBeginTagString(this HtmlNode node)
    {
        node.NotNull(nameof(node));
        if (!node.IsElement())
        {
            return string.Empty;
        }

        var sb = new StringBuilder().Append('<').Append(node.Name);

        foreach (var attribute in node.Attributes)
        {
            attribute.NotNull(nameof(attribute));
            sb.Append(' ')
              .Append(attribute.Name)
              .Append("=\"")
              .Append(HtmlDocument.HtmlEncode(attribute.Value))
              .Append('\"');
        }

        return sb.Append('>').ToString();
    }
}
