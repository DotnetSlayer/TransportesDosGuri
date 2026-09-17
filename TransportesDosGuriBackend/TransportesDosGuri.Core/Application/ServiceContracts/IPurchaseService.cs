using TransportesDosGuri.Core.Application.DTOs;

namespace TransportesDosGuri.Core.Application.ServiceContracts
{
    public interface IPurchaseService
    {
        Task<IEnumerable<PurchaseDTO>> GetAllAsync();

        Task<PurchaseDTO?> GetByIdAsync(long id);

        Task<PurchaseDTO> CreateAsync(PurchaseDTO purchase);

        Task<bool> UpdateAsync(long id, PurchaseDTO purchase);

        Task<bool> DeleteAsync(long id);

        Task<PurchaseResultDTO> BuySeatsAsync(
            long applicationUserId,
            BuySeatRequestDTO request);

        Task<PurchaseDetailsDTO?> GetUserPurchaseAsync(
            long purchaseId,
            long applicationUserId);

        Task<bool> ProcessPaymentAsync(
            long purchaseId,
            long applicationUserId);

        Task<IEnumerable<PurchaseDetailsDTO>> GetMyTripsAsync(
            long applicationUserId);

        Task<byte[]?> GenerateReceiptPdfAsync(long purchaseId, long userId);
    }
}
