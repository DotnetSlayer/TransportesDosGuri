using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IReservationService
    {
        Task<List<ReservationDTO>> GetAllAsync();
        Task<ReservationDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(ReservationDTO dto);
        Task<bool> UpdateAsync(long id, ReservationDTO dto);
        Task<bool> DeleteAsync(long id);
    }
}
