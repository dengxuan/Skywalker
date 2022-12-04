namespace Skywalker.Extensions.Snowflake;

public class SnowflakeGeneratorOptions
{
    public ushort WorkerId { get; set; }

    public sbyte WorkerIdLength { get; set; } = 10;

    public TimeSpan RefreshAliveInterval { get; set; }

    public DateTime StartTimestamp { get; set; } = DateTime.Parse("1970-01-01");
}
