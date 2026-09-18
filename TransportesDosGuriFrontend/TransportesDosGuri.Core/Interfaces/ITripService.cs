using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface ITripService
    {
        Task<List<TripDTO>> GetAllAsync();

        Task<List<TripDTO>> SearchAsync(
            string? searchTerm);

        Task<TripDetailsDTO?> GetDetailsAsync(
            long id);
    }
}
