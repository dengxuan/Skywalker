using System.IO;

namespace Abstractions
{
    public interface IEncryptionProvider
    {
        /// <summary>
        /// The encoding name used in the 'Accept-Encoding' request header and 'Content-Encoding' response header.
        /// </summary>
        string EncodingName { get; }

        /// <summary>
        /// Indicates if the given provider supports Flush and FlushAsync. If not, compression may be disabled in some scenarios.
        /// </summary>
        bool SupportsFlush { get; }

        /// <summary>
        /// Create a new compression stream.
        /// </summary>
        /// <param name="outputStream">The stream where the compressed data have to be written.</param>
        /// <returns>The compression stream.</returns>
        Stream CreateStream(Stream outputStream);
    }
}
