namespace Skywalker.IdentifierGenerator;

public class IdentifierGeneratorOptions
{
    public int WorkId { get; set; }

    public int WorkIdLength { get; set; } = 10;

    public TimeSpan RefreshAliveInterval { get; set; }

    public DateTime StartTimestamp { get; set; } = DateTime.Parse("1970-01-01");
}
