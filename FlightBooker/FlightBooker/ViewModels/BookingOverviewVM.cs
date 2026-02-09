using System.ComponentModel.DataAnnotations;


namespace FlightBooker.ViewModels
{


    public class CancelDateAttribute : ValidationAttribute
    {
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime departureDate = (DateTime)value;

            
            if (departureDate < DateTime.Now.AddDays(7))
            {
                return new ValidationResult("Departure date must be at least one week in the future.");
            }

            return ValidationResult.Success;
        }
    }

    public class BookingOverviewVM
    {
        public BookingVM? Bookingvm { get; set; }
        
    }
}
