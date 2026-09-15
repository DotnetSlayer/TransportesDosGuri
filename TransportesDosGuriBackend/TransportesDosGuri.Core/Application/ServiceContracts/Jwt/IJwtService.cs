using System.Security.Claims;
using System.Threading.Tasks;
using TransportesDosGuri.Core.Application.DTOs.Jwt;
using TransportesDosGuri.Core.Domain.Entities.Identity;

namespace TransportesDosGuri.Core.Application.ServiceContracts.Jwt
{
    public interface IJwtService
    {
        Task<AuthenticationResponseDTO> CreateJwtToken(ApplicationUser user);

        ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
    }
}