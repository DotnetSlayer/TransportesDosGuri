using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TransportesDosGuri.Core.DTOs
{
    public class UpdateUserDTO
    {
        public string? Name { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? IdentityNumber { get; set; }

        public string? ZipCode { get; set; }

        public string? Address { get; set; }

        public int AddressNumber { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }
    }
}
