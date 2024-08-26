using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Repository;
using AutoMapper;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/ProductLines")]
    public class ProductLinesController : Controller
    {
        private readonly IProductLinesRepository _repo;
        private readonly IMapper _mapper;
        private readonly IProductCategoriesRepository _repoProductCategories;

        public ProductLinesController(
            IProductLinesRepository repo, 
            IMapper mapper, 
            IProductCategoriesRepository repoProductCategories)
        {
            _repo = repo;
            _mapper = mapper;
            _repoProductCategories = repoProductCategories;
        }

        // GET: api/ProductLines
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

        // GET: api/ProductLines/5
        [HttpGet("{id}", Name = "GetProductLine")]
        public IActionResult Get(int id)
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

        [HttpPost]
        public IActionResult Create([FromBody] ProductLinesDto productLinesDto)
        {
            try
            {
            
                //var productLine = _mapper.Map<Cat_Linea_Producto>(productLinesDto);
                var productLine = new Cat_Linea_Producto
                {
                    descripcion = productLinesDto.descripcion,
                    estatus = productLinesDto.estatus
                };
                productLine = _repo.Add(productLine);
                return CreatedAtRoute("GetProductLine", new { productLine.id }, productLine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT: api/ProductLines/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]ProductLinesUpdateDto productLinesUpdateDto)
        {
            try
            {
                if (!_repo.Exists(id))
                    return NotFound($"Product line with Id { id } Not Found");

                var result = _repo.Get(id);

                result.descripcion = productLinesUpdateDto.descripcion;
                result.estatus = productLinesUpdateDto.estatus;
                _repo.Update(result, id);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        //// POST: api/ProductLines
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
