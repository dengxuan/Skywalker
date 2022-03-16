namespace System;

/// <summary>
/// This class is used to simulate a Disposable that does nothing.
/// </summary>
public sealed class NullDisposable : IDisposable
{
    public static NullDisposable Instance { get; } = new NullDisposable();

    private NullDisposable() { }

    public void Dispose()
    {

    }
}
