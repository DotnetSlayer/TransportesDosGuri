using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IPurchaseService
    {
        Task<PurchaseResultDTO?> BuySeatsAsync(BuySeatRequestDTO request);

        Task<PurchaseDetailsDTO?> GetMyPurchaseAsync(long purchaseId);

        Task<bool> ProcessPaymentAsync(long purchaseId);

        Task<List<PurchaseDetailsDTO>> GetMyTripsAsync();

        Task<byte[]?> DownloadReceiptAsync(long purchaseId);

        Task<AsaasCheckoutResultDTO?> CreateCheckoutAsync(long purchaseId);

    }
}