using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class CotizacionModel
    {
        public long Id { get; set; }
        public string Numero { get; set; }
        public long Id_Cliente { get; set; }
        public long Id_Vendedor { get; set; }
        public DateTime fecha_cotiza { get; set; }
        public int Estatus { get; set; }
        public int Acciones { get; set; }
        public long Id_Canal { get; set; }
        public long Id_Cuenta { get; set; }
        public int Id_Estado_Instalacion { get; set; }
        public bool acepto_terminos_condiciones { get; set; }
        public string Observaciones { get; set; }
        public int creadopor { get; set; }
        public int id_formapago { get; set; }
        public bool entrega_sol { get; set; }
        public long id_cuenta { get; set; }
        public string estatus_desc { get; set; }
        public string ibs { get; set; }
        public DateTime cambio_ord_comp_generada { get; set; }
        public int Id_sucursal { get; set; }

        public double importe_precio_lista { get; set; }
        public double iva_precio_lista { get; set; }
        public double importe_condiciones_com { get; set; }
        public double iva_condiciones_com { get; set; }
        public double importe_promociones { get; set; }
        public double iva_promociones { get; set; }
        public double descuento_acumulado { get; set; }
        public double descuento_acumulado_cond_com { get; set; }
        public double comision_vendedor { get; set; }
        public string motivo_rechazo { get; set; }
        public bool rechazada { get; set; }
        public bool requiere_fact { get; set; }
        public long id_cotizacion_padre { get; set; }
        public int tiene_envio { get; set; }
        public int tiene_home { get; set; }
        public int tiene_certificado { get; set; }
        public bool faltan_certificados { get; set; }

        public List<ProductosCotizacionModel> productos { get; set; }
        public List<PromocionesModel> promociones_respueta { get; set; }

    }

    public class ProductosCotizacionModel
    {
        public int id { get; set; }
        public string sku { get; set; }
        public string modelo { get; set; }
        public string nombre { get; set; }
        public string descripcion_corta { get; set; }
        public int cantidad { get; set; }
        public float margen_cc { get; set; }
        public float importe_precio_lista { get; set; }
        public float importe_total_bruto { get; set; }
        public float importe_condiciones_com { get; set; }
        public float importe_con_descuento { get; set; }
        public float descuento { get; set; }
        public float importetotal { get; set; }
        public bool es_regalo { get; set; }
        public bool agregado_automaticamente { get; set; }
        public int? id_sublinea { get; set; }
        public int id_linea { get; set; }
        public int? id_linea_orden { get; set; }
        public int id_superlinea_orden { get; set; }
        public string nombre_linea_orden { get; set; }

        public List<Cat_Imagenes_Producto> cat_imagenes_producto { get; set; }
        

    }

    public class PromocionesModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string beneficios { get; set; }
        public string inicio { get; set; }
        public string fin { get; set; }
        public bool vigencia_indefinida { get; set; }
        public bool aplicada { get; set; }
        public bool beneficio_obligatorio { get; set; }
    }

}
