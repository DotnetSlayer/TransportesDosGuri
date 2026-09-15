using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IAirportService
    {
        Task<List<AirportDTO>> GetAllAsync();
        Task<AirportDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(AirportDTO dto);
        Task<bool> UpdateAsync(long id, AirportDTO dto);
        Task<bool> DeleteAsync(long id);
    }
}
