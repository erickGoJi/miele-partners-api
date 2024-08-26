using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("MyPolicy")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class Dashboard_ServiciosController : Controller
    {
        private IConfiguration _config;
        private readonly MieleContext _context;

        public Dashboard_ServiciosController(IConfiguration config, MieleContext context)
        {
            _config = config;
            _context = context;
        }

        // GET api/Dashboard_Servicios/values
        [HttpGet("values")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}