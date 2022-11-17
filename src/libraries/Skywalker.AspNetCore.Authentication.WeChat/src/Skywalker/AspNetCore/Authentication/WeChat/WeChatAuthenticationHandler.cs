// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#if NETSTANDARD
using Newtonsoft.Json.Linq;
#else
using System.Text.Json;
#endif

namespace Skywalker.AspNetCore.Authentication.WeChat;

public class WeChatAuthenticationHandler : OAuthHandler<WeChatAuthenticationOptions>
{
    public WeChatAuthenticationHandler(IOptionsMonitor<WeChatAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

#if NETSTANDARD

    protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
    {
        var address = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, new Dictionary<string, string?>
        {
            ["access_token"] = tokens.AccessToken,
            ["openid"] = tokens.Response.Value<string>("openid"),
        });

        using var response = await Backchannel.GetAsync(address);
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError("An error occurred while retrieving the user profile: the remote server returned a {Status} response with the following payload: {Headers} {Body}.", response.StatusCode, response.Headers.ToString(), await response.Content.ReadAsStringAsync());

            throw new HttpRequestException("An error occurred while retrieving user information.");
        }

        var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
        if (!string.IsNullOrEmpty(payload.Value<string>("errcode")))
        {
            Logger.LogError("An error occurred while retrieving the user profile: the remote server returned a {Status} response with the following payload: {Headers} {Body}.", response.StatusCode, response.Headers.ToString(), await response.Content.ReadAsStringAsync());

            throw new HttpRequestException("An error occurred while retrieving user information.");
        }

        var principal = new ClaimsPrincipal(identity);
        var context = new OAuthCreatingTicketContext(principal, properties, Context, Scheme, Options, Backchannel, tokens, payload);
        context.RunClaimActions();

        await Options.Events.CreatingTicket(context);
        return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);
    }

    protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
    {
        var address = QueryHelpers.AddQueryString(Options.TokenEndpoint, new Dictionary<string, string?>()
        {
            ["appid"] = Options.ClientId,
            ["secret"] = Options.ClientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code"
        });

        using var response = await Backchannel.GetAsync(address);
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError("An error occurred while retrieving an access token: the remote server returned a {Status} response with the following payload: {Headers} {Body}.", response.StatusCode, response.Headers.ToString(), await response.Content.ReadAsStringAsync());

            return OAuthTokenResponse.Failed(new Exception("An error occurred while retrieving an access token."));
        }

        var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
        if (!string.IsNullOrEmpty(payload.Value<string>("errcode")))
        {
            Logger.LogError("An error occurred while retrieving an access token: the remote server returned a {Status} response with the following payload: {Headers} {Body}.", response.StatusCode, response.Headers.ToString(), await response.Content.ReadAsStringAsync());

            return OAuthTokenResponse.Failed(new Exception("An error occurred while retrieving an access token."));
        }

        return OAuthTokenResponse.Success(payload);
    }

#else

    protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
    {
        var address = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, new Dictionary<string, string?>
        {
            ["access_token"] = tokens.AccessToken,
            ["openid"] = tokens.Response?.RootElement.GetString("openid"),
        });

        using var response = await Backchannel.GetAsync(address);
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError("An error occurred while retrieving the user profile: the remote server returned a {Status} response with the following payload: {Headers} {Body}.", response.StatusCode, response.Headers.ToString(), await response.Content.ReadAsStringAsync());

            throw new HttpRequestException("An error occurred while retrieving user information.");
        }

        var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        if (!string.IsNullOrEmpty(payload.RootElement.GetString("errcode")))
        {
            Logger.LogError("An error occurred while retrieving the user profile: the remote server returned a {Status} response with the following payload: {Headers} {Body}.", response.StatusCode, response.Headers.ToString(), await response.Content.ReadAsStringAsync());

            throw new HttpRequestException("An error occurred while retrieving user information.");
        }

        var principal = new ClaimsPrincipal(identity);
        var context = new OAuthCreatingTicketContext(principal, properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);
        context.RunClaimActions();

        await Options.Events.CreatingTicket(context);
        return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);
    }

    protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
    {
        var address = QueryHelpers.AddQueryString(Options.TokenEndpoint, new Dictionary<string, string?>()
        {
            ["appid"] = Options.ClientId,
            ["secret"] = Options.ClientSecret,
            ["code"] = context.Code,
            ["grant_type"] = "authorization_code"
        });

        using var response = await Backchannel.GetAsync(address);
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError("An error occurred while retrieving an access token: the remote server returned a {Status} response with the following payload: {Headers} {Body}.", response.StatusCode, response.Headers.ToString(), await response.Content.ReadAsStringAsync());

            return OAuthTokenResponse.Failed(new Exception("An error occurred while retrieving an access token."));
        }

        var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        if (!string.IsNullOrEmpty(payload.RootElement.GetString("errcode")))
        {
            Logger.LogError("An error occurred while retrieving an access token: the remote server returned a {Status} response with the following payload: {Headers} {Body}.", response.StatusCode, response.Headers.ToString(), await response.Content.ReadAsStringAsync());

            return OAuthTokenResponse.Failed(new Exception("An error occurred while retrieving an access token."));
        }

        return OAuthTokenResponse.Success(payload);
    }
#endif

    protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
    {
        var stateValue = Options.StateDataFormat.Protect(properties);
        return QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, new Dictionary<string, string?>
        {
            ["appid"] = Options.ClientId,
            ["scope"] = FormatScope(),
            ["response_type"] = "code",
            ["redirect_uri"] = redirectUri,
            ["state"] = stateValue
        });
    }

    protected override string FormatScope() => string.Join(",", Options.Scope);

}
