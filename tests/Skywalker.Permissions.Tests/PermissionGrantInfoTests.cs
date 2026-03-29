// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Text.Json;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions.Tests;

public class PermissionGrantInfoTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var info = new PermissionGrantInfo("read", true, "U", "user1");

        Assert.Equal("read", info.Name);
        Assert.True(info.IsGranted);
        Assert.Equal("U", info.ProviderName);
        Assert.Equal("user1", info.ProviderKey);
    }

    [Fact]
    public void Constructor_DefaultOptionalParams()
    {
        var info = new PermissionGrantInfo("read", false);

        Assert.Equal("read", info.Name);
        Assert.False(info.IsGranted);
        Assert.Null(info.ProviderName);
        Assert.Null(info.ProviderKey);
    }

    [Fact]
    public void Constructor_NullName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentNullException>(() => new PermissionGrantInfo(null!, true));
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new PermissionGrantInfo("", true));
    }

    [Fact]
    public void Constructor_IsGrantedFalse_PreservesValue()
    {
        var info = new PermissionGrantInfo("deny_perm", false, "R", "admin");

        Assert.False(info.IsGranted);
    }

    // === JSON serialization/deserialization ===

    [Fact]
    public void JsonSerialize_RoundTrip_PreservesAllFields()
    {
        var original = new PermissionGrantInfo("read:users", true, "U", "user-123");

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<PermissionGrantInfo>(json)!;

        Assert.Equal(original.Name, deserialized.Name);
        Assert.Equal(original.IsGranted, deserialized.IsGranted);
        Assert.Equal(original.ProviderName, deserialized.ProviderName);
        Assert.Equal(original.ProviderKey, deserialized.ProviderKey);
    }

    [Fact]
    public void JsonSerialize_NotGranted_RoundTrip()
    {
        var original = new PermissionGrantInfo("write", false, "R", "admin");

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<PermissionGrantInfo>(json)!;

        Assert.False(deserialized.IsGranted);
        Assert.Equal("write", deserialized.Name);
    }

    [Fact]
    public void JsonSerialize_NullOptionalFields_RoundTrip()
    {
        var original = new PermissionGrantInfo("perm1", true);

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<PermissionGrantInfo>(json)!;

        Assert.Equal("perm1", deserialized.Name);
        Assert.True(deserialized.IsGranted);
        Assert.Null(deserialized.ProviderName);
        Assert.Null(deserialized.ProviderKey);
    }

    [Fact]
    public void JsonDeserialize_FromJsonString_WorksCorrectly()
    {
        var json = """{"Name":"manage","IsGranted":true,"ProviderName":"C","ProviderKey":"app1"}""";

        var info = JsonSerializer.Deserialize<PermissionGrantInfo>(json)!;

        Assert.Equal("manage", info.Name);
        Assert.True(info.IsGranted);
        Assert.Equal("C", info.ProviderName);
        Assert.Equal("app1", info.ProviderKey);
    }

    [Fact]
    public void JsonDeserialize_CaseInsensitive_WorksCorrectly()
    {
        var json = """{"name":"perm","isGranted":false,"providerName":"U","providerKey":"u1"}""";

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var info = JsonSerializer.Deserialize<PermissionGrantInfo>(json, options)!;

        Assert.Equal("perm", info.Name);
        Assert.False(info.IsGranted);
    }

    [Fact]
    public void JsonDeserialize_List_WorksCorrectly()
    {
        var json = """
        [
            {"Name":"p1","IsGranted":true,"ProviderName":"U","ProviderKey":"user1"},
            {"Name":"p2","IsGranted":false,"ProviderName":"R","ProviderKey":"admin"},
            {"Name":"p3","IsGranted":true,"ProviderName":null,"ProviderKey":null}
        ]
        """;

        var list = JsonSerializer.Deserialize<List<PermissionGrantInfo>>(json)!;

        Assert.Equal(3, list.Count);
        Assert.Equal("p1", list[0].Name);
        Assert.True(list[0].IsGranted);
        Assert.Equal("p2", list[1].Name);
        Assert.False(list[1].IsGranted);
        Assert.Equal("p3", list[2].Name);
        Assert.True(list[2].IsGranted);
        Assert.Null(list[2].ProviderName);
    }

    [Fact]
    public void JsonSerialize_ContainsIsGrantedField()
    {
        var info = new PermissionGrantInfo("test", false, "U", "u1");
        var json = JsonSerializer.Serialize(info);

        Assert.Contains("\"IsGranted\":false", json);
    }
}
