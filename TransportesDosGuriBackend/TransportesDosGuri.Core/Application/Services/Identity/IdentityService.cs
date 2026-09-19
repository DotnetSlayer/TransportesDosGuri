using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TransportesDosGuri.Core.Application.DTOs.Identity;
using TransportesDosGuri.Core.Application.DTOs.Jwt;
using TransportesDosGuri.Core.Application.ServiceContracts.Identity;
using TransportesDosGuri.Core.Application.ServiceContracts.Jwt;
using TransportesDosGuri.Core.Domain.Entities.Identity;

namespace TransportesDosGuri.Core.Application.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> DeleteAsync(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return (false, new[] { "Requisição Inválida!" });
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return (false, result.Errors.Select(e => e.Description));
            }

            return (true, Enumerable.Empty<string>());
        }

        public async Task<(bool Success, AuthenticationResponseDTO? Response, IEnumerable<string> Errors)> GenerateNewAccessTokenAsync(TokenModelDTO tokenModel)
        {
            if (tokenModel == null)
            {
                return (false, null, new[] { "Requisição Inválida!" });
            }

            string? jwtToken = tokenModel.Token;
            ClaimsPrincipal? principal = _jwtService.GetPrincipalFromJwtToken(jwtToken);

            if (principal == null)
            {
                return (false, null, new[] { "Requisição Inválida!" });
            }

            string? email = principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
            {
                return (false, null, new[] { "Requisição Inválida!" });
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpirationDateTime <= DateTime.UtcNow)
            {
                return (false, null, new[] { "Requisição Inválida!" });
            }

            AuthenticationResponseDTO authenticationResponse = await _jwtService.CreateJwtToken(user);

            user.RefreshToken = authenticationResponse.RefreshToken;
            user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;

            await _userManager.UpdateAsync(user);

            return (true, authenticationResponse, Enumerable.Empty<string>());
        }

        public async Task<IEnumerable<UserProfileResponseDTO>> GetAllAsync()
        {
            return await _userManager.Users
               .Select(user => MapToUserProfileDTO(user))
               .ToListAsync();
        }

        public async Task<UserProfileResponseDTO?> GetByIdAsync(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return null;
            }

            return MapToUserProfileDTO(user);
        }

        public async Task<(bool Success, IEnumerable<string> Errors, long? UserId, AuthenticationResponseDTO? Jwt)> LoginAsync(LoginDTO login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user == null)
            {
                return (false, new[] { "Requisição Inválida!" }, null, null);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return (false, new[] { "Requisição Inválida!" }, null, null);
            }

            var authenticationResponse = await _jwtService.CreateJwtToken(user);

            user.RefreshToken = authenticationResponse.RefreshToken;
            user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;

            await _userManager.UpdateAsync(user);

            return (true, Enumerable.Empty<string>(), user.Id, authenticationResponse);
        }

        public async Task<(bool Success, IEnumerable<string> Errors, long? UserId)> RegisterAsync(RegisterDTO register)
        {
            var user = new ApplicationUser
            {
                UserName = register.Email,
                Email = register.Email,
                Name = register.Name,
                LastName = register.LastName,
                IdentityNumber = register.IdentityNumber,
                ZipCode = register.ZipCode,
                Address = register.Address,
                AddressNumber = register.AddressNumber,
                District = register.District,
                City = register.City,
                State = register.State,
                PhoneNumber = register.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
            {
                return (false, result.Errors.Select(e => e.Description), null);
            }

            const string defaultRole = "User";
            if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = defaultRole });
            }
            await _userManager.AddToRoleAsync(user, defaultRole);

            return (true, Enumerable.Empty<string>(), user.Id);
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> UpdateAsync(long id, UpdateDTO update)
        {
            var existingUser = await _userManager.FindByIdAsync(id.ToString());

            if (existingUser == null)
            {
                return (false, new[] { "Requisição Inválida!" });
            }

            existingUser.Name = update.Name;
            existingUser.LastName = update.LastName;
            existingUser.IdentityNumber = update.IdentityNumber;
            existingUser.PhoneNumber = update.PhoneNumber;
            existingUser.ZipCode = update.ZipCode;
            existingUser.Address = update.Address;
            existingUser.AddressNumber = update.AddressNumber;
            existingUser.District = update.District;
            existingUser.City = update.City;
            existingUser.State = update.State;

            var result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
            {
                return (false, result.Errors.Select(e => e.Description));
            }

            return (true, Enumerable.Empty<string>());
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> LogoutAsync(TokenModelDTO tokenModel)
        {
            if (tokenModel == null || string.IsNullOrWhiteSpace(tokenModel.Token))
            {
                return (false, new[] { "Requisição Inválida!" });
            }

            ClaimsPrincipal? principal = _jwtService.GetPrincipalFromJwtToken(tokenModel.Token);

            if (principal == null)
            {
                return (false, new[] { "Requisição Inválida!" });
            }

            string? email = principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return (false, new[] { "Requisição Inválida!" });
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken)
            {
                return (false, new[] { "Requisição Inválida!" });
            }

            user.RefreshToken = null;
            user.RefreshTokenExpirationDateTime = DateTime.MinValue;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return (false, result.Errors.Select(e => e.Description));
            }

            await _signInManager.SignOutAsync();

            return (true, Enumerable.Empty<string>());
        }

        private static UserProfileResponseDTO MapToUserProfileDTO(ApplicationUser user) => new()
        {
            Id = user.Id,
            Name = user.Name,
            LastName = user.LastName,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IdentityNumber = user.IdentityNumber,
            CustomerAsaasId = user.CustomerAsaasId,
            ZipCode = user.ZipCode,
            Address = user.Address,
            AddressNumber = user.AddressNumber,
            District = user.District,
            City = user.City,
            State = user.State
        };
    }
}