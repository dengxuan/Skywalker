// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.Collections;

namespace Skywalker.Extensions.Timezone;

public interface ITimezoneProvider
{
    List<NameValue> GetWindowsTimezones();

    List<NameValue> GetIanaTimezones();

    string WindowsToIana(string windowsTimeZoneId);

    string IanaToWindows(string ianaTimeZoneName);

    TimeZoneInfo GetTimeZoneInfo(string windowsOrIanaTimeZoneId);
}
