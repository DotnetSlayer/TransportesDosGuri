using TransportesDosGuri.Core.Application.DTOs.Asaas;
using TransportesDosGuri.Core.Domain.RepositoryContracts.Misc;

namespace TransportesDosGuri.Infrastructure.ExternalServices.Asaas
{
    public class AsaasGateway : IAsaasGateway
    {
        private readonly AsaasClient _asaasClient;

        public AsaasGateway(AsaasClient asaasClient)
        {
            _asaasClient = asaasClient;
        }

        public async Task<string> CreateCustomerAsync(string name, string email, string phone, string cpfCnpj)
        {
            var dto = new AsaasCustomerRequestDTO
            {
                Name = name,
                Email = email,
                MobilePhone = phone,
                CpfCnpj = cpfCnpj
            };

            return await _asaasClient.CreateCustomerAsync(dto);
        }

        public async Task<string> CreateSinglePaymentAsync(string customerId, decimal amount, DateOnly dueDate)
        {
            var dto = new AsaasPaymentRequestDTO
            {
                Customer = customerId,
                Value = amount,

                DueDate = dueDate.ToString("yyyy-MM-dd"),
                BillingType = "BOLETO"
            };

            return await _asaasClient.CreateSinglePaymentAsync(dto);
        }
    }
}
