using AirlineReservationSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace AirlineReservationSystem.Repositories
{
    public class PassengerRepository
    {
        private List<Passenger> _passengers = new List<Passenger>();
        private Dictionary<int, Passenger> _passengerIndex = new Dictionary<int, Passenger>();
        private int _nextId = 1;

        public void Add(Passenger passenger)
        {
            passenger.Id = _nextId++;
            _passengers.Add(passenger);
            _passengerIndex[passenger.Id] = passenger;
        }

        public Passenger? GetById(int id) => _passengerIndex.GetValueOrDefault(id);

        public List<Passenger> GetAll() => _passengers;

        public void LoadData(List<Passenger> passengers)
        {
            _passengers = passengers;
            _passengerIndex = passengers.ToDictionary(p => p.Id);
            _nextId = passengers.Any() ? passengers.Max(p => p.Id) + 1 : 1;
        }
    }
}