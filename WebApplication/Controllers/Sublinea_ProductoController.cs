using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Repository;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Sublinea_Producto")]
    public class Sublinea_ProductoController : Controller
    {

        //private IConfiguration _config;
        private readonly MieleContext _context;
        private readonly IMapper _mapper;
        private readonly IProductSubLinesRepository _repo;
        private readonly IProductSubLineTypeRepository _relacion;

        public Sublinea_ProductoController(
            //IConfiguration config, 
            IMapper mapper,
            MieleContext context,
            IProductSubLinesRepository repo,
            IProductSubLineTypeRepository relacion)
        {
            //_config = config;
            _mapper = mapper;
            _context = context;
            _repo = repo;
            _relacion = relacion;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            //IActionResult response = Unauthorized();
            //List<Cat_SubLinea_Producto> resultado = new List<Cat_SubLinea_Producto>();
            try
            {
                //resultado = _context.Cat_SubLinea_Producto.ToList();
                var res = (from sl in _context.Cat_SubLinea_Producto
                           join l in _context.Cat_Linea_Producto on sl.id_linea_producto equals l.id
                           orderby sl.descripcion ascending
                           select new
                           {
                               id = sl.id,
                               descripcion = sl.descripcion,
                               //id_linea_producto = sl.id_linea_producto,
                               linea_producto_desc = l.descripcion,
                               estatus = sl.estatus,

                           }).ToList();


                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            //IActionResult response = Unauthorized();
            List<Cat_SubLinea_Producto> resultado = new List<Cat_SubLinea_Producto>();
            try
            {
                //_context.Rel_Categoria_Producto_Tipo_Producto.Include(cr=> cr.tipo_servicio)
                //var res = _context.Cat_SubLinea_Producto
                //    .Include(sl => sl.rel_categoria).
                //    ThenInclude(cat => cat.tipo_servicio)
                //    .FirstOrDefault(sl => sl.id == id);
                var res = _context.Cat_SubLinea_Producto
                    .Include(sl => sl.rel_categoria)
                    .FirstOrDefault(sl => sl.id == id);
                

                if (res == null)
                {
                    return NotFound();
                }
                return Ok(res);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody]ProductSubLineCreateDto createDto)
        {
            IActionResult response = Unauthorized();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                Cat_SubLinea_Producto cat_SubLinea = _mapper.Map<Cat_SubLinea_Producto>(createDto);
                cat_SubLinea.estatus = true;
                _context.Cat_SubLinea_Producto.Add(cat_SubLinea);
                _context.SaveChanges();
                List<Cat_Sucursales> all_sucursales = _context.Cat_Sucursales.Where(s => s.Id > 0).ToList();
                foreach (var suc in all_sucursales)
                {
                    condiones_comerciales_sucursal cc = new condiones_comerciales_sucursal();
                    cc.id_Cat_SubLinea_Producto = cat_SubLinea.id;
                    cc.id_Cat_Sucursales = suc.Id;
                    cc.margen = 0;
                    _context.condiones_comerciales_sucursal.Add(cc);
                    _context.SaveChanges();
                }


                //cat_SubLinea = _repo.Add(cat_SubLinea);
                return Ok(new { id = cat_SubLinea.id, result = "Success" });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }

        }

        [HttpPut("{id}")]
        public IActionResult Update([FromBody]ProductSubLineDto updateDto, int id)
        {
            IActionResult response = Unauthorized();
            try
            {
                if (updateDto.id == 0)
                {
                    Cat_SubLinea_Producto cat_SubLinea = _mapper.Map<Cat_SubLinea_Producto>(updateDto);
                    _context.Cat_SubLinea_Producto.Add(cat_SubLinea);
                    _context.SaveChanges();
                    cat_SubLinea = _repo.Add(cat_SubLinea);
                    return response = Ok(new { id = cat_SubLinea.id, result = "Success" });
                }
                else
                {
                    ProductSubLinesUpdateDto dto = _mapper.Map<ProductSubLinesUpdateDto>(updateDto);
                    var cat_SubLinea = _repo.Find(sl => sl.id == updateDto.id);
                    
                    if (cat_SubLinea == null)
                        return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + updateDto.id, id = updateDto.id });
                    else
                    {

                        foreach (ProductSubLineTypeDto item in updateDto.rel_categoria)
                        {
                            var rel = _relacion.Find(r => r.id == item.id && r.id_categoria == item.id_categoria);
                            if (rel == null)
                            {
                                //insert
                                item.id_categoria = id;
                                item.estatus = true;
                                Rel_Categoria_Producto_Tipo_Producto insRel = _relacion.Add(_mapper.Map<Rel_Categoria_Producto_Tipo_Producto>(item));
                                //response.Result = _mapper.Map<UserDto>(user);
                            }
                            else
                            {
                                //update
                                //_mapper.Map(item, rel);
                                _context.Rel_Categoria_Producto_Tipo_Producto.Update(rel);

                            }
                        }
                        _mapper.Map(dto, cat_SubLinea);
                        //cat_SubLinea.rel_categoria = null;
                        _repo.Update(cat_SubLinea, updateDto.id);
                        return response = Ok(new { id = cat_SubLinea.id, result = "Success" });
                    }
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
        }

        [HttpPost("Search")]
        public IActionResult Search([FromBody] ProductSubLineSearchDto dto)
        {
            try
            {
                var item = _repo.FindAllSearch(dto);

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
        //public IActionResult 
    }
}