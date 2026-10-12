using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Skywalker.Template;

/// <summary>
/// 未注册 <see cref="IStringLocalizerFactory"/> 时渲染引擎使用的兜底工厂：
/// 返回的本地化器原样输出 key（带参数时按 <see cref="string.Format(IFormatProvider, string, object[])"/> 格式化），
/// 使不使用模板本地化的应用无需引入本地化家族即可渲染（#312）。
/// </summary>
public sealed class NullStringLocalizerFactory : IStringLocalizerFactory
{
    public static NullStringLocalizerFactory Instance { get; } = new();

    private NullStringLocalizerFactory()
    {
    }

    public IStringLocalizer Create(Type resourceSource) => PassThroughStringLocalizer.Instance;

    public IStringLocalizer Create(string baseName, string location) => PassThroughStringLocalizer.Instance;
}

internal sealed class PassThroughStringLocalizer : IStringLocalizer
{
    public static PassThroughStringLocalizer Instance { get; } = new();

    private PassThroughStringLocalizer()
    {
    }

    public LocalizedString this[string name] => new(name, name, resourceNotFound: true);

    public LocalizedString this[string name, params object[] arguments]
        => new(name, string.Format(CultureInfo.CurrentCulture, name, arguments), resourceNotFound: true);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];
}
