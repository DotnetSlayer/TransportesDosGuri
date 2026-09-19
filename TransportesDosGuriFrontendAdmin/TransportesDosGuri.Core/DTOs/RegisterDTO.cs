namespace TransportesDosGuri.Core.DTOs
{
    public class RegisterDTO
    {
        public string Name { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string IdentityNumber { get; set; } = string.Empty;

        public string? ZipCode { get; set; }

        public string? Address { get; set; }

        public int AddressNumber { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }
    }
}
