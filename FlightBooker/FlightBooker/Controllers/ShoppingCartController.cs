using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Repositories;
using FlightBooker.Services;
using FlightBooker.Services.Interfaces;
using FlightBooker.Repositories.Interfaces;
using System.Security.Claims;
using FlightBooker.ViewModels;
using FlightBooker.Extentions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using FlightBooker.Data;

namespace FlightBooker.Controllers
{
    
    public class ShoppingCartController : Controller  
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShoppingCartService _cartService;
        private readonly IBookingService _bookingService;
        private readonly IFlightService _flightService;
        private readonly IMapper _mapper;

        public ShoppingCartController(UserManager<ApplicationUser> userManager, 
            IShoppingCartService cartService, 
            IBookingService bookingService,
            IFlightService flightService,
            IMapper mapper)
        {
            _userManager = userManager;
            _cartService = cartService;
            _bookingService = bookingService;
            _flightService = flightService;
            _mapper = mapper;
        }
        
        //maakt een ShoppingCart enkel met een Session object als de user aangelogd is, anders met de database
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                CartVM? cartList = HttpContext.Session.GetObject<CartVM>("ShoppingCart");
                ShoppingCartContentVM SCcontentvm = new ShoppingCartContentVM();

                
                if (cartList != null)
                {
                    SCcontentvm.Cart = cartList;
                }
                else
                {
                   
                    SCcontentvm.Cart = new CartVM();
                }

                return View("Cart", SCcontentvm);
            }

            else
            {
              
                
                var cart = await _cartService.GetOrCreateCartForUserAsync(user.Id);
                


                CartVM cartList = new CartVM { Carts = new List<CartItemVM>() };

                foreach (var item in cart.CartItems)
                {
                    Booking booking = await _bookingService.FindByIdAsync(item.BookingId);
                    var bookingDetails = booking.BookingDetails;
                    List<int> flightIds = new List<int>();
                    List<FlightVM> flightVMs = new List<FlightVM>();
                    
                    foreach (var detail in bookingDetails)
                    {
                        flightIds.Add(detail.FlightId);
                    }

                    flightVMs = await CreateFlightVMs(flightIds, booking.Class);

                    cartList.Carts.Add(new CartItemVM
                    {
                        cartId = item.CartItemId,
                        TravelClass = booking.Class,
                        Flights = flightVMs,
                        Price = flightVMs?.Sum(e => e.RealPrice) ?? 0,
                        bookingID = booking.BookingId

                    });

                }

                ShoppingCartContentVM SCcontentvm = new ShoppingCartContentVM();
                SCcontentvm.Cart = cartList;





                return View("Cart", SCcontentvm);



            }

                

            

            
            
        }

       
        [HttpPost]
        public async Task<IActionResult> AddToCart(FSResultsVM resultmodel)
        {
            var flightIdArray = resultmodel.FlightIds.Split(',').Select(int.Parse).ToArray();
            List<int> flightIdList = new List<int>(flightIdArray);

            var user = await _userManager.GetUserAsync(User);

            List<Flight> flights = new List<Flight>();
            List<FlightVM> flightVMs = new List<FlightVM>();


            flightVMs = await CreateFlightVMs(flightIdList, resultmodel.TravelClass);


            if (user != null)
            {

                int bookingid = await _bookingService.CreateBookingAsync(flightIdArray, resultmodel.TravelClass, user.Id,true);



                await _cartService.AddToCartForUser(bookingid, user.Id);
            }
            else
            {
                
                var shoppingCartVM = HttpContext.Session.GetObject<CartVM>("ShoppingCart") ?? new CartVM { Carts = new List<CartItemVM>() };
                
                
                
                   shoppingCartVM.Carts.Add(new CartItemVM
                   {
                       TravelClass = resultmodel.TravelClass,
                       Flights = flightVMs,
                       Price = flightVMs?.Sum(e => e.RealPrice) ?? 0

                    });
                


                HttpContext.Session.SetObject("ShoppingCart", shoppingCartVM);
                return RedirectToAction("Index", "ShoppingCart");


            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> RemoveFromCart(int cartID)
        {
            await _cartService.RemoveItemFromCartAsync(cartID);

            return RedirectToAction("Index");
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
