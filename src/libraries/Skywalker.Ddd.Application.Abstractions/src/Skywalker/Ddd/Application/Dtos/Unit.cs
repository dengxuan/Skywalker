using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.Application.Dtos;

[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
{
    private static readonly Unit s_value = new();

    public static ref readonly Unit Value => ref s_value;

    public static ValueTask<Unit> ValueTask => new(s_value);

    public int CompareTo(Unit other) => 0;

    int IComparable.CompareTo(object? _) => 0;

    public override int GetHashCode() => 0;

    public bool Equals(Unit other) => true;

    public override bool Equals(object? @object) => @object is Unit;

    public static bool operator ==(Unit _, Unit __) => true;

    public static bool operator !=(Unit _, Unit __) => false;

    public override string ToString() => "()";
}
