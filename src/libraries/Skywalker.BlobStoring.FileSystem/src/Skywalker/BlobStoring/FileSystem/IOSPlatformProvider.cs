// Licensed to the zshiot.com under one or more agreements.
// zshiot.com licenses this file to you under the license.

using System.Runtime.InteropServices;

namespace Skywalker.BlobStoring.FileSystem;
public interface IOSPlatformProvider
{
    OSPlatform GetCurrentOSPlatform();
}
