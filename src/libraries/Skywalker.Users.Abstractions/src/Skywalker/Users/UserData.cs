// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Users.Abstractions;

namespace Skywalker.Users;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class UserData : IUserData
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Id { get; set; } = default!;


    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string UserName { get; set; } = default!;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public UserData() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public UserData(IUserData userData)
    {
        Id = userData.Id;
        UserName = userData.UserName;
        Email = userData.Email;
        IsActive = userData.IsActive;
        EmailConfirmed = userData.EmailConfirmed;
        PhoneNumber = userData.PhoneNumber;
        PhoneNumberConfirmed = userData.PhoneNumberConfirmed;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public UserData(string id, string userName, string? email = null, bool emailConfirmed = false, string? phoneNumber = null, bool phoneNumberConfirmed = false, bool isActive = true)
    {
        Id = id;
        UserName = userName;
        Email = email;
        IsActive = isActive;
        EmailConfirmed = emailConfirmed;
        PhoneNumber = phoneNumber;
        PhoneNumberConfirmed = phoneNumberConfirmed;
    }
}
