using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TransportesDosGuri.Core.Application.DTOs.QuestPDF;
using TransportesDosGuri.Core.Application.ServiceContracts.QuestPDF;

namespace TransportesDosGuri.Infrastructure.QuestPDF
{
    public class ReceiptPdfGenerator : IReceiptPdfGenerator
    {
        public byte[] Generate(ReceiptDTO receipt)
        {
            var document = new ReceiptDocument(receipt);
            return document.GeneratePdf();
        }

        private class ReceiptDocument : IDocument
        {
            private readonly ReceiptDTO _receipt;

            public ReceiptDocument(ReceiptDTO receipt)
            {
                _receipt = receipt;
            }

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Transportes Dos Guri - ");
                        text.Span(DateTime.Now.Year.ToString());
                        text.Span(" - Todos os direitos reservados.");
                    });
                });
            }

            private void ComposeHeader(IContainer container)
            {
                container.PaddingBottom(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Transportes Dos Guri").FontSize(20).Bold().FontColor(Colors.Indigo.Darken2);
                        col.Item().Text("Recibo de Compra / Passagem Aérea").FontSize(12).FontColor(Colors.Grey.Darken1);
                    });

                    row.ConstantItem(150).AlignRight().Column(col =>
                    {
                        col.Item().Text($"Recibo #{_receipt.PurchaseId}").Bold();
                        col.Item().Text($"Data: {_receipt.PurchaseDate:dd/MM/yyyy HH:mm}");
                        col.Item().Text($"Status: {_receipt.Status}").FontColor(Colors.Green.Darken2);
                    });
                });
            }

            private void ComposeContent(IContainer container)
            {
                container.PaddingVertical(15).Column(col =>
                {
                    col.Spacing(12);

                    col.Item().Element(ComposeCustomerInfo);

                    col.Item().Element(ComposeTripInfo);

                    col.Item().Element(ComposeReservations);

                    col.Item().Element(ComposeTotal);
                });
            }

            private void ComposeCustomerInfo(IContainer container)
            {
                container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
                {
                    col.Item().Text("Dados do Passageiro").Bold().FontSize(13).FontColor(Colors.Indigo.Darken2);
                    col.Item().PaddingTop(5).Text($"Nome: {_receipt.CustomerName}");
                    col.Item().Text($"Email: {_receipt.CustomerEmail}");
                    col.Item().Text($"Documento: {_receipt.CustomerIdentityNumber}");
                });
            }

            private void ComposeTripInfo(IContainer container)
            {
                container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
                {
                    col.Item().Text("Dados da Viagem").Bold().FontSize(13).FontColor(Colors.Indigo.Darken2);
                    col.Item().PaddingTop(5).Text($"Viagem: {_receipt.TripName}");
                    col.Item().Text($"Origem: {_receipt.OriginAirport} ({_receipt.OriginCity})");
                    col.Item().Text($"Destino: {_receipt.DestinyAirport} ({_receipt.DestinyCity})");
                    col.Item().Text($"Partida: {_receipt.TripDepartureTime:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Chegada: {_receipt.TripArrivalTime:dd/MM/yyyy HH:mm}");
                });
            }

            private void ComposeReservations(IContainer container)
            {
                container.Column(col =>
                {
                    col.Item().Text("Assentos Reservados").Bold().FontSize(13).FontColor(Colors.Indigo.Darken2);
                    col.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Assento").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Voo").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Aeronave").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Classe").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Localização").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Lado").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Preço").Bold();
                        });

                        foreach (var reservation in _receipt.Reservations)
                        {
                            table.Cell().Padding(5).Text(reservation.SeatNumber);
                            table.Cell().Padding(5).Text($"#{reservation.FlightId}");
                            table.Cell().Padding(5).Text(reservation.AircraftModel);
                            table.Cell().Padding(5).Text(reservation.SeatClass);
                            table.Cell().Padding(5).Text(reservation.SeatLocation);
                            table.Cell().Padding(5).Text(reservation.SeatSide);
                            table.Cell().Padding(5).AlignRight().Text(reservation.Price.ToString("C"));
                        }
                    });
                });
            }

            private void ComposeTotal(IContainer container)
            {
                container.AlignRight().PaddingTop(10).BorderTop(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
                {
                    col.Item().Text($"Total: {_receipt.TotalPrice:C}").Bold().FontSize(16).FontColor(Colors.Green.Darken2);
                });
            }
        }
    }
}
