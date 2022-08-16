// Licensed to the zshiot.com under one or more agreements.
// zshiot.com licenses this file to you under the license.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.BlobStoring.FileSystem;
public class OSPlatformProvider : IOSPlatformProvider, ITransientDependency
{
    public virtual OSPlatform GetCurrentOSPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OSPlatform.OSX; //MAC
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OSPlatform.Windows;
        }

        return OSPlatform.Linux;
    }
}
