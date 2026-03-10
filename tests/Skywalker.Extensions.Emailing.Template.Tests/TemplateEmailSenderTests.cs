using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Extensions.Emailing.Template;
using Skywalker.Template.Abstractions;

namespace Skywalker.Extensions.Emailing.Template.Tests;

public class TemplateEmailSenderTests
{
    private readonly IEmailSender _emailSender;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly IOptions<EmailTemplateOptions> _options;
    private readonly TemplateEmailSender _sut;

    public TemplateEmailSenderTests()
    {
        _emailSender = Substitute.For<IEmailSender>();
        _templateRenderer = Substitute.For<ITemplateRenderer>();
        _options = Options.Create(new EmailTemplateOptions());
        _sut = new TemplateEmailSender(_emailSender, _templateRenderer, _options);
    }

    [Fact]
    public async Task SendAsync_ShouldRenderSubjectAndBody()
    {
        // Arrange
        const string templateName = "TestTemplate";
        const string to = "user@example.com";
        var model = new { Name = "John" };

        _templateRenderer.RenderAsync($"{templateName}.Subject", model, null, null)
            .Returns("Test Subject");
        _templateRenderer.RenderAsync($"{templateName}.Body", model, null, null)
            .Returns("<p>Test Body</p>");

        // Act
        await _sut.SendAsync(to, templateName, model);

        // Assert
        await _templateRenderer.Received(1).RenderAsync($"{templateName}.Subject", model, null, null);
        await _templateRenderer.Received(1).RenderAsync($"{templateName}.Body", model, null, null);
        await _emailSender.Received(1).SendAsync(to, "Test Subject", "<p>Test Body</p>", true);
    }

    [Fact]
    public async Task SendAsync_WithFrom_ShouldIncludeSender()
    {
        // Arrange
        const string from = "sender@example.com";
        const string to = "user@example.com";
        const string templateName = "TestTemplate";

        _templateRenderer.RenderAsync(Arg.Any<string>(), Arg.Any<object?>(), null, null)
            .Returns("Content");

        // Act
        await _sut.SendAsync(from, to, templateName);

        // Assert
        await _emailSender.Received(1).SendAsync(from, to, "Content", "Content", true);
    }

    [Fact]
    public async Task SendAsync_WithCultureName_ShouldPassCultureToRenderer()
    {
        // Arrange
        const string templateName = "TestTemplate";
        const string to = "user@example.com";
        const string cultureName = "zh-CN";

        _templateRenderer.RenderAsync(Arg.Any<string>(), null, cultureName, null)
            .Returns("Content");

        // Act
        await _sut.SendAsync(to, templateName, cultureName: cultureName);

        // Assert
        await _templateRenderer.Received().RenderAsync($"{templateName}.Subject", null, cultureName, null);
        await _templateRenderer.Received().RenderAsync($"{templateName}.Body", null, cultureName, null);
    }

    [Fact]
    public async Task SendAsync_WithMultipleRecipients_ShouldSendToAll()
    {
        // Arrange
        var recipients = new[] { "user1@example.com", "user2@example.com", "user3@example.com" };
        const string templateName = "TestTemplate";

        _templateRenderer.RenderAsync(Arg.Any<string>(), null, null, null)
            .Returns("Content");

        // Act
        await _sut.SendAsync(recipients, templateName);

        // Assert
        await _emailSender.Received(1).SendAsync(Arg.Any<System.Net.Mail.MailMessage>(), true);
    }
}

