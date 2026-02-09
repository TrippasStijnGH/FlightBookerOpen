using FlightBooker.Domains.EntitiesDB;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FlightBooker.ViewModels


{
    public class DepartDateAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime departureDate = (DateTime)value;

            // Check if departure date is at least one week in the future
            if (departureDate < DateTime.Now.AddDays(3))
            {
                return new ValidationResult("Departure date must be at least 3 days in the future.");
            }

            return ValidationResult.Success;
        }
    }

    
    public class ArriveDateAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime arriveDate = (DateTime)value;

            // Check if departure date is at least one week in the future
            if (arriveDate > DateTime.Now.AddMonths(6))
            {
                return new ValidationResult("Arrival date must be at least within the next 6 months.");
            }

            return ValidationResult.Success;
        }
    }

    public class DifferentCitiesAttribute : ValidationAttribute
    {


        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {


            var model = (FlightSearchVM)validationContext.ObjectInstance;


            if (!string.IsNullOrEmpty(model.DepartCity) &&
                !string.IsNullOrEmpty(model.ArrivalCity) &&
                model.DepartCity == model.ArrivalCity)
            {
                return new ValidationResult("Vertrek en aankonst moeten verschillen.");
            }

            return ValidationResult.Success;
        }
    }

    public class FlightSearchVM
    {
        [DifferentCities]
        [Required(ErrorMessage = "Departure city is required")]
        [Display(Name = "Departure City")]
        public string DepartCity { get; set; }

        [Required(ErrorMessage = "Arrival city is required")]
        [Display(Name = "Arrival City")]
        public string ArrivalCity { get; set; }

        [DepartDate]
        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        
        [ArriveDate]
        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Class is required")]
        [Display(Name = "Class")]
        public string TravelClass { get; set; }

        public IEnumerable<SelectListItem>? AvailableCities { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> AvailableClasses { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Economy", Text = "Economy" },
            new SelectListItem { Value = "Business", Text = "Business" },

        };

        public IEnumerable<IEnumerable<Flight>> GroupedFlights { get; set; } = new List<List<Flight>>();

        public BannerVM? BannerVM { get; set; }




    }
}
