using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Service
{
    public class EmailService : IEmailService
    {
        public EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendMailAsync(EmailModel email)
        {
            bool res = false;
            try
            {
                await Send(email.To, email.Subject, email.IsBodyHtml, email.Body);
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return res;
        }

        public async Task Send(string email, string subject, bool isBodyHtml, string message)
        {
            
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_emailSettings.UsernameEmail, "Miele");
            mail.To.Add(new MailAddress(email));
            mail.Subject = subject;
            mail.IsBodyHtml = isBodyHtml;
            mail.Body = message;

            using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
            {
                smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
        }
    }
}
