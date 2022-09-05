// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Security.Clients;

public interface ICurrentClient
{
    bool IsAuthenticated { get; }

    string? Id { get; }
}
