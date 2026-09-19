namespace TransportesDosGuri.Core.DTOs
{
    public class AsaasCheckoutResultDTO
    {
        public string AsaasPaymentId { get; set; } = string.Empty;
        public string? InvoiceUrl { get; set; }
        public decimal Price { get; set; }
        public DateTime DueDate { get; set; }
    }
}
