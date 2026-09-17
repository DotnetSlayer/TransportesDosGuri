using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.DTOs
{
    public class UserRequestDTO
    {
        public long Id { get; set; }

        public decimal Price { get; set; }

        public DateOnly DueDate { get; set; }

        public long ApplicationUserId { get; set; }

        public string? AsaasSubscriptionId { get; set; }
    }
}
