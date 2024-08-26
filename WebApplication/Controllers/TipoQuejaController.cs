using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WebApplication.Models;
using WebApplication.Repository;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/TipoQueja")]
    public class TipoQuejaController : Controller
    {
        private readonly ITipoQuejaRepository _repo;

        public TipoQuejaController(ITipoQuejaRepository repo)
        {
            _repo = repo;
        }

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

        [HttpGet("{id}", Name = "GetTipoQueja")]
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

        //[HttpPost]
        //public IActionResult Create([FromBody] TipoQueja tipoQueja)
        //{
        //    try
        //    {
        //        _repo.Add(tipoQueja);
        //        return CreatedAtRoute("GetTipoQueja", new { id = tipoQueja.Id }, tipoQueja);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}

        //[HttpPut("{id}")]
        //public IActionResult Update(int id, [FromBody] TipoQueja tipoQueja)
        //{
        //    try
        //    {
        //        var entity = _repo.Find(c => c.Id == id);
        //        if (entity == null)
        //        {
        //            return NotFound();
        //        }

        //        entity.Nombre = tipoQueja.Nombre;
        //        _repo.Update(entity, id);
        //        return NoContent();

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}

        //[HttpDelete("{id}")]
        //public IActionResult Delete(int id)
        //{
        //    try
        //    {
        //        var entity = _repo.Find(c => c.Id == id);
        //        if (entity == null)
        //        {
        //            return NotFound();
        //        }

        //        _repo.Delete(entity);
        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}
    }
}