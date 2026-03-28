namespace Skywalker.Exceptions;

public class ErrorInfo
{
    public string? Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public ValidationErrorInfo[]? ValidationErrors { get; set; }

    public ErrorInfo() { }
    public ErrorInfo(string message) { Message = message; }
    public ErrorInfo(string? code, string message) { Code = code; Message = message; }
    public ErrorInfo(string? code, string message, string? details) { Code = code; Message = message; Details = details; }
}

public class ValidationErrorInfo
{
    public string Message { get; set; } = string.Empty;
    public string[]? Members { get; set; }

    public ValidationErrorInfo() { }
    public ValidationErrorInfo(string message) { Message = message; }
    public ValidationErrorInfo(string message, string[] members) { Message = message; Members = members; }
}
