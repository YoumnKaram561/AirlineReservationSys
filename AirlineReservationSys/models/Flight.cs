using System;

namespace AirlineReservationSystem.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime DepartureTime { get; set; }
        public int TotalSeats { get; set; }
    }
}