using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IUserRequestService
    {
        Task<UserRequestDTO?> CreateWithPaymentAsync(
            UserRequestDTO request);
    }
}
