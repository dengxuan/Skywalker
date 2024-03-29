﻿using Microsoft.Extensions.FileProviders;

namespace Skywalker.Extensions.VirtualFileSystem;

public interface IDynamicFileProvider : IFileProvider
{
    void AddOrUpdate(IFileInfo fileInfo);

    bool Delete(string filePath);
}
