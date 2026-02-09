using AutoMapper;
using FlightBooker.Data;
using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Services;
using FlightBooker.Services.Interfaces;
using FlightBooker.ViewModels;
using FlightBooker.Util;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using FlightBooker.Util.Mail.Interfaces;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FlightBooker.Controllers
{
    public class BookingController : Controller


    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookingService _bookingService;
        private readonly IFlightService _flightService;
        private readonly IService<Meal> _mealService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IMapper _mapper;
        private readonly IEmailSend _emailSend;
        public BookingController(UserManager<ApplicationUser> userManager,
            IShoppingCartService cartService,
            IBookingService bookingService,
            IFlightService flightService,
            IService<Meal> mealService,
            IShoppingCartService shoppingCartService,
            IMapper mapper,
            IEmailSend emailSend)


        {
            _userManager = userManager;
            _bookingService = bookingService;
            _flightService = flightService;
            _mealService = mealService;
            _shoppingCartService = shoppingCartService;
            _mapper = mapper;
            _emailSend = emailSend;
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);

            var Bookings = await _bookingService.FindByUserIdAsync(user.Id);

            var BookingVMs = new List<BookingVM>();

            foreach (var booking in Bookings)
            {

                var flightlist = await _flightService.GetFlightsForBookingAsync(booking.BookingId);
                var flightIdList = flightlist.Select(flight => flight.FlightId).ToList();

                BookingVMs.Add(await CreateBookingVM(booking.BookingId));

            }

            var model = new MyBookingsVM();
            model.Bookings = BookingVMs;

            return View("MyBookings", model);
        }


        [HttpPost]
        
        public async Task<IActionResult> StartBookingProcessSC(ShoppingCartContentVM model)
        {
            var user = await _userManager.GetUserAsync(User);

            await _shoppingCartService.RemoveItemFromCartAsync(model.SelectedCartItemId ?? 0);
            
            if (user == null)
            {
                return View("Home", "index");
            }

            else
            {

                int bookingIntId = Convert.ToInt32(model.BookingId);
                var booking = await _bookingService.FindByIdAsync(bookingIntId);
                booking.Status = "Pending";
                await _bookingService.UpdateAsync(booking);
                bool available = await _bookingService.IsBookingAvailable(bookingIntId);
                if (available)
                {
                    var BPvm = await CreateBookingProcessVM(bookingIntId);

                    return View("BookingProcess", BPvm);



                }
                else
                {
                    return View("Home", "index");
                }




            }
        }

        [HttpPost]
        // boeking object vanuit de search results (booking object moet nog gemaakt worden)
        public async Task<IActionResult> StartBookingProcessFS(FSResultsVM model)
        {
            var user = await _userManager.GetUserAsync(User);


            var flightIdArray = model.FlightIds.Split(',').Select(int.Parse).ToArray();
            List<int> flightIdList = new List<int>(flightIdArray);

            List<Flight> flights = new List<Flight>();
            List<FlightVM> flightVMs = new List<FlightVM>();

            if (user == null)
            {
                return Redirect("~/Identity/Account/Login");
            }


            int bookingid = await _bookingService.CreateBookingAsync(flightIdArray, model.TravelClass, user.Id);
            var booking = await _bookingService.FindByIdAsync(bookingid);
            bool available = await _bookingService.IsBookingAvailable(bookingid);
            if (available)
            {
                var BPvm = await CreateBookingProcessVM(booking.BookingId);
                return View("BookingProcess", BPvm);


            }

            return View("NotAvailable");


        }

        //herzie maaltijdselectie van een bestaande boeking
        public async Task<IActionResult> EditBooking(int bookingId)
        {
            var booking = await _bookingService.FindByIdAsync(bookingId);
            bool available = await _bookingService.IsBookingAvailable(bookingId);
            if (available)
            {

                var bookingvm = await CreateBookingVM(bookingId);



                var meals = await _mealService.GetAllAsync();

                
                var viewModel = new BookingProcessVM
                {
                    Bookingvm = bookingvm,
                    BookingId = bookingvm.BookingId,
                    FlightMeals = booking.BookingDetails.Select(bd => new FlightMealSelectionViewModel
                    {
                        FlightId = bd.FlightId,
                        CurrentMeal = bd.MealId,
                        Route = $"{bd.Flight.DepartCityNavigation.FullName} to {bd.Flight.ArriveCityNavigation.FullName}",
                        SelectedMealId = bd.MealId ?? 0, // Use existing selection if any
                        AvailableMeals = meals.Where(m => m.AreaCode == bd.Flight.ArriveCity || m.AreaCode == null)
                        .Select(m => new SelectListItem
                        {
                            Value = m.MealId.ToString(),
                            Text = m.Name,
                            Selected = bd.MealId == m.MealId
                        }).ToList()
                    }).ToList()
                };






                return View("BookingProcess", viewModel);


            }
            else
            {
                return View("NotAvailable");
            }


        }

        
        [HttpPost]
        public async Task<IActionResult> SaveMealSelections(BookingProcessVM model)
        {

            var bookingId = model.BookingId;
            var available = await _bookingService.IsBookingAvailable(bookingId);
            if (available)
            {
                var bookingvm = await CreateBookingVM(bookingId);
                if (!ModelState.IsValid)

                {
                    //de pagina opniew maken als die niet klopt

                    var booking = await _bookingService.FindByIdAsync(bookingId);


                    var meals = await _mealService.GetAllAsync();

                    model.Bookingvm = bookingvm;
                    model.BookingId = bookingvm.BookingId;
                    model.FlightMeals = booking.BookingDetails.Select(bd => new FlightMealSelectionViewModel
                    {
                        FlightId = bd.FlightId,
                        CurrentMeal = bd.MealId,
                        Route = $"{bd.Flight.DepartCityNavigation.FullName} to {bd.Flight.ArriveCityNavigation.FullName}",
                        SelectedMealId = bd.MealId ?? 0, // Use existing selection if any
                        AvailableMeals = meals.Where(m => m.AreaCode == bd.Flight.ArriveCity || m.AreaCode == null)
                        .Select(m => new SelectListItem
                        {
                            Value = m.MealId.ToString(),
                            Text = m.Name,
                            Selected = bd.MealId == m.MealId
                        }).ToList()
                    }).ToList();



                    return View("BookingProcess", model);

                }


                // de maaltijden toevoegen
                foreach (var flightMeal in model.FlightMeals)
                {

                    await _bookingService.UpdateFlightMealAsync(
                        model.BookingId,
                        flightMeal.FlightId,
                        flightMeal.SelectedMealId);


                }

                var bookingVm = await CreateBookingVM(bookingId);

                var bookingOverviewvm = new BookingOverviewVM
                {
                    Bookingvm = bookingVm,

                };


                return View("BookingOverView", bookingOverviewvm);


            }
            else
            {
                return View("NotAvailable");
            }
        }


        
        public async Task<IActionResult> ConfirmBooking(int bookingId)
        {
            var user = await _userManager.GetUserAsync(User);
            var booking = await _bookingService.FindByIdAsync(bookingId);
            var bookingvm = await CreateBookingVM(bookingId);

            // zien of de boeking ondertussen niet te laat is om te bevestigen
            if (IsConfirmDateValid(bookingvm))
            {


                var available = await _bookingService.IsBookingAvailable(bookingId);

                if (available)
                {
                    await _bookingService.ConfirmBookingAsync(bookingId);

                    //de bookingvm na het toevoegen van de seatnumbers
                    var bookingvmCF = await CreateBookingVM(bookingId);

                    var message = await GenerateEmailAsync(bookingvmCF);

                    await _emailSend.SendEmailAsync(user.Email, "Confirmation", message);

                    var htmlFormattedEmail = message.Replace(Environment.NewLine, "<br />");

                    ViewBag.message = htmlFormattedEmail;
                    return View("BookingThanks");
                }
                else
                {
                    return View("NotAvailable");
                }


            }

            var bookingVm = await CreateBookingVM(bookingId);

            var bookingOverviewvm = new BookingOverviewVM
            {
                Bookingvm = bookingVm
                
            };
            ViewBag.ConfirmMessage = "Vlucthen kunnen ten laatste geboekt worden 3 dagen voor vertrek, de boeking is niet bevestigd";

            return View("BookingOverView", bookingOverviewvm);



        }

        
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
           
            var bookingvm = await CreateBookingVM(bookingId);
           // zien of het ondertussen niet te laat is om te cancellen
            if (IsCancleDateValid(bookingvm))
            {
                
                await _bookingService.CancelBookingAsync(bookingId);
                return View("BookingCancelled");
                
            }

            var bookingOverviewvm = new BookingOverviewVM
            {
                Bookingvm = bookingvm
            };


            ViewBag.CancelMessage = "Boekingen kunnen ten laatste 7 dagen voor vertrek kosteloos gecancelt worden";
            return View("BookingOverView", bookingOverviewvm);

        }

        public async Task<BookingVM> CreateBookingVM(int bookingId)
        {
            Booking booking = await _bookingService.FindByIdAsync(bookingId);
            BookingVM bookingvm = _mapper.Map<BookingVM>(booking);
            var flights = await _flightService.GetFlightsForBookingAsync(booking.BookingId);
            var flightIds = flights.Select(e => e.FlightId).ToList();
            var travelClass = booking.Class;

            var flightslist = await CreateFlightVMs(flightIds, travelClass);
            flightslist = flightslist.OrderBy(f => f.DateTimeDepart).ToList();

            // informatie over begin en eind van de het hele traject 
            if (flightslist.Any())
            {


                bookingvm.DepartureCity = flightslist.First().DepartCityName;
                bookingvm.ArrivalCity = flightslist.Last().ArriveCityName;

                bookingvm.DepartureTimeFirst = flightslist.First().DateTimeDepart;
                bookingvm.ArrivalTimeFirst = flightslist.First().DateTimeArrive;

                bookingvm.DepartureTimeLast = flightslist.Last().DateTimeDepart;
                bookingvm.ArrivalTimeLast = flightslist.Last().DateTimeArrive; 


            }


            bookingvm.Flights = flightslist;

            var bookingDetails = booking.BookingDetails;

            // de juiste maaltijd bij de juiste vlucht plaatsen
            foreach (var flightVM in flightslist)
            {

                var matchingDetail = bookingDetails.FirstOrDefault(bd => bd.FlightId == flightVM.FlightID);

                if (matchingDetail.Meal != null)
                {
                    flightVM.Meal = matchingDetail.Meal.Name;
                }
            }

            //enkel bevestigde vluchten hebben een Seatnumber dus dan die erbij plaatsen
            if (booking.Status == "Confirmed")
            {
                foreach (var flightVM in flightslist)
                {

                    var matchingDetail = bookingDetails.FirstOrDefault(bd => bd.FlightId == flightVM.FlightID);

                    flightVM.SeatNumber = matchingDetail.SeatNumber;

                }
            }




            return bookingvm;

        }




        public async Task<List<FlightVM>> CreateFlightVMs(List<int> flightIds, String travelClass)
        {

            List<Flight> flights = new List<Flight>();

            foreach (var flightId in flightIds)
            {

                Flight flight = await _flightService.FindByIdAsync(flightId);
                flights.Add(flight);
            }

            var flightVMs = new List<FlightVM>();
            foreach (var flight in flights)
            {

                FlightVM flightvm = _mapper.Map<FlightVM>(flight);
                flightvm.RealPrice = _bookingService.CalculateRealPrice(flight, travelClass);



                flightVMs.Add(flightvm);
            }

            return flightVMs;
        }


        public async Task<BookingProcessVM> CreateBookingProcessVM(int bookingId)
        {
            var booking = await _bookingService.FindByIdAsync(bookingId);
            var flights = await _flightService.GetFlightsForBookingAsync(bookingId);
            var flightIds = flights.Select(e => e.FlightId).ToList();
            var bookingvm = await CreateBookingVM(bookingId);



            var meals = await _mealService.GetAllAsync();

            
            var viewModel = new BookingProcessVM
            {
                Bookingvm = bookingvm,
                BookingId = bookingvm.BookingId,
                FlightMeals = booking.BookingDetails.Select(bd => new FlightMealSelectionViewModel
                {
                    FlightId = bd.FlightId,
                    CurrentMeal = bd.MealId,
                    Route = $"{bd.Flight.DepartCityNavigation.FullName} to {bd.Flight.ArriveCityNavigation.FullName}",
                    SelectedMealId = bd.MealId ?? 0, // Use existing selection if any
                    AvailableMeals = meals.Where(m => m.AreaCode == bd.Flight.ArriveCity || m.AreaCode == null)
                    .Select(m => new SelectListItem
                    {
                        Value = m.MealId.ToString(),
                        Text = m.Name,
                        Selected = bd.MealId == m.MealId
                    }).ToList()
                }).ToList()
            };

            return viewModel;



        }

        public bool IsCancleDateValid(BookingVM bookingvm)
        {
            return bookingvm.DepartureTimeFirst > DateTime.Now.AddDays(7);

        }

        public bool IsConfirmDateValid(BookingVM bookingvm)
        {
            return bookingvm.DepartureTimeFirst > DateTime.Now.AddDays(3);
        }

       
        public async Task<String> GenerateEmailAsync(BookingVM booking)
        {
            var user = await _userManager.GetUserAsync(User);

            StringBuilder emailBuilder = new StringBuilder();

            
            emailBuilder.AppendLine("FlightBooker - Booking Confirmation");
            emailBuilder.AppendLine("=======================================");
            emailBuilder.AppendLine();

            
            emailBuilder.AppendLine($"Dear {user.FirstName} {user.LastName},");
            emailBuilder.AppendLine();
            emailBuilder.AppendLine("Bedankt voor het kiezen van FlightBooker, hieronder de informatie betreffende u vlucht");
            emailBuilder.AppendLine();

            
            emailBuilder.AppendLine("BOOKING SUMMARY");
            emailBuilder.AppendLine("---------------");
            emailBuilder.AppendLine($"Booking Reference: {booking.BookingId}");
            emailBuilder.AppendLine($"Status: {booking.Status}");
            emailBuilder.AppendLine($"Travel Class: {booking.Class}");
            emailBuilder.AppendLine($"Booking Date: {booking.BookingDate.ToString("dd MMMM yyyy")}");
            emailBuilder.AppendLine();

            
            emailBuilder.AppendLine("FLIGHT DETAILS");
            emailBuilder.AppendLine("-------------");

            foreach (var flight in booking.Flights)
            {
                emailBuilder.AppendLine($"Flight: {flight.FlightID}");
                emailBuilder.AppendLine($"Route: {flight.DepartCityName} to {flight.ArriveCityName}");
                emailBuilder.AppendLine($"Departure: {flight.LocalDateTimeDepart.ToString("dd/MM/yyyy HH:mm")}");
                emailBuilder.AppendLine($"Arrival: {flight.LocalDateTimeArrive.ToString("dd/MM/yyyy HH:mm")}");
                emailBuilder.AppendLine($"SeatNumber: {flight.SeatNumber}");
                emailBuilder.AppendLine();
            }

            
            if (booking.Flights.Any(f => f.Meal != null))
            {
                emailBuilder.AppendLine("MEAL SELECTIONS");
                emailBuilder.AppendLine("---------------");

                foreach (var flight in booking.Flights)
                {
                    emailBuilder.AppendLine($"Flight {flight.FlightID} ({flight.DepartCityName} to {flight.ArriveCityName}): {flight.Meal ?? "None"}");
                }
                emailBuilder.AppendLine();
            }

            
            emailBuilder.AppendLine("PRICE SUMMARY");
            emailBuilder.AppendLine("-------------");

            foreach (var flight in booking.Flights)
            {
                emailBuilder.AppendLine($"Flight {flight.FlightID} ({flight.DepartCityName} to {flight.ArriveCityName}): {flight.RealPrice.ToString("C")}");
            }

            decimal totalPrice = booking.Flights.Sum(f => f.RealPrice);
            emailBuilder.AppendLine();
            emailBuilder.AppendLine($"Total Price: {totalPrice.ToString("C")}");
            emailBuilder.AppendLine();

            
            
            emailBuilder.AppendLine("---------------------");
           
            
            emailBuilder.AppendLine("Breng een geldig paspoort");
            emailBuilder.AppendLine();



            return emailBuilder.ToString();
        }




    }

    

}
