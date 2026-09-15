using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IFlightSeatService
    {
        Task<List<FlightSeatDTO>> GetAllAsync();
        Task<FlightSeatDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(FlightSeatDTO dto);
        Task<bool> UpdateAsync(long id, FlightSeatDTO dto);
        Task<bool> DeleteAsync(long id);
    }
}
