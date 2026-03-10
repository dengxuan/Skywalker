namespace System.Net;

public static class HttpStatusCodeExtensions
{
    public static bool IsSuccessStatusCode(this HttpStatusCode statusCode) => statusCode is >= HttpStatusCode.OK and <= ((HttpStatusCode)299);
}
