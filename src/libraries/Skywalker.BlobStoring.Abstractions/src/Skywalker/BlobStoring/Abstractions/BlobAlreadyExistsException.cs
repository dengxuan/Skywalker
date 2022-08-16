using System.Runtime.Serialization;
using Skywalker.ExceptionHandler;

namespace Skywalker.BlobStoring.Abstractions;

public class BlobAlreadyExistsException : SkywalkerException
{
    public BlobAlreadyExistsException()
    {

    }

    public BlobAlreadyExistsException(string message)
        : base(message)
    {

    }

    public BlobAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException)
    {

    }

    public BlobAlreadyExistsException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {

    }
}
