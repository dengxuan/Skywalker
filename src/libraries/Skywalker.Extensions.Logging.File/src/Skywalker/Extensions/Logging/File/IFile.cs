// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Skywalker.Extensions.Logging.File;

internal interface IFile
{
    void Write(string message);
}
