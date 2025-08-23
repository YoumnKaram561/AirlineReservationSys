namespace AirlineReservationSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int PassengerId { get; set; }
        public string SeatNumber { get; set; }
    }
}