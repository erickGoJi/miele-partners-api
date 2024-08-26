using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using WebApplication.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TokenController : Controller
    {
        private IConfiguration _config;
        private readonly MieleContext _context;

        public TokenController(IConfiguration config, MieleContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpGet, Authorize]
        public IEnumerable<Token_Activo> GetAll()
        {
            return _context.TokenItems.ToList();
        }

        [HttpGet("{id}", Name = "GetToken")]
        public IActionResult GetById(long id)
        {
            var item = _context.TokenItems.FirstOrDefault(t => t.Id == id);
            if (item == null)

            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpGet("GetUserToken/{id}", Name = "GetUserToken")]
        public IActionResult GetByUserId(long id)
        {
            IActionResult response = Unauthorized();
            var item = _context.TokenItems.FirstOrDefault(t => t.Id_user == id);
            if (item == null)
            {
                return response = Ok(new { result = "Error", detalle = "No hay sesiones abiertas"});
            }
            else
            {
                return response = Ok(new { result = "Success", detalle = "Sesion Valida", token = item.Token });
            }
            
        }

        [Route("token")]
        [HttpPost("{token}")]
        public IActionResult Create([FromBody]Token_Activo item)
        {
            IActionResult response;

            var item_token = _context.TokenItems.FirstOrDefault(t => t.Token == item.Token);
            if (item_token == null)
            {
                response = Ok(new { token = "" });
            }
            else
            {
                response = Ok(new { token = item.Token });
            }

            return new ObjectResult(response);
        }

        [AllowAnonymous]
        [Route("Salir")]
        [HttpPost("{Salir}")]
        public IActionResult DeleteToken([FromBody] tokenbyid item)
        {
            var todo = _context.TokenItems.FirstOrDefault(t => t.Id_user == item.id && t.Token == item.token);
            var result = new Models.Response();
            if (todo == null)
            {
                return NotFound();
            }

            try
            {
                _context.TokenItems.Remove(todo);
                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "Sesion cerrada correctamente"
                };
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody]LoginModel login)
        {
            //Se implementa esta validacion porque no estoy seguro si este mismo endpoint se consume desde la app
            if (login.id_app == 0)
            {
                login.id_app = 1;
            }
            IActionResult response = Unauthorized();
            var user = Authenticate(login);

            if (user.name != "none" && user.email != "none")
            {
                var tokenString = BuildToken(user);
                Token_Activo item = new Token_Activo
                {
                    Token = tokenString,
                    Id_user = user.id,
                    Fecha = DateTime.Now.ToString()
                };

                _context.TokenItems.Add(item);
                _context.SaveChanges();

                response = Ok(new { token = tokenString, user });

            }
            else
            {
                if (user.name == "none")
                {
                    response = Ok(new { token = "usuarios no existe" });
                }

                if (user.email == "none")
                {
                    response = Ok(new { token = "password incorrecto" });
                }
            }

            return response;
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _context.TokenItems.Count(t => t.Id_user == id);
            var result = new Models.Response();
            if (todo == 0)
            {
                return NotFound();
            }

            try
            {
                for (var i = 0; i < todo; i++)
                {
                    var query = _context.TokenItems.FirstOrDefault(t => t.Id_user == id);
                    _context.TokenItems.Remove(query);
                    _context.SaveChanges();
                }
                result = new Models.Response
                {
                    response = "Sesiones cerradas correctamente"
                };
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        private string BuildToken(UserModel user)
        {

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.name),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
               };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel Authenticate(LoginModel login)
        {
            UserModel user = null;

            var item = (from a in _context.Users
                        join b in _context.Tecnicos on a.id equals b.id into es
                        from fd in es.DefaultIfEmpty()
                        join c in _context.Cat_Tecnicos_Tipo on fd.id_tipo_tecnico equals c.id into cat_tec
                        from tipo in cat_tec.DefaultIfEmpty()
                        where a.email == login.Username && a.id_app == login.id_app && a.estatus == true
                        select new
                        {
                            id = a.id,
                            username = a.username,
                            name = a.name,
                            paterno = a.paterno,
                            materno = a.materno,
                            password = a.password,
                            email = a.email,
                            telefono = a.telefono,
                            movil = a.telefono_movil,
                            noalmacen = fd.noalmacen,
                            tipo = tipo.desc_tipo,
                            rol = a.Rol.rol,
                            id_rol = a.Rol.id,
                            id_app = a.id_app,
                            nivel = a.nivel
                        }).ToList();

            //var item = _context.Users.FirstOrDefault(t => t.email == login.Username);esc_actividad

            if (item.Count == 0)
            {
                user = new UserModel { id = 0, name = "none", paterno = "none", materno = "none", email = "" };
            }
            else
            {
                if (login.Username == item[0].email && login.Password == item[0].password)
                {
                    user = new UserModel
                    {
                        id = item[0].id,
                        username = item[0].username,
                        name = item[0].name,
                        paterno = item[0].paterno,
                        materno = item[0].materno,
                        //password = item[0].password,
                        email = item[0].email,
                        telefono = item[0].telefono,
                        movil = item[0].movil,
                        rol = item[0].rol,
                        id_rol = item[0].id_rol,
                        nivel = item[0].nivel,
                        actividad = (from x in _context.Tecnicos_Actividad
                                     join j in _context.Cat_Actividad on x.id_actividad equals j.id
                                     where x.id_user == item[0].id
                                     select j)
                                         .Select(c => new ActividadModel
                                         {
                                             id = c.id,
                                             actividad = c.desc_actividad
                                         }).ToList(),
                        area_cobertura = (from x in _context.Tecnicos_Cobertura
                                          join j in _context.Cat_Area_Cobertura on x.id_cobertura equals j.id
                                          where x.id_user == item[0].id
                                          select j)
                                         .Select(c => new CoberturaModel
                                         {
                                             id = c.id,
                                             cobertura = c.desc_cobertura
                                         }).ToList(),
                        productos = (from x in _context.Tecnicos_Producto
                                     join j in _context.Cat_Categoria_Producto on x.id_categoria_producto equals j.id
                                     where x.id_user == item[0].id
                                     select j)
                                         .Select(c => new ProductoModel
                                         {
                                             id = c.id,
                                             producto = c.descripcion
                                         }).ToList(),
                        tipo_tecnico = item[0].tipo,
                        noalmacen = item[0].noalmacen
                    };
                }
                else
                {
                    if (login.Password != item[0].password)
                    {
                        user = new UserModel { name = "", paterno = "none", materno = "none", email = "none", };
                    }
                }
            }

            return user;

        }

        private Tecnicos GetTecnicos(Users login)
        {
            Tecnicos user = null;
            var item = _context.Tecnicos.FirstOrDefault(t => t.id == login.id);

            user = item;

            return user;

        }

        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public long id_app { get; set; }
        }

        public class ActividadModel
        {
            public long id { get; set; }
            public string actividad { get; set; }
        }

        public class CoberturaModel
        {
            public long id { get; set; }
            public string cobertura { get; set; }
        }

        public class ProductoModel
        {
            public long id { get; set; }
            public string producto { get; set; }
        }

        private class UserModel
        {
            public long id { get; set; }
            public string username { get; set; }
            public string name { get; set; }
            public string paterno { get; set; }
            public string materno { get; set; }
            public string password { get; set; }
            public string email { get; set; }
            public string telefono { get; set; }
            public string movil { get; set; }
            public string rol { get; set; }
            public long id_rol { get; set; }
            public List<ActividadModel> actividad { get; set; }
            public List<CoberturaModel> area_cobertura { get; set; }
            public string noalmacen { get; set; }
            public List<ProductoModel> productos { get; set; }
            public string tipo_tecnico { get; set; }
            public string nivel { get; set; }
        }

        public class tokenbyid
        {
            public long id { get; set; }
            public string token { get; set; }
        }
    }
}
