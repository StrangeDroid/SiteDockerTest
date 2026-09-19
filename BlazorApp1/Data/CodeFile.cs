using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

public class CookieAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;

    public CookieAuthStateProvider(IJSRuntime js)
      => _js = js;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _js.InvokeAsync<string>("CookieHelper.get", "authToken");
        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        // parse JWT claims (helper shown below)
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    // simple JWT payload parser:
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(
          payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=')
        ));
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        return data.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }
}
