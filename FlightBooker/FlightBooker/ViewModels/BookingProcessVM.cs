using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FlightBooker.ViewModels
{
    public class BookingProcessVM
    {
        public BookingVM? Bookingvm { get; set; }



        public int BookingId { get; set; }
        
        
        public List<FlightMealSelectionViewModel> FlightMeals { get; set; } = new List<FlightMealSelectionViewModel>();
    }

    public class FlightMealSelectionViewModel
    {
        public int FlightId { get; set; }

        public int? CurrentMeal {  get; set; }
        public string? Route { get; set; }

        [Required(ErrorMessage = "Selecteer een maaltijd")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecteer een maaltijd")]
        public int SelectedMealId { get; set; }

        
        public IEnumerable<SelectListItem>? AvailableMeals { get; set; } = new List<SelectListItem>();
    }
}
