 public async Task<IEnumerable<IEnumerable<Flight>?>> SearchFlightsAsync(int departCity, int arriveCity, DateTime startDate, DateTime endDate, string Class)
 {
     List<List<Flight>> answer = new List<List<Flight>>();

     var route = await dbContext.RouteItems
     .Where(r => r.DepartCity == departCity && r.ArrivalCity == arriveCity)
     .FirstOrDefaultAsync();

     if (route.Direct == true)
     {

         var directflights = await dbContext.Flights
         .Where(f => f.DateTimeDepart >= startDate && f.DateTimeDepart <= endDate)
         .Where(f => f.DepartCity == departCity && f.ArriveCity == arriveCity)
         .Include(f => f.DepartCityNavigation)
         .Include(f => f.ArriveCityNavigation)
         .Include(f => f.FlightClasses)
         .Include(f => f.Route)
         .ToListAsync();

         var listflights = new List<List<Flight>>();
         foreach (var flight in directflights)
         {
             var aflight = new List<Flight>();
             aflight.Add(flight);
             listflights.Add(aflight);
         }
         answer = listflights;
     }
     else
     {
         var flightsGrouped = await dbContext.Flights
             .Where(f => f.RouteId == route.RouteId)
             .Where(f => f.DateTimeDepart >= startDate && f.DateTimeDepart <= endDate)
             .Include(f => f.DepartCityNavigation)
             .Include(f => f.ArriveCityNavigation)
             .Include(f => f.FlightClasses)
             .Include(f => f.Route)
             .GroupBy(fr => fr.JourneyId)
             .Select(group => group.OrderBy(f => f.SequenceNumber)
                                  .ToList())
             .ToListAsync();

         answer = flightsGrouped;
     }

     List<List<Flight>> answerchecked = new List<List<Flight>>();

     foreach (List<Flight> flights in answer)
     {
         bool groupAvailable = true;

         foreach (Flight f in flights)
         {
             bool available = await IsFlightAvailableAsync(f.FlightId, Class, 1);
             if (!available)
             {
                 groupAvailable = false;
                 break; 
             }
         }
         if (groupAvailable)
         {
             answerchecked.Add(flights); 
         }
     }
     return answerchecked;

     
 }