using Microsoft.AspNetCore.Mvc;
using FlightBooker.ViewModels;
using FlightBooker.Domains;
using FlightBooker.Domains.DataDB;
using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Services.Interfaces;
using FlightBooker.Services;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlightBooker.Repositories;
using Microsoft.AspNetCore.Identity;
using FlightBooker.Data;


namespace FlightBooker.Controllers
{
    
    public class FlightSearchController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly IService<City> _cityService;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public FlightSearchController(IMapper mapper,
            IFlightService flightService, 
            IService<City> cityService, 
            IBookingService bookingService,
            UserManager<ApplicationUser> userManager
            )


        {
            _mapper = mapper;
            _flightService = flightService;
            _cityService = cityService;
            _bookingService = bookingService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            
            var searchVM =  new FlightSearchVM();


            var cities = await _cityService.GetAllAsync();
            List<CityVM> list = _mapper.Map<List<CityVM>>(cities);

            searchVM.AvailableCities = list.Select(c => new SelectListItem
            {
                Text = c.FullName,
                Value = c.CityId.ToString()
            });

            searchVM.StartDate = DateTime.Now;
            searchVM.EndDate = DateTime.Today.AddDays(7);

            var user = await _userManager.GetUserAsync(User);


            // een stad toekennen op basis van de meest geboekte bestemming
            City city = new City();
            int mostVisitedCity = 1;

            if (user != null)
            {
                mostVisitedCity = await _bookingService.PopularDestinationUser(user.Id);
                city = await _cityService.FindByIdAsync(mostVisitedCity);


            }

            else
            {
                Random random = new Random();
                mostVisitedCity = random.Next(1, 8);
                city = await _cityService.FindByIdAsync(mostVisitedCity);
            }

            var bannerVM = new BannerVM
            {
                DestinationName = city.FullName,
                ImageUrl = $"/images/{mostVisitedCity}.jpg"

            };

            searchVM.BannerVM = bannerVM;
            


            return View(searchVM);
        }

        //zoekresultaten tonen
        [HttpPost]
        public async Task<IActionResult> Results(FlightSearchVM model)
        {
            var resultmodel = new FSResultsVM()
            {
                TravelClass = model.TravelClass

            };


            if (ModelState.IsValid)
            {
                int Depart = Convert.ToInt32(model.DepartCity);
                int Arrive = Convert.ToInt32(model.ArrivalCity);

                var groups = await _flightService.SearchFlightsAsync(
                    Depart,
                    Arrive,
                    model.StartDate,
                    model.EndDate.AddDays(1).AddSeconds(-1), 
                    model.TravelClass

                  
                );

                

                var groupsVM = new List<List<FlightVM>>();

                
                foreach (var group in groups)
                {
                    
                    

                    List<FlightVM> list = _mapper.Map<List<FlightVM>>(group);
                    var altList = new List<FlightVM> ();
                    foreach (var flight in group)
                    {
                        FlightVM flightvm = _mapper.Map<FlightVM>(flight);
                        

                        flightvm.RealPrice = _bookingService.CalculateRealPrice(flight,model.TravelClass);
                            
                        altList.Add(flightvm);

                    }


                    resultmodel.GroupedFlights.Add(altList);
                }

                


                return View("Results", resultmodel);
            }





            // als er een fout is model terug maken en de pagina opniew weergeven
            var cities = await _cityService.GetAllAsync();
            List<CityVM> citylist = _mapper.Map<List<CityVM>>(cities);

            model.AvailableCities = citylist.Select(c => new SelectListItem
            {
                Text = c.FullName,
                Value = c.CityId.ToString()
            });

            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Today.AddDays(7);

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var mostVisitedCity = await _bookingService.PopularDestinationUser(user.Id);


                var City = await _cityService.FindByIdAsync(mostVisitedCity);


                var bannerVM = new BannerVM
                {
                    DestinationName = City.FullName,
                    ImageUrl = $"/images/{mostVisitedCity}.jpg"

                };

                model.BannerVM = bannerVM;
            }

            return View("Index", model);

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



    }
}
