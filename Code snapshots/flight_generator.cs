 public async Task GenerateFlights()
 {

     DateTime startDate = new DateTime(2025, 4, 1);
     DateTime endDate = new DateTime(2026, 5, 31);
     Random random = new Random();


     var cities = _context.Cities.ToList();
     var routes = _context.RouteItems.ToList();
     var flightRoutesFills = _context.FlightRoutesFills.ToList();
    
     int nextJourneyId = 2;

     for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
     {

         foreach (var destCity in cities)
         {

             var arrivingRoutes = routes.Where(r => r.ArrivalCity.Equals(destCity.CityId)).ToList();

             int dayOffset = (int)(currentDate - startDate).TotalDays % arrivingRoutes.Count;

             var route = arrivingRoutes.Count > 0 ? arrivingRoutes[dayOffset % arrivingRoutes.Count] : null;

             if (route != null)
             {

                 int arrivalHour = random.Next(0, 24);

                 DateTime localArrivalTime = new DateTime(
                     currentDate.Year,
                     currentDate.Month,
                     currentDate.Day,
                     arrivalHour,
                     0,
                     0
                 );


                 DateTime utcArrivalTime = localArrivalTime.AddHours(-(double)destCity.Utcoffset);

                 if (route.Direct)
                 {
                     var flightDetails = flightRoutesFills
                         .FirstOrDefault(f => f.OwnRouteIds == route.RouteId);

                     if (flightDetails != null)
                     {
                         DateTime utcDepartureTime = utcArrivalTime.AddMinutes(-(double)flightDetails.FlightTime);


                         Flight newFlight = new Flight
                         {
                             
                             DepartCity = route.DepartCity,
                             ArriveCity = route.ArrivalCity,
                             DateTimeDepart = utcDepartureTime,
                             DateTimeArrive = utcArrivalTime,
                             BasePrice = flightDetails.Price,
                             RouteId = route.RouteId,                                   
                             JourneyId = 1

                         };

                         await _context.Flights.AddAsync(newFlight);
                         


                     }
                 }
                 else
                 {
                     var componentFlights = flightRoutesFills
                         .Where(f => f.ParentRouteIds == route.RouteId)
                         .OrderByDescending(f => f.SequenceNumber)
                         .ToList();

                     if (componentFlights.Any())
                     {
                         DateTime currentArrivalTime = utcArrivalTime;

                         foreach (var component in componentFlights)
                         {
                             DateTime departureTime = currentArrivalTime.AddMinutes(-(double)component.FlightTime);

                             Flight newFlight = new Flight
                             {

                                 DepartCity = component.DepartCityS,
                                 ArriveCity = component.ArrivalCityS,
                                 DateTimeDepart = departureTime,
                                 DateTimeArrive = currentArrivalTime,
                                 BasePrice = component.Price,
                                 RouteId = route.RouteId,
                                 SequenceNumber = component.SequenceNumber,
                                 JourneyId = nextJourneyId

                             };

                             await _context.Flights.AddAsync(newFlight);
                             



                             currentArrivalTime = departureTime.AddHours(-3);
                         }
                         nextJourneyId++;
                     }

                     
                 }

                 
             }

             
         }
     }

     await _context.SaveChangesAsync();