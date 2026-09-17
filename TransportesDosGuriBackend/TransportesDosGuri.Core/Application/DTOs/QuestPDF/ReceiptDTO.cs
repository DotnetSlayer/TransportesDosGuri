using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.Application.DTOs.QuestPDF
{
    public class ReceiptDTO
    {
        public long PurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerIdentityNumber { get; set; } = string.Empty;

        public long TripId { get; set; }
        public string TripName { get; set; } = string.Empty;
        public string OriginAirport { get; set; } = string.Empty;
        public string OriginCity { get; set; } = string.Empty;
        public string DestinyAirport { get; set; } = string.Empty;
        public string DestinyCity { get; set; } = string.Empty;
        public DateTime TripDepartureTime { get; set; }
        public DateTime TripArrivalTime { get; set; }

        public List<ReceiptReservationDTO> Reservations { get; set; } = new();
    }

    public class ReceiptReservationDTO
    {
        public long ReservationId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string SeatClass { get; set; } = string.Empty;
        public string SeatLocation { get; set; } = string.Empty;
        public string SeatSide { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public long FlightId { get; set; }
        public string AircraftModel { get; set; } = string.Empty;
        public DateTime FlightDepartureTime { get; set; }
        public DateTime FlightArrivalTime { get; set; }
        public string FlightOriginAirport { get; set; } = string.Empty;
        public string FlightDestinyAirport { get; set; } = string.Empty;
    }
}
