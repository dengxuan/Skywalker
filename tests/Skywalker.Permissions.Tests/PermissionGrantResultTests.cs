using Skywalker.Permissions;

namespace Skywalker.Permissions.Tests;

public class PermissionGrantResultTests
{
    [Fact]
    public void PermissionGrantResult_ShouldHaveUndefinedValue()
    {
        // Assert
        Assert.True(Enum.IsDefined(PermissionGrantResult.Undefined));
        Assert.Equal(0, (int)PermissionGrantResult.Undefined);
    }

    [Fact]
    public void PermissionGrantResult_ShouldHaveGrantedValue()
    {
        // Assert
        Assert.True(Enum.IsDefined(PermissionGrantResult.Granted));
        Assert.Equal(1, (int)PermissionGrantResult.Granted);
    }

    [Fact]
    public void PermissionGrantResult_ShouldHaveProhibitedValue()
    {
        // Assert
        Assert.True(Enum.IsDefined(PermissionGrantResult.Prohibited));
        Assert.Equal(2, (int)PermissionGrantResult.Prohibited);
    }

    [Fact]
    public void PermissionGrantResult_ShouldHaveThreeValues()
    {
        // Assert
        Assert.Equal(3, Enum.GetValues<PermissionGrantResult>().Length);
    }
}

