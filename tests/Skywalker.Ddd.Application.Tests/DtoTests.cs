using Skywalker.Ddd.Application.Dtos;
using Xunit;

namespace Skywalker.Ddd.Application.Tests;

public class DtoTests
{
    #region EntityDto Tests

    [Fact]
    public void EntityDto_ShouldSetId()
    {
        // Arrange & Act
        var dto = new EntityDto<int>(123);

        // Assert
        Assert.Equal(123, dto.Id);
    }

    [Fact]
    public void EntityDto_ShouldWorkWithGuid()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var dto = new EntityDto<Guid>(id);

        // Assert
        Assert.Equal(id, dto.Id);
    }

    [Fact]
    public void EntityDto_ShouldWorkWithString()
    {
        // Arrange & Act
        var dto = new EntityDto<string>("test-id");

        // Assert
        Assert.Equal("test-id", dto.Id);
    }

    #endregion

    #region PagedResultDto Tests

    [Fact]
    public void PagedResultDto_ShouldSetTotalCount()
    {
        // Arrange
        var items = new List<string> { "a", "b", "c" };

        // Act
        var dto = new PagedResultDto<string>(100, items);

        // Assert
        Assert.Equal(100, dto.TotalCount);
    }

    [Fact]
    public void PagedResultDto_ShouldSetItems()
    {
        // Arrange
        var items = new List<string> { "a", "b", "c" };

        // Act
        var dto = new PagedResultDto<string>(100, items);

        // Assert
        Assert.Equal(3, dto.Items.Count);
        Assert.Contains("a", dto.Items);
    }

    [Fact]
    public void PagedResultDto_ShouldSupportEmptyItems()
    {
        // Arrange & Act
        var dto = new PagedResultDto<string>(0, new List<string>());

        // Assert
        Assert.Equal(0, dto.TotalCount);
        Assert.Empty(dto.Items);
    }

    #endregion

    #region ListResponseDto Tests

    [Fact]
    public void ListResponseDto_ShouldSetItems()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };

        // Act
        var dto = new ListResponseDto<int>(items);

        // Assert
        Assert.Equal(3, dto.Items.Count);
        Assert.Contains(2, dto.Items);
    }

    #endregion

    #region PagedRequestDto Tests

    [Fact]
    public void PagedRequestDto_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var dto = new PagedRequestDto();

        // Assert
        Assert.Equal(0, dto.Skip);
        Assert.Equal(20, dto.Limit);
    }

    [Fact]
    public void PagedRequestDto_ShouldSetSkipAndLimit()
    {
        // Arrange & Act
        var dto = new PagedRequestDto(10, 50);

        // Assert
        Assert.Equal(10, dto.Skip);
        Assert.Equal(50, dto.Limit);
    }

    #endregion

    #region Serializable Tests

    [Fact]
    public void PagedResultDto_ShouldBeSerializable()
    {
        // Assert
        Assert.NotNull(typeof(PagedResultDto<>).GetCustomAttributes(typeof(SerializableAttribute), false).FirstOrDefault());
    }

    [Fact]
    public void PagedRequestDto_ShouldBeSerializable()
    {
        // Assert
        Assert.NotNull(typeof(PagedRequestDto).GetCustomAttributes(typeof(SerializableAttribute), false).FirstOrDefault());
    }

    #endregion
}

