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