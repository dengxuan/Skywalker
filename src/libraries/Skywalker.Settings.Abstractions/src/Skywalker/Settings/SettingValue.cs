using Skywalker.Extensions.Collections;

namespace Volo.Abp.Settings;

/// <summary>
/// 
/// </summary>
[Serializable]
public class SettingValue : NameValue
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public SettingValue(string name, string value) : base(name, value) { }
}
