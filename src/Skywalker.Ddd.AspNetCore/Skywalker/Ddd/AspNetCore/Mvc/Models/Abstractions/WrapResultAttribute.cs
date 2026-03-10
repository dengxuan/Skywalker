namespace Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;

/// <summary>
/// Used to determine how webapi should wrap response on the web layer.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WrapResultAttribute"/> class.
/// </remarks>
/// <param name="wrapOnSuccess">Wrap result on success.</param>
/// <param name="wrapOnError">Wrap result on error.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
public class WrapResultAttribute(bool wrapOnSuccess = true, bool wrapOnError = true) : Attribute
{
    /// <summary>
    /// Wrap result on success.
    /// </summary>
    public bool WrapOnSuccess { get; set; } = wrapOnSuccess;

    /// <summary>
    /// Wrap result on error.
    /// </summary>
    public bool WrapOnError { get; set; } = wrapOnError;

    /// <summary>
    /// Log errors.
    /// Default: true.
    /// </summary>
    public bool LogError { get; set; } = true;
}
