 public async Task<int?> PopularDestinationUser(String userId)
 {
     var userBookings = await dbContext.Bookings
  .Where(b => b.UserId == userId)
  .Include(f => f.BookingDetails)
     .ThenInclude(bd => bd.Flight)
  .ToListAsync();

     if (!userBookings.Any())
         return null; 

     
     var destinationCounts = new Dictionary<int, int>();

     
     foreach (var booking in userBookings)
     {
         
         var details = booking.BookingDetails;
         var flights = booking.BookingDetails.Select(booking => booking.Flight).ToList();    
         if (flights.Any())
         {
             var flightsList = flights.ToList().OrderBy(f => f.DateTimeDepart).ToList();
             var arrivalCity = flightsList.Last().ArriveCity;

             
             if (destinationCounts.ContainsKey(arrivalCity))
                 destinationCounts[arrivalCity]++;
             else
                 destinationCounts[arrivalCity] = 1;
         }
     }

     
     if (!destinationCounts.Any())
         return null;

     return destinationCounts.OrderByDescending(x => x.Value).First().Key;

     
 }