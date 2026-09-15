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
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
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
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void MarkUserAsLoggedOut()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            var claims = new List<Claim>();

            if (keyValuePairs == null) return claims;

            foreach (var kvp in keyValuePairs)
            {
                var value = kvp.Value?.ToString() ?? "";

                if (kvp.Key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" || kvp.Key == "unique_name" || kvp.Key == "sub")
                {
                    claims.Add(new Claim(ClaimTypes.Name, value));
                }
                else if (kvp.Key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" || kvp.Key == "email")
                {
                    claims.Add(new Claim(ClaimTypes.Email, value));
                }
                else if (kvp.Key == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" || kvp.Key == "role")
                {
                    claims.Add(new Claim(ClaimTypes.Role, value));
                }
                else
                {
                    claims.Add(new Claim(kvp.Key, value));
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