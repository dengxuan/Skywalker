using System;

namespace Skywalker.Extensions.Linq.Parser.SupportedOperands;

internal interface ISubtractSignatures : IAddSignatures
{
    void F(DateTime x, DateTime y);
    void F(DateTime? x, DateTime? y);
}
