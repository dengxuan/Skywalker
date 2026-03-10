using Skywalker.Extensions.Emailing.Template;

namespace Skywalker.Extensions.Emailing.Template.Tests;

public class EmailTemplateOptionsTests
{
    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        var options = new EmailTemplateOptions();

        Assert.Equal(".Subject", options.SubjectTemplateSuffix);
        Assert.Equal(".Body", options.BodyTemplateSuffix);
        Assert.Null(options.DefaultLayoutTemplate);
        Assert.Equal("EmailTemplates", options.TemplateBasePath);
    }

    [Fact]
    public void SetSubjectTemplateSuffix_ShouldUpdateValue()
    {
        var options = new EmailTemplateOptions
        {
            SubjectTemplateSuffix = ".Title"
        };

        Assert.Equal(".Title", options.SubjectTemplateSuffix);
    }

    [Fact]
    public void SetBodyTemplateSuffix_ShouldUpdateValue()
    {
        var options = new EmailTemplateOptions
        {
            BodyTemplateSuffix = ".Content"
        };

        Assert.Equal(".Content", options.BodyTemplateSuffix);
    }

    [Fact]
    public void SetDefaultLayoutTemplate_ShouldUpdateValue()
    {
        var options = new EmailTemplateOptions
        {
            DefaultLayoutTemplate = "MyLayout"
        };

        Assert.Equal("MyLayout", options.DefaultLayoutTemplate);
    }

    [Fact]
    public void SetTemplateBasePath_ShouldUpdateValue()
    {
        var options = new EmailTemplateOptions
        {
            TemplateBasePath = "Templates/Emails"
        };

        Assert.Equal("Templates/Emails", options.TemplateBasePath);
    }
}

