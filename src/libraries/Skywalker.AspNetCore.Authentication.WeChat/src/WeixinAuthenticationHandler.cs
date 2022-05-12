using System.Security.Claims;
using System.Text.Encodings.Web;
#if !NETSTANDARD
using System.Text.Json;
#endif
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Skywalker.AspNetCore.Authentication.Weixin
{
    public class WeixinAuthenticationHandler : OAuthHandler<WeixinAuthenticationOptions>
    {
        public WeixinAuthenticationHandler( IOptionsMonitor<WeixinAuthenticationOptions> options, ILoggerFactory logger,  UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        private const string OauthState = "_oauthstate";
        private const string State = "state";

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            if (!IsWeixinAuthorizationEndpointInUse())
            {
                if (Request.Query.TryGetValue(OauthState, out var stateValue))
                {
                    var query = Request.Query.ToDictionary(c => c.Key, c => c.Value, StringComparer.OrdinalIgnoreCase);
                    if (query.TryGetValue(State, out var _))
                    {
                        query[State] = stateValue;
                        Request.QueryString = QueryString.Create(query);
                    }
                }
            }

            return await base.HandleRemoteAuthenticateAsync();
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync( ClaimsIdentity identity, AuthenticationProperties properties,  OAuthTokenResponse tokens)
        {
            string address = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, new Dictionary<string, string>
            {
                ["access_token"] = tokens.AccessToken,
                ["openid"] = tokens.Response.Value<string>("openid")
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
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }

        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            string address = QueryHelpers.AddQueryString(Options.TokenEndpoint, new Dictionary<string, string>()
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
                Logger.LogError("An error occurred while retrieving an access token: the remote server returned a {Status} response with the following payload: {Headers} {Body}.",  response.StatusCode, response.Headers.ToString(), await response.Content.ReadAsStringAsync());

                return OAuthTokenResponse.Failed(new Exception("An error occurred while retrieving an access token."));
            }

            return OAuthTokenResponse.Success(payload);
        }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            string stateValue = Options.StateDataFormat.Protect(properties);
            bool addRedirectHash = false;

            if (!IsWeixinAuthorizationEndpointInUse())
            {
                // 授权微信网页防止“状态参数过长”错误时，将状态存储在redirectUri中
                redirectUri = QueryHelpers.AddQueryString(redirectUri, OauthState, stateValue);
                addRedirectHash = true;
            }

            redirectUri = QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, new Dictionary<string, string>
            {
                ["appid"] = Options.ClientId,
                ["scope"] = FormatScope(),
                ["response_type"] = "code",
                ["redirect_uri"] = redirectUri,
                [State] = addRedirectHash ? OauthState : stateValue
            });

            if (addRedirectHash)
            {
                // 微信Web授权所需参数
                redirectUri += "#wechat_redirect";
            }

            return redirectUri;
        }

        protected override string FormatScope() => string.Join(",", Options.Scope);

        private bool IsWeixinAuthorizationEndpointInUse()
        {
            return string.Equals(Options.AuthorizationEndpoint, WeixinAuthenticationDefaults.AuthorizationEndpoint, StringComparison.OrdinalIgnoreCase);
        }
    }
}
