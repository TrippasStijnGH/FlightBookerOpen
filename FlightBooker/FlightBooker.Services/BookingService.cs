using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Repositories;
using FlightBooker.Repositories.Interfaces;
using FlightBooker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBooker.Services

{
    
    public class BookingService : IBookingService
    {
        private readonly IBookingDAO _bookingDAO;
        private readonly IBookingDetailDAO _bookingDetailDAO;
        private readonly IFlightService _flightService;
        private readonly IFlightClassDAO _flightClassDAO;
        

        public BookingService(IBookingDAO bookingDAO, IBookingDetailDAO bookingdetailDAO, IFlightService flightService, IFlightClassDAO flightClassDAO)
        {
            _bookingDAO = bookingDAO;
            _flightService = flightService;
            _flightClassDAO = flightClassDAO;
            _bookingDetailDAO  = bookingdetailDAO;
            
        }

        public async Task<Booking?> FindByIdAsync(int id)
        {
            return await _bookingDAO.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Booking>?> GetAllAsync()
        {
            return await _bookingDAO.GetAllAsync();
        }

        public async Task AddAsync(Booking entity)
        {
            await _bookingDAO.AddAsync(entity);
        }

        public async Task UpdateAsync(Booking entity)
        {
            await _bookingDAO.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Booking entity)
        {
            await _bookingDAO.DeleteAsync(entity);
        }

        
        public async Task<IEnumerable<Booking>?> FindByUserIdAsync(string userId)
        {
            return await _bookingDAO.FindByUserIdAsync(userId);
        }
       


        public async Task ConfirmBookingAsync(int bookingId)
        {
            var booking = await _bookingDAO.FindByIdAsync(bookingId);
            booking.Status = "Confirmed";
            await _bookingDAO.UpdateAsync(booking);

            
            foreach (var detail in booking.BookingDetails)
            {
                detail.SeatNumber = await GetFirstAvailableSeatAsync(detail.FlightId, booking.Class);
                await _bookingDetailDAO.UpdateAsync(detail);
            }


        }

        public async Task CancelBookingAsync(int bookingId)
        {
            var booking = await _bookingDAO.FindByIdAsync(bookingId);

            booking.Status = "Cancelled";
            await _bookingDAO.UpdateAsync(booking);
            
        }

        public async Task<int> PopularDestinationUser(String userId)
        {

            var mostVisitedCity = await _bookingDAO.PopularDestinationUser(userId);

            if (mostVisitedCity == null)
            {
                Random random = new Random();
                mostVisitedCity = random.Next(1, 8);
            }

            return mostVisitedCity ?? 1;
        }
          





        public async Task<int> CreateBookingAsync(int[] flightIds, string classtype, string userId, bool isCart = false)
        {
            var flights = new List<Flight>();
            foreach (int id in flightIds)
            {
                flights.Add(await _flightService.FindByIdAsync(id));
            }

            flights = flights.OrderBy(f => f.DateTimeDepart).ToList();




            var booking = new Booking
            {
                UserId = userId,
                Status = isCart ? "Cart" : "Pending", 
                Class = classtype,
                Price = 0,
                BookingDetails = new List<BookingDetail>(),
                BookingDate = DateTime.Now,
                DepartureCity = flights.First().ArriveCity,
                ArrivalCity = flights.Last().DepartCity,
                DepartureDate = flights.First().DateTimeDepart,            
                ArrivalDate = flights.Last().DateTimeArrive
            };

            decimal totalPrice = 0;
            
            foreach (var flightId in flightIds)
            {
                var flight = await _flightService.FindByIdAsync(flightId);
                if (flight == null) continue;
                decimal flightPrice = CalculateRealPrice(flight, classtype);
                totalPrice += flightPrice;
                var bookingDetail = new BookingDetail
                {
                    
                    FlightId = flightId,
                    Price = flightPrice,
                    Booking = booking,
                };
                booking.BookingDetails.Add(bookingDetail);
            }
            booking.Price = totalPrice;
            await _bookingDAO.AddAsync(booking);
            return booking.BookingId;
        }



        public decimal CalculateRealPrice(Flight flight, string Class)
        {
            decimal finalPrice = flight.BasePrice;
            


            DateTime? travelDate = flight.DateTimeArrive;
            int destinationId = flight.ArriveCity;



            bool isOneMonthBeforeChristmas =
                (travelDate.Value.Month == 12 && travelDate.Value.Day <= 25) || 
                (travelDate.Value.Month == 11 && travelDate.Value.Day >= 25);   

            if ((destinationId == 3 || destinationId == 4) && isOneMonthBeforeChristmas)
            {
               
                finalPrice *= 1.3m;
            }

            
            if ((destinationId == 7 || destinationId == 6 || destinationId == 1) &&
                (travelDate.Value.Month == 7 || travelDate.Value.Month == 8))
            {
               
                finalPrice *= 1.3m;
            }

            switch (Class.ToLower())
            {
                case "business":
                    finalPrice *= 2.5m;
                    break;

                default:
                    finalPrice *= 1m;
                    break;
            }


            return finalPrice;
        }


        public async Task<int> GetFirstAvailableSeatAsync(int flightId, string travelClass)
        {
            
            var flightClasses = await _flightClassDAO.GetFCsByFlightAsync(flightId);

            
            var requestedClass = flightClasses.FirstOrDefault(fc => fc.ClassType == travelClass);
            if (requestedClass == null) return -1;

           
            var economyClass = flightClasses.FirstOrDefault(fc => fc.ClassType == "Economy");
            var businessClass = flightClasses.FirstOrDefault(fc => fc.ClassType == "Business");

            if (economyClass == null || businessClass == null) return -1;

            
            int startSeat, endSeat;
            if (requestedClass.ClassType == "Economy")
            {
                startSeat = 1;
                endSeat = economyClass.MaxBookings;
            }
            else
            { 
                startSeat = economyClass.MaxBookings + 1;
                endSeat = economyClass.MaxBookings + businessClass.MaxBookings;
            }

            var bookedSeats = await _bookingDetailDAO.BookingsInFlightClassAsync(flightId, travelClass);

            


            // Increment elke seat en kijk of die in de lijst booked seats zit, if not wijs die toe
            for (int seat = startSeat; seat <= endSeat; seat++)
            {
                if (!bookedSeats.Contains(seat))
                {
                    return seat;
                }
            }

            return -1;
        }




        public async Task<bool> IsBookingAvailable(int bookingId)
        {
            Booking booking = await FindByIdAsync(bookingId);

            bool available = true;

            var flights = await _flightService.GetFlightsForBookingAsync(bookingId);

            foreach (var flight in flights)
            {
                available = await IsFlightAvailableAsync(flight.FlightId, booking.Class, 1);
                

            }

            return available;

        }

        public async Task<bool> IsFlightAvailableAsync(int flightId, string classType, int numberOfSeats)
        {
            var flight = await _flightService.FindByIdAsync(flightId);
            if (flight == null)
                return false;

           
            var flightClass = flight.FlightClasses.FirstOrDefault(fc => fc.ClassType == classType);
            if (flightClass == null)
                return false;



            
            var bookingDetails = await BookingsInFlightClassAsync(flightId, classType);


            int bookedSeats = bookingDetails?.Count() ?? 0;



            
            return (flightClass.MaxBookings - bookedSeats) >= numberOfSeats;
        }



        public async Task<IEnumerable<int?>> BookingsInFlightClassAsync(int flightId, string travelClass)
        {
            return await _bookingDetailDAO.BookingsInFlightClassAsync(flightId, travelClass);
        }

        public async Task UpdateFlightMealAsync(int bookingId, int flightId, int mealId)
        {
            
            var bookingDetail = await _bookingDetailDAO.FindByBookingAndFlight(bookingId, flightId);

                
            bookingDetail.MealId = mealId;

                
             await _bookingDetailDAO.UpdateAsync(bookingDetail);
            
          
        }





    }


}
