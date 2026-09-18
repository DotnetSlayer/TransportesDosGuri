using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IUserRequestService
    {
        Task<List<UserRequestDTO>> GetAllAsync();
        Task<UserRequestDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(UserRequestDTO dto);
        Task<bool> UpdateAsync(long id, UserRequestDTO dto);
        Task<(bool Success, string? Error)> DeleteAsync(long id);
    }
}