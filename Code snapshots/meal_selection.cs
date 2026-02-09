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