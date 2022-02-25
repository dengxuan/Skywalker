﻿namespace Skywalker;

public class UserFriendlyException : Exception, IHasErrorCode
{
    public string? Code { get; }

    public UserFriendlyException(string? code) : this(code, string.Empty)
    {
    }

    public UserFriendlyException(string? code, string message) : base(message)
    {
        Code = code;
    }
}