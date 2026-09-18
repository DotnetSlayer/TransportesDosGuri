using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.Application.DTOs.Asaas;

namespace TransportesDosGuri.Core.Application.ServiceContracts.Asaas
{
    public interface IAsaasCheckoutService
    {
        Task<AsaasCheckoutResultDTO> CreateCheckoutAsync(long userId, decimal price, DateTime dueDate, string externalReference);
    }
}
