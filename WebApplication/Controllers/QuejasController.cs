using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using WebApplication.Models;
using WebApplication.Repository;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Quejas")]
    public class QuejasController : Controller
    {
        private readonly IQuejasRepository _repo;
        private readonly ICanalesRepository _repoCanales;
        private readonly IClienteRepository _repoCliente;
        private readonly IProductoRepository _repoProducto;
        private readonly ITipoQuejaRepository _repoTipoQueja;
        private readonly IUsersRepository _repoUsers;
        private readonly IMapper _mapper;
        private readonly IEmailRepository _repoEmail;
        private readonly IPropuestasRepository _repoPropuestas;

        public QuejasController(
            IMapper mapper,
            IQuejasRepository repo,
            IUsersRepository repoUsers,
            IEmailRepository repoEmail,
            IClienteRepository repoCliente,
            ICanalesRepository repoCanales,
            IProductoRepository repoProducto,
            ITipoQuejaRepository repoTipoQueja,
            IPropuestasRepository repoPropuestas
            )
        {
            _repo = repo;
            _mapper = mapper;
            _repoUsers = repoUsers;
            _repoEmail = repoEmail;
            _repoCanales = repoCanales;
            _repoCliente = repoCliente;
            _repoProducto = repoProducto;
            _repoTipoQueja = repoTipoQueja;
            _repoPropuestas = repoPropuestas;
        }

        [HttpGet("{id}", Name = "GetQueja")]
        public IActionResult GetById(int id)
        {
            try
            {
                var item = _repo.Get(id);

                if (item == null)
                {
                    return NotFound();
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("Folio/{folio}")]
        public IActionResult GetByFolio(string folio)
        {
            try
            {
                var item = _repo.Find(c => c.Folio == folio);

                if (item == null)
                {
                    return NotFound();
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] QuejasDto quejaDto)
        {
            try
            {
                if (!_repoCanales.Exists(quejaDto.CanalId))
                {
                    return NotFound($"Canal Id { quejaDto.CanalId } Not Found");
                }

                if (!_repoTipoQueja.Exists(quejaDto.TipoQuejaId))
                {
                    return NotFound($"Tipo de Queja Id { quejaDto.CanalId } Not Found");
                }

                if (!_repoCliente.Exists(quejaDto.ClienteId))
                {
                    return NotFound($"Cliente Id { quejaDto.ClienteId } Not Found");
                }

                if (quejaDto.ProductosQuejas != null
                    && quejaDto.ProductosQuejas.Count > 0)
                {
                    foreach (var item in quejaDto.ProductosQuejas)
                    {
                        if (!_repoProducto.Exists(item.ProductoId))
                        {
                            return NotFound($"Product Id { item.ProductoId } Not Found");
                        }
                    }
                }

                if (quejaDto.Propuestas != null && quejaDto.Propuestas.Count > 2)
                {
                    return StatusCode(422, $"Número máximo de propuestas excedido");
                }

                var queja = _mapper.Map<Quejas>(quejaDto);
                queja = _repo.Add(queja);
                _repo.UpdateStatus(queja.Id);

                List<string> emails = _repoUsers
                    .FindAll(c => c.id_rol == queja.CanalId)
                    .Select(c => c.email)
                    .ToList();

                if (emails != null && emails.Count > 0)
                {
                    queja.Canal = _repoCanales.Get(queja.CanalId);
                    queja.Cliente = _repoCliente.Get(queja.ClienteId);

                    if (queja.ProductosQuejas != null
                        && queja.ProductosQuejas.Count > 0)
                    {
                        foreach (var item in queja.ProductosQuejas)
                        {
                            var producto = _repoProducto.Get(item.ProductoId);
                            item.Producto = producto;
                        }
                    }

                    var path = Path.GetFullPath("TemplateMail/Queja_abierta.html");
                    StreamReader reader = new StreamReader(path);
                    string body = string.Empty;
                    body = reader.ReadToEnd();
                    body = body.Replace("{{Id}}", queja.Id.ToString());
                    body = body.Replace("{{Atendio}}", queja.Atendio);
                    body = body.Replace("{{CanalId}}", queja.Canal.Id.ToString());
                    body = body.Replace("{{NombreCanal}}", queja.Canal.Nombre);
                    body = body.Replace("{{ClienteId}}", queja.Cliente.id.ToString());
                    body = body.Replace("{{NombreCliente}}", queja.Cliente.nombre + " "
                        + queja.Cliente.paterno + " " + queja.Cliente.materno);
                    body = body.Replace("{{DetalleReclamo}}", queja.DetalleReclamo);
                    body = body.Replace("{{Fecha}}", queja.Fecha.ToString("dd-MM-yyyy hh:mm"));
                    body = body.Replace("{{Folio}}", queja.Folio);
                    body = body.Replace("{{TipoQuejaId}}", queja.TipoQueja.Id.ToString());
                    body = body.Replace("{{NombreTipo}}", queja.TipoQueja.Nombre.ToString());
                    body = body.Replace("{{Telefono}}", queja.Telefono);

                    if (queja.ProductosQuejas != null
                        && queja.ProductosQuejas.Count > 0)
                    {
                        List<string> productosNombres = new List<string>();

                        foreach (var item in queja.ProductosQuejas)
                            productosNombres.Add(item.Producto.nombre);

                        body = body.Replace("{{NombreProductos}}", string.Join(",", productosNombres));
                    }
                    else
                        body = body.Replace("{{NombreProductos}}", "No productos relacionados");


                    if (queja.Propuestas != null
                        && queja.Propuestas.Count > 0)
                    {
                        List<string> soluciones = new List<string>();

                        foreach (var item in quejaDto.Propuestas)
                            soluciones.Add(item.Solucion);

                        body = body.Replace("{{Soluciones}}", string.Join(",", soluciones));
                    }
                    body = body.Replace("{{Soluciones}}", string.Empty);

                    EmailModel email = new EmailModel
                    {
                        To = string.Join(",", emails),
                        Body = body,
                        Subject = "Queja Miele",
                        IsBodyHtml = true,
                        id_app = 1
                    };

                    _repoEmail.SendEmail(email);

                    if (queja.Propuestas != null 
                        && queja.Propuestas.Count > 0)
                    {
                        foreach (var item in queja.Propuestas)
                        {
                            if (item.FechaCierre != null
                                && item.FechaCierre.HasValue
                                && !item.Email)
                            {
                                path = Path.GetFullPath("TemplateMail/Queja_cerrada.html");
                                reader = new StreamReader(path);
                                body = string.Empty;
                                body = reader.ReadToEnd();

                                body = body.Replace("{{Id}}", item.Id.ToString());
                                body = body.Replace("{{QuejaId}}", item.Queja.Id.ToString());
                                body = body.Replace("{{Folio}}", item.Queja.Folio.ToString());
                                body = body.Replace("{{Fecha}}", item.Fecha.ToString("dd-MM-yyyy hh:mm"));
                                body = body.Replace("{{Solucion}}", item.Solucion);
                                body = body.Replace("{{DetalleCierre}}", item.DetalleCierre);
                                body = body.Replace("{{FechaCierre}}", item.FechaCierre.Value.ToString("dd-MM-yyyy hh:mm"));

                                email.Body = body;
                                email.Subject = "Cierre de Queja Miele";

                                _repoEmail.SendEmail(email);
                                _repoPropuestas.CorreoEnviado(item.Id);
                            }
                        }
                    }
                }

                return CreatedAtRoute("GetQueja", new { id = queja.Id, folio = queja.Folio }, queja);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}