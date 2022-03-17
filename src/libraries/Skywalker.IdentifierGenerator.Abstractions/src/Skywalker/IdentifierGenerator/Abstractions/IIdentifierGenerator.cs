namespace Skywalker.IdentifierGenerator.Abstractions;

/// <summary>
/// Used to generate Identifier.
/// </summary>
public interface IIdentifierGenerator<TIdentifier> where TIdentifier : struct
{
    TIdentifier Create();
}
