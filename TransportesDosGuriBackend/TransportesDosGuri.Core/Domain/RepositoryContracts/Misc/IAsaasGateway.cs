namespace TransportesDosGuri.Core.Domain.RepositoryContracts.Misc
{
    public interface IAsaasGateway
    {
        Task<string> CreateCustomerAsync(string name, string email, string phone, string cpfCnpj);

        Task<(string paymentId, string? invoiceUrl)> CreatePaymentAsync(
            string customerId,
            decimal value,
            DateTime dueDate,
            string description,
            string externalReference);


    }
}


