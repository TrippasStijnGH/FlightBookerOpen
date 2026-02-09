using System.ComponentModel.DataAnnotations;

namespace FlightBooker.ViewModels
{
    public class SendMailVM
    {
        [Required, Display(Name = "Jouw naam")]
        public string? FromName { get; set; }
        [Required, Display(Name = "Jouw email"), EmailAddress]

        public string? FromEmail { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string? Message { get; set; }
        public bool Invoice { get; set; }
    }
}
