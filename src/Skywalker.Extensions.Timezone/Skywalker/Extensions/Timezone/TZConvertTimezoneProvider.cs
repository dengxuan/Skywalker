// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.Collections;
using TimeZoneConverter;

namespace Skywalker.Extensions.Timezone;

public class TZConvertTimezoneProvider : ITimezoneProvider
{
    public virtual List<NameValue> GetWindowsTimezones()
    {
        return TZConvert.KnownIanaTimeZoneNames.OrderBy(x => x).Select(x => new NameValue(x, x)).ToList();
    }

    public virtual List<NameValue> GetIanaTimezones()
    {
        return TZConvert.KnownIanaTimeZoneNames.OrderBy(x => x).Select(x => new NameValue(x, x)).ToList();
    }

    public virtual string WindowsToIana(string windowsTimeZoneId)
    {
        return TZConvert.WindowsToIana(windowsTimeZoneId);
    }

    public virtual string IanaToWindows(string ianaTimeZoneName)
    {
        return TZConvert.IanaToWindows(ianaTimeZoneName);
    }

    public virtual TimeZoneInfo GetTimeZoneInfo(string windowsOrIanaTimeZoneId)
    {
        return TZConvert.GetTimeZoneInfo(windowsOrIanaTimeZoneId);
    }
}
