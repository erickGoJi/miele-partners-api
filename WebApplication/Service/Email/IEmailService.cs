using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Service
{
    public interface IEmailService
    {
        Task<bool> SendMailAsync(EmailModel email);
    }
}
