using Microsoft.AspNetCore.Mvc;
using System;
using WebApplication.Repository.Dashboard;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Dashboard")]
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _repo;

        public DashboardController(IDashboardRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("CantidadCasos")]
        public IActionResult CantidadCasos(
            bool? Garantia = null, 
            string PeriodoTiempo = null, 
            int? TipoServicio = null, 
            int? SubTipoServicio = null, 
            int? TecnicoId = null, 
            int? TipoTecnico = null, 
            int? SubTipoTecnico = null, 
            int? ProductoId = null
            )
        {
            try
            {
                return Ok(_repo.CantidadCasos(Garantia, PeriodoTiempo, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico, ProductoId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("CantidadCasosIncumplimiento")]
        public IActionResult CantidadCasosIncumplimiento(
            bool? Garantia = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null
            )
        {
            try
            {
                return Ok(_repo.CantidadCasosIncumplimiento(Garantia, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("CantidadCasosPendientes")]
        public IActionResult CantidadCasosPendientes(
            bool? Garantia = null,
            string PeriodoTiempo = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null
            )
        {
            try
            {
                return Ok(_repo.CantidadCasosPendientes(Garantia, PeriodoTiempo, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("PorcentajeDiasRespuestaMayor")]
        public IActionResult PorcentajeDiasRespuestaMayor(
            bool? Garantia = null,
            string PeriodoTiempo = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null,
            int? dia = null
            )
        {
            try
            {
                return Ok(_repo.PorcentajeDiasRespuestaMayor(Garantia, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico, dia));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("PorcentajeDiasRespuestaMenorIgual")]
        public IActionResult PorcentajeDiasRespuestaMenorIgual(
            bool? Garantia = null,
            string PeriodoTiempo = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null,
            int? dia = null
            )
        {
            try
            {
                return Ok(_repo.PorcentajeDiasRespuestaMenorIgual(Garantia, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico, dia));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("PorcentajeServicioQuejas")]
        public IActionResult PorcentajeServicioQuejas(int? productoId = null)
        {
            try
            {
                return Ok(_repo.PorcentajeServicioQuejas(productoId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("PorcentajeServiciosResultosPrimeraVisita")]
        public IActionResult PorcentajeServiciosResultosPrimeraVisita(
            bool? Garantia = null,
            string PeriodoTiempo = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null
            )
        {
            try
            {
                return Ok(_repo.PorcentajeServiciosResultosPrimeraVisita(Garantia, PeriodoTiempo, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("PromedioDiasRespuesta")]
        public IActionResult PromedioDiasRespuesta(
            bool? Garantia = null,
            string PeriodoTiempo = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null
            )
        {
            try
            {
                return Ok(_repo.PromedioDiasRespuesta(Garantia, PeriodoTiempo, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("PromedioDiasSolucion")]
        public IActionResult PromedioDiasSolucion(
            bool? Garantia = null,
            string PeriodoTiempo = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null
            )
        {
            try
            {
                return Ok(_repo.PromedioDiasSolucion(Garantia, PeriodoTiempo, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("PorcentajePrimeraInstalacion")]
        public IActionResult PorcentajePrimeraInstalacion(
            bool? Garantia = null, 
            string PeriodoTiempo = null, 
            int? TipoServicio = null, 
            int? SubTipoServicio = null, 
            int? TecnicoId = null, 
            int? TipoTecnico = null, 
            int? SubTipoTecnico = null
            )
        {
            try
            {
                return Ok(_repo.PorcentajePrimeraInstalacion(Garantia, PeriodoTiempo, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("PromedioTiempoReaccionQuejas")]
        public IActionResult PromedioTiempoReaccionQuejas(int? productoId = null)
        {
            try
            {
                return Ok(_repo.PromedioTiempoReaccionQuejas(productoId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("PromedioTiempoSolucionQuejas")]
        public IActionResult PromedioTiempoSolucionQuejas(int? productoId = null)
        {
            try
            {
                return Ok(_repo.PromedioTiempoSolucionQuejas(productoId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        #region Graficas
        [HttpGet("ResumenDashboard")]
        public IActionResult ResumenDashboard(
            bool? Garantia = null,
            string PeriodoTiempo = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null
            )
        {
            try
            {
                return Ok(_repo.ResumenDashboard(Garantia, PeriodoTiempo, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("GraficaCasosPendientes")]
        public IActionResult GraficaCasosPendientes(
            bool? Garantia = null,
            string PeriodoTiempo = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null
            )
        {
            try
            {
                return Ok(_repo.GraficaCasosPendientes(Garantia, PeriodoTiempo, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("GraficaReparacionVisita")]
        public IActionResult GraficaReparacionVisita(
            bool? Garantia = null,
            string PeriodoTiempo = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null
            )
        {
            try
            {
                return Ok(_repo.GraficaReparacionVisita(Garantia, PeriodoTiempo, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("GraficaTiempoRespuesta")]
        public IActionResult GraficaTiempoRespuesta(
            bool? Garantia = null,
            string PeriodoTiempo = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null
            )
        {
            try
            {
                return Ok(_repo.GraficaTiempoRespuesta(Garantia, PeriodoTiempo, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("GraficaTiempoSolucion")]
        public IActionResult GraficaTiempoSolucion(
            bool? Garantia = null,
            string PeriodoTiempo = null,
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null
            )
        {
            try
            {
                return Ok(_repo.GraficaTiempoSolucion(Garantia, PeriodoTiempo, TipoServicio, SubTipoServicio, TecnicoId, TipoTecnico, SubTipoTecnico));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        #endregion
    }
}