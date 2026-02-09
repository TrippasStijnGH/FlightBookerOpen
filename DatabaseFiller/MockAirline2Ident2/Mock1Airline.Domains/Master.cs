using Mock1Airline.Domains.DataDB;
using Mock1Airline.Domains.EntitiesDB;


using System;
using System.IO;

using OfficeOpenXml;
using Mock1Airline.Domains.DataDB;
using Mock1Airline.Domains.EntitiesDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;
using System.Globalization;
using Microsoft.Identity.Client;


namespace Mock1Airline.Domains
{
    public class Master
    {
        private readonly FlightBookingDbContext _context;

        public Master()
        {
            _context = new FlightBookingDbContext();


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task ImportRoutesFromExcel()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "FlightRoutes2.xlsx");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Excel file not found", filePath);

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;


            for (int row = 2; row <= rowCount; row++)
            {
                var routeID = Convert.ToInt32(worksheet.Cells[row, 5].Value);
                var departCity = Convert.ToInt32(worksheet.Cells[row, 1].Value?.ToString().Trim());
                var arrivalCity = Convert.ToInt32(worksheet.Cells[row, 4].Value?.ToString().Trim());
                var isDirect = Convert.ToBoolean(worksheet.Cells[row, 6].Value);





                var route = new RouteItem
                {
                    RouteId = routeID,
                    DepartCity = departCity,
                    ArrivalCity = arrivalCity,
                    Direct = isDirect
                };

                _context.RouteItems.Add(route);

            }

            await _context.SaveChangesAsync();
        }

        public async Task ImportFlightRoutesFill()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "FlightRoutes4.xlsx");

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;


            for (int row = 2; row <= rowCount; row++)
            {


                int flightRouteId = Convert.ToInt32(worksheet.Cells[row, 1].Value);


                int? parentRouteId = null;
                if (worksheet.Cells[row, 2].Value != null)
                    parentRouteId = Convert.ToInt32(worksheet.Cells[row, 2].Value);

                int? ownRouteId = null;
                if (worksheet.Cells[row, 3].Value != null)
                    ownRouteId = Convert.ToInt32(worksheet.Cells[row, 3].Value);

                int departCity = Convert.ToInt32(worksheet.Cells[row, 4].Value?.ToString().Trim());
                int arrivalCity = Convert.ToInt32(worksheet.Cells[row, 5].Value?.ToString().Trim());

                int? sequenceNumber = null;
                if (worksheet.Cells[row, 6].Value != null)
                    sequenceNumber = Convert.ToInt32(worksheet.Cells[row, 6].Value);

                int? flightTime = null;
                if (worksheet.Cells[row, 7].Value != null)
                    flightTime = Convert.ToInt32(worksheet.Cells[row, 7].Value);

                float? price = null;
                if (worksheet.Cells[row, 8].Value != null)
                    price = Convert.ToSingle(worksheet.Cells[row, 8].Value);




                var flightRoutesFill = new FlightRoutesFill
                {
                    FlightRouteIds = flightRouteId,
                    ParentRouteIds = parentRouteId ?? 1 ,
                    OwnRouteIds = ownRouteId ?? 1,
                    DepartCityS = departCity,
                    ArrivalCityS = arrivalCity,
                    FlightTime = flightTime,
                };

                _context.FlightRoutesFills.Add(flightRoutesFill);



            }

            await _context.SaveChangesAsync();
        }


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




        }

        public async Task CreateFlightClasses()
        {
            var flights = await _context.Flights
        .Include(f => f.FlightClasses)
        .Where(f => !f.FlightClasses.Any())
        .ToListAsync();


            foreach (var flight in flights)
            {

                var businessClass = new FlightClass
                {
                    FlightId = flight.FlightId,
                    ClassType = "Business",
                    MaxBookings = 40,
                    Flight = flight
                };


                var economyClass = new FlightClass
                {
                    FlightId = flight.FlightId,
                    ClassType = "Economy",
                    MaxBookings = 300,
                    Flight = flight
                };


                _context.FlightClasses.Add(businessClass);
                _context.FlightClasses.Add(economyClass);
            }


            await _context.SaveChangesAsync();


        }



    }
}

