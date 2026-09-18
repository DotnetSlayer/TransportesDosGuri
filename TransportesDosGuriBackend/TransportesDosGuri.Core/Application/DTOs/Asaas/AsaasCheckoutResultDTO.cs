using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.Application.DTOs.Asaas
{
    public class AsaasCheckoutResultDTO
    {
        public string AsaasPaymentId { get; set; } = string.Empty;
        public string? InvoiceUrl { get; set; }
        public decimal Price { get; set; }
        public DateTime DueDate { get; set; }
    }
}
