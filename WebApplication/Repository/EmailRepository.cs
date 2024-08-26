using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private EmailSettings _emailSettings { get; set; }
        private EmailSettingsTickets _emailSettingsTickets { get; set; }

        public EmailRepository(IOptions<EmailSettings> emailSettings, IOptions<EmailSettingsTickets> emailSettingsTickets)
        {
            _emailSettings = emailSettings.Value;
            _emailSettingsTickets = emailSettingsTickets.Value;
        }

        //Servicio de envio de correos
        public void SendEmail(EmailModel email)
        {
            try
            {
                //new SmtpClient
                //{
                //    Host = "Smtp.Gmail.com",
                //    Port = 587,
                //    EnableSsl = true,
                //    Timeout = 10000,
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                //    UseDefaultCredentials = false,
                //    Credentials = new NetworkCredential("rodrigo.stps@gmail.com", "bostones01")
                //}.Send(new MailMessage
                //{
                //    From = new MailAddress("no-reply@techo.org", "Miele"),
                //    To = { email.To },
                //    Subject = email.Subject,
                //    IsBodyHtml = email.IsBodyHtml,
                //    Body = email.Body
                //});
                EmailSettings settings = SeleccionaCuenta(email.id_app);
                if (settings == null) return;

                if (settings.DevTest)
                {
                    email.To = settings.MailTest;
                }

                new SmtpClient
                {
                    Host = settings.PrimaryDomain,
                    Port = settings.PrimaryPort,
                    EnableSsl = true,
                    Timeout = 10000,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(settings.UsernameEmail, settings.UsernamePassword)
                }.Send(new MailMessage
                {
                    From = new MailAddress(settings.UsernameEmail, "Miele"),
                    To = { email.To },
                    Subject = email.Subject,
                    IsBodyHtml = email.IsBodyHtml,
                    Body = email.Body
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Servicio asincrono para envio de correos 
        public async Task<string> SendMailAsync(EmailModel email)
        {
            string res = "";
            try
            {
                res = await Send(email.To, email.Subject, email.IsBodyHtml, email.Body, email.id_app);
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        public bool SendMultipleMails(List<MailAddress> mails, string subject, bool isBodyHtml, string message, int id_app, List<Attachment> attachments)
        {
            try
            {
                EmailSettings settings = SeleccionaCuenta(id_app);
                if (settings == null) return false;
               
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(settings.UsernameEmail, settings.MailName);
                mail.Subject = subject;
                mail.IsBodyHtml = isBodyHtml;
                mail.Body = message;
                using (SmtpClient smtp = new SmtpClient(settings.PrimaryDomain, settings.PrimaryPort))
                {
                    smtp.Timeout = 90000;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(settings.UsernameEmail, settings.UsernamePassword);
                    smtp.EnableSsl = _emailSettings.EnableSsl;
                    foreach (var item in attachments)
                    {
                        mail.Attachments.Add(item);
                    }
                    smtp.SendCompleted += (s, e) =>
                    {
                        if (e.Cancelled)
                            reintentoMail(mail, settings);
                        smtp.Dispose();
                        mail.Dispose();
                    };
                    foreach (MailAddress address in mails)
                    {
                        mail.To.Clear();
                        if (settings.DevTest)
                            mail.To.Add(new MailAddress(settings.MailTest));
                        else
                            mail.To.Add(address);

                        smtp.Send(mail);
                    }
                    return true;

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //Servicio asincrono para envio de correos con archivos adjuntos
        public async Task SendAttachment(string email, string subject, bool isBodyHtml, string message, int id_app, List<Attachment> attachments)
        {
            try
            {
                EmailSettings settings = SeleccionaCuenta(id_app);
                if (settings == null) return ;
                if (settings.DevTest)
                {
                    email = settings.MailTest;
                }
                var correos = email.Split(",");
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(settings.UsernameEmail, settings.MailName);
                foreach (var item in correos)
                {
                    mail.To.Add(new MailAddress(item));
                }
                
                mail.Subject = subject;
                mail.IsBodyHtml = isBodyHtml;
                mail.Body = message;
                //var resultado = "";
                
                using (SmtpClient smtp = new SmtpClient(settings.PrimaryDomain, settings.PrimaryPort))
                {
                    smtp.Timeout = 50000;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(settings.UsernameEmail, settings.UsernamePassword);
                    smtp.EnableSsl = _emailSettings.EnableSsl;
                    foreach (var item in attachments)
                    {
                        mail.Attachments.Add(item);
                    }
                    smtp.SendCompleted += (s, e) =>
                    {
                        if (e.Cancelled)
                            reintentoMail(mail, settings);
                        smtp.Dispose();
                        mail.Dispose();
                    };

                    await smtp.SendMailAsync(mail);
                    //return resultado;

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void reintentoMail(MailMessage message, EmailSettings settings)
        {
            try
            {
                using (SmtpClient smtp = new SmtpClient(settings.PrimaryDomain, settings.PrimaryPort))
                {
                    smtp.Timeout = 50000;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(settings.UsernameEmail, settings.UsernamePassword);
                    smtp.EnableSsl = _emailSettings.EnableSsl;
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<string> Send(string email, string subject, bool isBodyHtml, string message, int id_app)
        {

            try
            {
                EmailSettings settings = SeleccionaCuenta(id_app);
                if (settings == null) return "Error";
                if (settings.DevTest)
                {
                    email = settings.MailTest;
                }
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(settings.UsernameEmail, settings.MailName);
                mail.To.Add(new MailAddress(email));
                mail.Subject = subject;
                mail.IsBodyHtml = isBodyHtml;
                mail.Body = message;

                using (SmtpClient smtp = new SmtpClient(settings.PrimaryDomain, settings.PrimaryPort))
                {
                    smtp.Timeout = 10000;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(settings.UsernameEmail, settings.UsernamePassword);
                    smtp.EnableSsl = settings.EnableSsl;
                    await smtp.SendMailAsync(mail);
                }
                return "Enviado";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private EmailSettings SeleccionaCuenta(int id_app)
        {
            EmailSettings cuenta = new EmailSettings();
            if (id_app == 1) //app 1 tickets, app 2 partners
            {
                cuenta.PrimaryDomain = _emailSettingsTickets.PrimaryDomain;
                cuenta.MailName = _emailSettingsTickets.MailName;
                cuenta.PrimaryPort = _emailSettingsTickets.PrimaryPort;
                cuenta.UsernameEmail = _emailSettingsTickets.UsernameEmail;
                cuenta.UsernamePassword = _emailSettingsTickets.UsernamePassword;
                cuenta.DevTest = _emailSettingsTickets.DevTest;
                cuenta.MailTest = _emailSettingsTickets.MailTest;
                cuenta.FromEmail = _emailSettingsTickets.FromEmail;
                cuenta.ToEmail = _emailSettingsTickets.ToEmail;
                cuenta.EnableSsl = _emailSettingsTickets.EnableSsl;
            }
            else if (id_app == 2)
            {
                cuenta.PrimaryDomain = _emailSettings.PrimaryDomain;
                cuenta.MailName = _emailSettings.MailName;
                cuenta.PrimaryPort = _emailSettings.PrimaryPort;
                cuenta.UsernameEmail = _emailSettings.UsernameEmail;
                cuenta.UsernamePassword = _emailSettings.UsernamePassword;
                cuenta.DevTest = _emailSettings.DevTest;
                cuenta.MailTest = _emailSettings.MailTest;
                cuenta.FromEmail = _emailSettings.FromEmail;
                cuenta.ToEmail = _emailSettings.ToEmail;
                cuenta.EnableSsl = _emailSettings.EnableSsl;
            }
            else cuenta = null;

            return cuenta;

        }
    }


}
