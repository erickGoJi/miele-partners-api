using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Repository;
using AutoMapper;
using WebApplication.ViewModels;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/ProductSubLines")]
    public class ProductSubLinesController : Controller
    {
        private readonly IProductSubLinesRepository _repo;
        private readonly IProductLinesRepository _repoProductLines;
        private readonly IMapper _mapper;

        public ProductSubLinesController (IProductSubLinesRepository repo, IProductLinesRepository repoProductLines, IMapper mapper)
        {
            _repo = repo;
            _repoProductLines = repoProductLines;
            _mapper = mapper;
        }

        // GET: api/ProductSubLines
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

        // GET: api/ProductSubLines/5
        [HttpGet("{id}", Name = "GetProductSubLines")]
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

        [HttpGet("ProductLine/{productLineId}")]
        public IActionResult GetByProductLineId(int productLineId)
        {
            try
            {
                var item = _repo.FindAll(s => s.id_linea_producto == productLineId);

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

        // POST: api/ProductSubLines
        [HttpPost]
        public IActionResult Create([FromBody] ProductSubLinesDto productSubLinesDto)
        {

            try
            {
                if(!_repoProductLines.Exists(productSubLinesDto.id_linea_producto))
                    return NotFound($"Product line with Id {productSubLinesDto.id_linea_producto} Not Found");

                Cat_SubLinea_Producto productSubLine = _mapper.Map<Cat_SubLinea_Producto>(productSubLinesDto);
                productSubLine = _repo.Add(productSubLine);
                return CreatedAtRoute("GetProductSubLines", new { productSubLine.id }, productSubLine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT: api/ProductSubLines/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]ProductSubLinesUpdateDto productSubLinesUpdateDto)
        {
            try
            {
                if (!_repo.Exists(id))
                    return NotFound($"Product sub line with Id { id } Not Found");

                if (!_repoProductLines.Exists(productSubLinesUpdateDto.id_linea_producto))
                    return NotFound($"Product line with Id { productSubLinesUpdateDto.id_linea_producto } Not Found");

                //productSubLinesUpdateDto.id = id;

                //var result = _repo.Update(_mapper.Map<Cat_SubLinea_Producto>(productSubLinesUpdateDto), id);

                var result = _repo.Get(id);

                result.id_linea_producto = productSubLinesUpdateDto.id_linea_producto;
                result.descripcion = productSubLinesUpdateDto.descripcion;
                result.estatus = productSubLinesUpdateDto.estatus;
                _repo.Update(result, id);

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
