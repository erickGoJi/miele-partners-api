using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Models;
using WebApplication.Repository;
using WebApplication.Service;
using WebApplication.Utility;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/ParametrosCondiciones")]
    public class Parametros_Condiciones : Controller
    {
        //private IConverter _converter;
        //private IConfiguration _config;
        //private ICotizacionPDF _documentoService;
        //private IEmailRepository _emailService;
        private readonly MieleContext _context;

        public Parametros_Condiciones( MieleContext context)
        {
            //_converter = converter;
            //_config = config;
            //_documentoService = documentoService;
            //_emailService = emailService;
            _context = context;
        }

        [HttpGet("getParametroTerminosCondiciones")]
        public IActionResult getParametroTerminosCondiciones()
        {
            Response<Parametro_Archivo_Terminos_Condiciones> response = new Response<Parametro_Archivo_Terminos_Condiciones>();
            try
            {
                response.Success = true;
                response.Message = "";
                response.Result = _context.parametro_Terminos_Condiciones.Where(a=> a.id==1).FirstOrDefault();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost("saveParametrosTerminosCondicionesFile/{id_TC}")]
        //[Authorize]
        public async Task<IActionResult> saveParametrosTerminosCondicionesFile(int id_TC, List<IFormFile> file)
        {
            Parametro_Archivo_Terminos_Condiciones CatDocumentsFile1 = new Parametro_Archivo_Terminos_Condiciones();
            Response<Parametro_Archivo_Terminos_Condiciones> response = new Response<Parametro_Archivo_Terminos_Condiciones>();
            try
            {
                long size = file.Sum(f => f.Length);
                var filePath = Environment.CurrentDirectory;
                var extencion = file[0].FileName.Split(".");
                var _guid = Guid.NewGuid();
                var path = "/Imagenes/PDF_terminos/" + _guid + "." + extencion[extencion.Length - 1].ToLower();
                CatDocumentsFile1.id = 1;
                CatDocumentsFile1.ruta = path;
                CatDocumentsFile1.funcion = "http://23.253.173.64/mieletickets/pdf_terminos/";
                //CatDocumentsFile1. = id_TC;
                foreach (var formFile in file)
                {
                    if (formFile.Length > 0)
                    {
                        if (formFile.ContentType.StartsWith("image"))
                        {
                            using (var stream = new FileStream(filePath + "/Imagenes/PDF_terminos/" + _guid + "." + extencion[extencion.Length - 1], FileMode.Create))
                            {
                                formFile.CopyTo(stream);
                            }
                        }
                        else
                        {
                            using (var stream = new FileStream(filePath + path, FileMode.Create))
                            {
                                formFile.CopyTo(stream);
                            }
                        }
                    }
                }
                _context.Update(CatDocumentsFile1);
                await _context.SaveChangesAsync();
                response.Success = true;
                response.Message = "";
                response.Result = CatDocumentsFile1;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.ToString();
            }
            return Ok(response);
        }
    }
}
