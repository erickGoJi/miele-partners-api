using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApplication.Models;
using WebApplication.Repository;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Propuestas")]
    public class PropuestasController : Controller
    {
        private readonly IPropuestasRepository _repo;
        private readonly IQuejasRepository _repoQuejas;
        private readonly IEmailRepository _repoEmail;
        private readonly IUsersRepository _repoUsers;
        private readonly IMapper _mapper;


        public PropuestasController(
            IPropuestasRepository repo,
            IQuejasRepository repoQuejas,
            IMapper mapper,
            IEmailRepository repoEmail,
            IUsersRepository repoUsers)
        {
            _repo = repo;
            _repoQuejas = repoQuejas;
            _mapper = mapper;
            _repoEmail = repoEmail;
            _repoUsers = repoUsers;
        }

        [HttpGet("{id}", Name = "GetPropuesta")]
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

        [HttpGet("Queja/{quejaId}")]
        public IActionResult GetByQueja(int quejaId)
        {
            try
            {
                var item = _repo.FindAll(c => c.QuejaId == quejaId);

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
                var item = _repo.FindAll(c => c.Queja.Folio == folio);

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

        //[HttpPost]
        //public IActionResult Create(int quejaId, [FromBody] PropuestasDto propuesta)
        //{
        //    try
        //    {
        //        var entity = _repoQuejas.Find(c => c.Id == quejaId);

        //        if (entity == null)
        //        {
        //            return NotFound($"Queja Id { quejaId } Not Found");
        //        }

        //        if (entity.Propuestas != null && entity.Propuestas.Count >= 2)
        //        {
        //            return StatusCode(422, $"Número máximo de propuestas: { 2 }");
        //        }

        //        Propuestas prop = new Propuestas
        //        {
        //            QuejaId = quejaId,
        //            Solucion = propuesta.Solucion
        //        };

        //        prop = _repo.Add(prop);
        //        return CreatedAtRoute("GetPropuesta", new { id = prop.Id }, prop);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}


        [HttpPut("quejaId")]
        public IActionResult Update(int quejaId, [FromBody] List<PropuestasUpdateDto> propuestas)
        {
            try
            {
                if (!_repoQuejas.Exists(quejaId))
                {
                    return NotFound($"Queja Id { quejaId } Not Found");
                }

                if (propuestas == null
                    && propuestas.Count <= 0)
                {
                    return NotFound();
                }

                foreach (var item in propuestas)
                {
                    if (item.Id != 0)
                    {
                        if (_repo.Find(c => c.Id == item.Id && c.QuejaId == quejaId) == null)
                        {
                            return NotFound($"Propuesta Id { item.Id } no pertenece a QuejaId { quejaId }");
                        }
                    }
                    item.QuejaId = quejaId;
                }

                int insertados = _repo.FindBy(c => c.QuejaId == quejaId).Count();
                int propuestasNuevas = propuestas.Where(c => c.Id == 0).Count();

                if (propuestasNuevas + insertados > 2)
                {
                    return StatusCode(422, $"Número máximo de propuestas: { 2 }");
                }

                var result = _repo.AddOrModify(_mapper.Map<List<Propuestas>>(propuestas));
                _repoQuejas.UpdateStatus(quejaId);

                if (result != null
                    && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        if (item.FechaCierre != null
                            && item.FechaCierre.HasValue
                            && !item.Email)
                        {
                            List<string> emails = _repoUsers
                                .FindAll(c => c.id_rol == item.Queja.CanalId)
                                .Select(c => c.email)
                                .ToList();

                            var path = Path.GetFullPath("TemplateMail/Queja_cerrada.html");
                            StreamReader reader = new StreamReader(path);
                            string body = string.Empty;
                            body = reader.ReadToEnd();

                            body = body.Replace("{{Id}}", item.Id.ToString());
                            body = body.Replace("{{QuejaId}}", item.Queja.Id.ToString());
                            body = body.Replace("{{Folio}}", item.Queja.Folio.ToString());
                            body = body.Replace("{{Fecha}}", item.Fecha.ToString("dd-MM-yyyy hh:mm"));
                            body = body.Replace("{{Solucion}}", item.Solucion);
                            body = body.Replace("{{DetalleCierre}}", item.DetalleCierre);
                            body = body.Replace("{{FechaCierre}}", item.FechaCierre.Value.ToString("dd-MM-yyyy hh:mm"));

                            EmailModel email = new EmailModel
                            {
                                To = string.Join(",", emails),
                                Body = body,
                                Subject = "Cierre de Queja Miele",
                                IsBodyHtml = true,
                                id_app = 1
                            };

                            _repoEmail.SendEmail(email);
                            _repo.CorreoEnviado(item.Id);
                        }
                    }
                }

                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // POST: api/Propuestas/Propuesta
        //[Route("Propuesta")]
        //[HttpPost("{Propuesta}")]
        //public IActionResult Propuesta([FromBody] List<PropuestasUpdateDto> propuestas)
        //{
        //    try
        //    {
        //        if (!_repoQuejas.Exists(propuestas[0].QuejaId))
        //        {
        //            return NotFound($"Queja Id { propuestas[0].QuejaId } Not Found");
        //        }

        //        if (propuestas == null && propuestas.Count <= 0)
        //        {
        //            return NotFound();
        //        }

        //        foreach (var item in propuestas)
        //        {
        //            if (item.Id != 0)
        //            {
        //                if (_repo.Find(c => c.Id == item.Id && c.QuejaId == propuestas[0].QuejaId) == null)
        //                {
        //                    return NotFound($"Propuesta Id { item.Id } no pertenece a QuejaId { propuestas[0].QuejaId }");
        //                }
        //            }
        //            item.QuejaId = propuestas[0].QuejaId;
        //        }

        //        int insertados = _repo.FindBy(c => c.QuejaId == propuestas[0].QuejaId).Count();
        //        int propuestasNuevas = propuestas.Where(c => c.Id == 0).Count();

        //        if (propuestasNuevas + insertados > 2)
        //        {
        //            return StatusCode(422, $"Número máximo de propuestas: { 2 }");
        //        }

        //        var result = _repo.AddOrModify(_mapper.Map<List<Propuestas>>(propuestas));
        //        _repoQuejas.UpdateStatus(propuestas[0].QuejaId);

        //        if (result != null
        //            && result.Count > 0)
        //        {
        //            foreach (var item in result)
        //            {
        //                if (item.FechaCierre != null
        //                    && item.FechaCierre.HasValue
        //                    && !item.Email)
        //                {
        //                    List<string> emails = _repoUsers
        //                        .FindAll(c => c.id_rol == item.Queja.CanalId)
        //                        .Select(c => c.email)
        //                        .ToList();

        //                    var path = Path.GetFullPath("TemplateMail/Queja_cerrada.html");
        //                    StreamReader reader = new StreamReader(path);
        //                    string body = string.Empty;
        //                    body = reader.ReadToEnd();

        //                    body = body.Replace("{{Id}}", item.Id.ToString());
        //                    body = body.Replace("{{QuejaId}}", item.Queja.Id.ToString());
        //                    body = body.Replace("{{Folio}}", item.Queja.Folio.ToString());
        //                    body = body.Replace("{{Fecha}}", item.Fecha.ToString("dd-MM-yyyy hh:mm"));
        //                    body = body.Replace("{{Solucion}}", item.Solucion);
        //                    body = body.Replace("{{DetalleCierre}}", item.DetalleCierre);
        //                    body = body.Replace("{{FechaCierre}}", item.FechaCierre.Value.ToString("dd-MM-yyyy hh:mm"));

        //                    Email email = new Email
        //                    {
        //                        To = string.Join(",", emails),
        //                        Body = body,
        //                        Subject = "Cierre de Queja Miele",
        //                        IsBodyHtml = true
        //                    };

        //                    _repoEmail.SendEmail(email);
        //                    _repo.CorreoEnviado(item.Id);
        //                }
        //            }
        //        }

        //        return Ok(result);

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}
    }
}