using NSubstitute;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Extensions.Specifications;
using Skywalker.Settings.Abstractions;
using Skywalker.Settings.EntityFrameworkCore;
using Skywalker.Settings.EntityFrameworkCore.Entities;

namespace Skywalker.Settings.Tests;

public class SettingManagerTests
{
    [Fact]
    public async Task GetAllByProviderAsync_ReturnsStoredValues_ForExplicitProvider()
    {
        var repository = Substitute.For<IRepository<Setting>>();
        repository.GetListAsync(Arg.Is<ISpecification<Setting>>(specification => MatchesUserScope(specification)), Arg.Any<CancellationToken>())
            .Returns(
            [
                new Setting("Plain", "plain-value", "U", "user-1"),
                new Setting("Secret", "encrypted-value", "U", "user-1"),
            ]);

        var definitionManager = Substitute.For<ISettingDefinitionManager>();
        definitionManager.GetAll().Returns(
        [
            new SettingDefinition("Plain"),
            new SettingDefinition("Secret", isEncrypted: true),
        ]);

        var encryptionService = Substitute.For<ISettingEncryptionService>();
        encryptionService.Decrypt("encrypted-value").Returns("decrypted-value");

        var manager = new SettingManager(repository, definitionManager, encryptionService);

        var values = await manager.GetAllByProviderAsync("U", "user-1");

        Assert.Collection(
            values.OrderBy(value => value.Name),
            value =>
            {
                Assert.Equal("Plain", value.Name);
                Assert.Equal("plain-value", value.Value);
            },
            value =>
            {
                Assert.Equal("Secret", value.Name);
                Assert.Equal("decrypted-value", value.Value);
            });
    }

    [Fact]
    public async Task FindAsync_ReturnsNull_WhenSettingDoesNotExist()
    {
        var repository = Substitute.For<IRepository<Setting>>();
        repository.FindAsync(Arg.Any<ISpecification<Setting>>(), Arg.Any<CancellationToken>())
            .Returns((Setting?)null);

        var manager = new SettingManager(
            repository,
            Substitute.For<ISettingDefinitionManager>(),
            Substitute.For<ISettingEncryptionService>());

        var value = await manager.FindAsync("Missing", "U", "user-1");

        Assert.Null(value);
    }

    [Fact]
    public async Task FindAsync_DecryptsStoredValue_WhenDefinitionIsEncrypted()
    {
        var repository = Substitute.For<IRepository<Setting>>();
        repository.FindAsync(Arg.Is<ISpecification<Setting>>(specification => MatchesSecretUserScope(specification)), Arg.Any<CancellationToken>())
            .Returns(new Setting("Secret", "encrypted-value", "U", "user-1"));

        var definitionManager = Substitute.For<ISettingDefinitionManager>();
        definitionManager.GetOrNull("Secret").Returns(new SettingDefinition("Secret", isEncrypted: true));

        var encryptionService = Substitute.For<ISettingEncryptionService>();
        encryptionService.Decrypt("encrypted-value").Returns("decrypted-value");

        var manager = new SettingManager(repository, definitionManager, encryptionService);

        var value = await manager.FindAsync("Secret", "U", "user-1");

        Assert.NotNull(value);
        Assert.Equal("Secret", value.Name);
        Assert.Equal("decrypted-value", value.Value);
    }

    private static bool MatchesUserScope(ISpecification<Setting> specification)
    {
        var predicate = specification.ToExpression().Compile();
        return predicate(new Setting("Any", "value", "U", "user-1")) &&
               !predicate(new Setting("Any", "value", "U", "user-2")) &&
               !predicate(new Setting("Any", "value", "G"));
    }

    private static bool MatchesSecretUserScope(ISpecification<Setting> specification)
    {
        var predicate = specification.ToExpression().Compile();
        return predicate(new Setting("Secret", "value", "U", "user-1")) &&
               !predicate(new Setting("Secret", "value", "U", "user-2")) &&
               !predicate(new Setting("Other", "value", "U", "user-1"));
    }
}