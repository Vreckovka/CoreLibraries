using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DomainCore.DomainClasses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace APICore.Authentication
{
  public class AuthUser : IIdentity
  {

    public AuthUser(string authenticationType, bool isAuthenticated, string token )
    {
      AuthenticationType = authenticationType ?? throw new ArgumentNullException(nameof(authenticationType));
      Token = authenticationType ?? throw new ArgumentNullException(nameof(authenticationType));
      IsAuthenticated = isAuthenticated;
    }


    public string AuthenticationType { get; }
    public bool IsAuthenticated { get; }
    public string? Name { get; }
    public string Token { get; }
  }


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

        var identy = new ClaimsIdentity(new AuthUser("asd",true,token), claims);
        var principle = new GenericPrincipal(identy, null);
        var ticket = new AuthenticationTicket(principle, Scheme.Name);


        System.Threading.Thread.CurrentPrincipal = principle;

        var asd = System.Threading.Thread.CurrentPrincipal?.Identity?.Name;

        return AuthenticateResult.Success(ticket);
      }


      return AuthenticateResult.Fail("Unauthorized");
    }
  }
}

