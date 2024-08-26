using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WebApplication.Models;
using WebApplication.Repository;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Canales")]
    public class CanalesController : Controller
    {
        private readonly ICanalesRepository _repo;

        public CanalesController(ICanalesRepository repo)
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

        [HttpGet("{id}", Name = "GetCanal")]
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
        //public IActionResult Create([FromBody] Canales canal)
        //{
        //    try
        //    {
        //        _repo.Add(canal);
        //        return CreatedAtRoute("GetCanal", new { id = canal.Id }, canal);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}

        //[HttpPut("{id}")]
        //public IActionResult Update(int id, [FromBody] Canales canal)
        //{
        //    try
        //    {
        //        var entity = _repo.Find(c => c.Id == id);
        //        if (entity == null)
        //        {
        //            return NotFound();
        //        }

        //        entity.Nombre = canal.Nombre;
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