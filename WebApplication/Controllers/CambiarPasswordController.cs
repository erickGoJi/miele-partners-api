using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Repository;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/CambiarPassword")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CambiarPasswordController : Controller
    {
        private IEmailRepository _emailRepository;
        private readonly MieleContext _context;

        public CambiarPasswordController(MieleContext context, IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
            _context = context;
        }


        // GET: api/CambiarPassword
        [HttpGet]
        public IEnumerable<Users> Get()
        {
            return _context.Users.ToList();
        }

        // GET: api/CambiarPassword/email
        [HttpGet("{email}", Name = "Get")]
        public IActionResult Get(string email)
        {
            var item = _context.Users.FirstOrDefault(t => t.email == email);
            var result = new Models.Response();

            if (item == null)
            {
                result = new Models.Response
                {
                    response = "Usuario invalido, favor de verificar"
                };
            }
            else
            {
                var guid = Guid.NewGuid().ToString().Substring(0, 4);
                var path = Path.GetFullPath("TemplateMail/Email.html");
                StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email.html"));
                string body = string.Empty;
                body = reader.ReadToEnd();
                body = body.Replace("{username}", "Hola," + " " + item.name + " " + item.paterno + " " + item.materno);
                body = body.Replace("{pass}", "Tu nueva contraseña es:" + " " + "$miele" + guid);
                try
                {

                    EmailModel newmail = new EmailModel();
                    newmail.To = email;
                    newmail.Subject = "Recuperar contraseña";
                    newmail.Body = body;
                    newmail.IsBodyHtml = true;
                    newmail.id_app = Convert.ToInt32(item.id_app);
                    _emailRepository.SendMailAsync(newmail);

                    //new SmtpClient
                    //{
                    //    Host = "Smtp.Gmail.com",
                    //    Port = 587,
                    //    EnableSsl = true,
                    //    Timeout = 10000,
                    //    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //    UseDefaultCredentials = false,
                    //    Credentials = new NetworkCredential("conf modificada", "bostones01")
                    //}.Send(new MailMessage
                    //{
                    //    From = new MailAddress("no-reply@techo.org", "Miele"),
                    //    To = { email },
                    //    Subject = "Recuperar contraseña",
                    //    IsBodyHtml = true,
                    //    Body = body
                    //    //Body = "Nueva contraseña: " + "$miele" + guid, BodyEncoding = Encoding.UTF8 
                    //});

                    item.password = "$miele" + guid;
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "La contraseña se cambio correctamente"
                    };
                }
                catch (Exception ex)
                {
                    result = new Models.Response
                    {
                        response = ex.ToString()
                    };

                }
            }
            return new ObjectResult(result);
        }

        // POST: api/CambiarPassword
        [HttpPost]
        public void Post([FromBody]int id, string pass)
        {

        }

        // PUT: api/CambiarPassword/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] password item)
        {
            var todo = _context.Users.FirstOrDefault(t => t.id == id);
            var result = new Models.Response();
            var guid = Guid.NewGuid().ToString().Substring(0, 4);
            var path = Path.GetFullPath("TemplateMail/Email.html");
            StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email.html"));
            string body = string.Empty;
            body = reader.ReadToEnd();
            body = body.Replace("{username}", "Hola," + " " + todo.name + " " + todo.paterno + " " + todo.materno);
            body = body.Replace("{pass}", "Tu contraseña ha sido modificada correctamente.");

            if (todo == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    if (todo.password == item.OldPassword)
                    {
                        todo.password = item.NewPassword;
                        _context.SaveChanges();

                        EmailModel email = new EmailModel();
                        email.To = todo.email;
                        email.Subject = "Cambio de contraseña";
                        email.Body = body;
                        email.IsBodyHtml = true;
                        email.id_app = Convert.ToInt32(todo.id_app);
                        _emailRepository.SendMailAsync(email);

                        //new SmtpClient
                        //{
                        //    Host = "Smtp.Gmail.com",
                        //    Port = 587,
                        //    EnableSsl = true,
                        //    Timeout = 10000,
                        //    DeliveryMethod = SmtpDeliveryMethod.Network,
                        //    UseDefaultCredentials = false,
                        //    Credentials = new NetworkCredential("conf modificada", "bostones01")
                        //}.Send(new MailMessage
                        //{
                        //    From = new MailAddress("no-reply@techo.org", "Miele"),
                        //    To = { todo.email },
                        //    Subject = "Cambio de contraseña",
                        //    IsBodyHtml = true,
                        //    Body = body
                        //    //Body = "Nueva contraseña: " + "$miele" + guid, BodyEncoding = Encoding.UTF8 
                        //});

                        result = new Models.Response
                        {
                            response = "La contraseña se cambio correctamente"
                        };
                    }
                    else
                    {
                        result = new Models.Response
                        {
                            response = "Contraseña incorrecta"
                        };
                    }

                }
                catch (Exception ex)
                {
                    result = new Models.Response
                    {
                        response = ex.ToString()
                    };

                }
            }
            return new ObjectResult(result);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public class password
        {
            public string NewPassword { get; set; }
            public string OldPassword { get; set; }
        }
    }
}
