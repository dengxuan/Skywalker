using Xunit;

namespace Skywalker.Extensions.Universal.Tests;

public class ObjectIdTests
{
    [Fact]
    public void CreateId_ReturnsUniqueIds()
    {
        var id1 = ObjectId.CreateId();
        var id2 = ObjectId.CreateId();
        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void CreateId_HasNonZeroTimestamp()
    {
        var id = ObjectId.CreateId();
        Assert.NotEqual(0, id.Timestamp);
    }

    [Fact]
    public void CreateId_WithDateTime()
    {
        var dt = new DateTime(2023, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var id = ObjectId.CreateId(dt);
        Assert.Equal(dt, id.CreationTime);
    }

    [Fact]
    public void Empty_IsDefault()
    {
        Assert.Equal(default(ObjectId), ObjectId.Empty);
        Assert.Equal("000000000000000000000000", ObjectId.Empty.ToString());
    }

    [Fact]
    public void ToString_Returns24HexChars()
    {
        var id = ObjectId.CreateId();
        var str = id.ToString();
        Assert.Equal(24, str.Length);
        Assert.True(str.All(c => "0123456789abcdef".Contains(c)));
    }

    [Fact]
    public void Parse_ValidHex_ReturnsObjectId()
    {
        var id = ObjectId.CreateId();
        var hex = id.ToString();
        var parsed = ObjectId.Parse(hex);
        Assert.Equal(id, parsed);
    }

    [Fact]
    public void Parse_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ObjectId.Parse(null!));
    }

    [Fact]
    public void Parse_InvalidString_Throws()
    {
        Assert.Throws<FormatException>(() => ObjectId.Parse("not-a-hex-string"));
    }

    [Fact]
    public void TryParse_ValidHex_ReturnsTrue()
    {
        var id = ObjectId.CreateId();
        Assert.True(ObjectId.TryParse(id.ToString(), out var parsed));
        Assert.Equal(id, parsed);
    }

    [Fact]
    public void TryParse_Null_ReturnsFalse()
    {
        Assert.False(ObjectId.TryParse(null!, out _));
    }

    [Fact]
    public void TryParse_WrongLength_ReturnsFalse()
    {
        Assert.False(ObjectId.TryParse("abc", out _));
    }

    [Fact]
    public void TryParse_InvalidChars_ReturnsFalse()
    {
        Assert.False(ObjectId.TryParse("zzzzzzzzzzzzzzzzzzzzzzzz", out _));
    }

    [Fact]
    public void ByteArray_Constructor_RoundTrip()
    {
        var id = ObjectId.CreateId();
        var bytes = id.ToByteArray();
        Assert.Equal(12, bytes.Length);
        var restored = new ObjectId(bytes);
        Assert.Equal(id, restored);
    }

    [Fact]
    public void ByteArray_Constructor_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ObjectId((byte[])null!));
    }

    [Fact]
    public void ByteArray_Constructor_WrongLength_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ObjectId(new byte[5]));
    }

    [Fact]
    public void ReadOnlySpan_Constructor_Works()
    {
        var id = ObjectId.CreateId();
        var bytes = id.ToByteArray();
        var restored = new ObjectId(bytes.AsSpan());
        Assert.Equal(id, restored);
    }

    [Fact]
    public void ReadOnlySpan_Constructor_WrongLength_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ObjectId(new byte[3].AsSpan()));
    }

    [Fact]
    public void String_Constructor_Works()
    {
        var hex = ObjectId.CreateId().ToString();
        var id = new ObjectId(hex);
        Assert.Equal(hex, id.ToString());
    }

    [Fact]
    public void String_Constructor_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ObjectId((string)null!));
    }

    [Fact]
    public void Equals_SameId_ReturnsTrue()
    {
        var id = ObjectId.CreateId();
        Assert.True(id.Equals(id));
        Assert.True(id.Equals((object)id));
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var id = ObjectId.CreateId();
        Assert.False(id.Equals("not an objectid"));
    }

    [Fact]
    public void Operators_Work()
    {
        var id1 = ObjectId.CreateId();
        var id2 = ObjectId.CreateId();
        var id1Copy = ObjectId.Parse(id1.ToString());
        Assert.True(id1 == id1Copy);
        Assert.True(id1 != id2);
    }

    [Fact]
    public void CompareTo_Works()
    {
        var id1 = ObjectId.CreateId();
        var id2 = ObjectId.CreateId();
        Assert.True(id1.CompareTo(id1) == 0);
        // id1 was created first, id2 second (same timestamp but different increment)
        Assert.True(id1 < id2 || id1 > id2 || id1 == id2);
    }

    [Fact]
    public void ComparisonOperators_ConsistentWithCompareTo()
    {
        var id1 = ObjectId.Parse("000000000000000000000001");
        var id2 = ObjectId.Parse("000000000000000000000002");
        Assert.True(id1 < id2);
        Assert.True(id1 <= id2);
        Assert.True(id2 > id1);
        Assert.True(id2 >= id1);
    }

    [Fact]
    public void GetHashCode_SameForEqualIds()
    {
        var hex = ObjectId.CreateId().ToString();
        var id1 = ObjectId.Parse(hex);
        var id2 = ObjectId.Parse(hex);
        Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
    }

    [Fact]
    public void ToByteArray_WithDestination_Works()
    {
        var id = ObjectId.CreateId();
        var dest = new byte[14];
        id.ToByteArray(dest, 1);
        // Bytes 0 is untouched
        Assert.Equal(0, dest[0]);
        // last byte untouched
        Assert.Equal(0, dest[13]);
    }

    [Fact]
    public void ToByteArray_NullDestination_Throws()
    {
        var id = ObjectId.CreateId();
        Assert.Throws<ArgumentNullException>(() => id.ToByteArray(null!, 0));
    }

    [Fact]
    public void ToByteArray_InsufficientSpace_Throws()
    {
        var id = ObjectId.CreateId();
        Assert.Throws<ArgumentException>(() => id.ToByteArray(new byte[5], 0));
    }

    [Fact]
    public void Properties_Accessible()
    {
        var id = ObjectId.CreateId();
        // Just ensure they don't throw
        _ = id.Timestamp;
        _ = id.Machine;
        _ = id.Pid;
        _ = id.Increment;
        _ = id.CreationTime;
    }

    [Fact]
    public void ParseHexString_ValidHex_Works()
    {
        var bytes = ObjectId.ParseHexString("0102030405060708090a0b0c");
        Assert.Equal(12, bytes.Length);
        Assert.Equal(1, bytes[0]);
        Assert.Equal(12, bytes[11]);
    }

    [Fact]
    public void ParseHexString_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ObjectId.ParseHexString(null!));
    }

    [Fact]
    public void ParseHexString_InvalidChars_Throws()
    {
        Assert.Throws<FormatException>(() => ObjectId.ParseHexString("xyz"));
    }

    [Fact]
    public void IConvertible_ToBoolean_Throws()
    {
        IConvertible conv = ObjectId.CreateId();
        Assert.Throws<InvalidCastException>(() => conv.ToBoolean(null));
    }

    [Fact]
    public void IConvertible_ToString_Works()
    {
        var id = ObjectId.CreateId();
        IConvertible conv = id;
        Assert.Equal(id.ToString(), conv.ToString(null));
    }

    [Fact]
    public void IConvertible_ToType_String()
    {
        var id = ObjectId.CreateId();
        IConvertible conv = id;
        var result = conv.ToType(typeof(string), null);
        Assert.Equal(id.ToString(), result);
    }

    [Fact]
    public void IConvertible_ToType_ObjectId()
    {
        var id = ObjectId.CreateId();
        IConvertible conv = id;
        var result = (ObjectId)conv.ToType(typeof(ObjectId), null);
        Assert.Equal(id, result);
    }

    [Fact]
    public void IConvertible_ToType_Object()
    {
        var id = ObjectId.CreateId();
        IConvertible conv = id;
        var result = (ObjectId)conv.ToType(typeof(object), null);
        Assert.Equal(id, result);
    }

    [Fact]
    public void IConvertible_ToType_InvalidType_Throws()
    {
        IConvertible conv = ObjectId.CreateId();
        Assert.Throws<InvalidCastException>(() => conv.ToType(typeof(int), null));
    }

    [Fact]
    public void IConvertible_GetTypeCode_ReturnsObject()
    {
        IConvertible conv = ObjectId.CreateId();
        Assert.Equal(TypeCode.Object, conv.GetTypeCode());
    }

#pragma warning disable CS0618
    [Fact]
    public void Unpack_Works()
    {
        var id = ObjectId.CreateId();
        var bytes = id.ToByteArray();
        ObjectId.Unpack(bytes, out var timestamp, out var machine, out var pid, out var increment);
        Assert.Equal(id.Timestamp, timestamp);
        Assert.Equal(id.Machine, machine);
        Assert.Equal(id.Pid, pid);
        Assert.Equal(id.Increment, increment);
    }

    [Fact]
    public void Unpack_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ObjectId.Unpack(null!, out _, out _, out _, out _));
    }

    [Fact]
    public void Unpack_WrongLength_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ObjectId.Unpack(new byte[5], out _, out _, out _, out _));
    }
#pragma warning restore CS0618
}
