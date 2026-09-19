using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO?> GetMeAsync();
        Task<bool> UpdateMeAsync(UpdateUserDTO updateDto);
        Task<bool> DeleteMeAsync();
    }
}