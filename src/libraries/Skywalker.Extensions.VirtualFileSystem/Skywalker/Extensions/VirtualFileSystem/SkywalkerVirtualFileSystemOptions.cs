﻿namespace Skywalker.Extensions.VirtualFileSystem
{
    public class SkywalkerVirtualFileSystemOptions
    {
        public VirtualFileSetList FileSets { get; }
        
        public SkywalkerVirtualFileSystemOptions()
        {
            FileSets = new VirtualFileSetList();
        }
    }
}