using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using AirlineReservationSystem.Models;
using AirlineReservationSystem.Repositories;

namespace AirlineReservationSystem
{
    public class Program
    {
        private static readonly FlightRepository _flightRepo = new FlightRepository();
        private static readonly PassengerRepository _passengerRepo = new PassengerRepository();
        private static readonly BookingRepository _bookingRepo = new BookingRepository();
        private static readonly string dataFilePath = "airline_data.json";

        public static void Main(string[] args)
        {
            LoadData();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n--- Airline Reservation System ---");
                Console.WriteLine("1. Add Flight");
                Console.WriteLine("2. Register Passenger");
                Console.WriteLine("3. Search for Flights");
                Console.WriteLine("4. Book a Seat");
                Console.WriteLine("5. View Flight Manifest (Passenger List)");
                Console.WriteLine("6. Cancel Booking");
                Console.WriteLine("7. Save Data");
                Console.WriteLine("8. Exit");
                Console.Write("Select an option: ");

                switch (Console.ReadLine())
                {
                    case "1": AddFlight(); break;
                    case "2": RegisterPassenger(); break;
                    case "3": SearchFlights(); break;
                    case "4": BookSeat(); break;
                    case "5": ViewFlightManifest(); break;
                    case "6": CancelBooking(); break;
                    case "7": SaveData(); break;
                    case "8": exit = true; break;
                    default: Console.WriteLine("Invalid option. Please try again."); break;
                }
            }
        }

        private static void AddFlight()
        {
            try
            {
                Console.Write("Enter Flight Number (e.g., SV123): ");
                string flightNumber = Console.ReadLine();
                Console.Write("Enter Departure City: ");
                string departureCity = Console.ReadLine();
                Console.Write("Enter Arrival City: ");
                string arrivalCity = Console.ReadLine();
                Console.Write("Enter Departure Time (YYYY-MM-DD HH:mm): ");
                DateTime departureTime = Convert.ToDateTime(Console.ReadLine());
                Console.Write("Enter Total Seats: ");
                int totalSeats = Convert.ToInt32(Console.ReadLine());

                _flightRepo.Add(new Flight
                {
                    FlightNumber = flightNumber,
                    DepartureCity = departureCity,
                    ArrivalCity = arrivalCity,
                    DepartureTime = departureTime,
                    TotalSeats = totalSeats
                });
                Console.WriteLine("=> Flight added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void RegisterPassenger()
        {
            try
            {
                Console.Write("Enter Passenger Name: ");
                string name = Console.ReadLine();
                Console.Write("Enter Passport Number: ");
                string passport = Console.ReadLine();

                _passengerRepo.Add(new Passenger { Name = name, PassportNumber = passport });
                Console.WriteLine("=> Passenger registered successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void SearchFlights()
        {
            try
            {
                Console.Write("Enter Departure City: ");
                string departureCity = Console.ReadLine();
                Console.Write("Enter Arrival City: ");
                string arrivalCity = Console.ReadLine();

                var flights = _flightRepo.FindByRoute(departureCity, arrivalCity);
                if (!flights.Any())
                {
                    Console.WriteLine("No flights found for this route.");
                    return;
                }

                Console.WriteLine("\n--- Available Flights ---");
                foreach (var flight in flights)
                {
                    var bookedSeats = _bookingRepo.GetByFlightId(flight.Id).Count;
                    Console.WriteLine($"ID: {flight.Id}, Number: {flight.FlightNumber}, Time: {flight.DepartureTime}, Available Seats: {flight.TotalSeats - bookedSeats}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void BookSeat()
        {
            try
            {
                Console.WriteLine("\n--- Available Flights ---");
                _flightRepo.GetAll().ForEach(f => Console.WriteLine($"ID: {f.Id}, Number: {f.FlightNumber}, From: {f.DepartureCity}, To: {f.ArrivalCity}"));
                Console.Write("Enter Flight ID to book on: ");
                int flightId = Convert.ToInt32(Console.ReadLine());
                var flight = _flightRepo.GetById(flightId);
                if (flight == null) { Console.WriteLine("Invalid Flight ID."); return; }

                var bookedSeats = _bookingRepo.GetByFlightId(flightId).Count;
                if (bookedSeats >= flight.TotalSeats)
                {
                    Console.WriteLine("Sorry, this flight is fully booked.");
                    return;
                }

                Console.WriteLine("\n--- Registered Passengers ---");
                _passengerRepo.GetAll().ForEach(p => Console.WriteLine($"ID: {p.Id}, Name: {p.Name}, Passport: {p.PassportNumber}"));
                Console.Write("Enter Passenger ID to book for: ");
                int passengerId = Convert.ToInt32(Console.ReadLine());
                if (_passengerRepo.GetById(passengerId) == null) { Console.WriteLine("Invalid Passenger ID."); return; }

                string seatNumber = $"S{bookedSeats + 1}"; // Simple seat assignment

                var booking = _bookingRepo.Add(new Booking { FlightId = flightId, PassengerId = passengerId, SeatNumber = seatNumber });
                Console.WriteLine($"=> Booking confirmed! ID: {booking.Id}, Seat Number: {seatNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void ViewFlightManifest()
        {
            try
            {
                Console.WriteLine("\n--- All Flights ---");
                _flightRepo.GetAll().ForEach(f => Console.WriteLine($"ID: {f.Id}, Number: {f.FlightNumber}, From: {f.DepartureCity} To: {f.ArrivalCity}"));
                Console.Write("Enter Flight ID to view its manifest: ");
                int flightId = Convert.ToInt32(Console.ReadLine());

                var flight = _flightRepo.GetById(flightId);
                if (flight == null) { Console.WriteLine("Invalid Flight ID."); return; }

                var bookings = _bookingRepo.GetByFlightId(flightId);
                if (!bookings.Any())
                {
                    Console.WriteLine("This flight has no passengers yet.");
                    return;
                }

                Console.WriteLine($"\n--- Passenger Manifest for Flight {flight.FlightNumber} ---");
                foreach (var booking in bookings)
                {
                    var passenger = _passengerRepo.GetById(booking.PassengerId);
                    Console.WriteLine($"Seat: {booking.SeatNumber}, Name: {passenger?.Name}, Passport: {passenger?.PassportNumber}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void CancelBooking()
        {
            try
            {
                Console.Write("Enter Booking ID to cancel: ");
                int bookingId = Convert.ToInt32(Console.ReadLine());

                if (_bookingRepo.Remove(bookingId))
                {
                    Console.WriteLine("=> Booking cancelled successfully.");
                }
                else
                {
                    Console.WriteLine("Booking ID not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void SaveData()
        {
            var dataStore = new DataStore
            {
                Flights = _flightRepo.GetAll(),
                Passengers = _passengerRepo.GetAll(),
                Bookings = _bookingRepo.GetAll() // الآن هذه الدالة موجودة وستعمل بشكل صحيح
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(dataStore, options);
            File.WriteAllText(dataFilePath, jsonString);
            Console.WriteLine($"=> Data successfully saved to: {dataFilePath}");
        }

        private static void LoadData()
        {
            if (!File.Exists(dataFilePath))
            {
                Console.WriteLine("Data file not found. Starting with a clean slate.");
                return;
            }

            string jsonString = File.ReadAllText(dataFilePath);
            var dataStore = JsonSerializer.Deserialize<DataStore>(jsonString);

            if (dataStore != null)
            {
                _flightRepo.LoadData(dataStore.Flights);
                _passengerRepo.LoadData(dataStore.Passengers);
                _bookingRepo.LoadData(dataStore.Bookings);
                Console.WriteLine("=> Data loaded successfully.");
            }
        }
    }
}