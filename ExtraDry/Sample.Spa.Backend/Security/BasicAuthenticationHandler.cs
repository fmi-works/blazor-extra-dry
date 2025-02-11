﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Sample.Spa.Backend.Security;

/// <summary>
/// A worthless authentication handler that just allows any request to masquarade as an admin. This
/// is here so that the use of the `[Authorize]` attribute can be demonstrated without implementing
/// a real security system.
/// </summary>
public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    /// <summary>
    /// Handles basic authentication by testing against hard-coded admin/admin credentials.
    /// </summary>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // skip authentication if endpoint has [AllowAnonymous] attribute
        var endpoint = Context.GetEndpoint();
        if(endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null) {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if(!Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value)) {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
        }
        try {
            var authHeader = AuthenticationHeaderValue.Parse(value.ToString() ?? "");
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? "");
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split([':'], 2);
            var username = credentials[0];
            var password = credentials[1];
            if(username != "admin" || password != "admin") {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }
        catch {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        }

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, "admin"),
            new Claim(ClaimTypes.Name, "admin"),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
