using TransportesDosGuri.Core.Application.DTOs.QuestPDF;

namespace TransportesDosGuri.Core.Application.ServiceContracts.QuestPDF
{
    public interface IReceiptPdfGenerator
    {
        byte[] Generate(ReceiptDTO receipt);
    }
}
