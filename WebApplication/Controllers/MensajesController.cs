using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication.Models;
using WebApplication.Repository;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Mensajes")]
    public class MensajesController : Controller
    {
        // Variables
        private IConverter _converter;
        private IConfiguration _config;
        private IEmailRepository _emailRepository;
        private readonly MieleContext _context; // DbContext

        public MensajesController(IConverter converter, IConfiguration configuration, IEmailRepository emailRepository, MieleContext context)
        {
            _converter = converter;
            _config = configuration;
            _emailRepository = emailRepository;
            _context = context;
        }

        [HttpGet("Motivos")] // Método en EndPoint GetMotivos
        public IActionResult GetMotivos()
        {
            IActionResult response = Unauthorized(); // Respuesta
            try
            {
                var motivos = _context.cat_Motivos.Where(m => m.estatus == true); // Muestra los motivos vigentes
                return response = Ok(new { result = "Success", item = motivos });
            }
            catch (Exception ex)
            {

                return response = Ok(new { result = "Error", detalle = ex.Message });
            }
        }

        [HttpPost] // Método en EndPoint PostMensajes
        public IActionResult AddMensajes([FromBody]Mensaje mensaje)
        {
            IActionResult response = Unauthorized();
            try
            {
                mensaje.fecha_msj = DateTime.Now;
                //mensaje.usuario_id = userId;
                _context.mensajes.Add(mensaje);
                _context.SaveChanges();

                // validación de usuario
                var usuario = _context.Users.FirstOrDefault(u => u.id == mensaje.usuario_id);
                if (usuario != null)
                {
                    StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                    string body = string.Empty;
                    body = reader.ReadToEnd();
                    body = body.Replace("{Titulo}", "Correo interno"); // revisar titutlo correcto
                    body = body.Replace("{Cuenta}", " ");
                    body = body.Replace("{Sucursal}", " ");
                    body = body.Replace("{Username}", " El usuario " + usuario.name + " " + usuario.paterno + " envía el siguiente mensaje:");
                    body = body.Replace("{Texto}", mensaje.detalle_msj + ". Con número de orden relacionado ");
                    body = body.Replace("{NumeroCotizacion}", mensaje.orden_id.ToString());

                    // validación del correo
                    var motivo = _context.cat_Motivos.FirstOrDefault(m => m.id == mensaje.motivo_id);
                    if (motivo != null)
                    {
                        SendMail(motivo.correo, body, "información de la orden # " + mensaje.orden_id.ToString()); // subject del correo
                        return response = Ok(new { result = "Success", item = mensaje });
                    }
                    
                }

                return response = Ok(new { result = "Error", item = "El usuario no existe o el motivo no es válido" });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message });
            } 
        }
        [Route("msjPrueba")]
        [HttpPost]
        public IActionResult MensajePrueba()
        {
            IActionResult response = Unauthorized();
            try
            {

                // validación de usuario
                //var usuario = _context.Users.FirstOrDefault(u => u.id == mensaje.usuario_id);
                //if (usuario != null)
                //{
                StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                string body = string.Empty;
                body = reader.ReadToEnd();
                body = body.Replace("{Titulo}", "Correo interno"); // revisar titutlo correcto
                body = body.Replace("{Cuenta}", " ");
                body = body.Replace("{Sucursal}", " ");
                body = body.Replace("{Username}", " El usuario prueba envía el siguiente mensaje:");
                body = body.Replace("{Texto}", "prueba mensajeCon número de orden relacionado ");
                body = body.Replace("{NumeroCotizacion}", "12345");
                var filePath = Environment.CurrentDirectory;
                var path = "/Imagenes/PDF_estimates/adjunto.pdf";
                //using (var fs = new FileStream(filePath + path, FileMode.Open))
                //{
                //    List<Attachment> adjuntos = new List<Attachment>();
                //    adjuntos.Add(new Attachment(fs, "Reporte de Actividades.pdf", "text/pdf"));
                //    //_emailRepository.SendAttachment("luishs@minimalist.mx", "información de la orden # 12345", true, body, 1, adjuntos); // subject del correo
                //}


                EmailModel emailModel = new EmailModel();
                emailModel.Body = body;
                emailModel.id_app = 1;
                emailModel.IsBodyHtml = true;
                emailModel.Subject = "Prueba";
                emailModel.To = "luishs@minimalist.mx";
                _emailRepository.SendEmail(emailModel);

                return response = Ok(new { result = "Success" });

            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message });
            }
        }

        private void SendMail(string emailTo, string body, string subject)
        {
            EmailModel email = new EmailModel();
            email.To = emailTo;
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = true;
            email.id_app = 2;
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
            //    To = { emailTo },
            //    Subject = subject,
            //    IsBodyHtml = true,
            //    Body = body
            //});
        }

    }
}