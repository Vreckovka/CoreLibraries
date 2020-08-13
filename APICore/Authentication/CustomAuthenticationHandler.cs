using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace APICore.Authentication
{
  public class CustomAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
  {
    private readonly IAuthenticationManager authenticationManager;
    public const string AuthorizationHeaderName = "Authorization";

    public CustomAuthenticationHandler(
      IOptionsMonitor<BasicAuthenticationOptions> options,
      ILoggerFactory logger,
      UrlEncoder encoder,
      ISystemClock clock,
      IAuthenticationManager authenticationManager) : base(options, logger, encoder, clock)
    {
      this.authenticationManager = authenticationManager ?? throw new ArgumentNullException(nameof(authenticationManager));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
      {
        return AuthenticateResult.NoResult();
      }

      var headerValue = Request.Headers[AuthorizationHeaderName];

      if (string.IsNullOrEmpty(headerValue))
      {
        return AuthenticateResult.Fail("Unauthorized");
      }

      return ValidateToken(headerValue);
    }

    private AuthenticateResult ValidateToken(string token)
    {
      if (authenticationManager.IsTokenValid(token))
      {
        var claims = new List<Claim>()
        {
          new Claim(ClaimTypes.Name, token)
        };

        var identy = new ClaimsIdentity(claims, Scheme.Name);
        var principle = new GenericPrincipal(identy, null);
        var ticket = new AuthenticationTicket(principle, Scheme.Name);

        return AuthenticateResult.Success(ticket);
      }


      return AuthenticateResult.Fail("Unauthorized");
    }
  }
}

