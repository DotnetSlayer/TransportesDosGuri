using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.DTOs
{
    public class AsaasIntegrationDTO
    {
        public long Id { get; set; }

        public string? Environment { get; set; }

        public string? ApiKey { get; set; }

        public string? BaseUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
