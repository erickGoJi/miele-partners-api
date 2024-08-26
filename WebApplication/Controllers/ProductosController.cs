using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication.Models;
using WebApplication.Repository;
using WebApplication.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Productos")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class ProductosController : Controller
    {
        private IConfiguration _config;
        private readonly MieleContext _context;
        private readonly IMapper _mapper;
        private readonly IProductoRepository _repo;
        
        private readonly IProductCategoriesRepository _repoProductCategories;
        private readonly IProductLinesRepository _repoProductLines;
        private readonly IProductSubLinesRepository _repoProductSubLines;

        public ProductosController(
            IConfiguration config, 
            MieleContext context, 
            IProductoRepository repo, 
            IMapper mapper, 
            IProductCategoriesRepository repoProductCategories, 
            IProductLinesRepository repoProductLines, 
            IProductSubLinesRepository repoProductSubLines
            )
        {
            _config = config;
            _context = context;
            _repo = repo;
            _mapper = mapper;
            _repoProductCategories = repoProductCategories;
            _repoProductLines = repoProductLines;
            _repoProductSubLines = repoProductSubLines;
        }

        // GET: api/Productos
        [HttpGet]
        public IActionResult GetAll()
        {
            var item = (from a in _context.Cat_Productos
                        //join b in _context.Cat_Categoria_Producto on a.id_categoria equals b.id
                        join c in _context.Cat_Linea_Producto on a.id_linea equals c.id
                        join d in _context.Cat_SubLinea_Producto on a.id_sublinea equals d.id
                        select new
                        {
                            id = a.id,
                            nombre = a.nombre,
                            categoria = c.descripcion,
                            a.id_linea, 
                            linea = c.descripcion,
                            a.id_sublinea, 
                            sublinea = d.descripcion, 
                            descripcion_corta = a.descripcion_corta,
                            descripcion_larga = a.descripcion_larga,
                            ficha_tecnica = a.ficha_tecnica,
                            precio_sin_iva = a.precio_sin_iva,
                            precio_con_iva = a.precio_con_iva,
                            sku = a.sku,
                            tipo = a.tipo, 
                            img_url = (from x in _context.Cat_Imagenes_Producto
                                       where x.id_producto == a.id
                                       select x)
                                         .Select(c => new Cat_Imagenes_Producto
                                         {
                                             id = c.id,
                                             id_producto = c.id_producto,
                                             url = c.url,
                                             estatus = c.estatus
                                         }).ToList()
                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }


        // GET: api/Productos
        //[Route("GetAllProducts")]
        [HttpGet("GetAllProducts", Name = "GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            var item = (from a in _context.Cat_Productos
                            //join b in _context.Cat_Categoria_Producto on a.id_categoria equals b.id
                        join c in _context.Cat_Linea_Producto on a.id_linea equals c.id
                        join d in _context.Cat_SubLinea_Producto on a.id_sublinea equals d.id
                        where d.id_linea_producto != 36 && d.id_linea_producto != 38
                        select new
                        {
                            id = a.id,
                            nombre = a.nombre,
                            categoria = c.descripcion,
                            a.id_linea,
                            linea = c.descripcion,
                            a.id_sublinea,
                            sublinea = d.descripcion,
                            descripcion_corta = a.descripcion_corta,
                            descripcion_larga = a.descripcion_larga,
                            ficha_tecnica = a.ficha_tecnica,
                            precio_sin_iva = a.precio_sin_iva,
                            precio_con_iva = a.precio_con_iva,
                            sku = a.sku,
                            tipo = a.tipo,
                            img_url = (from x in _context.Cat_Imagenes_Producto
                                       where x.id_producto == a.id
                                       select x)
                                         .Select(c => new Cat_Imagenes_Producto
                                         {
                                             id = c.id,
                                             id_producto = c.id_producto,
                                             url = c.url,
                                             estatus = c.estatus
                                         }).ToList()
                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        // GET: api/Productos/5
        [HttpGet("{id}", Name = "GetProductos")]
        public IActionResult Get(int id)
        {
            try
            {
                ProductsDto dto = _mapper.Map<ProductsDto>(_repo.Get(id));
                dto.relacionados = (from a in _context.productos_relacionados
                                 where (a.id_producto == id) || (a.id_producto_2 == id)
                                 select new ProductsRelatedDto
                                 {
                                     id = a.id,
                                     id_producto = id,
                                     id_producto_2 = (a.id_producto == id ? a.id_producto_2 : a.id_producto),
                                 }).ToList();
                //var item = (from a  )
                //    _context.Cat_Productos.Where(p => p.id == id)
                //.Include(p => p.sublinea)
                //.Include(p => p.linea)
                //.Include(p => p.sublinea)
                //.Include(p => p.cat_imagenes_producto)
                //.FirstOrDefault();


                if (dto == null)
                {
                    return NotFound();
                }

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete("Image/{prodId},{imgId}", Name = "DelImage_Producto")]
        public IActionResult DelImage_Producto( int prodId, int imgId)
        {
            try
            {
                var delImg = _context.Cat_Imagenes_Producto.First(a => a.id == imgId);
                if (delImg.id > 0)
                {
                    _context.Cat_Imagenes_Producto.Remove(delImg);
                    _context.SaveChanges();
                }
                List<ProductImagesDto> dto = _mapper.Map<List<ProductImagesDto>>(_context.Cat_Imagenes_Producto.Where(i => i.id_producto == prodId));
                return Ok(new { Success = true, dto });
            }
            catch (Exception ex)
            {
                return  Ok(new { Success = false });
                throw;
            }
        }


        [HttpGet("GetModels/{id}", Name = "GetModels")]
        public IActionResult GetModelosxSublinea(int id)
        {
            //Recibe el id de un producto, busca la sublinea de este producto y devuelve la lista de productos de la sublinea
            try
            {
                ProductsDto dto = _mapper.Map<ProductsDto>(_repo.Get(id));
                ProductSubLineDto lineDto = _mapper.Map<ProductSubLineDto>(_repoProductSubLines.Get(Convert.ToInt32(dto.id_sublinea)));
                
                if (dto == null )
                {
                    return NotFound();
                }
                //List<ProductModelDto> modelDtoAnt = _mapper.Map<List<ProductModelDto>>(_context.Cat_Productos.Where(p => p.id_sublinea == dto.id_sublinea).ToList());

                List<ProductModelDto> modelDto = (from a in _context.Cat_Productos
                                                   join b in _context.Cat_SubLinea_Producto on a.id_sublinea equals b.id
                                                   where b.id_linea_producto == lineDto.id_linea_producto
                                                   select new ProductModelDto
                                                   {
                                                       id = a.id,
                                                       nombre = a.nombre,
                                                       modelo = a.modelo
                                                   }).ToList();
                if (modelDto == null)
                {
                    return NotFound();
                }

                return Ok(modelDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // GET: api/Productos/5
        [HttpGet("GetRelated/{id}", Name = "GetRelatedProducts")]
        public IActionResult RelatedProducts(int id)
        {
            try
            {
                //var prods = (from rp in _context.productos_relacionados
                //            join p in _context.Cat_Productos on rp.id_producto_2 equals p.id
                //            where rp.id_producto == id
                //            select new {
                //                rp.id,
                //                rp.id_producto,
                //                id_prod_rel = rp.id_producto_2,
                //                p.nombre,
                //                p.sku,
                //                p.modelo,
                //                p.id_linea,
                //                p.id_sublinea
                //            });

                //var prods_2 = (from rp in _context.productos_relacionados
                //                join p in _context.Cat_Productos on rp.id_producto equals p.id
                //                where rp.id_producto_2 == id
                //                select new {
                //                    rp.id,
                //                    id_producto = rp.id_producto_2,
                //                    id_prod_rel = rp.id_producto,
                //                    p.nombre,
                //                    p.sku,
                //                    p.modelo,
                //                    p.id_linea,
                //                    p.id_sublinea
                //                });

                var prods_rel = (from a in _context.productos_relacionados
                                 where (a.id_producto == id) || (a.id_producto_2 == id)
                                 select new
                                 {
                                     id = a.id,
                                     id_prod_rel = (a.id_producto == id ? a.id_producto_2 : a.id_producto),
                                 }).ToList();



                if (prods_rel == null)
                {
                    return NotFound();
                }

                return Ok(prods_rel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // POST: api/Productos
        [HttpPost]
        public IActionResult Post([FromBody]Cat_Productos item)
        {
            var result = new Models.Response();

            try
            {
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    _context.Cat_Productos.Add(item);
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "OK"
                    };
                }
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

        [HttpPost("Create")]
        public IActionResult Create([FromBody]ProductCreateDto createDto)
        {
            try
            {
                if (!_repoProductCategories.Exists(createDto.id_categoria))
                    return NotFound($"Product category with Id { createDto.id_categoria } Not Found");

                if (!_repoProductLines.Exists(createDto.id_linea))
                    return NotFound($"Product line with Id {createDto.id_linea} Not Found");

                if (!_repoProductSubLines.Exists(createDto.id_sublinea))
                    return NotFound($"Product sub line with Id {createDto.id_sublinea} Not Found");

                //var productLine = _mapper.Map<Cat_Linea_Producto>(productLinesDto);
                Cat_Productos product = _mapper.Map<Cat_Productos>(createDto);
                product = _repo.Add(product);
                //foreach (ProductsRelatedDto item in createDto.relacionados)
                //{
                //    item.id_producto = product.id;
                //    productos_relacionados pr = _mapper.Map<productos_relacionados>(item);
                //    _context.productos_relacionados.Add(pr);
                //}
                foreach (productos_relacionados item in createDto.relacionados)
                {
                    item.id_producto = product.id;
                    _context.productos_relacionados.Add(item);
                }
                _context.SaveChanges();
                return CreatedAtRoute("GetProductSubLines", new { product.id }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost("Search")]
        public IActionResult Search([FromBody]ProductsSearchDto productsSearchDto)
        {
            try
            {
                var item = _repo.FindAllSearch(productsSearchDto);

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

        // PUT: api/Productos/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]ProductsUpdateDto productsUpdateDto)
        {
            try
            {
                if (!_repo.Exists(id))
                    return NotFound($"Product with Id { id } Not Found");

                if (!_repoProductCategories.Exists(productsUpdateDto.id_categoria))
                    return NotFound($"Product category with Id { productsUpdateDto.id_categoria } Not Found");

                if (!_repoProductLines.Exists(productsUpdateDto.id_linea))
                    return NotFound($"Product line with Id { productsUpdateDto.id_linea } Not Found");

                if (!_repoProductSubLines.Exists(productsUpdateDto.id_sublinea))
                    return NotFound($"Product sub line with Id { productsUpdateDto.id_sublinea } Not Found");


                //productsUpdateDto.cat_imagenes_producto
                //var result = _repo.Get(id);
                Cat_Productos product = _mapper.Map<Cat_Productos>(productsUpdateDto);
                product.id = id;
                var result =_repo.Update(product, id);
                //string res = GuardarRelacionados(productsUpdateDto.relacionados, id);
                List<productos_relacionados> nuevos = productsUpdateDto.relacionados.Where(r => r.id == 0).ToList();
                List<productos_relacionados> act = productsUpdateDto.relacionados.Where(r => r.id > 0).ToList();
                List<productos_relacionados> exis = _context.productos_relacionados.Where(p => p.id_producto == id || p.id_producto_2 == id).ToList();
                List<Cat_Imagenes_Producto> imgs = _context.Cat_Imagenes_Producto.Where(i => i.id_producto == id).ToList();
                foreach (ProductImagesDto item in productsUpdateDto.cat_imagenes_producto)
                {
                    if (!imgs.Exists(db => db.url == item.url))
                    {
                        _context.Cat_Imagenes_Producto.Add(new Cat_Imagenes_Producto { estatus = true, id_producto = id, url = item.url });
                    }
                }

                foreach (productos_relacionados item in exis)
                {
                    if (!act.Exists(c => c.id == item.id))
                    {
                        _context.productos_relacionados.Remove(item);
                    }
                }

                foreach (productos_relacionados item in nuevos)
                {
                    _context.productos_relacionados.Add(item);
                }
                _context.SaveChanges();
                //result.id_categoria = _repoProductCategories.Get(productLinesUpdateDto.id_categoria);
                //resul = productLinesUpdateDto.descripcion;
                //result.estatus = productLinesUpdateDto.estatus;
                //_repo.Update(result, id);

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

        
        [HttpGet("download_xls")]
        public IActionResult DescargarPlantillaUsuarios()
        {
            IActionResult response = Unauthorized();
            try
            {
                var selRuta = _context.parametro_Archivos.FirstOrDefault(p => p.id == 1);
                return response = Ok(new { result = "Success", data = selRuta.funcion + "plantilla_precios_productos.xls" });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalles = ex.Message });
            }
            
            
        }

        
    }

    
}
