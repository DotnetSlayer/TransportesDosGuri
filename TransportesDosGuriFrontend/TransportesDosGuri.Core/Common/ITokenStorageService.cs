using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.Common
{
    public interface ITokenStorageService
    {
        Task SetTokensAsync(string accessToken, string refreshToken);
        Task<(string? AccessToken, string? RefreshToken)> GetTokensAsync();
        Task ClearTokensAsync();
    }
}
