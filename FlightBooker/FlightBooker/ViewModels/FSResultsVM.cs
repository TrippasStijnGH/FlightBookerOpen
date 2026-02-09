using FlightBooker.Domains.EntitiesDB;
using System.ComponentModel.DataAnnotations;

namespace FlightBooker.ViewModels
{

    public class FSResultsVM
    {
        public List<List<FlightVM>> GroupedFlights { get; set; } = new List<List<FlightVM>>();

        public string TravelClass { get; set; }

        public string FlightIds {  get; set; }




        

        

         
    }

    
}
 