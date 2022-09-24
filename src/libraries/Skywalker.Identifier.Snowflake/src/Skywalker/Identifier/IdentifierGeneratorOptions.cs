namespace Skywalker.Identifier;

public class IdentifierGeneratorOptions
{
    public ushort WorkId { get; set; }

    public sbyte WorkIdLength { get; set; } = 10;

    public TimeSpan RefreshAliveInterval { get; set; }

    public DateTime StartTimestamp { get; set; } = DateTime.Parse("1970-01-01");
}
