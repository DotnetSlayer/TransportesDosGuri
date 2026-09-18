using TransportesDosGuri.Core.Application.DTOs;
using TransportesDosGuri.Core.Application.DTOs.Asaas;
using TransportesDosGuri.Core.Application.ServiceContracts;
using TransportesDosGuri.Core.Application.ServiceContracts.Asaas;
using TransportesDosGuri.Core.Domain.RepositoryContracts;
using TransportesDosGuri.Core.Domain.RepositoryContracts.Misc;

namespace TransportesDosGuri.Infrastructure.ExternalServices.Asaas
{
    public class AsaasCheckoutService : IAsaasCheckoutService
    {
        private readonly IAsaasGateway _asaas;
        private readonly IUserRepository _userRepository;

        public AsaasCheckoutService(IAsaasGateway asaas, IUserRepository userRepository)
        {
            _asaas = asaas;
            _userRepository = userRepository;
        }

        public async Task<AsaasCheckoutResultDTO> CreateCheckoutAsync(
            long userId, decimal price, DateTime dueDate, string externalReference)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException($"Usuário {userId} não encontrado.");

            string customerId = user.CustomerAsaasId;

            if (string.IsNullOrEmpty(customerId))
            {
                customerId = await _asaas.CreateCustomerAsync(
                    $"{user.Name} {user.LastName}",
                    user.Email,
                    user.PhoneNumber,
                    user.IdentityNumber);

                await _userRepository.UpdateAsaasCustomerIdAsync(user.Id, customerId);
            }

            var (paymentId, invoiceUrl) = await _asaas.CreatePaymentAsync(
                customerId, price, dueDate,
                $"Cobrança {externalReference}", externalReference);

            return new AsaasCheckoutResultDTO
            {
                AsaasPaymentId = paymentId,
                InvoiceUrl = invoiceUrl,
                Price = price,
                DueDate = dueDate
            };
        }
    }
}