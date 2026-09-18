using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDTO>?> GetAllAsync();
        Task<UserDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(RegisterDTO registerDto);
        Task<bool> UpdateAsync(long id, UpdateUserDTO updateDto);
        Task<(bool Success, string? Error)> DeleteAsync(long id);
    }
}