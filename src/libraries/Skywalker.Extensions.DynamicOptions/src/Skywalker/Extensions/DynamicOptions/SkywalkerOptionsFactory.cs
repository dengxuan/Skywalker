using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Skywalker.Extensions.DynamicOptions;

//TODO: Derive from OptionsFactory when this is released: https://github.com/aspnet/Options/pull/258 (or completely remove this!)
// https://github.com/dotnet/runtime/blob/master/src/libraries/Microsoft.Extensions.Options/src/OptionsFactory.cs
/// <summary>
/// 
/// </summary>
/// <typeparam name="TOptions"></typeparam>
public class SkywalkerOptionsFactory<TOptions> : IOptionsFactory<TOptions> where TOptions : class, new()
{
    private readonly IEnumerable<IConfigureOptions<TOptions>> _setups;
    private readonly IEnumerable<IPostConfigureOptions<TOptions>> _postConfigures;
    private readonly IEnumerable<IValidateOptions<TOptions>>? _validations;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="setups"></param>
    /// <param name="postConfigures"></param>
    public SkywalkerOptionsFactory(
        IEnumerable<IConfigureOptions<TOptions>> setups,
        IEnumerable<IPostConfigureOptions<TOptions>> postConfigures)
        : this(setups, postConfigures, validations: null)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="setups"></param>
    /// <param name="postConfigures"></param>
    /// <param name="validations"></param>
    public SkywalkerOptionsFactory(
        IEnumerable<IConfigureOptions<TOptions>> setups,
        IEnumerable<IPostConfigureOptions<TOptions>> postConfigures,
        IEnumerable<IValidateOptions<TOptions>>? validations)
    {
        _setups = setups;
        _postConfigures = postConfigures;
        _validations = validations;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual TOptions Create(string name)
    {
        var options = new TOptions();

        ConfigureOptions(name, options);
        PostConfigureOptions(name, options);
        ValidateOptions(name, options);

        return options;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    protected virtual void ConfigureOptions(string name, TOptions options)
    {
        foreach (var setup in _setups)
        {
            if (setup is IConfigureNamedOptions<TOptions> namedSetup)
            {
                namedSetup.Configure(name, options);
            }
            else if (name == Options.DefaultName)
            {
                setup.Configure(options);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    protected virtual void PostConfigureOptions(string name, TOptions options)
    {
        foreach (var post in _postConfigures)
        {
            post.PostConfigure(name, options);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    /// <exception cref="OptionsValidationException"></exception>
    protected virtual void ValidateOptions(string name, TOptions options)
    {
        if (_validations == null)
        {
            return;
        }
        var failures = new List<string>();
        foreach (var validate in _validations)
        {
            var result = validate.Validate(name, options);
            if (result.Failed)
            {
                failures.AddRange(result.Failures);
            }
        }
        if (failures.Count > 0)
        {
            throw new OptionsValidationException(name, typeof(TOptions), failures);
        }
    }
}
