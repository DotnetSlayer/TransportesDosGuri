using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IPurchaseService
    {
        Task<List<PurchaseDTO>> GetAllAsync();
        Task<PurchaseDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(PurchaseDTO dto);
        Task<bool> UpdateAsync(long id, PurchaseDTO dto);
        Task<(bool Success, string? Error)> DeleteAsync(long id);
    }
}