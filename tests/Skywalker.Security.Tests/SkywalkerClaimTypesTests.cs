using System.Security.Claims;
using Skywalker.Security.Claims;
using Xunit;

namespace Skywalker.Security.Tests;

public class SkywalkerClaimTypesTests
{
    [Fact]
    public void UserId_ShouldDefaultToNameIdentifier()
    {
        // Assert
        Assert.Equal(ClaimTypes.NameIdentifier, SkywalkerClaimTypes.UserId);
    }

    [Fact]
    public void UserName_ShouldDefaultToName()
    {
        // Assert
        Assert.Equal(ClaimTypes.Name, SkywalkerClaimTypes.UserName);
    }

    [Fact]
    public void NickName_ShouldDefaultToNickname()
    {
        // Assert
        Assert.Equal("nickname", SkywalkerClaimTypes.NickName);
    }

    [Fact]
    public void Avatar_ShouldDefaultToAvatar()
    {
        // Assert
        Assert.Equal("avatar", SkywalkerClaimTypes.Avatar);
    }

    [Fact]
    public void TgUserName_ShouldDefaultToTgUsername()
    {
        // Assert
        Assert.Equal("tg_username", SkywalkerClaimTypes.TgUserName);
    }

    [Fact]
    public void Role_ShouldDefaultToClaimTypesRole()
    {
        // Assert
        Assert.Equal(ClaimTypes.Role, SkywalkerClaimTypes.Role);
    }

    [Fact]
    public void Email_ShouldDefaultToClaimTypesEmail()
    {
        // Assert
        Assert.Equal(ClaimTypes.Email, SkywalkerClaimTypes.Email);
    }

    [Fact]
    public void EmailVerified_ShouldDefaultToEmailVerified()
    {
        // Assert
        Assert.Equal("email_verified", SkywalkerClaimTypes.EmailVerified);
    }

    [Fact]
    public void PhoneNumber_ShouldDefaultToPhoneNumber()
    {
        // Assert
        Assert.Equal("phone_number", SkywalkerClaimTypes.PhoneNumber);
    }

    [Fact]
    public void PhoneNumberVerified_ShouldDefaultToPhoneNumberVerified()
    {
        // Assert
        Assert.Equal("phone_number_verified", SkywalkerClaimTypes.PhoneNumberVerified);
    }

    [Fact]
    public void ClientId_ShouldDefaultToClientId()
    {
        // Assert
        Assert.Equal("client_id", SkywalkerClaimTypes.ClientId);
    }

    [Fact]
    public void TenantcyId_ShouldDefaultToTenantcyId()
    {
        // Assert
        Assert.Equal("tenantcy_id", SkywalkerClaimTypes.TenantcyId);
    }

    [Fact]
    public void SessionId_ShouldDefaultToSessionId()
    {
        // Assert
        Assert.Equal("session_id", SkywalkerClaimTypes.SessionId);
    }

    [Fact]
    public void RememberMe_ShouldDefaultToRememberMe()
    {
        // Assert
        Assert.Equal("remember_me", SkywalkerClaimTypes.RememberMe);
    }

    [Fact]
    public void UserId_ShouldBeSettable()
    {
        // Arrange
        var originalValue = SkywalkerClaimTypes.UserId;

        try
        {
            // Act
            SkywalkerClaimTypes.UserId = "custom_user_id";

            // Assert
            Assert.Equal("custom_user_id", SkywalkerClaimTypes.UserId);
        }
        finally
        {
            // Cleanup
            SkywalkerClaimTypes.UserId = originalValue;
        }
    }
}

