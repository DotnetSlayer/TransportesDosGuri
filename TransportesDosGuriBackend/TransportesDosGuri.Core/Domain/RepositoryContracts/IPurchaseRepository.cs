using TransportesDosGuri.Core.Application.DTOs;
using TransportesDosGuri.Core.Application.DTOs.QuestPDF;
using TransportesDosGuri.Core.Domain.Entities;

namespace TransportesDosGuri.Core.Domain.RepositoryContracts
{
    public interface IPurchaseRepository
    {
        Task<IEnumerable<Purchase>> GetAllAsync();

        Task<Purchase?> GetByIdAsync(long id);

        Task AddAsync(Purchase purchase);

        Task UpdateAsync(Purchase purchase);

        Task DeleteAsync(long id);

        Task<PurchaseDetailsDTO?> GetUserPurchaseAsync(
            long purchaseId,
            long applicationUserId);

        Task<IEnumerable<PurchaseDetailsDTO>> GetMyTripsAsync(
            long applicationUserId);

        Task<PurchaseResultDTO> CreatePurchaseAsync(
            long applicationUserId,
            BuySeatRequestDTO request);

        Task<bool> ProcessPaymentAsync(
            long purchaseId,
            long applicationUserId);

        Task<ReceiptDTO?> GetReceiptDataAsync(long purchaseId, long userId);
    }
}
