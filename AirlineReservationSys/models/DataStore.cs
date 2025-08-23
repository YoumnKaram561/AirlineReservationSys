using System.Collections.Generic;

namespace AirlineReservationSystem.Models
{
    // A container class for easy JSON serialization
    public class DataStore
    {
        public List<Flight> Flights { get; set; } = new List<Flight>();
        public List<Passenger> Passengers { get; set; } = new List<Passenger>();
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}