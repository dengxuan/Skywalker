using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var authResult = await HandleAuthenticateOnceSafeAsync();
            var eventContext = new SkywalkerChallengeContext(Context, Scheme, Options, properties)
            {
                AuthenticateFailure = authResult?.Failure
            };
            await Events.Challenge(eventContext!);
            if (eventContext.Handled)
            {
                return;
            }
            Response.StatusCode = 401;
            Response.Headers.Append(HeaderNames.WWWAuthenticate, Options.Challenge);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string skywalker = Request.Headers[SkywalkerAuthenticationDefaults.AuthenticationScheme];
            if (string.IsNullOrEmpty(skywalker))
            {
                return AuthenticateResult.NoResult();
            }
            ClaimsPrincipal claimsPrincipal = await _skywalkerTokenValidator.ValidateTokenAsync(skywalker!);
            var validatedContext = new SkywalkerTokenValidatedContext(Context, Scheme, Options)
            {
                Principal = claimsPrincipal
            };
            validatedContext.Success();
            return validatedContext.Result;
        }

        public Task SignInAsync(ClaimsPrincipal claimsPrincipal, AuthenticationProperties properties)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretKey123..jackyfei"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                Options.ClaimsIssuer,
                Options.ClaimsIssuer,
                claimsPrincipal.Claims,
                null,
                DateTime.MaxValue,
                credentials
            );
            Response.Headers.Append(SkywalkerAuthenticationDefaults.AuthenticationScheme, new JwtSecurityTokenHandler().WriteToken(token));
            return Task.CompletedTask;
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }
    }
}
