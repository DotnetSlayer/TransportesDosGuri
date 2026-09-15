using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IAircraftService
    {
        Task<List<AircraftDTO>> GetAllAsync();
        Task<AircraftDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(AircraftDTO dto);
        Task<bool> UpdateAsync(long id, AircraftDTO dto);
        Task<bool> DeleteAsync(long id);
    }
}
