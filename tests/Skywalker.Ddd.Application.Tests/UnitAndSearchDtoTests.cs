using Xunit;
using Skywalker.Ddd.Application.Dtos;

namespace Skywalker.Ddd.Application.Tests;

public class UnitTests
{
    [Fact]
    public void Value_ShouldBeDefault()
    {
        ref readonly var value = ref Unit.Value;
        Assert.Equal(default(Unit), value);
    }

    [Fact]
    public void Task_ShouldBeCompleted()
    {
        Assert.True(Unit.Task.IsCompleted);
    }

    [Fact]
    public void CompareTo_Unit_ShouldReturnZero()
    {
        var a = Unit.Value;
        var b = Unit.Value;
        Assert.Equal(0, a.CompareTo(b));
    }

    [Fact]
    public void CompareTo_Object_ShouldReturnZero()
    {
        IComparable a = Unit.Value;
        Assert.Equal(0, a.CompareTo(Unit.Value));
    }

    [Fact]
    public void GetHashCode_ShouldReturnZero()
    {
        Assert.Equal(0, Unit.Value.GetHashCode());
    }

    [Fact]
    public void Equals_Unit_ShouldReturnTrue()
    {
        Assert.True(Unit.Value.Equals(Unit.Value));
    }

    [Fact]
    public void Equals_Object_Unit_ShouldReturnTrue()
    {
        Assert.True(Unit.Value.Equals((object)Unit.Value));
    }

    [Fact]
    public void Equals_NonUnit_ShouldReturnFalse()
    {
        Assert.False(Unit.Value.Equals("not a unit"));
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue()
    {
        Assert.True(Unit.Value == default);
    }

    [Fact]
    public void InequalityOperator_ShouldReturnFalse()
    {
        Assert.False(Unit.Value != default);
    }

    [Fact]
    public void ToString_ShouldReturnParens()
    {
        Assert.Equal("()", Unit.Value.ToString());
    }
}

public class SearchRequestDtoTests
{
    [Fact]
    public void SearchRequestDto_ShouldHaveAllProperties()
    {
        var dto = new SearchRequestDto("name", "Name DESC", 10, 50);
        Assert.Equal("name", dto.Filter);
        Assert.Equal("Name DESC", dto.Sorting);
        Assert.Equal(10, dto.Skip);
        Assert.Equal(50, dto.Limit);
    }

    [Fact]
    public void SearchRequestDto_Defaults_ShouldWork()
    {
        var dto = new SearchRequestDto(null, null);
        Assert.Null(dto.Filter);
        Assert.Null(dto.Sorting);
        Assert.Equal(0, dto.Skip);
        Assert.Equal(20, dto.Limit);
    }

    [Fact]
    public void LimitedRequestDto_DefaultLimit_ShouldBe20()
    {
        var dto = new LimitedRequestDto();
        Assert.Equal(20, dto.Limit);
    }

    [Fact]
    public void LimitedRequestDto_CustomLimit_ShouldWork()
    {
        var dto = new LimitedRequestDto(50);
        Assert.Equal(50, dto.Limit);
    }
}
