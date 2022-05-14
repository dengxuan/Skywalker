// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace System.Security.Claims;

public readonly record struct WeChatUser(string UnionId, string OpenId, string Avatar, string Nickname, Genders Gender);
