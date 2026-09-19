using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using TransportesDosGuri.Core.Common;

namespace TransportesDosGuri.Infrastructure.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenStorageService _tokenStorage;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ITokenStorageService tokenStorage)
        {
            _tokenStorage = tokenStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var (accessToken, _) = await _tokenStorage.GetTokensAsync();

                if (string.IsNullOrWhiteSpace(accessToken))
                    return new AuthenticationState(_anonymous);

                var claims = ParseClaimsFromJwt(accessToken);
                var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public void MarkUserAsAuthenticated(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
        }

        public void MarkUserAsLoggedOut()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var parts = jwt.Split('.');
            if (parts.Length < 2) return claims;

            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs == null) return claims;

            foreach (var kvp in keyValuePairs)
            {
                var key = kvp.Key;
                if (kvp.Value is JsonElement element)
                {
                    if (key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" || key == "unique_name" || key == "sub")
                    {
                        claims.Add(new Claim(ClaimTypes.Name, element.ToString()));
                    }
                    else if (key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" || key == "email")
                    {
                        claims.Add(new Claim(ClaimTypes.Email, element.ToString()));
                    }
                    else if (key == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" || key == "role")
                    {
                        if (element.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in element.EnumerateArray())
                            {
                                claims.Add(new Claim(ClaimTypes.Role, item.ToString()));
                            }
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.Role, element.ToString()));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(key, element.ToString()));
                    }
                }
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}