using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Test.Helpers;
public class TestAuthHandlerUser : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandlerUser(IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // User
        // http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier: 7951bc9f-c769-42b7-a601-9c0a9da6a809
        // http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name: user@task.org
        // http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress: user@task.org
        // AspNet.Identity.SecurityStamp: VWS22KZC62QKQRIAPGPKKY6NCNML4J6B
        // amr: pwd        

        var claims = new[] { 
            new Claim(ClaimTypes.Name, "user@task.org"),
            new Claim(ClaimTypes.Email, "user@task.org"),
            new Claim(ClaimTypes.NameIdentifier, "7951bc9f-c769-42b7-a601-9c0a9da6a809"),
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}