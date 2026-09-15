using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.Common;

namespace TransportesDosGuri.Infrastructure.Auth
{
    public class TokenStorageService : ITokenStorageService
    {
        private string? _accessToken;
        private string? _refreshToken;

        public Task ClearTokensAsync()
        {
            _accessToken = null;
            _refreshToken = null;
            return Task.CompletedTask;
        }

        public Task<(string? AccessToken, string? RefreshToken)> GetTokensAsync()
        {
            return Task.FromResult((_accessToken, _refreshToken));
        }

        public Task SetTokensAsync(string accessToken, string refreshToken)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            return Task.CompletedTask;
        }
    }
}
