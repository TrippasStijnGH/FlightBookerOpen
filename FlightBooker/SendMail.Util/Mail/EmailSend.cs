using Microsoft.Extensions.Options;
using FlightBooker.Util.Mail;
using FlightBooker.Util.Mail.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace FlightBooker.Util
{


    public class EmailSend : IEmailSend
    {
        private readonly EmailSettings _emailSettings;


        public EmailSend(IOptions<EmailSettings> emailSettings)
        {

            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {

            var mail = new MailMessage();  
            mail.To.Add(new MailAddress(email));
            mail.From = new
                    MailAddress("stijntrippas1@gmail.com");  
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            try
            {
                await SmtpMailAsync(mail);
            }
            catch (Exception ex)
            { throw ex; }
        }




        private async Task SmtpMailAsync(MailMessage mail)
        {
            using (var smtp = new SmtpClient(_emailSettings.MailServer))
            {
                smtp.Port = _emailSettings.MailPort;
                smtp.EnableSsl = true;
                smtp.Credentials =
                    new NetworkCredential(_emailSettings.Sender,
                                            _emailSettings.Password);
                smtp.UseDefaultCredentials = false;
                await smtp.SendMailAsync(mail);
            }
        }
    }

}
