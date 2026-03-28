using System.Net;
using Xunit;

namespace Skywalker.Extensions.Tests;

public class MiscExtensionsTests
{
    // DisposeAction
    [Fact]
    public void DisposeAction_ExecutesActionOnDispose()
    {
        var executed = false;
        var action = new DisposeAction(() => executed = true);
        action.Dispose();
        Assert.True(executed);
    }

    [Fact]
    public void DisposeAction_DoesNotExecuteTwice()
    {
        var count = 0;
        var action = new DisposeAction(() => count++);
        action.Dispose();
        action.Dispose();
        Assert.Equal(1, count);
    }

    [Fact]
    public void DisposeAction_Empty_DoesNotThrow()
    {
        DisposeAction.Empty.Dispose();
    }

    // NullDisposable
    [Fact]
    public void NullDisposable_Instance_IsSingleton()
    {
        Assert.Same(NullDisposable.Instance, NullDisposable.Instance);
    }

    [Fact]
    public void NullDisposable_Dispose_DoesNotThrow()
    {
        NullDisposable.Instance.Dispose();
    }

    // ExceptionExtensions.ReThrow
    [Fact]
    public void ReThrow_PreservesOriginalException()
    {
        var original = new InvalidOperationException("test");
        var ex = Assert.Throws<InvalidOperationException>(() => original.ReThrow());
        Assert.Same(original, ex);
    }

    // EnumExtensions
    [Fact]
    public void GetEnumName_ReturnsName()
    {
        Assert.Equal("Monday", DayOfWeek.Monday.GetEnumName());
    }

    [Fact]
    public void GetEnumDescription_ReturnsEmpty_WhenNoDescription()
    {
        Assert.Equal(string.Empty, DayOfWeek.Monday.GetEnumDescription());
    }

    // EventHandlerExtensions
    [Fact]
    public void InvokeSafely_InvokesHandler()
    {
        var invoked = false;
        EventHandler handler = (s, e) => invoked = true;
        handler.InvokeSafely(this);
        Assert.True(invoked);
    }

    [Fact]
    public void InvokeSafely_WithEventArgs_InvokesHandler()
    {
        var invoked = false;
        EventHandler handler = (s, e) => invoked = true;
        handler.InvokeSafely(this, EventArgs.Empty);
        Assert.True(invoked);
    }

    [Fact]
    public void InvokeSafely_Generic_InvokesHandler()
    {
        var invoked = false;
        EventHandler<EventArgs> handler = (s, e) => invoked = true;
        handler.InvokeSafely(this, EventArgs.Empty);
        Assert.True(invoked);
    }

    // CorrelationIdGenerator
    [Fact]
    public void CorrelationIdGenerator_ProducesUniqueIds()
    {
        var id1 = CorrelationIdGenerator.Instance.Next;
        var id2 = CorrelationIdGenerator.Instance.Next;
        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void CorrelationIdGenerator_HasExpectedFormat()
    {
        var id = CorrelationIdGenerator.Instance.Next;
        Assert.Equal(20, id.Length);
        Assert.Equal('-', id[6]);
    }

    // UriUtilities
    [Fact]
    public void IsHttpUri_TrueForHttp()
    {
        Assert.True(UriUtilities.IsHttpUri(new Uri("http://example.com")));
    }

    [Fact]
    public void IsHttpUri_TrueForHttps()
    {
        Assert.True(UriUtilities.IsHttpUri(new Uri("https://example.com")));
    }

    [Fact]
    public void IsSupportedNonSecureScheme_TrueForWs()
    {
        Assert.True(UriUtilities.IsNonSecureWebSocketScheme("ws"));
    }

    [Fact]
    public void IsSecureWebSocketScheme_TrueForWss()
    {
        Assert.True(UriUtilities.IsSecureWebSocketScheme("wss"));
    }

    [Fact]
    public void CanonicalizeUrl_ResolvesRelative()
    {
        var result = UriUtilities.CanonicalizeUrl("/path", "http://example.com");
        Assert.Equal("http://example.com/path", result);
    }

    [Fact]
    public void CanonicalizeUrl_ProtocolRelative()
    {
        var result = UriUtilities.CanonicalizeUrl("//cdn.example.com/file.js", "https://example.com");
        Assert.Equal("https://cdn.example.com/file.js", result);
    }

    // HttpStatusCodeExtensions
    [Theory]
    [InlineData(HttpStatusCode.OK, true)]
    [InlineData(HttpStatusCode.Created, true)]
    [InlineData(HttpStatusCode.BadRequest, false)]
    [InlineData(HttpStatusCode.InternalServerError, false)]
    public void IsSuccessStatusCode_Tests(HttpStatusCode code, bool expected)
    {
        Assert.Equal(expected, code.IsSuccessStatusCode());
    }

    // StreamExtensions
    [Fact]
    public void GetAllBytes_ReturnsAllBytes()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(data);
        var result = stream.GetAllBytes();
        Assert.Equal(data, result);
    }

    [Fact]
    public async Task GetAllBytesAsync_ReturnsAllBytes()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(data);
        var result = await stream.GetAllBytesAsync();
        Assert.Equal(data, result);
    }
}
