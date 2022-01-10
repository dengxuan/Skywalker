// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace Skywalker.Extensions.Logging.File;

#if NET5_0_OR_GREATER
[UnsupportedOSPlatform("browser")]
#endif
internal class FileLoggerProcessor : IDisposable
{
    private const int MaxQueuedMessages = 1024;

    private readonly BlockingCollection<LogMessageEntry> _messageQueue = new(MaxQueuedMessages);
    private readonly Thread _outputThread;

    public IFile File;

    public IFile ErrorFile;

    public FileLoggerProcessor(IFile file, IFile errFile)
    {
        File = file;
        ErrorFile = errFile;
        // Start File message queue processor
        _outputThread = new Thread(ProcessLogQueue)
        {
            IsBackground = true,
            Name = "File logger queue processing thread"
        };
        _outputThread.Start();
    }

    public virtual void EnqueueMessage(LogMessageEntry message)
    {
        if (!_messageQueue.IsAddingCompleted)
        {
            try
            {
                _messageQueue.Add(message);
                return;
            }
            catch (InvalidOperationException) { }
        }

        // Adding is completed so just log the message
        try
        {
            WriteMessage(message);
        }
        catch (Exception) { }
    }

    // for testing
    internal virtual void WriteMessage(LogMessageEntry entry)
    {
        var file = entry.LogAsError ? ErrorFile : File;
        file.Write(entry.Message);
    }

    private void ProcessLogQueue()
    {
        try
        {
            foreach (var message in _messageQueue.GetConsumingEnumerable())
            {
                WriteMessage(message);
            }
        }
        catch
        {
            try
            {
                _messageQueue.CompleteAdding();
            }
            catch { }
        }
    }

    public void Dispose()
    {
        _messageQueue.CompleteAdding();

        try
        {
            _outputThread.Join(1500); // with timeout in-case Console is locked by user input
        }
        catch (ThreadStateException) { }
    }
}
