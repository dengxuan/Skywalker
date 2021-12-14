﻿using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace Skywalker.Extensions.VirtualFileSystem
{
    public class VirtualFileProvider : IVirtualFileProvider
    {
        private readonly IFileProvider _hybridFileProvider;
        private readonly SkywalkerVirtualFileSystemOptions _options;

        public VirtualFileProvider(IOptions<SkywalkerVirtualFileSystemOptions> options, IDynamicFileProvider dynamicFileProvider)
        {
            _options = options.Value;
            _hybridFileProvider = CreateHybridProvider(dynamicFileProvider);
        }

        public virtual IFileInfo GetFileInfo(string subpath)
        {
            return _hybridFileProvider.GetFileInfo(subpath);
        }

        public virtual IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (subpath == "")
            {
                subpath = "/";
            }
            
            return _hybridFileProvider.GetDirectoryContents(subpath);
        }

        public virtual IChangeToken Watch(string filter)
        {
            return _hybridFileProvider.Watch(filter);
        }

        protected virtual IFileProvider CreateHybridProvider(IDynamicFileProvider dynamicFileProvider)
        {
            var fileProviders = new List<IFileProvider>
            {
                dynamicFileProvider
            };

            foreach (var fileSet in _options.FileSets.AsEnumerable().Reverse())
            {
                fileProviders.Add(fileSet.FileProvider);
            }

            return new CompositeFileProvider(fileProviders);
        }
    }
}