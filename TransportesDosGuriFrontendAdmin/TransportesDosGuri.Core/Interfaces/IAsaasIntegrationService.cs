using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IAsaasIntegrationService
    {
        Task<List<AsaasIntegrationDTO>> GetAllAsync();
        Task<AsaasIntegrationDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(AsaasIntegrationDTO dto);
        Task<bool> UpdateAsync(long id, AsaasIntegrationDTO dto);
        Task<(bool Success, string? Error)> DeleteAsync(long id);
    }
}