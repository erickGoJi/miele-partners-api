using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Repository
{
    public interface IEmailRepository
    {
        void SendEmail(EmailModel email);
        Task<string> SendMailAsync(EmailModel email);
        Task SendAttachment(string email, string subject, bool isBodyHtml, string message, int id_app, List<Attachment> attachments);
        bool SendMultipleMails(List<MailAddress> mails, string subject, bool isBodyHtml, string message, int id_app, List<Attachment> attachments);
    }
}
