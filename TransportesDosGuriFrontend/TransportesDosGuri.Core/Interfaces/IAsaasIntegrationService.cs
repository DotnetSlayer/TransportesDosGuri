using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IAsaasIntegrationService
    {
        Task<List<AsaasIntegrationDTO>> GetAllAsync();
        Task<AsaasIntegrationDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(AsaasIntegrationDTO dto);
        Task<bool> UpdateAsync(long id, AsaasIntegrationDTO dto);
        Task<bool> DeleteAsync(long id);
    }
}
