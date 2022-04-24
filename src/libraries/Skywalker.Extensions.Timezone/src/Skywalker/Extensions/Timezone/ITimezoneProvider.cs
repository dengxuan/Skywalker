// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.Collections;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Extensions.Timezone;

public interface ITimezoneProvider : ISingletonDependency
{
    List<NameValue> GetWindowsTimezones();

    List<NameValue> GetIanaTimezones();

    string WindowsToIana(string windowsTimeZoneId);

    string IanaToWindows(string ianaTimeZoneName);

    TimeZoneInfo GetTimeZoneInfo(string windowsOrIanaTimeZoneId);
}
