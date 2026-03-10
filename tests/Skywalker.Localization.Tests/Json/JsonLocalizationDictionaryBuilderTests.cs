using Skywalker.Localization.Json;
using Xunit;

namespace Skywalker.Localization.Tests.Json;

public class JsonLocalizationDictionaryBuilderTests
{
    [Fact]
    public void Build_SimpleJson_ParsesCorrectly()
    {
        var json = """
        {
            "Hello": "你好",
            "Goodbye": "再见"
        }
        """;

        var dictionary = JsonLocalizationDictionaryBuilder.Build(json);

        Assert.Equal(2, dictionary.Count);
        Assert.Equal("你好", dictionary["Hello"]);
        Assert.Equal("再见", dictionary["Goodbye"]);
    }

    [Fact]
    public void Build_NestedJson_UsesColonSeparator()
    {
        var json = """
        {
            "Validation": {
                "Required": "字段是必填的",
                "MaxLength": "最大长度为 {0}"
            }
        }
        """;

        var dictionary = JsonLocalizationDictionaryBuilder.Build(json);

        Assert.Equal(2, dictionary.Count);
        Assert.Equal("字段是必填的", dictionary["Validation:Required"]);
        Assert.Equal("最大长度为 {0}", dictionary["Validation:MaxLength"]);
    }

    [Fact]
    public void Build_WithTextsWrapper_SkipsTextsProperty()
    {
        var json = """
        {
            "culture": "zh-CN",
            "texts": {
                "Hello": "你好",
                "Goodbye": "再见"
            }
        }
        """;

        var dictionary = JsonLocalizationDictionaryBuilder.Build(json);

        Assert.Equal(2, dictionary.Count);
        Assert.Equal("你好", dictionary["Hello"]);
        Assert.False(dictionary.ContainsKey("culture"));
        Assert.False(dictionary.ContainsKey("texts"));
    }

    [Fact]
    public void Build_ArrayValues_UsesIndexSuffix()
    {
        var json = """
        {
            "Colors": ["红色", "蓝色", "绿色"]
        }
        """;

        var dictionary = JsonLocalizationDictionaryBuilder.Build(json);

        Assert.Equal(3, dictionary.Count);
        Assert.Equal("红色", dictionary["Colors[0]"]);
        Assert.Equal("蓝色", dictionary["Colors[1]"]);
        Assert.Equal("绿色", dictionary["Colors[2]"]);
    }

    [Fact]
    public void Build_EmptyJson_ReturnsEmptyDictionary()
    {
        var dictionary = JsonLocalizationDictionaryBuilder.Build("");

        Assert.Empty(dictionary);
    }

    [Fact]
    public void Build_EmptyObject_ReturnsEmptyDictionary()
    {
        var dictionary = JsonLocalizationDictionaryBuilder.Build("{}");

        Assert.Empty(dictionary);
    }

    [Fact]
    public void Build_CaseInsensitiveKeys()
    {
        var json = """
        {
            "Hello": "你好"
        }
        """;

        var dictionary = JsonLocalizationDictionaryBuilder.Build(json);

        Assert.Equal("你好", dictionary["hello"]);
        Assert.Equal("你好", dictionary["HELLO"]);
    }

    [Fact]
    public void Build_NumberValues_ParsesAsString()
    {
        var json = """
        {
            "MaxCount": 100
        }
        """;

        var dictionary = JsonLocalizationDictionaryBuilder.Build(json);

        Assert.Equal("100", dictionary["MaxCount"]);
    }

    [Fact]
    public void Build_BooleanValues_ParsesAsString()
    {
        var json = """
        {
            "IsEnabled": true,
            "IsDisabled": false
        }
        """;

        var dictionary = JsonLocalizationDictionaryBuilder.Build(json);

        Assert.Equal("true", dictionary["IsEnabled"]);
        Assert.Equal("false", dictionary["IsDisabled"]);
    }
}

