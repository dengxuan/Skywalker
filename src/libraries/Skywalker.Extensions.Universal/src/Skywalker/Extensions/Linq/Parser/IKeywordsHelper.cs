// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.Linq.Parser;

interface IKeywordsHelper
{
    bool TryGetValue(string name, out object? type);
}
