using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Models
{
    public class Partners_Cotizacion
    {


    }

    public class Cotizaciones
    {
        public Cotizaciones()
        {
            Id_Cotizacion_Producto = new List<Cotizacion_Producto>();
            cotizacion_promocion = new List<Cotizacion_Promocion>();
            producto_promocion = new List<Producto_Promocion>();
            cotizacion_monto_descuento = new List<cotizacion_monto_descuento>();
            direcciones_cotizacion = new List<Direccion_Cotizacion>();
        }

        public long Id { get; set; }
        public string Numero { get; set; }
        public long Id_Cliente { get; set; }
        public long Id_Vendedor { get; set; }
        public long referencia { get; set; }
        public DateTime vigenica_ref { get; set; }
        public DateTime fecha_cotiza { get; set; }
        public float importe_condiciones_com { get; set; }
        public float importe_precio_lista  { get; set; }
        public float importe_promociones { get; set; }
        public float descuento_acumulado { get; set; }
        public float descuento_acumulado_cond_com { get; set; }
        public float comision_vendedor { get; set; }
        public float comision_sucrusal { get; set; }
        public float iva_precio_lista { get; set; }
        public float iva_condiciones_com { get; set; }
        public float iva_promociones { get; set; }
        public int Estatus { get; set; }
        public int Acciones { get; set; }
        public int puede_solicitar_env { get; set; }
        public long Id_Canal { get; set; }
        public Cat_canales Canal { get; set; }
        public long Id_Cuenta { get; set; }
        public int Id_Estado_Instalacion { get; set; }
        public string Observaciones { get; set; }
        public int creadopor { get; set; }
        public int usr_modifico { get; set; }
        public int id_formapago { get; set; }
        public string ibs { get; set; }
        public string motivo_rechazo { get; set; }
        public bool rechazada { get; set; }
        public long id_cotizacion_padre { get; set; }
        public bool id_user_entrega_sol { get; set; }
        public bool entrega_sol { get; set; }
        public bool requiere_fact { get; set; }
        public bool cancelada { get; set; }
        public int Id_sucursal { get; set; }
        public string coment_cancel { get; set; }
        public bool acepto_terminos_condiciones { get; set; } 
        public int numero_productos { get; set; }
        public DateTime cambio_ord_comp_generada { get; set; }
        public int id_tarjeta { get; set; }

        public List<Cotizacion_Producto> Id_Cotizacion_Producto { get; set; }
        public List<Cotizacion_Promocion> cotizacion_promocion { get; set; }
        public List<Producto_Promocion> producto_promocion { get; set; }
        public List<cotizacion_monto_descuento> cotizacion_monto_descuento { get; set; }
        public List<comisiones_vendedores> comisiones_vendedores { get; set; }
        public List<comisiones_sucursales> comisiones_sucursales { get; set; }

        public List<Direccion_Cotizacion> direcciones_cotizacion { get; set; } // Relación 1:N
        //public List<Cat_Tarjeta> cat_Tarjetas { get; set; }
    }

    public class Cat_Tarjeta
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public bool estatus { get; set; }

        //public Cotizaciones Cotizacion { get; set; }
    }

    public class Cat_Estatus_Cotizacion
    {
        public int id { get; set; }
        public string Estatus_es { get; set; }
        public string Estatus_en { get; set; }
    }


    public class comisiones_vendedores
    {
        public int id { get; set; }
        public Cotizaciones cotizaciones { get; set; }
        public long id_cotizacion { get; set; }
        public cat_tipos_comisiones cat_tipos_comisiones { get; set; }
        public int id_cat_tipo_comision { get; set; }
        public DateTime fecha_generacion { get; set; }
        public DateTime pago_programado { get; set; }
        public DateTime fecha_de_pago { get; set; }
        public bool pagada { get; set; }
        public long id_quienpago { get; set; }
        public Users Users { get; set; }
        public decimal monto_comision { get; set; }
        public decimal monto_com_sin_iva { get; set; }
    }

    public class comisiones_sucursales
    {
        public int id { get; set; }
        public Cotizaciones cotizaciones { get; set; }
        public long id_cotizacion { get; set; }
        public cat_tipos_comisiones cat_tipos_comisiones { get; set; }
        public int id_cat_tipo_comision { get; set; }
        public DateTime fecha_generacion { get; set; }
        public DateTime pago_programado { get; set; }
        public DateTime fecha_de_pago { get; set; }
        public bool pagada { get; set; }
        public long id_quienpago { get; set; }
    }

    public class cat_tipos_comisiones
    {
        public int id { get; set; }
        public string tipo_comision { get; set; }
        public List<comisiones_vendedores> comisiones_vendedores { get; set; }
        public List<comisiones_sucursales> comisiones_sucursales { get; set; }
    }

    public class Permisos_Flujo_Cotizacion
    {
        public int id { get; set; }
        public string permiso { get; set; }
        public bool visible { get; set; }
        public bool inhabilitado { get; set; }
        public long id_rol { get; set; }
        public int id_estatus_cotizacion { get; set; }
        public long id_Canal { get; set; }
        public int id_tabla { get; set; } // 1 creación cotizacion, 2 edición cotización
    }
public class Cotizacion_Producto
    {
        public long Id { get; set; }
        public long Id_Cotizacion { get; set; }
        public Cotizaciones Cotizacion { get; set; }
        public int Id_Producto { get; set; }
        public Cat_Productos Producto { get; set; }
        public int cantidad { get; set; }
        public float precio_lista { get; set; }
        public float iva_precio_lista { get; set; }
        public float precio_descuento { get; set; }
        public float iva_precio_descuento { get; set; }
        public float precio_condiciones_com { get; set; }
        public float iva_cond_comerciales { get; set; }
        public float margen_cc { get; set; }
        public bool es_regalo { get; set; }
        public bool agregado_automaticamente { get; set; }
    }

    public class Producto_Promocion
    {
        public int id { get; set; }
        public int id_promocion { get; set; }
        public promocion promocion { get; set; }
        public int id_producto { get; set; }
        public Cat_Productos Cat_Productos { get; set; }
        public long id_cotizacion { get; set; }
        public Cotizaciones cotizaciones { get; set; }
    }

    public class com_producto_promocion
    {
        public int id { get; set; }
        public int id_comisionv { get; set; }
        public config_comision config_comisiones { get; set; }
        public int id_producto { get; set; }
        public Cat_Productos Cat_Productos { get; set; }
        
    }



    public class Cotizacion_Promocion
    {
        public int id { get; set; }
        public int id_promocion { get; set; }
        public promocion promocion { get; set; }
        public long id_cotizacion { get; set; }
        public Cotizaciones Cotizaciones { get; set; }
    }

    public class cotizacion_monto_descuento
    {
        public int id { get; set; }
        public int id_promocion { get; set; }
        public promocion promocion { get; set; }
        public long id_cotizacion { get; set; }
        public Cotizaciones Cotizaciones { get; set; }
        public float monto_desc_sin_iva { get; set; }
        public float monto_desc_con_iva { get; set; }
    }


    public class documentos_cotizacion
    {
        public long Id { get; set; }
        public long Id_Cotizacion { get; set; }
        public string Id_foto { get; set; }
        public int tipo_docto { get; set; } // 1 foto 2 orden c.
        public int id_tipo_tipo_pago { get; set; }
        public int id_forma_pago { get; set;}
        public long id_user { get; set; }
        public DateTime fecha_subida { get; set; }
    }

    public class tipos_comprobantes
    {
        public int id { get; set; }
        public string tipo_pago { get; set; }
        public bool es_liquidacion { get; set; }
    }

    public class formas_pago_tipos_comprobantes
    {
        public int id { get; set; }
        public int id_Cat_Formas_Pago { get; set; }
        public int id_tipo_comprobantes { get; set; }
    }

    public class Cat_Cuentas
    {
        public long Id { get; set; }
        public string Cuenta_es { get; set; }
        public string Cuenta_en { get; set; }
        public long Id_Canal { get; set; }
        public Cat_canales Canal { get; set; }
        public List<Cat_Sucursales> Id_Cuenta_Sucursal { get; set; }
        public List<Cat_CondicionesPago> cat_Formas_Pago { get; set; }
    }

    public class Cat_Sucursales
    {
        public int Id { get; set; }
        public string Sucursal { get; set; }
        public long Id_Cuenta { get; set; }
        public string url_logo { get; set; }
        public float margen_vendedores { get; set; }
        public string tipo { get; set; }
        public Cat_Cuentas Cuenta { get; set; }
        public List<DatosFiscales_Sucursales> DatosFiscales_Sucursales { get; set; }
        public List<condiones_comerciales_sucursal> condiones_comerciales_sucursal { get; set; }
        public List<Cat_Direccion_sucursales> cat_direccion_sucursales { get; set; }
    }

    public class Cliente_Productos
    {
        public long Id { get; set; }
        public long Id_Cliente { get; set; }
        public Clientes Cliente { get; set; }
        public long Id_Producto { get; set; }
        public DateTime FinGarantia { get; set; }
        public DateTime FechaCompra { get; set; }
        public string NoPoliza { get; set; }
        public long Id_EsatusCompra { get; set; }
        public string NoOrdenCompra { get; set; }
        public int id_visita { get; set; }
        public int no_producto { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public int id_cotizacion { get; set; }
        public string no_serie { get; set; }

    }


    public class Cat_Estatus_Compra
    {
        public long Id { get; set; }
        public string Estatus_es { get; set; }
        public string Estatus_en { get; set; }
        public List<Cliente_Productos> Id_Cliente_Productos { get; set; }
    }


    public class Cat_canales
    {
        public long Id { get; set; }
        public string Canal_es { get; set; }
        public string Canal_en { get; set; }

        public List<Cotizaciones> Id_Canal_Cotizacion { get; set; }
        public List<Cat_Cuentas> Id_Canal_Cuenta { get; set; }
    }

    public class Vendedores
    {
        public long Id { get; set; }
        public string nombre { get; set; }
        public string paterno { get; set; }
        public string materno { get; set; }
        public string nombre_comercial { get; set; }
        public string nombre_contacto { get; set; }
        public string email { get; set; }
        public string calle_numero { get; set; }
        public string CP { get; set; }
        public int id_estado { get; set; }
        public Cat_Estado estado { get; set; }
        public int id_municipio { get; set; }
        public Cat_Municipio municipio { get; set; }
        public string colonia { get; set; }
        public string telefono { get; set; }
        public string telefono_movil { get; set; }
        public string referencias { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }


    }

    public class Cat_Accesorios
    {
        public int id { get; set; }
        public string sku { get; set; }
        public string modelo { get; set; }
        public string nombre { get; set; }
        public string descripcion_corta { get; set; }
        public string descripcion_larga { get; set; }
        public string atributos { get; set; }
        public string precio_sin_iva { get; set; }
        public string precio_con_iva { get; set; }
        public string ficha_tecnica { get; set; }
        public bool estatus { get; set; }
        public DateTime creado { get; set; }
        public long creadopor { get; set; }
        public DateTime actualizado { get; set; }
        public long actualizadopor { get; set; }
        public List<Cat_Imagenes_Accesosrios> cat_imagenes_accesorio { get; set; }
        public List<Cat_Accesorios_Relacionados> Cat_Accesorios_Relacionados { get; set; }
        //public List<Cotizacion_Producto> Id_Cotizacion_Producto { get; set; }
        //public List<Cliente_Productos> Id_Cliente_Productos { get; set; }
    }

    public class Cat_Imagenes_Accesosrios
    {
        public int id { get; set; }
        public int id_Accesorio { get; set; }
        public Cat_Accesorios accesorios { get; set; }
        public string url { get; set; }
        public bool estatus { get; set; }
    }

    public class FormasPago_Cotizaciones
    {
        public int id_formaPago { get; set; }
        public int id_canal { get; set; }
    }

    public class Cat_Accesorios_Relacionados
    {
        public int id { get; set; }
        public int id_Accesorio { get; set; }
        public Cat_Accesorios accesorios { get; set; }
        public string sku_sugerido { get; set; }
        public bool estatus { get; set; }
    }

    public class Productos_Carrito
    {
        public long Id { get; set; }
        public int Id_Producto { get; set; }
        public Cat_Productos Productos { get; set; }
        public int id_usuario { get; set; }
        public int cantidad { get; set; }
        public DateTime fecha_creacion { get; set; }
        public float precio_condiciones_com { get; set; }
        public float iva_cond_comerciales { get; set; }
        public float precio_descuento { get; set; }
        public float iva_precio_descuento { get; set; }
        public float precio_lista { get; set; }
        public float iva_precio_lista { get; set; }
        public float margen_cc { get; set; }
        public float agregado_automaticamente { get; set; }
        public float es_regalo { get; set; }
    }

    public class sublinea_certificado_partners
    {
        public int Id { get; set; }
        public int Id_producto_carrito { get; set; } // id_usuario
        public int Id_cotizacion_producto { get; set; } // siempre 0 
        public int Id_sublinea { get; set; }
    }

    public class producto_certificado_sublinea
    {
        public int Id { get; set; }
        public int id_producto { get; set; } // el id del certificado
        public int Id_sublinea { get; set; } // un mismo producto tiene n sublienas asociadas
    }

    public class Productos_Cotizacion
    {
        public long Id { get; set; }
        public int Id_Producto { get; set; }
        public Cat_Productos Productos { get; set; }
        public int Id_Cotizacion { get; set; }
        public int cantidad { get; set; }
        public DateTime fecha_creacion { get; set; }

    }


    public class Cat_Formas_Pago
    {
        public int id { get; set; }
        public string FormaPago { get; set; }
        public bool comprobantes_obligatorios { get; set; }
        public List<Cat_CondicionesPago> cat_condicionespago { get; set; }
    }

    public class Cat_CondicionesPago
    {
        public int id { get; set; }
        public int id_Cat_Formas_Pago { get; set; }
        public Cat_Formas_Pago Cat_Formas_Pago { get; set; }
        public long id_cuenta { get; set; }
        public Cat_Cuentas Cat_Cuentas { get; set; }
    }

    public class CondicionesComerciales_Cuenta
    {
        public int id { get; set; }
        public int id_condicion { get; set; }
        public int id_cuenta { get; set; }
        public DateTime Vigencia_inicial { get; set; }
        public DateTime Vigencia_final { get; set; }
    }

    public class Cat_CondicionesComerciales
    {
        public int id { get; set; }
        public string condicion_comercial { get; set; }
        public string tipo { get; set; }
        public float porcentaje_descuento { get; set; }
        public float monto_descuento { get; set; }
        public float meses_credito { get; set; }
        public float num_meses_sinint { get; set; }
        public float porcentaje_credito { get; set; }
        public bool activa { get; set; }
        
    }

    public class ResultadosBuscador
    {
        public int id { get; set; }
        public string tipo { get; set; }
        public string resultado { get; set; }
        public int cantidad { get; set; }
        public string nombre
        {
            get
            {
                return resultado;// + " | " + tipo + " (" + cantidad.ToString() + ")";
            }
            set
            {
                nombre = value;
            }
        }
    }

    public class accesorios_relacionados
    {
        public int id { get; set; }
        public int id_producto_padre { get; set; }
        public int id_producto_recomendado { get; set; }
    }
    public class config_comision
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string fecha_hora_inicio { get; set; }
        public string fecha_hora_fin { get; set; }
        public bool vigencia_indefinida { get; set; }
        public int id_tipos_herencia_promo { get; set; }
        public cat_tipos_herencia cat_tipos_herencia { get; set; }
        public int id_cat_tipo_condicion { get; set; }
        public cat_tipo_condicion cat_tipo_condicion { get; set; }
        public decimal monto_condicion { get; set; }
        public decimal monto_superior { get; set; }
        public bool aplica_comision { get; set; }
        public List<com_entidades_participantes> entidades_participantes { get; set; }
        public List<com_productos_condicion> productos_condicion { get; set; }
        public List<com_afectacion_cc> afectacion_cc { get; set; }
        public List<com_producto_promocion> producto_promocion { get; set; }




        //com_entidades_ // entidades participantes 
        //         public List<productos_condicion> productos_condicion { get; set; }

        //        public List<afectacion_cc> afectacion_cc { get; set; }

    }
    ////////////////////////////////// P  R  O  M  O  C  I  O  N  E  S  ////////////////////////////////////

    public class promocion
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string fecha_hora_inicio { get; set; }
        public string fecha_hora_fin { get; set; }
        public bool vigencia_indefinida { get; set; }
        public int id_tipos_herencia_promo { get; set; }
        public cat_tipos_herencia tipos_herencia_promo { get; set; }
        public int id_cat_tipo_condicion { get; set; }
        public cat_tipo_condicion cat_tipo_condicion { get; set; }
        public decimal monto_inferior_condicion { get; set; }
        public decimal monto_condicion { get; set; }
        public bool incluir_desc_adic { get; set; }
        public bool beneficio_obligatorio { get; set; }
        public int id_tipo_beneficio { get; set; }
        public bool aplica_cc { get; set; }
        public bool aplica_comision_config { get; set; }

        public List<entidades_participantes> entidades_participantes { get; set; }
        public List<entidades_excluidas> entidades_excluidas { get; set; }
        public List<productos_condicion> productos_condicion { get; set; }
        public List<beneficios_promocion> beneficios_promocion { get; set; }
        public List<beneficio_desc> beneficio_desc { get; set; }
        public List<beneficio_productos> beneficio_productos { get; set; }
        public List<beneficio_msi> beneficio_msi { get; set; }
        public List<afectacion_cc> afectacion_cc { get; set; }
        public List<productos_excluidos> productos_excluidos { get; set; }
        public List<entidades_obligatorias> entidades_obligatorias { get; set; }
        public List<promociones_compatibles> promociones_compatibles { get; set; }
        public List<Cotizacion_Promocion> cotizacion_promocion { get; set; }
        public List<cotizacion_monto_descuento> cotizacion_monto_descuento { get; set; }
        public List<Producto_Promocion> producto_promocion { get; set; }
        public List<comision_promo_sublinea_config> Comisiones_promociones { get; set; }

    }

    public class cat_tipos_herencia
    {
        public int id { get; set; }
        public string tipo { get; set; }
        public List<promocion> promocion { get; set; }
        public List<config_comision> config_comisiones{get; set; }
    }

    public class cat_tipo_entidades
    {
        public int id { get; set; }
        public string tipo_entidad { get; set; }

        public List<entidades_participantes> entidades_participantes { get; set; }
        public List<com_entidades_participantes> com_entidades_participantes { get; set; }
        public List<entidades_excluidas> entidades_excluidas { get; set; }
        public List<entidades_obligatorias> entidades_obligatorias { get; set; }
    }

    public class entidades_participantes
    {
        public int id { get; set; }
        public int id_promocion { get; set; }//promocion
        public promocion promocion { get; set; }
        public int id_entidad { get; set; }
        public int id_tipo_entidad { get; set; } //cat_tipo_entidades
        public cat_tipo_entidades cat_tipo_entidades { get; set; }
    }
    public class com_entidades_participantes
    {
        public int id { get; set; }
        public int id_comisionv { get; set; }//promocion
        public config_comision config_comisiones { get; set; }
        public int id_entidad { get; set; }
        public int id_tipo_entidad { get; set; } //cat_tipo_entidades
        public cat_tipo_entidades cat_tipo_entidades { get; set; }
    }

    public class entidades_excluidas
    {
        public int id { get; set; }
        public int id_promocion { get; set; } //promocion
        public promocion promocion { get; set; }
        public int id_entidad { get; set; }
        public int id_tipo_entidad { get; set; } //cat_tipo_entidades
        public cat_tipo_entidades cat_tipo_entidades { get; set; }
    }

    public class entidades_obligatorias
    {
        public int id { get; set; }
        public int id_promocion { get; set; } //promocion
        public promocion promocion { get; set; }
        public int id_entidad { get; set; }
        public int id_tipo_entidad { get; set; } //cat_tipo_entidades
        public cat_tipo_entidades cat_tipo_entidades { get; set; }
    }

    public class productos_excluidos
    {
        public int id { get; set; }
        public int id_promocion { get; set; } //promocion
        public promocion promocion { get; set; }
        public int id_producto { get; set; } // productos, sublinea, linea
        public int id_tipo_categoria { get; set; } // 1 Línea , 2 Sublínea , 3 producto 
    }

    public class promociones_compatibles
    {
        public int id { get; set; }
        public int id_promocion { get; set; } //promocion
        public promocion promocion { get; set; }
        public int id_promocion_2 { get; set; } //promocion
        //public promocion promocion2 { get; set; }
    }

    public class cat_tipo_condicion
    {
        public int id { get; set; }
        public string tipo_condicion { get; set; }
        public List<promocion> promocion { get; set; }
        public List<config_comision> config_comisiones { set; get;  }
    }

    public class productos_condicion
    {
        public int id { get; set; }
        public int id_promocion { get; set; } //promocion
        public promocion promocion { get; set; }
        public int id_producto { get; set; } // productos, sublinea, linea
        public int id_tipo_categoria { get; set; } // 1 Línea , 2 Sublínea , 3 producto 
        public int cantidad { get; set; }
    }

    public class com_productos_condicion
    {
        public int id { get; set; }
        public int id_comisionv { get; set; } //promocion
        public config_comision config_comisiones { get; set; }
        public int id_producto { get; set; } // productos, sublinea, linea
        public int id_tipo_categoria { get; set; } // 1 Línea , 2 Sublínea , 3 producto 
        public int cantidad { get; set; }
    }

    public class cat_beneficios
    {
        public int id { get; set; }
        public string beneficio { get; set; }
        public List<beneficios_promocion> beneficios_promocion { get; set; }
    }

    public class beneficios_promocion
    {
        public int id { get; set; }
        public int id_promocion { get; set; } //promocion
        public promocion promocion { get; set; }
        public int id_cat_beneficios { get; set; } //cat_beneficios
        public cat_beneficios cat_beneficios { get; set; }
    }

    public class beneficio_desc
    {
        public int id { get; set; }
        public int id_promocion { get; set; } //promocion
        public promocion promocion { get; set; }
        public int cantidad { get; set; }
        public bool es_porcentaje { get; set; }
    }

    public class beneficio_productos
    {
        public int id { get; set; }
        public int id_promocion { get; set; } //promocion
        public promocion promocion { get; set; }
        public int id_producto { get; set; } //productos
        public Cat_Productos Cat_Producto;
        public int cantidad { get; set; }
    }

    public class beneficio_msi
    {
        public int id { get; set; }
        public int id_promocion { get; set; } //promocion
        public promocion promocion { get; set; }
        public int id_cat_msi { get; set; } //cat_msi
        public cat_msi cat_msi { get; set; }
    }

    public class cat_msi
    {
        public int id { get; set; }
        public string desc_msi { get; set; }
        public List<beneficio_msi> beneficio_msi { get; set; }
    }

    public class afectacion_cc
    {
        public int id { get; set; }
        public int id_promocion { get; set; }
        public promocion promocion { get; set; }
        public int id_condiones_comerciales_sucursal { get; set; }
        public condiones_comerciales_sucursal condiones_comerciales_sucursal { get; set; }
        public int margen { get; set; }
    }

    public class com_afectacion_cc
    {
        public int id { get; set; }
        public int id_comisionv { get; set; }
        public int id_condiones_comerciales_sucursal { get; set; }
        public config_comision config_comisiones { get; set;  }
        public condiones_comerciales_sucursal condiones_comerciales_sucursal { get; set; }
        public float margen { get; set; }
    }

    public class productos_relacionados
    {
        public int id { get; set; }
        public int id_producto { get; set; }
        public int id_producto_2 { get; set; }
    }

    public class cat_tipo_productos
    {
        public int id { get; set; }
        public string tipo { get; set; }
    }

    public class promociones_aplicables
    {
        public int id { get; set; }
        public int id_promocion { get; set; }
        public int id_cotizacion { get; set; }
    }

    ////////////////////////////// C O N D I C I O N E S   C O M E R C I A L E S /////////////////////////////// 


    public class condiones_comerciales_canal
    {
        public int id { get; set; }
        public int condicion { get; set; }
        public float margen { get; set; }
        public int id_Cat_SubLinea_Producto { get; set; }
        public Cat_SubLinea_Producto Cat_SubLinea_Producto { get; set; }
        public int id_Cat_canales { get; set; }
        public Cat_canales Cat_canales { get; set; }
    }

    public class condiones_comerciales_cuenta
    {
        public int id { get; set; }
        public int condicion { get; set; }
        public float margen { get; set; }
        public int id_Cat_SubLinea_Producto { get; set; }
        public Cat_SubLinea_Producto Cat_SubLinea_Producto { get; set; }
        public int id_Cat_Cuentas { get; set; }
        public Cat_Cuentas Cat_Cuentas { get; set; }
    }

    public class comision_promo_sublinea_config
    {
        public int id { get; set; }
        public int id_cc_sucursal { get; set; }
        //public int id_sublinea { get; set; }
        public float margen { get; set; }
        //public bool activo { get; set; }
        public int id_promocion { get; set; }
        public promocion Promocion { get; set; }
        public condiones_comerciales_sucursal condiciones_promos { get; set; }

    }

    public class condiones_comerciales_sucursal
    {
        public int id { get; set; }
        public float margen { get; set; }
        public int id_Cat_SubLinea_Producto { get; set; }
        public Cat_SubLinea_Producto Cat_SubLinea_Producto { get; set; }
        public int id_Cat_Sucursales { get; set; }
        public Cat_Sucursales Cat_Sucursales { get; set; }
        //public List<comision_promo_sublinea_config> Comisiones_promos { get; set; }
        public List<comision_promo_sublinea_config> comision_Promos { get; set; }
        public List<afectacion_cc> afectacion_cc { get; set; }
        public List<com_afectacion_cc> com_afectacion_cc { get; set; } 
    }
}
