namespace Skywalker.BlobStoring.Abstractions;

public abstract class BlobProviderBase : IBlobProvider
{
    public abstract Task SaveAsync(BlobProviderSaveArgs args);

    public abstract Task<bool> DeleteAsync(BlobProviderDeleteArgs args);

    public abstract Task<bool> ExistsAsync(BlobProviderExistsArgs args);

    public abstract Task<Stream?> GetOrNullAsync(BlobProviderGetArgs args);

    protected virtual async Task<Stream?> TryCopyToMemoryStreamAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream == null)
        {
            return null;
        }

        // 10M缓冲区
        var bufferSize = 1024 * 1024 * 10;

        var buffer = new byte[1024 * 1024 * 10];
        var memoryStream = new MemoryStream();

        // 表示当前读取的内存流的长度
        int count;
        while ((count = await stream.ReadAsync(buffer, 0, bufferSize, cancellationToken)) != 0)
        {
            await memoryStream.WriteAsync(buffer, 0, count, cancellationToken);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}
