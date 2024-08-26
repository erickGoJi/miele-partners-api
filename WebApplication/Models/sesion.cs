using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Users
    {
        public long id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string paterno { get; set; }
        public string materno { get; set; }
        public DateTime fecha_nacimiento { get; set; }
        public string email { get; set; }
        public string avatar { get; set; }
        public string telefono { get; set; }
        public string telefono_movil { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public long id_rol { get; set; }
        public Cat_Roles Rol { get; set; }
        public long id_app { get; set; }
        public Cat_Aplicaciones App { get; set; }
        public int id_cuenta { get; set; }
        public int id_canal { get; set; }
        public int id_Sucursales { get; set; }
        public string nivel { get; set; }// 1 Sucursal, 2 cuenta, 3 Canal 
        public List<Tecnicos> tecnicos { get; set; }
        public List<comisiones_vendedores> comisiones_vendedores { get; set; }
    }

    public class Cat_Aplicaciones
    {
        public long id { get; set; }
        public string App { get; set; }
        public string Descripcion { get; set; }

        public List<Users> users { get; set; }
    }

    public class Cat_Roles
    {
        public long id { get; set; }
        public string rol { get; set; }
        public string siglas { get; set; }
        public List<Users> users { get; set; }
    }


    public class Tecnicos
    {
        public long id { get; set; }
        public Users users { get; set; }
        public string noalmacen { get; set; }
        public int id_tipo_tecnico { get; set; }
        public string color { get; set; }
        public Cat_Tecnicos_Tipo tecnicos_Tipo { get; set; }
        public int id_cat_tecnicos_sub_Tipo { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }

        public List<Tecnicos_Actividad> tecnicos_actividad { get; set; }
        public List<Tecnicos_Cobertura> tecnicos_cobertura { get; set; }
        public List<Tecnicos_Producto> tecnicos_producto { get; set; }
        public List<Visita> visita { get; set; }
        public List<rel_tecnico_visita> rel_tecnico_visita { get; set; }
    }

    public class Cat_Tecnicos_Tipo
    {
        public int id { get; set; }
        public string desc_tipo { get; set; }

        public List<Tecnicos> tecnicos { get; set; }
    }

    public class Cat_Tecnicos_Sub_Tipo
    {
        public int id { get; set; }
        public int id_tipo { get; set; }
        public string Sub_desc_tipo { get; set; }
    }

    public class Tecnicos_Actividad
    {
        public long id { get; set; }
        public long id_user { get; set; }
        public Tecnicos users { get; set; }
        public long id_actividad { get; set; }
        //public Cat_Actividad actividad { get; set; }
    }

    public class Cat_Actividad
    {
        public long id { get; set; }
        public string desc_actividad { get; set; }
        public bool estatus { get; set; }

        //public List<Tecnicos_Actividad> tecnicos_actividad { get; set; }
    }

    public class Tecnicos_Cobertura
    {
        public long id { get; set; }
        public long id_user { get; set; }
        public Tecnicos users { get; set; }
        public long id_cobertura { get; set; }
        public Cat_Area_Cobertura cobertura { get; set; }
    }

    public class Cat_Area_Cobertura
    {
        public long id { get; set; }
        public string desc_cobertura { get; set; }
        public bool estatus { get; set; }

        public List<Tecnicos_Cobertura> tecnicos_cobertura { get; set; }
    }

    public class Tecnicos_Producto
    {
        public long id { get; set; }
        public long id_user { get; set; }
        public Tecnicos users { get; set; }
        public int id_categoria_producto { get; set; }
        public Cat_SubLinea_Producto producto { get; set; }
    }

    public class Cat_Producto
    {
        public long id { get; set; }
        public string desc_producto { get; set; }
        public bool estatus { get; set; }


    }

    /// </Productos y refacciones>

    /// <Servicios-tickets>
    public class Cat_tipo_servicio
    {
        public int id { get; set; }
        public string desc_tipo_servicio { get; set; }
        public bool estatus { get; set; }

        public List<Servicio> servicio { get; set; }
        public List<Sub_cat_tipo_servicio> Sub_cat_tipo_servicio { get; set; }
        public List<Rel_Categoria_Producto_Tipo_Producto> rel_categoria { get; set; }
    }

    public class Sub_cat_tipo_servicio
    {
        public int id { get; set; }
        public int id_tipo_servicio { get; set; }
        public Cat_tipo_servicio tipo_servicio { get; set; }//catalogo
        public string sub_desc_tipo_servicio { get; set; }
        public bool estatus { get; set; }

    }

    public class cat_garantia
    {
        public int id { get; set; }
        public bool garantia { get; set; }
        public string desc_garantia { get; set; }
    }

    public class cat_periodo
    {
        public int id { get; set; }
        public string desc_periodio { get; set; }
    }

    public class Cat_solicitado_por
    {
        public int id { get; set; }
        public string desc_solicitado_por { get; set; }
        public bool estatus { get; set; }

        public List<Servicio> servicio { get; set; }
    }

    public class Cat_distribuidor_autorizado
    {
        public long id { get; set; }
        public string desc_distribuidor { get; set; }
        public bool estatus { get; set; }

    }

    public class Cat_solicitud_via
    {
        public int id { get; set; }
        public string desc_solicitud_via { get; set; }
        public bool estatus { get; set; }

        public List<Servicio> servicio { get; set; }
    }

    public class Cat_estatus_servicio
    {
        public int id { get; set; }
        public string desc_estatus_servicio { get; set; }
        public string desc_estatus_servicio_en { get; set; }
        public int id_tipo_servicio { get; set; }
        public bool estatus { get; set; }

        public List<Servicio> servicio { get; set; }
    }

    public class CatEstatus_Visita
    {
        public int id { get; set; }
        public string desc_estatus_visita { get; set; }
        public string desc_estatus_visita_en { get; set; }
        public int id_tipo_servicio { get; set; }
        public bool estatus { get; set; }
    }

    public class CatEstatus_Producto
    {
        public int id { get; set; }
        public string desc_estatus_producto { get; set; }
        public string desc_estatus_producto_en { get; set; }
        public int id_tipo_servicio { get; set; }
        public bool estatus { get; set; }
    }

    public class Cat_Categoria_Servicio
    {
        public int id { get; set; }
        public string desc_categoria_servicio { get; set; }
        public int id_tipo_servicio { get; set; }
        public bool estatus { get; set; }

        public List<Servicio> servicio { get; set; }
    }

    public class Cat_Direccion
    {
        public int id { get; set; }
        public Clientes cliente { get; set; }
        public long id_cliente { get; set; }
        public string calle_numero { get; set; }
        public string cp { get; set; }
        public int id_estado { get; set; }
        public int id_municipio { get; set; }
        public string colonia { get; set; }
        public string telefono { get; set; }
        public string telefono_movil { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public int? tipo_direccion { get; set; } //NOTA: No hay catalogo 1 es Instalacion , 2 es Envio 
        public string nombrecontacto { get; set; }
        public string numExt { get; set; }
        public string numInt { get; set; }
        public string Fecha_Estimada { get; set; }
        public int id_localidad { get; set; }
        public int id_prefijo_calle { get; set; }
    }

    public class Direcciones_Cliente
    {
        public int id { get; set; }
        public Clientes cliente { get; set; }
        public long id_cliente { get; set; }
        public string calle_numero { get; set; }
        public string cp { get; set; }
        public int id_estado { get; set; }
        public int id_municipio { get; set; }
        public string colonia { get; set; }
        public string telefono { get; set; }
        public string telefono_movil { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public int? tipo_direccion { get; set; } //NOTA: No hay catalogo 1 es Instalacion , 2 es Envio 
        public string nombrecontacto { get; set; }
        public string numExt { get; set; }
        public string numInt { get; set; }
        public string Fecha_Estimada { get; set; }
        public int id_localidad { get; set; }
        public int id_prefijo_calle { get; set; }
    }

    public class Notificaciones {
        public int id { get; set; }
        public string evento { get; set; }
        public int rol_notificado { get; set; }
        public string descripcion { get; set; }
        public bool estatus_leido { get; set; }
        public string url { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
    }

    public class Cat_Motivo_Cierre_Cliente
    {
        public int id { get; set; }
        public string desc_motivo_cierre { get; set; }
        public bool estatus { get; set; }
    }

    public class His_Servicio_Estatus
    {
        public int id { get; set; }
        public long id_servicio { get; set; }//catalogo
        public Servicio servicio { get; set; }
        public int estatus_inicial { get; set; }
        public int estatus_final { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
    }

    public class Rel_servicio_visita_producto
    {
        public int id { get; set; }
        public long id_vista { get; set; }
        public Visita visita { get; set; }
        public long id_producto { get; set; }
        public int id_categoria { get; set; }
        public string descripcion_cierre { get; set; }
        public string no_serie { get; set; }
        public bool primera_visita { get; set; }
        public bool garantia { get; set; }
        public int estatus { get; set; }
        public int reparacion { get; set; }
    }

    public class Rel_servicio_visita_producto_cat_producto
    {
        public int id { get; set; }
        public long id_vista { get; set; }
        public Visita visita { get; set; }
        public long id_producto { get; set; }
        public long id_producto_id_cat_producto { get; set; }
        public int id_categoria { get; set; }
        public string descripcion_cierre { get; set; }
        public string no_serie { get; set; }
        public bool primera_visita { get; set; }
        public bool garantia { get; set; }
        public int estatus { get; set; }
        public int reparacion { get; set; }
    }

    public class Rel_servicio_visita_Refaccion
    {
        public int id { get; set; }
        public long id_vista { get; set; }
        public Visita visita { get; set; }
        public long id_producto { get; set; }
        public string actividades { get; set; }
        public string fallas { get; set; }
        public int estatus { get; set; }
        public string comentarios { get; set; }

        public List<Piezas_Repuesto> piezas_repuesto { get; set; }
        public List<Piezas_Repuesto_Tecnico> piezas_repuesto_tecnico { get; set; }
    }

    public class Piezas_Repuesto
    {
        public int id { get; set; }
        public int id_rel_servicio_refaccion { get; set; }
        public Rel_servicio_visita_Refaccion refacciones { get; set; }
        public int cantidad { get; set; }
        public bool garantia { get; set; }
        public long id_material { get; set; }
        public bool solicitada { get; set; }
        public bool llegada { get; set; }
        public string comentarios { get; set; }
    }

    public class Informe_parte_recibida
    {
        public int id { get; set; }
        public int id_visita { get; set; }
        public string no_material { get; set; }
        public int id_producto { get; set; }
        public int cantidad { get; set; }
    }

    public class Piezas_Repuesto_Tecnico
    {
        public int id { get; set; }
        public int id_rel_servicio_refaccion { get; set; }
        public Rel_servicio_visita_Refaccion refacciones { get; set; }
        public int cantidad { get; set; }
        public string tipo_refaccion { get; set; }
        public long id_material { get; set; }
    }

    //public class Visita_solicitada
    //{
    //    public int id { get; set; }
    //    public int id_servicio { get; set; }
    //    public int cantidad { get; set; }
    //    public long id_material { get; set; }
    //    public bool solicitada { get; set; }
    //}

    //public class Visita_llegada
    //{
    //    public int id { get; set; }
    //    public int id_servicio { get; set; }
    //    public int cantidad { get; set; }
    //    public long id_material { get; set; }
    //    public bool llegada { get; set; }
    //}

    public class cat_tipo_refaccion
    {
        public int id { get; set; }
        public string desc_tipo_refaccion { get; set; }
        public bool estatus { get; set; }
    }

    public class cat_falla
    {
        public int id { get; set; }
        public string desc_falla_en { get; set; }
        public string desc_falla_es { get; set; }
        public bool estatus { get; set; }
    }

    public class cat_reparacion
    {
        public int id { get; set; }
        public string desc_reparacion_en { get; set; }
        public string desc_reparacion_es { get; set; }
        public bool estatus { get; set; }
    }

    public class cat_checklist_producto
    {
        public int id { get; set; }
        public string desc_checklist_producto { get; set; }
        public bool estatus { get; set; }
    }

    public class Cat_Servicio_Sin_Pago
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public bool Estatus { get; set; }
    }

    public class Servicio
    {
        public long id { get; set; }
        public long id_cliente { get; set; }
        public Clientes cliente { get; set; }
        public int id_tipo_servicio { get; set; }
        public Cat_tipo_servicio tipo_servicio { get; set; }//catalogo
        public int id_sub_tipo_servicio { get; set; }
        public Sub_cat_tipo_servicio sub_tipo_servicio { get; set; }//catalogo
        public int id_solicitado_por { get; set; }
        public Cat_solicitado_por solicitado_por { get; set; }//catalogo
        public long id_distribuidor_autorizado { get; set; }
        public string contacto { get; set; }
        public int id_solicitud_via { get; set; }//catalogo
        public Cat_solicitud_via solicitud_via { get; set; }//catalogo
        public string descripcion_actividades { get; set; }
        public int id_categoria_servicio { get; set; }//catalogo
        public string no_servicio { get; set; }
        public DateTime fecha_servicio { get; set; }
        public int? id_estatus_servicio { get; set; }//catalogo
        public string IBS { get; set; }
        public int? id_motivo_cierre { get; set; }
        public bool? activar_credito { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public int servicio_sin_pago { get; set; }

        public List<Visita> visita { get; set; }
        public List<His_Servicio_Estatus> historial { get; set; }
        public List<Servicio_Troubleshooting> servicio_troubleshooting { get; set; }
        public List<quejas_servicios> quejas_servicios { get; set; }
        public List<encuesta_general> e_general { get; set; }
    }

    public class Prediagnostico
    {
        public long id { get; set; }
        public long id_visita { get; set; }
        public Visita visita { get; set; }
        public string observaciones { get; set; }

        public List<Prediagnostico_Refacciones> refacciones { get; set; }
    }

    public class Prediagnostico_Refacciones
    {
        public long id { get; set; }
        public long id_prediagnostico { get; set; }
        public Prediagnostico prediagnostico { get; set; }
        public long id_material { get; set; }
        public int cantidad { get; set; }
        public string numero_ir { get; set; }
        public bool garantia { get; set; }
        public bool estatus { get; set; }
    }

    public class Visita
    {
        public long id { get; set; }
        public long id_servicio { get; set; }//catalogo
        public Servicio servicio { get; set; }
        public int id_direccion { get; set; }//catalogo
        public DateTime fecha_visita { get; set; }
        public DateTime fecha_entrega_refaccion { get; set; }
        public string hora { get; set; }
        public string hora_fin { get; set; }
        public string actividades_realizar { get; set; }
        public string concepto { get; set; }
        public decimal cantidad { get; set; }
        public bool pagado { get; set; }
        public bool pago_pendiente { get; set; }
        public bool garantia { get; set; }
        public string fecha_deposito { get; set; }
        public DateTime? fecha_inicio_visita { get; set; }
        public DateTime? fecha_fin_visita { get; set; }
        public string no_operacion { get; set; }
        public string comprobante { get; set; }
        public bool terminos_condiciones { get; set; }
        public bool factura { get; set; }
        public int? estatus { get; set; }
        public bool? pre_diagnostico { get; set; }
        public bool? asignacion_refacciones { get; set; }
        public bool? entrega_refacciones { get; set; }
        public bool? si_acepto_tecnico_refaccion { get; set; }
        public string latitud_inicio { get; set; }
        public string longitud_inicio { get; set; }
        public string latitud_fin { get; set; }
        public string longitud_fin { get; set; }
        public string imagen_firma { get; set; }
        public string imagen_pago_referenciado { get; set; }
        public string persona_recibe { get; set; }
        public string url_ppdf_reporte { get; set; }
        public DateTime? fecha_agendado { get; set; }
        public string fecha_limite_pago { get; set; }
        public string fec_pago { get; set; }
        public string id_pago { get; set; }
        public string succes_pago { get; set; }
        public DateTime fecha_cancelacion { get; set; }
        public string motiva_completado { get; set; }
        public DateTime fecha_completado { get; set; }
        public string motiva_cancelación { get; set; }
        public string url_pdf_checklist { get; set; }
        public string url_pdf_cotizacion { get; set; }

        public string url_pdf_confirmacion_visita { get; set; }
        public List<Rel_servicio_visita_producto> servicio_producto { get; set; }
        public List<rel_tecnico_visita> rel_tecnico_visita { get; set; }
        public List<Rel_servicio_visita_Refaccion> servicio_refaccion { get; set; }
        public List<Producto_Check_List_Respuestas> producto_check_List_respuestas { get; set; }
    }
    /// </Servicios-tickets>

    /// <rel_tecnicos_visita>
    public class rel_tecnico_visita
    {
        public int id { get; set; }
        public long id_tecnico { get; set; }//catalogo
        public Tecnicos tecnico { get; set; }
        public long id_vista { get; set; }
        public Visita visita { get; set; }
        public bool tecnico_responsable { get; set; }
    }
    /// </rel_tecnicos_visita>

    /// <Clientes>
    public class Clientes
    {
        public long id { get; set; }
        public string folio { get; set; }
        public string nombre { get; set; }
        public string paterno { get; set; }
        public string materno { get; set; }
        public string nombre_comercial { get; set; }
        public string nombre_contacto { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public string telefono_movil { get; set; }
        public string referencias { get; set; }
        public string tipo_persona { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public long referidopor { get; set; }
        public DateTime vigencia_ref { get; set; }
        public int Id_sucursal { get; set; }
        public int tipo_cliente { get; set; }

        public List<Servicio> servicio { get; set; }
        public List<Cat_Direccion> direccion { get; set; }
        public List<DatosFiscales> datos_fiscales { get; set; }
        public List<Cotizaciones> Id_Cliente_Cotizacion { get; set; }
        public List<Cliente_Productos> Id_Cliente_Productos { get; set; }
        public List<Cer_producto_cliente> cer_producto_cliente { get; set; }
        public List<Home_producto_cliente> home_producto_cliente { get; set; }
        public List<Direcciones_Cliente> direcciones_Clientes { get; set; }
    }

    public class Rel_Imagen_Producto_Visita
    {
        public long id { get; set; }
        public int id_visita { get; set; }
        public int id_producto { get; set; }
        public string path { get; set; }
        public string actividad { get; set; }
        public bool estatus { get; set; }
        public bool checklist { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
    }

    public class DatosFiscales
    {
        public long id { get; set; }
        public string nombre_fact { get; set; }
        public string razon_social { get; set; }
        public string rfc { get; set; }
        public string email { get; set; }
        public string calle_numero { get; set; }
        public string cp { get; set; }
        public int id_estado { get; set; }
        public int id_municipio { get; set; }
        public string colonia { get; set; }
        public string Ext_fact { get; set; }
        public string Int_fact { get; set; }
        public string telefono_fact { get; set; }
        public Clientes cliente { get; set; }
        public long id_cliente { get; set; }
        public int id_prefijo_calle { get; set; }
    }

    public class DatosFiscales_Canales
    {
        public long id { get; set; }
        public string nombre_fact { get; set; }
        public string razon_social { get; set; }
        public string rfc { get; set; }
        public string email { get; set; }
        public string calle_numero { get; set; }
        public string cp { get; set; }
        public int id_estado { get; set; }
        public int id_municipio { get; set; }
        public string colonia { get; set; }
        public string Ext_fact { get; set; }
        public string Int_fact { get; set; }
        public string telefono_fact { get; set; }
        public long id_canal { get; set; }
    }

    public class DatosFiscales_Sucursales
    {
        public long id { get; set; }
        public string nombre_fact { get; set; }
        public string razon_social { get; set; }
        public string rfc { get; set; }
        public string email { get; set; }
        public string calle_numero { get; set; }
        public string cp { get; set; }
        public int id_estado { get; set; }
        public int id_municipio { get; set; }
        public string colonia { get; set; }
        public string Ext_fact { get; set; }
        public string Int_fact { get; set; }
        public string telefono_fact { get; set; }
        public int id_Sucursal { get; set; }
        public Cat_Sucursales Cat_Sucursales { get; set; }
    }

    public class Cat_Direccion_sucursales
    {
        public int id { get; set; }
        public Cat_Sucursales cat_sucursales { get; set; }
        public int id_sucursales { get; set; }
        public string calle_numero { get; set; }
        public string cp { get; set; }
        public int id_estado { get; set; }
        public int id_municipio { get; set; }
        public string colonia { get; set; }
        public string telefono { get; set; }
        public string telefono_movil { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public int? tipo_direccion { get; set; } //NOTA: No hay catalogo 1 es Instalacion , 2 es Envio 
        public string nombrecontacto { get; set; }
        public string numExt { get; set; }
        public string numInt { get; set; }
        public string Fecha_Estimada { get; set; }
        public int id_localidad { get; set; }
        public int id_prefijo_calle { get; set; }
    }

    public class Cat_Motivo
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public string correo { get; set; }
        public bool estatus { get; set; } // Muestra los motivos vigentes
        public List<Mensaje> mensajes { get; set; } // Relación 1:N
    }

    public class Mensaje
    {
        public int id { get; set; }
        public int motivo_id { get; set; }
        public int orden_id { get; set; }
        public string detalle_msj { get; set; }
        public int usuario_id { get; set; }
        public DateTime fecha_msj { get; set; }
        public Cat_Motivo cat_Motivo { get; set; } // Relación 1:1
    }

    public class Direccion_Cotizacion
    {
        public int id { get; set; }
        public Cotizaciones cotizacion { get; set; } // Relación 1:1
        public long id_cotizacion { get; set; }
        public string calle_numero { get; set; }
        public string cp { get; set; }
        public int id_estado { get; set; }
        public int id_municipio { get; set; }
        public string colonia { get; set; }
        public string telefono { get; set; }
        public string telefono_movil { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public int? tipo_direccion { get; set; } // NOTA: No hay catalogo, 1 es Instalacion , 2 es Envio 
        public string nombrecontacto { get; set; }
        public string numExt { get; set; }
        public string numInt { get; set; }
        public string Fecha_Estimada { get; set; }
        public int id_localidad { get; set; }
        public int id_prefijo_calle { get; set; }
    }


    public class Cat_Estado
    {
        public int id { get; set; }
        public string desc_estado { get; set; }
        public bool estatus { get; set; }

        public List<Clientes> clientes { get; set; }
        public List<Vendedores> Id_Estado_Vendedores { get; set; }
        public List<Home_Producto_Estado> Home_Productos { get; set; }
    }

    public class Cat_Municipio
    {
        public int id { get; set; }
        public Cat_Estado estado { get; set; }
        public string desc_municipio { get; set; }
        public bool estatus { get; set; }

        public List<Clientes> clientes { get; set; }
        public List<Vendedores> Id_Municipio_Vendedores { get; set; }
    }

    public class Cat_Localidad
    {
        public int id { get; set; }
        public Cat_Municipio municipio { get; set; }
        public string desc_localidad { get; set; }
        public long cp { get; set; }
        public string zona { get; set; }
        public bool estatus { get; set; }

        public List<Cer_viaticos> viaticos { get; set; }
    }

    /// </Clientes>

    /// <Productos y refacciones>
    public class Cat_Lista_Precios
    {
        public int id { get; set; }
        public string grupo_precio { get; set; }
        public decimal precio_sin_iva { get; set; }
        public bool estatus { get; set; }

        public List<Cat_Materiales> materiales { get; set; }
        //public List<Cat_Materiales_Tecnico> materiales_tecnico { get; set; }
    }

    public class Cat_Materiales
    {
        public int id { get; set; }
        public string no_material { get; set; }
        public string descripcion { get; set; }
        public Cat_Lista_Precios grupo_precio { get; set; }
        public int id_grupo_precio { get; set; }
        public int? cantidad { get; set; }
        public bool estatus { get; set; }

        //public List<Cat_Materiales_Tecnico> materiales_tecnico { get; set; }
    }

    public class Cat_Materiales_Tecnico
    {
        public int id { get; set; }       
        //public Cat_Materiales material { get; set; }
        public int id_material { get; set; }
        public int? id_tecnico { get; set; }
        public int cantidad { get; set; }
        public bool estatus { get; set; }

    }

    public class Cat_Categoria_Producto
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public string codigo { get; set; }
        //public int no_tecnicos { get; set; }
        //public string horas_tecnicos { get; set; }
        //public int? precio_visita { get; set; }
        //public int? precio_hora_tecnico { get; set; }
        //public int id_tipo_servicio { get; set; }
        public bool estatus { get; set; }

    }

    public class Rel_Categoria_Producto_Tipo_Producto
    {
        public int id { get; set; }
        public int no_tecnicos { get; set; }
        public string horas_tecnicos { get; set; }
        public int? precio_visita { get; set; }
        public int? precio_hora_tecnico { get; set; }
        public Cat_SubLinea_Producto categoria { get; set; }
        public int id_categoria { get; set; }
        public Cat_tipo_servicio tipo_servicio { get; set; }
        public int id_tipo_servicio { get; set; }
        public bool estatus { get; set; }
    
    }

    public class Parametro_Archivo
    {
        public int id { get; set; }
        public string funcion { get; set; }
        public string ruta { get; set; }
        [Column(TypeName ="decimal(18,4)")]
        public decimal col_uno { get; set; }
    }

    public class Parametro_Archivo_Terminos_Condiciones
    {
        public int id { get; set; }
        public string funcion { get; set; }
        public string ruta { get; set; }
    }

    public class Cat_SuperLinea
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public bool estatus { get; set; }

        public List<Cat_Linea_Producto> cat_Linea_Productos { get; set; }
    }

    public class Cat_Linea_Producto
    {
        public int id { get; set; }
        public Cat_SuperLinea cat_SuperLinea { get; set; }
        public int id_superlinea { get; set; }
        public string descripcion { get; set; }
        public bool estatus { get; set; }

        public List<Cat_SubLinea_Producto> cat_sublinea_producto { get; set; }
        public List<Cat_Productos> productos { get; set; }
    }

    public class Cat_SubLinea_Producto
    {
        public int id { get; set; }
        public Cat_Linea_Producto cat_linea_producto { get; set; }
        public int id_linea_producto { get; set; }
        public string descripcion { get; set; }
        public float hp_horas { get; set; }
        public bool estatus { get; set; }

        public List<condiones_comerciales_sucursal> condiones_comerciales_sucursal { get; set; }
        public List<Cat_Productos> productos { get; set; }
        public List<Rel_Categoria_Producto_Tipo_Producto> rel_categoria { get; set; }
        public List<Tecnicos_Producto> tecnicos_producto { get; set; }
    }

    
    public class Cat_Imagenes_Producto
    {
        public int id { get; set; }
        public int id_producto { get; set; }
        public Cat_Productos productos { get; set; }
        public string url { get; set; }
        public bool estatus { get; set; }
    }

    public class Cat_Sugeridos_Producto
    {
        public int id { get; set; }
        public int id_producto { get; set; }
        public Cat_Productos productos { get; set; }
        public string sku_sugerido { get; set; }
        public bool estatus { get; set; }
    }

    public class Caracteristica_Base
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public bool estatus { get; set; }

        public List<Cat_Productos> cat_Productos { get; set; }
    }


    public class Cat_Productos
    {
        public int id { get; set; }
        public string sku { get; set; }
        public string no_serie { get; set; }
        public string modelo { get; set; }
        public string nombre { get; set; }
        public string descripcion_corta { get; set; }
        public string descripcion_larga { get; set; }
        public string atributos { get; set; }
        public float precio_sin_iva { get; set; }
        public float precio_con_iva { get; set; }
        //public Cat_Categoria_Producto categoria { get; set; }
        public int id_categoria { get; set; }
        public Cat_Linea_Producto linea { get; set; }
        public int? id_linea { get; set; }
        public Cat_SubLinea_Producto sublinea { get; set; }
        public int? id_sublinea { get; set; }
        public string ficha_tecnica { get; set; }
        public int? horas_tecnico { get; set; }
        public int? no_tecnico { get; set; }
        public decimal? precio_hora { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public int tipo { get; set; } //  1 es producto, 2 es accesorio, 3 es consumibles, 4 es "no instalable" !!! HAY QUE HACER CATALGO  
        public string url_guia { get; set; }
        public string url_manual { get; set; }
        public string url_impresion { get; set; }
        public Caracteristica_Base caracteristica_Base { get; set; }
        public int id_caracteristica_base { get; set; }
        public Boolean requiere_instalacion { get; set; }
        public Boolean visible_partners { get; set; }

        public List<Cat_Imagenes_Producto> cat_imagenes_producto { get; set; }
        public List<Cat_Sugeridos_Producto> cat_sugeridos_producto { get; set; }
        public List<Cotizacion_Producto> Id_Cotizacion_Producto { get; set; }
        public List<Cliente_Productos> Id_Cliente_Productos { get; set; }
        public List<Productos_Carrito> Id_Carritos_Productos { get; set; }
        public List<Cat_Productos_Preguntas_Troubleshooting> cat_productos_preguntas_troubleshooting { get; set; }
        public List<Producto_Promocion> producto_promocion { get; set; }
        public List<Home_Producto_Estado> Productos_Estados { get; set; }
    }

    public class Home_Producto_Estado
    {
        public int id { get; set; }
        public int id_producto_home { get; set; }
        public int id_estado { get; set; }
        public Cat_Productos Producto { get; set; }
        public Cat_Estado Estado { get; set; }
    }

    public class Cat_Productos_Problema_Troubleshooting
    {
        public int id { get; set; }
        public string problema { get; set; }
        public string modelo { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
    }

    public class Cat_Productos_Preguntas_Troubleshooting
    {
        public int id { get; set; }
        public int id_problema { get; set; }
        public string pregunta { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
    }

    public class Cat_Productos_Respuestas_Troubleshooting
    {
        public int id { get; set; }
        public int id_pregunta { get; set; }
        public string falla { get; set; }
        public string solucion { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
    }

    public class Cat_Productos_Estatus_Troubleshooting
    {
        public int id { get; set; }
        public string desc_troubleshooting { get; set; }
        public string codigo { get; set; }
        public bool estatus { get; set; }

        public List<Servicio_Troubleshooting> servicio_troubleshooting { get; set; }
    }

    public class Servicio_Troubleshooting
    {
        public int id { get; set; }
        public Cat_Productos_Estatus_Troubleshooting estatus_troubleshooting { get; set; }
        public int id_estatus_troubleshooting { get; set; }
        public Servicio servicio { get; set; }
        public long id_servicio { get; set; }
        public string observciones { get; set; }
        public bool estatus { get; set; }
    }

    public class Check_List_Preguntas
    {
        public int id { get; set; }
        public string pregunta { get; set; }
        public string pregunta_en { get; set; }
        //public Check_List_Categoria_Producto categoria { get; set; }
        public int id_categoria { get; set; }

    }

    public class Producto_Check_List_Respuestas
    {
        public int id { get; set; }
        public int estatus { get; set; }
        public long id_vista { get; set; }
        public Visita visita { get; set; }
        public long id_producto { get; set; }

        public List<Check_List_Respuestas> check_list_respuestas { get; set; }
    }

    public class Check_List_Respuestas
    {
        public int id { get; set; }
        public int id_pregunta { get; set; }
        public bool respuesta { get; set; }
        public string comentarios { get; set; }
        public string comentarios_en { get; set; }
        public int id_producto_check_list_respuestas { get; set; }
        public Producto_Check_List_Respuestas producto_check_list_respuestas { get; set; }
    }

    //public class Check_List_Categoria_Producto
    //{
    //    public int id { get; set; }
    //    public string categoria { get; set; }
    //    public string categoria_en { get; set; }
    //    public string sku { get; set; }

    //    public List<Check_List_Preguntas> Preguntas { get; set; }
    //}

    public class Registro
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsComplete { get; set; }
    }

    public class Response
    {
        public string response { get; set; }
    }

    public class Token_Activo
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public long Id_user { get; set; }
        public string Fecha { get; set; }
    }

    //Procesar Excel Productos
    public class EntidadHojaExcel
    {
        public string descripcion { get; set; }
        public string estatus { get; set; }
        public string id_grupo_precio { get; set; }
        public string no_material { get; set; }
        public string cantidad { get; set; }
    }

    public class pendiente_partes {
        public string id { get; set; }
        public string IBS { get; set; }
        public string id_visita { get; set; }
        public string cliente { get; set; }
        public string desc_estatus_servicio { get; set; }
        public string fecha_visita { get; set; }
        public List<refacciones> refacciones { get; set; }
    }

    public class refacciones
    {
        public string id_material { get; set; }
        public string descripcion { get; set; }
    }

    //log refacciones 
    public class Log_refacciones
    {
        public int id { get; set; }
        public int id_usuario { get; set; }
        public int id_refaccion { get; set; }
        public string almacen_entrada { get; set; }
        public string almacen_salida { get; set; }
        public int cantidad { get; set; }
    }

    //Certificados Mantenimiento
    public class Cer_labor
    {
        public int id { get; set; }
        public int cantidad_equipos { get; set; }
        public decimal costo_base { get; set; }
        public decimal costo_unitario { get; set; }
        public decimal anual { get; set; }

        public List<Cer_producto_cliente> cer_producto_cliente { get; set; }
    }

    public class Cer_viaticos
    {
        public int id { get; set; }
        public Cat_Localidad localidad { get; set; }
        public int id_cat_localidad { get; set; }
        public decimal costo_unitario { get; set; }
        public decimal anual { get; set; }

        public List<Cer_producto_cliente> cer_producto_cliente { get; set; }
    }

    public class Cer_consumibles
    {
        public int id { get; set; }
        public string consumible { get; set; }
        public decimal costo_unitario { get; set; }

        public List<rel_certificado_producto_consumibles> cer_producto_cliente { get; set; }
        public List<rel_consumible_sublinea> rel_consumible_sublinea { get; set; }
    }
    
    public class rel_consumible_sublinea
    {
        public int id { get; set; }
        public int id_consumible { get; set; }
        public Cer_consumibles consumible { get; set; }
        public int id_sublinea { get; set; }
        
    }

    public class Home_producto_cliente
    {
        public int id { get; set; }
        public string folio { get; set; }
        public long id_cliente { get; set; }
        public int no_visitas { get; set; }
        public int costo { get; set; }
        public int horas { get; set; }
        public int? id_cotizacion { get; set; }
        public bool estatus_activo { get; set; }
        public bool estatus_venta { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public int id_producto { get; set; }

        public List<rel_homep_producto> homep_Productos { get; set; }
    }

    public class Cer_producto_cliente
    {
        public int id { get; set; }
        public string folio { get; set; }
        public long id_cliente { get; set; }
        public int id_viaticos { get; set; }
        public Cer_viaticos viaticos { get; set; }
        public int id_labor { get; set; }
        public Cer_labor labor { get; set; }
        public int costo { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public bool estatus_venta { get; set; }
        public long id_cotizacion { get; set; }

        public List<rel_certificado_producto> rel_certificado_producto { get; set; }
    }

    public class Cer_producto_carrito
    {
        public int id { get; set; }
        public string folio { get; set; }
        public long id_carrito { get; set; }
        public int id_viaticos { get; set; }
        public int id_labor { get; set; }
        public int costo { get; set; }
    }

    public class rel_certificado_carrito
    {
        public int id { get; set; }
        public int id_carrito { get; set; }
        public int id_producto { get; set; }
        public bool estatus_activo { get; set; }
        public int no_visitas { get; set; }
        public int id_sub_linea { get; set; }
    }

    public class Home_producto_carrito
    {
        public int id { get; set; }
        public long id_carrito { get; set; }
        public int no_visitas { get; set; }
        public int costo { get; set; }
        public int horas { get; set; }
        public bool estatus_activo { get; set; }
        public bool estatus_venta { get; set; }
    }

    public class rel_homep_producto
    {
        public int id { get; set; }
        public int id_homep { get; set; }
        public Home_producto_cliente homep { get; set; }
        public int id_producto { get; set; }
        public bool estatus_activo { get; set; }
        public int cantidad { get; set; }
        public int id_sub_linea { get; set; }
        public DateTime creado { get; set; }
        public DateTime fecha_visita_1 { get; set; }
        public DateTime fecha_visita_2 { get; set; }

    }

    public class rel_certificado_producto
    {
        public int id { get; set; }
        public int id_certificado { get; set; }
        public Cer_producto_cliente certificado { get; set; }
        public int id_producto { get; set; }
        public bool estatus_activo { get; set; }
        public int no_visitas { get; set; }
        public int id_sub_linea { get; set; }
        public DateTime creado { get; set; }
        public DateTime fecha_visita_1 { get; set; }
        public DateTime fecha_visita_2 { get; set; }

        public List<rel_certificado_producto_consumibles> rel_certificado_producto_consumibles { get; set; }
    }

    public class rel_certificado_producto_consumibles
    {
        public int id { get; set; }
        public int id_rel_certificado_producto { get; set; }
        public rel_certificado_producto rel_certificado_producto { get; set; }
        public int id_consumible { get; set; }
        public Cer_consumibles consumible { get; set; }
    }

    public class quejas_servicios
    {
        public int id { get; set; }
        public int id_queja { get; set; }
        public long id_servicio { get; set; }
        public Servicio Servicio { get; set; }
    }

    public class encuesta_general
    {
        public int id { get; set; }
        public string pregunta_1 { get; set; }
        public string pregunta_2 { get; set; }
        public string pregunta_3 { get; set; }
        public string pregunta_4 { get; set; }
        public string pregunta_5 { get; set; }
        public string pregunta_6 { get; set; }
        public string pregunta_7 { get; set; }
        public string pregunta_8 { get; set; }
        public string pregunta_9 { get; set; }
        public string pregunta_10 { get; set; }
        public string pregunta_11 { get; set; }
        public string pregunta_12 { get; set; }
        public string pregunta_13 { get; set; }
        public string pregunta_14 { get; set; }
        public string pregunta_15 { get; set; }
        public long id_servicio { get; set; }
        public Servicio Servicio { get; set; }
        public long id_cliente { get; set; }
        public int estatus_encuesta { get; set; }
        public bool estatus_activo { get; set; }
        public int intentos { get; set; }
        public string folio { get; set; }
        public DateTime fecha { get; set; }
    }

    public class encuesta_queja
    {
        public int id { get; set; }
        public string pregunta_1 { get; set; }
        public string pregunta_2 { get; set; }
        public string pregunta_3 { get; set; }
        public string pregunta_4 { get; set; }
        public string pregunta_5 { get; set; }
        public string pregunta_6 { get; set; }
        public string pregunta_7 { get; set; }
        public string pregunta_8 { get; set; }
        public string pregunta_9 { get; set; }
        public string pregunta_10 { get; set; }
        public string pregunta_11 { get; set; }
        public int id_queja { get; set; }
        public long id_cliente { get; set; }
        public int estatus_encuesta { get; set; }
        public bool estatus_activo { get; set; }
        public int intentos { get; set; }
        public string folio { get; set; }
        public DateTime fecha { get; set; }
    }
}
