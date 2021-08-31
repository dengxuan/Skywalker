namespace Skywalker.Spider.Abstractions;

public readonly struct SpiderId
{
    public string Id { get; }

    public string Name { get; }

    public SpiderId(string id, string name)
    {
        Id = id.LengthOf(nameof(id), 36, 1);
        Name = name;
    }

    public override string ToString()
    {
        return Id;
    }
}
