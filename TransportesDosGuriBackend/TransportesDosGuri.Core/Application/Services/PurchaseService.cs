using TransportesDosGuri.Core.Application.DTOs;
using TransportesDosGuri.Core.Application.ServiceContracts;
using TransportesDosGuri.Core.Application.ServiceContracts.QuestPDF;
using TransportesDosGuri.Core.Domain.Entities;
using TransportesDosGuri.Core.Domain.RepositoryContracts;

namespace TransportesDosGuri.Core.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IReceiptPdfGenerator _receiptPdfGenerator;

        public PurchaseService(
            IPurchaseRepository purchaseRepository,
            IReceiptPdfGenerator receiptPdfGenerator)
        {
            _purchaseRepository = purchaseRepository;
            _receiptPdfGenerator = receiptPdfGenerator;
        }

        public async Task<PurchaseDTO> CreateAsync(PurchaseDTO purchase)
        {
            var purchaseEntity = new Purchase
            {
                ApplicationUserId = purchase.ApplicationUserId,
                PurchaseDate = purchase.PurchaseDate,
                PurchasePrice = purchase.PurchasePrice,
                Status = purchase.Status,
                TripId = purchase.TripId
            };

            await _purchaseRepository.AddAsync(purchaseEntity);

            purchase.Id = purchaseEntity.Id;

            return purchase;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var purchaseEntity = await _purchaseRepository.GetByIdAsync(id);

            if (purchaseEntity == null)
            {
                return false;
            }

            await _purchaseRepository.DeleteAsync(id);

            return true;
        }

        public async Task<IEnumerable<PurchaseDTO>> GetAllAsync()
        {
            var purchaseEntities = await _purchaseRepository.GetAllAsync();

            return purchaseEntities.Select(purchaseEntities => new PurchaseDTO
            {
                Id = purchaseEntities.Id,
                ApplicationUserId = purchaseEntities.ApplicationUserId,
                PurchaseDate = purchaseEntities.PurchaseDate,
                PurchasePrice = purchaseEntities.PurchasePrice,
                Status = purchaseEntities.Status,
                TripId= purchaseEntities.TripId
            });
        }

        public async Task<PurchaseDTO?> GetByIdAsync(long id)
        {
            var purchaseEntity = await _purchaseRepository.GetByIdAsync(id);

            if (purchaseEntity == null)
            {
                return null;
            }

            return new PurchaseDTO
            {
                Id = purchaseEntity.Id,
                ApplicationUserId = purchaseEntity.ApplicationUserId,
                PurchaseDate = purchaseEntity.PurchaseDate,
                PurchasePrice = purchaseEntity.PurchasePrice,
                Status = purchaseEntity.Status,
                TripId = purchaseEntity.TripId
            };
        }

        public async Task<bool> UpdateAsync(long id, PurchaseDTO purchase)
        {
            var existingPurchase = await _purchaseRepository.GetByIdAsync(id);

            if (existingPurchase == null)
            {
                return false;
            }

            existingPurchase.ApplicationUserId = purchase.ApplicationUserId;
            existingPurchase.PurchaseDate = purchase.PurchaseDate;
            existingPurchase.PurchasePrice = purchase.PurchasePrice;
            existingPurchase.Status = purchase.Status;
            existingPurchase.TripId = purchase.TripId;

            await _purchaseRepository.UpdateAsync(existingPurchase);

            return true;
        }

        public async Task<PurchaseResultDTO> BuySeatsAsync(long userId, BuySeatRequestDTO request)
        {
            if (userId <= 0)
                throw new ArgumentException("Requisição Inválida!");

            if (request == null)
                throw new ArgumentException("Requisição Inválida!");

            if (request.TripId <= 0)
                throw new ArgumentException("Requisição Inválida!");

            if (request.FlightSeatIds == null ||
                !request.FlightSeatIds.Any())
            {
                throw new ArgumentException(
                    "Requisição Inválida!");
            }

            return await _purchaseRepository.CreatePurchaseAsync(
                userId,
                request);
        }

        public async Task<PurchaseDetailsDTO?> GetUserPurchaseAsync(long purchaseId, long userId)
        {
            return await _purchaseRepository.GetUserPurchaseAsync(
                purchaseId,
                userId);
        }

        public async Task<bool> ProcessPaymentAsync(long purchaseId, long userId)
        {
            return await _purchaseRepository.ProcessPaymentAsync(
                purchaseId,
                userId);
        }

        public async Task<IEnumerable<PurchaseDetailsDTO>> GetMyTripsAsync(long userId)
        {
            return await _purchaseRepository.GetMyTripsAsync(userId);
        }

        public async Task<byte[]?> GenerateReceiptPdfAsync(long purchaseId, long userId)
        {
            var receiptData = await _purchaseRepository.GetReceiptDataAsync(purchaseId, userId);

            if (receiptData == null)
                return null;

            // O Core chama a interface, não a implementação concreta
            return _receiptPdfGenerator.Generate(receiptData);
        }
    }
}
