using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Net;
using System.Text;
using System;
using System.IO;
using WebApplication.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RegistroController : Controller
    {
        private readonly MieleContext _context;
        private IEmailRepository _emailRepository;

        public RegistroController(MieleContext context, IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
            _context = context;
        }

        [HttpGet, Authorize]
        public IEnumerable<Users> GetAll()
        {
            return _context.Users.ToList();
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(long id)
        {
            var item = _context.Users.FirstOrDefault(t => t.id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create([FromBody] Users item)
        {
            var result = new Models.Response();

            if (item == null)
            {
                return BadRequest();
            }
            else
            {
                var path = Path.GetFullPath("TemplateMail/Email.html");
                StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email.html"));
                string body = string.Empty;
                body = reader.ReadToEnd();
                body = body.Replace("{username}", "Bienvenido," + " " + item.name + " " + item.paterno + " " + item.materno);
                body = body.Replace("{pass}", "Ahora te encuentras registrado como técnico en miele.com</br><p>Tu contraseña es: " + item.password + "</p>");

                try
                {
                    var _item = _context.Users.FirstOrDefault(t => t.email == item.email);
                    if (_item == null)
                    {
                        _context.Users.Add(item);
                        _context.SaveChanges();

                        EmailModel email = new EmailModel();
                        email.To = item.email;
                        email.Subject = "Nuevo Técnico";
                        email.Body = body;
                        email.IsBodyHtml = true;
                        email.id_app = 1;
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
                        //    To = { item.email },
                        //    Subject = "Nuevo Técnico",
                        //    IsBodyHtml = true,
                        //    Body = body
                        //});

                        result = new Models.Response
                        {
                            response = "Técnico creado correctamente."
                        };
                    }
                    else
                    {
                        result = new Models.Response
                        {
                            response = "El email ingresado ya existe."
                        };
                    }
                }
                catch(Exception ex)
                {
                    result = new Models.Response
                    {
                        response = ex.ToString()
                    };
                }
            }

            //return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
            return new ObjectResult(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Users item)
        {
            if (item == null || item.id != id)
            {
                return BadRequest();
            }

            var todo = _context.Users.FirstOrDefault(t => t.id == id);
            if (todo == null)
            {
                return NotFound();
            }

            //todo.estatus = item.estatus;
            //todo.name = item.name;

            _context.Users.Update(item);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _context.Users.FirstOrDefault(t => t.id == id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.Users.Remove(todo);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}
