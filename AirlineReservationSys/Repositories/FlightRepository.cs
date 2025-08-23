using AirlineReservationSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace AirlineReservationSystem.Repositories
{
    public class FlightRepository
    {
        private List<Flight> _flights = new List<Flight>();
        private Dictionary<int, Flight> _flightIndex = new Dictionary<int, Flight>();
        private int _nextId = 1;

        public void Add(Flight flight)
        {
            flight.Id = _nextId++;
            _flights.Add(flight);
            _flightIndex[flight.Id] = flight;
        }

        public Flight? GetById(int id) => _flightIndex.GetValueOrDefault(id);

        public List<Flight> GetAll() => _flights;

        public List<Flight> FindByRoute(string departureCity, string arrivalCity)
        {
            return _flights.Where(f =>
                f.DepartureCity.Equals(departureCity, System.StringComparison.OrdinalIgnoreCase) &&
                f.ArrivalCity.Equals(arrivalCity, System.StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        public void LoadData(List<Flight> flights)
        {
            _flights = flights;
            _flightIndex = flights.ToDictionary(f => f.Id);
            _nextId = flights.Any() ? flights.Max(f => f.Id) + 1 : 1;
        }
    }
}