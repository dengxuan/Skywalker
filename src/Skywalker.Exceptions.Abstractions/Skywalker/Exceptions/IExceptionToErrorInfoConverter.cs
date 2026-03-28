namespace Skywalker.Exceptions;

public interface IExceptionToErrorInfoConverter
{
    ErrorInfo Convert(Exception exception, ExceptionConvertOptions? options = null);
}

public class ExceptionConvertOptions
{
    public bool IncludeDetails { get; set; }
    public bool SendNotifications { get; set; } = true;
}
