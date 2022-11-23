using Skywalker.Extensions.Collections;

namespace Skywalker.Settings;

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
