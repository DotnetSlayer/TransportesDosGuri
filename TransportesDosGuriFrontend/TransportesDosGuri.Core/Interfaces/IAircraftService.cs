using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IAircraftService
    {
        Task<List<AircraftDto>> GetAllAsync();
        Task<AircraftDto?> GetByIdAsync(long id);
        Task<bool> CreateAsync(AircraftDto dto);
        Task<bool> UpdateAsync(long id, AircraftDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
