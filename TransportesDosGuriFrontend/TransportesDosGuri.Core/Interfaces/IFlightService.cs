using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IFlightService
    {
        Task<List<FlightDTO>> GetAllAsync();
        Task<FlightDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(FlightDTO dto);
        Task<bool> UpdateAsync(long id, FlightDTO dto);
        Task<bool> DeleteAsync(long id);
    }
}
