namespace Skywalker.IdentifierGenerator.Abstractions;

/// <summary>
/// Used to generate Identifier.
/// </summary>
public interface IIdentifierGenerator<TIdentifier> where TIdentifier : notnull
{
    TIdentifier Create();
}
