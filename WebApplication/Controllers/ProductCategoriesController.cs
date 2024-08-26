using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Repository;
using AutoMapper;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/ProductCategories")]
    public class ProductCategoriesController : Controller
    {
        private readonly IProductCategoriesRepository _repo;

        public ProductCategoriesController(IProductCategoriesRepository repo)
        {
            _repo = repo;
        }

        // GET: api/ProductCategories
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

        // GET: api/ProductCategories/5
        [HttpGet("{id}", Name = "GetProductCategory")]
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
        
        //// POST: api/ProductCategories
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}
        
        //// PUT: api/ProductCategories/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}
        
        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
