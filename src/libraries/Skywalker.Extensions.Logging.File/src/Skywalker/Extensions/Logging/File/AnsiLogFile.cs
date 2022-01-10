namespace Skywalker.Extensions.Logging.File;

/// <summary>
/// For consoles which understand the ANSI escape code sequences to represent color
/// </summary>
internal sealed class AnsiLogFile : IFile
{
    private readonly TextWriter _textWriter;

    public AnsiLogFile(bool stdErr = false)
    {
        var ext = stdErr ? "err" : "txt";
        var path = Path.Combine(AppContext.BaseDirectory, $"{DateTime.Now:yyyyMMdd}.{ext}");
        _textWriter = new StreamWriter(path, true);
    }

    public void Write(string message)
    {
        _textWriter.Write(message);
    }
}
