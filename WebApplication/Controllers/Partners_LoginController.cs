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

namespace WebApplication.Controllers
{
     [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class Partners_LoginController : Controller
    {

        private IConfiguration _config;
        private readonly MieleContext _context;

        public Partners_LoginController(IConfiguration config, MieleContext context)
        {
            _config = config;
            _context = context;
        }

        // GET: api/Partners_Login
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Partners_Login/5
        [HttpGet("{id}", Name = "Get_PartnersLogin")]
        public string Get(int id)
        {
            return "value";
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult CargarUsuario([FromBody]LoginModel login)
        {
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

        private UserModel Authenticate(LoginModel login)
        {
            UserModel user = null;

            var item = (from a in _context.Users
                        join b in _context.Cat_Roles on a.id_rol equals b.id
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
                            rol = b.id,
                            a.id_canal, 
                            a.nivel
                        }).ToList();
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
                        email = item[0].email,
                        telefono = item[0].telefono,
                        movil = item[0].movil,
                        rol = item[0].rol.ToString(),
                        id_canal = item[0].id_canal,
                        nivel = item[0].nivel
                    };
                }
                else
                {
                    if (login.Password != item[0].password)
                    {
                        user = new UserModel { name = "", paterno = "none", materno = "none", email = "none", };
                    }
                }

                //Cotizacion1904 Permite logear cuando el usuario tenga mas de un registro
                //user = new UserModel { name = "", paterno = "none", materno = "none", email = "none", };
                //for (int i = 0; i < item.Count; i++)
                //{
                //    if (login.Username == item[i].email && login.Password == item[i].password)
                //    {
                //        user.id = item[i].id;
                //        user.username = item[i].username;
                //        user.name = item[i].name;
                //        user.paterno = item[i].paterno;
                //        user.materno = item[i].materno;
                //        user.email = item[i].email;
                //        user.telefono = item[i].telefono;
                //        user.movil = item[i].movil;
                //        user.rol = item[i].rol.ToString();
                //        user.id_canal = item[i].id_canal;
                //        user.nivel = item[i].nivel;
                //    }
                //}
            }

            return user;

        }


        
        // PUT: api/Partners_Login/5 update
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5 Delete 
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        //////////////////////////  DEFINICIONES ///////////////////


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
            public List<ActividadModel> actividad { get; set; }
            public List<CoberturaModel> area_cobertura { get; set; }
            public string noalmacen { get; set; }
            public List<ProductoModel> productos { get; set; }
            public string tipo_tecnico { get; set; }
            public long id_app { get; set; }
            public string rol { get; set; }
            public int id_canal { get; set; }
            public string nivel { get; set; }
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
    }
}
