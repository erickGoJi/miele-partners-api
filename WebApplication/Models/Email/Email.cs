using System.Net.Mail;

namespace WebApplication.Models
{
    public class EmailModel
    {
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public bool IsBodyHtml { get; set; }
        public int id_app { get; set; }
    }

    public class EmailSettings
    {
        public string PrimaryDomain { get; set; }
        public string MailName { get; set; }
        public int PrimaryPort { get; set; }
        public string UsernameEmail { get; set; }
        public string UsernamePassword { get; set; }
        public bool DevTest { get; set; }
        public string MailTest { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public bool EnableSsl { get; set; }

    }

    public class EmailSettingsTickets
    {
        public string PrimaryDomain { get; set; }
        public string MailName { get; set; }
        public int PrimaryPort { get; set; }
        public string UsernameEmail { get; set; }
        public string UsernamePassword { get; set; }
        public bool DevTest { get; set; }
        public string MailTest { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public bool EnableSsl { get; set; }

    }
}
