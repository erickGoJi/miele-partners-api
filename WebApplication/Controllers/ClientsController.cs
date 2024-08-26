using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Repository;
using WebApplication.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Clients")]
    public class ClientsController : Controller
    {
        private readonly IClientsRepository _repo;
        private readonly IMapper _mapper;
        private readonly IBranchOfficesRepository _repoBranchOffices;
        private readonly MieleContext _context;

        public ClientsController(
            IClientsRepository repo, 
            IMapper mapper, 
            IBranchOfficesRepository repoBranchOffices, 
            MieleContext context)
        {
            _repo = repo;
            _mapper = mapper;
            _repoBranchOffices = repoBranchOffices;
            _context = context;
        }

        // GET: api/Clients
        [HttpGet]
        public IActionResult GetAll()
        {

            try
            {
                return Ok(_repo.GetAll().ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }


        public class busqueda_entidades
        {
            public int IDUSR { get; set; }
            public int id { get; set; }
            public string TextoLibre { get; set; }
            public string Canal { get; set; }
            public string Cuenta { get; set; }
            public int tipo_entidad { get; set; }
            public string FecIni { get; set; }
            public string FecFin { get; set; }
        }

        [Route("get_allclients")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_allclients([FromBody]busqueda_entidades busqueda)
        {
            var U_session = (from a in _context.Users
                             where a.id == busqueda.IDUSR
                             select new { id_canal = a.id_canal, id_cuenta = a.id_cuenta, Id_sucursal = a.id_Sucursales, id_rol = a.id_rol, nivel = a.nivel, id = a.id }).ToList();
            IActionResult response = Unauthorized();
            long id_nivel = 0;
            long _cuenta = 0;
            long _canal = 0;
            /*  && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                 && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                 && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)*/

            switch (U_session[0].nivel) { case "sucursal": id_nivel = 1; break; case "cuenta": id_nivel = 2; break; case "canal": id_nivel = 3; break; case "global": id_nivel = -1; break; }

            if (busqueda.id == 0 && _cuenta == 0 && _canal == 0)
            {
                var clients_id = (from c in _context.Clientes
                                   join   a in _context.Cat_Sucursales on c.Id_sucursal equals a.Id
                                  join b in _context.Cat_Cuentas on a.Id_Cuenta equals b.Id
                                  join d in _context.Cat_canales on b.Id_Canal equals d.Id
                                  where
                                  c.Id_sucursal == (id_nivel == 1 ? U_session[0].Id_sucursal : c.Id_sucursal)
                               
                                  select new
                                  {
                                      c.id,
                                      c.nombre,
                                      c.paterno,
                                      c.materno,
                                      c.Id_sucursal,
                                      c.telefono,
                                      c.telefono_movil,
                                      c.email,
                                      sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == a.Id).Sucursal,
                                      cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == b.Id).Cuenta_es,
                                      id_cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == b.Id).Id,
                                      canal = _context.Cat_canales.FirstOrDefault(x => x.Id == d.Id).Canal_es,
                                      id_canal = _context.Cat_canales.FirstOrDefault(x => x.Id == d.Id).Id,
                                   });
                //var clients_ = (from c in _context.Clientes
                //                where
                //                c.Id_sucursal == (id_nivel == 1 ? U_session[0].Id_sucursal : c.Id_sucursal)
                //                && c.Id_sucursal == 0

                //                  select new
                //                  {
                //                      c.id,
                //                      c.nombre,
                //                      c.paterno,
                //                      c.materno,
                //                      c.Id_sucursal,
                //                      c.telefono,
                //                      c.telefono_movil,
                //                      c.email,
                //                      sucursal = "",
                //                      cuenta = "",
                //                       id_cuenta =(long)0,
                //                      canal = "",
                //                      id_canal = (long) 0,

                                      
                //                  });

               // clients_id = clients_id.Union(clients_);
                if (clients_id == null)
                {
                    return response = Ok(new { result = "Error", item = clients_id });
                }
                return response = Ok(new { result = "Success", item = clients_id });
            }
            else
            {
                var clients_id = (from c in _context.Clientes
                                  join a in _context.Cat_Sucursales on c.Id_sucursal equals a.Id
                                  join b in _context.Cat_Cuentas on a.Id_Cuenta equals b.Id
                                  join d in _context.Cat_canales on b.Id_Canal equals d.Id
                                  where
                                  c.Id_sucursal == (id_nivel == 1 ? U_session[0].Id_sucursal : c.Id_sucursal)
                                 && b.Id_Canal == (id_nivel == 3 ? U_session[0].id_canal : b.Id_Canal)
                                 && a.Id_Cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : a.Id_Cuenta)
                                  select new
                                  {
                                      c.id,
                                      c.nombre,
                                      c.paterno,
                                      c.materno,
                                      c.Id_sucursal,
                                      c.telefono,
                                      c.telefono_movil,
                                      c.email,

                                      sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == a.Id).Sucursal,
                                      cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == b.Id).Cuenta_es,
                                      id_cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == b.Id).Id,
                                      canal = _context.Cat_canales.FirstOrDefault(x => x.Id == d.Id).Canal_es,
                                      id_canal = _context.Cat_canales.FirstOrDefault(x => x.Id == d.Id).Id,




                                  }).ToList();

                if (clients_id == null)
                {
                    return response = Ok(new { result = "Error", item = clients_id });
                }
                return response = Ok(new { result = "Success", item = clients_id });
            }
            // return new ObjectResult(promocion); 
            /*
              join u in _context.Cat_Formas_Pago on c.Id equals u.id
                             join a in _context.Cat_CondicionesPago on c.Id equals a.id
                             where c.Id == busqueda.Id &&
                             a.id_cuenta== busqueda.Id
             */


        }


        // GET: api/Clients/5
        [HttpGet("{id}", Name = "GetClient")]
        public IActionResult Get(long id)
        {
            try
            {
                var client = _repo.Get(id);

                var sucursal = client.Id_sucursal == 0 ? "No asignada" 
                    : _context.Cat_Sucursales.Where(bo => bo.Id == client.Id_sucursal).FirstOrDefault().Sucursal;
                
                var item = new { client.actualizado, client.actualizadopor, client.creado, client.creadopor, client.datos_fiscales,
                    client.direccion, client.email, client.estatus, client.folio, client.id, client.Id_Cliente_Cotizacion, client.Id_Cliente_Productos,
                    client.Id_sucursal, client.materno, client.nombre, client.nombre_comercial, client.nombre_contacto, client.paterno,
                    client.referencias, client.referidopor, client.servicio, client.telefono, client.telefono_movil, client.tipo_persona,
                    client.vigencia_ref, sucursal };

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

        // POST: api/Clients
        [HttpPost]
        public IActionResult Create([FromBody]ClientsDto clientsDto)
        {
            try
            {
                //if (!_repoProductCategories.Exists(productsDto.id_categoria))
                //    return NotFound($"Product category with Id { productsDto.id_categoria } Not Found");
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Clientes client = _mapper.Map<Clientes>(clientsDto);
                client = _repo.Add(client);
                return CreatedAtRoute("GetClient", new { client.id }, client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost("Search")]
        public IActionResult Search([FromBody]ClientsSearchDto clientsSearchDto)
        {
            try
            {
                var item = _repo.FindAllSearch(clientsSearchDto);
                //var item = (string.IsNullOrEmpty(clientsSearchDto.text) || string.IsNullOrWhiteSpace(clientsSearchDto.text)) 
                //    ? _repo.FindAll(c => c.Id_sucursal == (clientsSearchDto.Id_sucursal == 0 ? c.Id_sucursal : clientsSearchDto.Id_sucursal)
                //&& branchOffice.Contains(c.Id_sucursal))
                //    : _repo.FindAll(c => (c.nombre.Contains(clientsSearchDto.text) || c.paterno.Contains(clientsSearchDto.text)
                //|| c.materno.Contains(clientsSearchDto.text) || c.email.Contains(clientsSearchDto.text))
                //&& (c.Id_sucursal == (clientsSearchDto.Id_sucursal == 0 ? c.Id_sucursal : clientsSearchDto.Id_sucursal)
                //&& branchOffice.Contains(c.Id_sucursal)));

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

        // PUT: api/Clients/5
        [HttpPut("{id}")]
        public IActionResult Put(long id, [FromBody]ClientsUpdateDto clientsUpdateDto)
        {
            try
            {
                if (!_repo.Exists(id))
                    return NotFound($"Client with Id { id } Not Found");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Clientes client = _mapper.Map<Clientes>(clientsUpdateDto);
                client.id = id;
                client.actualizado = DateTime.Now;
                //if (client.referidopor != 0)
                //    client.vigencia_ref = DateTime.Now.AddMonths(6);
                var result = _repo.Update(client, id);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // GET: api/Clients/5
        [HttpGet("{id}/Estimates", Name = "GetEstimatesByClient")]
        public IActionResult GetEstimatesByClient(long id)
        {
            try
            {
                var item = _context.Cotizaciones.Where(e => e.Id_Cliente == id)
                .Join(_context.Cat_Cuentas, e => e.Id_Cuenta, a => a.Id,
                (e, a) => new
                {
                    e.Id,
                    e.Numero,
                    a.Cuenta_es,
                    e.fecha_cotiza,
                    e.Id_Vendedor,
                    e.importe_precio_lista,
                    e.iva_precio_lista,
                    e.numero_productos,
                    e.Estatus,
                    e.Id_sucursal,
                    e.cambio_ord_comp_generada
                }).Join(_context.Users, ea => ea.Id_Vendedor, u => u.id, (ea, u) =>
                new
                {
                    ea,
                    u.name,
                    u.paterno,
                    u.materno

                }).Join(_context.Cat_Estatus_Cotizacion, eau => eau.ea.Estatus, ec => ec.id, (eau, ec)
                => new
                {
                    eau,
                    ec.Estatus_es

                }).Join(_context.Cat_Sucursales, eauc => eauc.eau.ea.Id_sucursal, bo => bo.Id, (eauc, bo)
                => new
                {
                    id = eauc.eau.ea.Id,
                    folio = eauc.eau.ea.Numero,
                    cuenta = eauc.eau.ea.Cuenta_es,
                    sucursal = bo.Sucursal,
                    fecha = eauc.eau.ea.fecha_cotiza,
                    vendedor = eauc.eau.name + " " + eauc.eau.paterno + " " + eauc.eau.materno,
                    importe = (eauc.eau.ea.importe_precio_lista + eauc.eau.ea.iva_precio_lista).ToString("C"),
                    nprod = eauc.eau.ea.numero_productos,
                    estatus_desc = eauc.Estatus_es,
                    cambio_ord_comp_generada = (eauc.eau.ea.cambio_ord_comp_generada.Year < 2000 ? "" : eauc.eau.ea.cambio_ord_comp_generada.ToString("dd/MM/yyyy")),

                }).ToList();

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

        [HttpGet("UpdateRefExpiration/{id}", Name = "UpdateReferenceExpiration")]
        public IActionResult UpdateRefExpiration(long id)
        {
            try
            {
                if (!_repo.Exists(id))
                    return NotFound($"Client with Id { id } Not Found");

                var client = _repo.Get(id);
                client.vigencia_ref = DateTime.Now.AddMonths(6);
                var result = _repo.Update(client, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
