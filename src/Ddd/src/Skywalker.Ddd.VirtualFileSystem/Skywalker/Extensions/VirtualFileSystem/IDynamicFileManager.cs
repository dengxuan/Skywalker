using Microsoft.Extensions.FileProviders;

namespace Skywalker.VirtualFileSystem
{
    public interface IDynamicFileProvider : IFileProvider
    {
        void AddOrUpdate(IFileInfo fileInfo);

        bool Delete(string filePath);
    }
}
