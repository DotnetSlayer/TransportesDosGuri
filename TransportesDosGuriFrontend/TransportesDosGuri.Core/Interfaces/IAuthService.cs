using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginDTO loginDto);
        Task<bool> RegisterAsync(RegisterDTO registerDto);
        Task<string?> RefreshTokenAsync();
        Task LogoutAsync();
    }
}
