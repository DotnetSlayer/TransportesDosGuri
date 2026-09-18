using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.Enums;

namespace TransportesDosGuri.Core.DTOs
{
    public class PurchaseResultDTO
    {
        public long PurchaseId { get; set; }

        public decimal TotalPrice { get; set; }

        public PurchaseStatus Status { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
