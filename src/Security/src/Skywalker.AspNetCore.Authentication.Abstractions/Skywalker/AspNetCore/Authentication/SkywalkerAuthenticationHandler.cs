using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Skywalker.AspNetCore.Authentication.Abstractions;

namespace Skywalker.AspNetCore.Authentication
{
    public class SkywalkerAuthenticationHandler : AuthenticationHandler<SkywalkerAuthenticationOptions>, IAuthenticationSignInHandler, IAuthenticationSignOutHandler
    {
        private readonly ISkywalkerTokenValidator _skywalkerTokenValidator;

        protected new SkywalkerAuthenticationEvents Events
        {
            get => (SkywalkerAuthenticationEvents)base.Events;
            set => base.Events = value;
        }

        public SkywalkerAuthenticationHandler(IOptionsMonitor<SkywalkerAuthenticationOptions> options, ILoggerFactory logger, ISkywalkerTokenValidator skywalkerTokenValidator, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _skywalkerTokenValidator = skywalkerTokenValidator;
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new SkywalkerAuthenticationEvents());

        //protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        //{
        //    var authResult = await HandleAuthenticateOnceSafeAsync();
        //    var eventContext = new SkywalkerChallengeContext(Context, Scheme, Options, properties)
        //    {
        //        AuthenticateFailure = authResult?.Failure
        //    };
        //                await Events.Challenge(eventContext!);
        //    if (eventContext.Handled)
        //    {
        //        return;
        //    }
        //    Response.StatusCode = 401;
        //    Response.Headers.Append(HeaderNames.WWWAuthenticate, Options.Challenge);
        //}
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            return base.HandleForbiddenAsync(properties);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? securityToken = null;

            string authorization = Request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(authorization))
            {
                return AuthenticateResult.NoResult();
            }
            if (authorization.StartsWith($"{SkywalkerAuthenticationDefaults.AuthenticationScheme} ", StringComparison.OrdinalIgnoreCase))
            {
                securityToken = authorization[$"{SkywalkerAuthenticationDefaults.AuthenticationScheme} ".Length..].Trim();
            }

            // If no token found, no further work possible
            if (string.IsNullOrEmpty(securityToken))
            {
                return AuthenticateResult.NoResult();
            }
            ClaimsPrincipal claimsPrincipal = await _skywalkerTokenValidator.ValidateTokenAsync(securityToken!);
            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
            var validatedContext = new SkywalkerTokenValidatedContext(Context, Scheme, Options)
            {
                Principal = claimsPrincipal
            };
            validatedContext.Success();
            return validatedContext.Result;
        }

        public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            AuthenticationProperties authenticationProperties = properties ?? new AuthenticationProperties();
            string token = authenticationProperties.GetTokenValue("Auth");
            throw new NotImplementedException();
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            throw new NotImplementedException();
        }
    }
}
