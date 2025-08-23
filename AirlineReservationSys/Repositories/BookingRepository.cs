using AirlineReservationSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace AirlineReservationSystem.Repositories
{
    public class BookingRepository
    {
        private List<Booking> _bookings = new List<Booking>();
        private Dictionary<int, Booking> _bookingIndex = new Dictionary<int, Booking>();
        private int _nextId = 1;

        public Booking Add(Booking booking)
        {
            booking.Id = _nextId++;
            _bookings.Add(booking);
            _bookingIndex[booking.Id] = booking;
            return booking;
        }

        public Booking? GetById(int id) => _bookingIndex.GetValueOrDefault(id);

        public List<Booking> GetByFlightId(int flightId)
        {
            return _bookings.Where(b => b.FlightId == flightId).ToList();
        }

        // --- هذا هو الإصلاح ---
        // تمت إضافة هذه الدالة التي كانت مفقودة
        public List<Booking> GetAll() => _bookings;

        public bool Remove(int bookingId)
        {
            var booking = GetById(bookingId);
            if (booking != null)
            {
                _bookings.Remove(booking);
                _bookingIndex.Remove(bookingId);
                return true;
            }
            return false;
        }

        public void LoadData(List<Booking> bookings)
        {
            _bookings = bookings;
            _bookingIndex = bookings.ToDictionary(b => b.Id);
            _nextId = bookings.Any() ? bookings.Max(b => b.Id) + 1 : 1;
        }
    }
}