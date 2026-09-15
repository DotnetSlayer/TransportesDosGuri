using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.Common
{
    public interface ITokenStorageService
    {
        ValueTask SetTokensAsync(string accessToken, string refreshToken);
        ValueTask<(string? AccessToken, string? RefreshToken)> GetTokensAsync();
        ValueTask ClearTokensAsync();
    }
}
