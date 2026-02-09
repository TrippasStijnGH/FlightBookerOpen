
using FlightBooker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using FlightBooker.Domains;
using FlightBooker.Util.Mail.Interfaces;
using FlightBooker.ViewModels;

namespace FlightBooker.Controllers
{
    public class SendController : Controller
    {
        private readonly IEmailSend _emailSender;


        public SendController(IEmailSend emailSender)
        {
            _emailSender = emailSender;


        }

        public IActionResult Contact()

        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(SendMailVM sendMailVM)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    _emailSender.SendEmailAsync
                        (sendMailVM.FromEmail,
                        "contact pagina",
                        sendMailVM.Message);
                    return View("Thanks");
                }
                catch (Exception ex)
                {

                }

            }

            return View();

        }
    }
}


