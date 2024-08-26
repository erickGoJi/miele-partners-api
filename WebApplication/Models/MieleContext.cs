using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models.Dashboard;

namespace WebApplication.Models
{
    public class MieleContext : DbContext
    {
        public MieleContext(DbContextOptions<MieleContext> options)
            : base(options) { }

        public DbSet<Registro> RegistroItems { get; set; }
        public DbSet<Token_Activo> TokenItems { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Tecnicos> Tecnicos { get; set; }
        public DbSet<Cat_Actividad> Cat_Actividad { get; set; }
        public DbSet<Tecnicos_Actividad> Tecnicos_Actividad { get; set; }
        public DbSet<Cat_Area_Cobertura> Cat_Area_Cobertura { get; set; }
        public DbSet<Tecnicos_Cobertura> Tecnicos_Cobertura { get; set; }
        public DbSet<Cat_Producto> Cat_Producto { get; set; }
        public DbSet<Tecnicos_Producto> Tecnicos_Producto { get; set; }
        public DbSet<Cat_Tecnicos_Tipo> Cat_Tecnicos_Tipo { get; set; }
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Cat_Estado> Cat_Estado { get; set; }
        public DbSet<Cat_Municipio> Cat_Municipio { get; set; }
        public DbSet<Cat_Localidad> Cat_Localidad { get; set; }
        public DbSet<Cat_Lista_Precios> Cat_Lista_Precios { get; set; }
        public DbSet<Cat_Materiales> Cat_Materiales { get; set; }
        public DbSet<Cat_Materiales_Tecnico> Cat_Materiales_Tecnico { get; set; }
        public DbSet<Cat_Categoria_Producto> Cat_Categoria_Producto { get; set; }
        public DbSet<Cat_Linea_Producto> Cat_Linea_Producto { get; set; }
        public DbSet<Cat_SubLinea_Producto> Cat_SubLinea_Producto { get; set; }
        public DbSet<Cat_Imagenes_Producto> Cat_Imagenes_Producto { get; set; }
        public DbSet<Cat_Sugeridos_Producto> Cat_Sugeridos_Producto { get; set; }
        public DbSet<Cat_Productos> Cat_Productos { get; set; }
        public DbSet<Cat_tipo_servicio> Cat_tipo_servicio { get; set; }
        public DbSet<Cat_solicitado_por> Cat_solicitado_por { get; set; }
        public DbSet<Cat_distribuidor_autorizado> Cat_distribuidor_autorizado { get; set; }
        public DbSet<Cat_solicitud_via> Cat_solicitud_via { get; set; }
        public DbSet<Cat_Categoria_Servicio> Cat_Categoria_Servicio { get; set; }
        public DbSet<Cat_estatus_servicio> Cat_estatus_servicio { get; set; }
        public DbSet<CatEstatus_Visita> CatEstatus_Visita { get; set; }
        public DbSet<CatEstatus_Producto> CatEstatus_Producto { get; set; }
        public DbSet<Cat_Direccion> Cat_Direccion { get; set; }
        public DbSet<Direcciones_Cliente> Direcciones_Cliente { get; set; }
        public DbSet<Rel_servicio_visita_producto> Rel_servicio_producto { get; set; }
        public DbSet<Rel_servicio_visita_Refaccion> Rel_servicio_Refaccion { get; set; }
        public DbSet<Piezas_Repuesto> Piezas_Repuesto { get; set; }
        public DbSet<Piezas_Repuesto_Tecnico> Piezas_Repuesto_Tecnico { get; set; }
        public DbSet<Servicio> Servicio { get; set; }
        public DbSet<His_Servicio_Estatus> historial_estatus { get; set; }
        public DbSet<Cat_Roles> Cat_Roles { get; set; }
        public DbSet<Cat_Aplicaciones> Cat_Aplicaciones { get; set; }
        public DbSet<Visita> Visita { get; set; }
        public DbSet<Check_List_Preguntas> Check_List_Preguntas { get; set; }
        public DbSet<Check_List_Respuestas> Check_List_Respuestas { get; set; }
        //public DbSet<Check_List_Categoria_Producto> Check_List_Categoria_Producto { get; set; }
        public DbSet<Cotizaciones> Cotizaciones { get; set; }
        public DbSet<Cotizacion_Producto> Cotizacion_Producto { get; set; }
        public DbSet<Cat_canales> Cat_canales { get; set; }
        public DbSet<Vendedores> Vendedores { get; set; }
        public DbSet<Cat_Cuentas> Cat_Cuentas { get; set; }
        public DbSet<Cat_Sucursales> Cat_Sucursales { get; set; }
        public DbSet<Cliente_Productos> Cliente_Productos { get; set; }
        public DbSet<Cat_Estatus_Compra> Cat_Estatus_Compra { get; set; }
        public DbSet<Cat_Estatus_Cotizacion> Cat_Estatus_Cotizacion { get; set; }
        public DbSet<DatosFiscales> DatosFiscales { get; set; }
        public DbSet<Cat_Accesorios> Cat_Accesorios { get; set; }
        public DbSet<Cat_Imagenes_Accesosrios> Cat_Imagenes_Accesosrios { get; set; }
        public DbSet<Cat_Accesorios_Relacionados> Cat_Accesorios_Relacionados { get; set; }
        public DbSet<Rel_Imagen_Producto_Visita> Rel_Imagen_Producto_Visita { get; set; }
        public DbSet<Productos_Carrito> Productos_Carrito { get; set; }
        public DbSet<Cat_Formas_Pago> Cat_Formas_Pago { get; set; }
        public DbSet<Cat_CondicionesPago> Cat_CondicionesPago { get; set; }
        public DbSet<documentos_cotizacion> documentos_cotizacion { get; set; }
        public DbSet<accesorios_relacionados> AccesoriosRelacionados { get; set; }
        public DbSet<CondicionesComerciales_Cuenta> CondicionesComerciales_Cuenta { get; set; }
        public DbSet<Cat_CondicionesComerciales> Cat_CondicionesComerciales { get; set; }
        public DbSet<DatosFiscales_Canales> DatosFiscales_Canales { get; set; }
        public DbSet<DatosFiscales_Sucursales> DatosFiscales_Sucursales { get; set; }
        public DbSet<tipos_comprobantes> tipos_comprobantes { get; set; }
        public DbSet<Producto_Check_List_Respuestas> Producto_Check_List_Respuestas { get; set; }
        public DbSet<cat_tipo_refaccion> Cat_Tipo_Refaccion { get; set; }
        public DbSet<cat_checklist_producto> Cat_Checklist_Producto { get; set; }
        public DbSet<Cat_Productos_Problema_Troubleshooting> Cat_Productos_Problema_Troubleshooting { get; set; }
        public DbSet<Cat_Productos_Preguntas_Troubleshooting> Cat_Productos_Preguntas_Troubleshooting { get; set; }
        public DbSet<Cat_Productos_Respuestas_Troubleshooting> Cat_Productos_Respuestas_Troubleshooting { get; set; }
        public DbSet<Rel_Categoria_Producto_Tipo_Producto> Rel_Categoria_Producto_Tipo_Producto { get; set; }
        public DbSet<Cat_Productos_Estatus_Troubleshooting> Cat_Productos_Estatus_Troubleshooting { get; set; }
        public DbSet<Servicio_Troubleshooting> Servicio_Troubleshooting { get; set; }
        public DbSet<Prediagnostico> Prediagnostico { get; set; }
        public DbSet<Prediagnostico_Refacciones> Prediagnostico_Refacciones { get; set; }
        public DbSet<Permisos_Flujo_Cotizacion> Permisos_Flujo_Cotizacion { get; set; }
        public DbSet<formas_pago_tipos_comprobantes> formas_pago_tipos_comprobantes { get; set; }
        public DbSet<rel_tecnico_visita> rel_tecnico_visita { get; set; }
        public DbSet<Sub_cat_tipo_servicio> Sub_cat_tipo_servicio { get; set; }
        public DbSet<Log_refacciones> Log_refacciones { get; set; }
        public DbSet<cat_falla> cat_falla { get; set; }
        public DbSet<cat_reparacion> cat_reparacion { get; set; }
        public DbSet<Notificaciones> Notificaciones { get; set; }
        public DbSet<Cat_Tecnicos_Sub_Tipo> Cat_Tecnicos_Sub_Tipo { get; set; }
        public DbSet<Informe_parte_recibida> Informe_parte_recibida { get; set; }
        public DbSet<quejas_servicios> quejas_servicios { get; set; }
        public DbSet<cat_garantia> cat_garantia { get; set; }
        public DbSet<cat_periodo> cat_periodo { get; set; }
        public DbSet<Home_producto_cliente> home_producto_cliente { get; set; }
        public DbSet<Home_Producto_Estado> home_Producto_Estados { get; set; }
        public DbSet<Cat_Tarjeta> cat_Tarjetas { get; set; }

        public DbSet<cat_tipos_herencia> cat_tipos_herencia { get; set; }
        public DbSet<cat_tipo_entidades> cat_tipo_entidades { get; set; }
        public DbSet<cat_tipo_condicion> cat_tipo_condicion { get; set; }
        public DbSet<cat_msi> cat_msi { get; set; }
        public DbSet<cat_beneficios> cat_beneficios { get; set; }
        public DbSet<promocion> promocion { get; set; }
        public DbSet<entidades_participantes> entidades_participantes { get; set; }
        public DbSet<entidades_excluidas> entidades_excluidas { get; set; }
        public DbSet<productos_excluidos> productos_excluidos { get; set; } // agregado despues
        public DbSet<entidades_obligatorias> entidades_obligatorias { get; set; } // agregado despues 
        public DbSet<promociones_compatibles> promociones_compatibles { get; set; } // agregado despues 
        public DbSet<productos_condicion> productos_condicion { get; set; }
        public DbSet<beneficios_promocion> beneficios_promocion { get; set; }
        public DbSet<beneficio_desc> beneficio_desc { get; set; }
        public DbSet<beneficio_productos> beneficio_productos { get; set; }
        public DbSet<beneficio_msi> beneficio_msi { get; set; }
        public DbSet<afectacion_cc> afectacion_cc { get; set; }
        public DbSet<condiones_comerciales_sucursal> condiones_comerciales_sucursal { get; set; }
        public DbSet<Producto_Promocion> Producto_Promocion { get; set; }
        public DbSet<Cotizacion_Promocion> Cotizacion_Promocion { get; set; }
        public DbSet<productos_relacionados> productos_relacionados { get; set; }
        public DbSet<cat_tipo_productos> cat_tipo_productos { get; set; }

        public DbSet<comision_promo_sublinea_config> comisiones_promo_sublinea_config { get; set; }
        public DbSet<comisiones_vendedores> comisiones_vendedores { get; set; }
        public DbSet<comisiones_sucursales> comisiones_sucursales { get; set; }
        public DbSet<cat_tipos_comisiones> cat_tipos_comisiones { get; set; }
        public DbSet<Cat_Direccion_sucursales> cat_direccion_sucursales { get; set; }
        public DbSet<Cat_Motivo> cat_Motivos { get; set; }
        public DbSet<Mensaje> mensajes { get; set; }
        public DbSet<Direccion_Cotizacion> direcciones_cotizacion { get; set; }
        public DbSet<Cat_SuperLinea> cat_SuperLineas { get; set; }
        public DbSet<Caracteristica_Base> caracteristicas_Bases { get; set; }
        public DbSet<Parametro_Archivo> parametro_Archivos { get; set; }
        public DbSet<Parametro_Archivo_Terminos_Condiciones> parametro_Terminos_Condiciones { get; set; }
        public DbSet<config_comision> config_comisiones { get; set; }

        public DbSet<com_afectacion_cc> com_afectacion_ccs { get; set; }
        public DbSet<com_entidades_participantes> com_entidades_participantes { get; set; }
        public DbSet<com_productos_condicion> com_productos_condicions { get; set; }
        public DbSet<com_producto_promocion> com_producto_promocions { get; set; }

        public DbSet<encuesta_general> encuesta_general { get; set; }
        public DbSet<encuesta_queja> encuesta_queja { get; set; }
        public DbSet<Cat_Servicio_Sin_Pago> Servicio_Sin_Pagos { get; set; }


        #region Quejas
        public virtual DbSet<Canales> Canales { get; set; }
        public virtual DbSet<ProductosQuejas> ProductosQuejas { get; set; }
        public virtual DbSet<Propuestas> Propuestas { get; set; }
        public virtual DbSet<Quejas> Quejas { get; set; }
        public virtual DbSet<TipoQueja> TipoQueja { get; set; }
        #endregion

        #region Certificado de manteniemiento
        public DbSet<Cer_labor> Cer_labor { get; set; }
        public DbSet<Cer_viaticos> Cer_viaticos { get; set; }
        public DbSet<Cer_consumibles> Cer_consumibles { get; set; }
        public DbSet<Cer_producto_cliente> Cer_producto_cliente { get; set; }
        public DbSet<rel_consumible_sublinea> rel_consumible_sublinea { get; set; }
        public DbSet<rel_certificado_producto> rel_certificado_producto { get; set; }
        public DbSet<rel_homep_producto> rel_homep_productos { get; set; }

        public DbSet<rel_certificado_producto_consumibles> rel_certificado_producto_consumibles { get; set; }
        #endregion
        public DbSet<sublinea_certificado_partners> sublinea_certificado_partners { get; set; }
        public DbSet<producto_certificado_sublinea> producto_certificado_sublinea { get; set; }
        public DbSet<Cer_producto_carrito> Cer_producto_carrito { get; set; }
        public DbSet<rel_certificado_carrito> rel_certificado_carrito { get; set; }
        public List<Home_producto_carrito> Home_producto_carrito { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cer_viaticos>()
           .HasOne(p1 => p1.localidad)
           .WithMany(b => b.viaticos)
           .HasForeignKey(p1 => p1.id_cat_localidad)
           .HasConstraintName("ForeignKey_cat_localidad_viaticos");

            modelBuilder.Entity<Cat_Direccion_sucursales>()
            .HasOne(p1 => p1.cat_sucursales)
            .WithMany(b => b.cat_direccion_sucursales)
            .HasForeignKey(p1 => p1.id_sucursales)
            .HasConstraintName("ForeignKey_cat_direcciones_sucu_cat_suc");

            modelBuilder.Entity<condiones_comerciales_sucursal>()
            .HasOne(p1 => p1.Cat_SubLinea_Producto)
            .WithMany(b => b.condiones_comerciales_sucursal)
            .HasForeignKey(p1 => p1.id_Cat_SubLinea_Producto)
            .HasConstraintName("ForeignKey_condiones_comerciales_sucursal_sublinea");

            modelBuilder.Entity<condiones_comerciales_sucursal>()
            .HasOne(p1 => p1.Cat_Sucursales)
            .WithMany(b => b.condiones_comerciales_sucursal)
            .HasForeignKey(p1 => p1.id_Cat_Sucursales)
            .HasConstraintName("ForeignKey_condiones_comerciales_sucursal_sucursales");

            modelBuilder.Entity<comisiones_vendedores>()
            .HasOne(p1 => p1.cotizaciones)
            .WithMany(b => b.comisiones_vendedores)
            .HasForeignKey(p1 => p1.id_cotizacion)
            .HasConstraintName("ForeignKey_comisiones_vendedores_cotizaciones");

            modelBuilder.Entity<comisiones_sucursales>()
            .HasOne(p1 => p1.cotizaciones)
            .WithMany(b => b.comisiones_sucursales)
            .HasForeignKey(p1 => p1.id_cotizacion)
            .HasConstraintName("ForeignKey_comisiones_sucursales_cotizaciones");

            modelBuilder.Entity<comision_promo_sublinea_config>()
                .HasOne(p => p.Promocion)
                .WithMany(f => f.Comisiones_promociones)
                .HasForeignKey(p => p.id_promocion)
                .HasConstraintName("ForeignKey_Promocion_comisiones_sublinea_config");

            modelBuilder.Entity<comision_promo_sublinea_config>()
                .HasOne(p => p.condiciones_promos)
                .WithMany(f1 => f1.comision_Promos)
                .HasForeignKey(p => p.id_cc_sucursal)
                .HasConstraintName("ForeignKey_comisiones_sublinea_config_cc_suc");

            modelBuilder.Entity<comisiones_vendedores>()
          .HasOne(p1 => p1.cat_tipos_comisiones)
          .WithMany(b => b.comisiones_vendedores)
          .HasForeignKey(p1 => p1.id_cat_tipo_comision)
          .HasConstraintName("ForeignKey_comisiones_vendedores_tipo_comisiones");

            modelBuilder.Entity<comisiones_sucursales>()
            .HasOne(p1 => p1.cat_tipos_comisiones)
            .WithMany(b => b.comisiones_sucursales)
            .HasForeignKey(p1 => p1.id_cat_tipo_comision)
            .HasConstraintName("ForeignKey_comisiones_sucursales_tipo_comisiones");

            modelBuilder.Entity<Cat_SubLinea_Producto>()
           .HasOne(p1 => p1.cat_linea_producto)
            .WithMany(b => b.cat_sublinea_producto)
            .HasForeignKey(p1 => p1.id_linea_producto)
            .HasConstraintName("ForeignKey_Cat_sublinea_cat_linea");

            modelBuilder.Entity<Cat_Linea_Producto>()
                .HasOne(p => p.cat_SuperLinea)
                .WithMany(b => b.cat_Linea_Productos)
                .HasForeignKey(p => p.id_superlinea)
                .HasConstraintName("ForeignKey_Cat_linea_Cat_superlinea");

            modelBuilder.Entity<Cat_Productos>()
                .HasOne(p => p.caracteristica_Base)
                .WithMany(b => b.cat_Productos)
                .HasForeignKey(p => p.id_caracteristica_base)
                .HasConstraintName("ForeignKey_cat_productos_caracteristica_base");

            modelBuilder.Entity<Cat_CondicionesPago>()
             .HasOne(p1 => p1.Cat_Formas_Pago)
              .WithMany(b => b.cat_condicionespago)
              .HasForeignKey(p1 => p1.id_Cat_Formas_Pago)
              .HasConstraintName("ForeignKey_Cat_Formas_Pago_Cat_CondicionesPago");

            modelBuilder.Entity<Cat_CondicionesPago>()
             .HasOne(p1 => p1.Cat_Cuentas)
              .WithMany(b => b.cat_Formas_Pago)
              .HasForeignKey(p1 => p1.id_cuenta)
              .HasConstraintName("ForeignKey_cuentas_Cat_CondicionesPago");

            modelBuilder.Entity<DatosFiscales_Sucursales>()
             .HasOne(p1 => p1.Cat_Sucursales)
              .WithMany(b => b.DatosFiscales_Sucursales)
              .HasForeignKey(p1 => p1.id_Sucursal)
              .HasConstraintName("ForeignKey_DatosFiscales_Sucursales_datos_fiscales");

            #region CustomResponses
            modelBuilder.Query<Promedio>();
            modelBuilder.Query<Porcentaje>();
            modelBuilder.Query<Resumen>();
            modelBuilder.Query<Grafica>();
            modelBuilder.Query<GraficaVisita>();
            #endregion

            modelBuilder.Entity<Producto_Promocion>()
              .HasOne(p1 => p1.Cat_Productos)
              .WithMany(b => b.producto_promocion)
              .HasForeignKey(p1 => p1.id_producto)
              .HasConstraintName("ForeignKey_producto_promocion_cat_productos");

            modelBuilder.Entity<Producto_Promocion>()
              .HasOne(p1 => p1.promocion)
              .WithMany(b => b.producto_promocion)
              .HasForeignKey(p1 => p1.id_promocion)
              .HasConstraintName("ForeignKey_producto_promocion_promociones");

            modelBuilder.Entity<Producto_Promocion>()
              .HasOne(p1 => p1.cotizaciones)
              .WithMany(b => b.producto_promocion)
              .HasForeignKey(p1 => p1.id_cotizacion)
              .HasConstraintName("ForeignKey_producto_promocion_cotizaciones");

            modelBuilder.Entity<Cotizacion_Promocion>()
             .HasOne(p1 => p1.Cotizaciones)
             .WithMany(b => b.cotizacion_promocion)
             .HasForeignKey(p1 => p1.id_cotizacion)
             .HasConstraintName("ForeignKey_producto_promocion_cotizacion");

            modelBuilder.Entity<Cotizacion_Promocion>()
             .HasOne(p1 => p1.promocion)
             .WithMany(b => b.cotizacion_promocion)
             .HasForeignKey(p1 => p1.id_promocion)
             .HasConstraintName("ForeignKey_producto_promocion_promocion");


            modelBuilder.Entity<cotizacion_monto_descuento>()
             .HasOne(p1 => p1.Cotizaciones)
             .WithMany(b => b.cotizacion_monto_descuento)
             .HasForeignKey(p1 => p1.id_cotizacion)
             .HasConstraintName("ForeignKey_cotizacion_monto_descuento_cotizacion");

            modelBuilder.Entity<cotizacion_monto_descuento>()
            .HasOne(p1 => p1.promocion)
             .WithMany(b => b.cotizacion_monto_descuento)
             .HasForeignKey(p1 => p1.id_promocion)
             .HasConstraintName("ForeignKey_cotizacion_monto_descuento_promocion");

            


            modelBuilder.Entity<promocion>()
                .HasOne(p1 => p1.tipos_herencia_promo)
                .WithMany(b => b.promocion)
                .HasForeignKey(p1 => p1.id_tipos_herencia_promo)
                .HasConstraintName("ForeignKey_1promocion_herencia2");

            modelBuilder.Entity<promocion>()
                .HasOne(p2 => p2.cat_tipo_condicion)
                .WithMany(b => b.promocion)
                .HasForeignKey(p2 => p2.id_cat_tipo_condicion)
                .HasConstraintName("ForeignKey_2promocion_condicion2");

            modelBuilder.Entity<entidades_participantes>()
                .HasOne(p3 => p3.promocion)
                .WithMany(b => b.entidades_participantes)
                .HasForeignKey(p3 => p3.id_promocion)
                .HasConstraintName("ForeignKey_3entidadesparticipantes_promocion2");

            modelBuilder.Entity<entidades_excluidas>()
                .HasOne(p4 => p4.promocion)
                .WithMany(b => b.entidades_excluidas)
                .HasForeignKey(p4 => p4.id_promocion)
                .HasConstraintName("ForeignKey_4entidadesexcluidas_promocion2");

            modelBuilder.Entity<entidades_obligatorias>()
                .HasOne(p4 => p4.promocion)
                .WithMany(b => b.entidades_obligatorias)
                .HasForeignKey(p4 => p4.id_promocion)
                .HasConstraintName("ForeignKey_4entidadesobli_promocion2");

            modelBuilder.Entity<productos_condicion>()
                .HasOne(p5 => p5.promocion)
                .WithMany(b => b.productos_condicion)
                .HasForeignKey(p5 => p5.id_promocion)
                .HasConstraintName("ForeignKey_5productoscondicion_promocion2");

            modelBuilder.Entity<promociones_compatibles>()
                .HasOne(p5 => p5.promocion)
                .WithMany(b => b.promociones_compatibles)
                .HasForeignKey(p5 => p5.id_promocion)
                .HasConstraintName("ForeignKey_5promocompatibles1_promocion2");

            //modelBuilder.Entity<promociones_compatibles>()
            //   .HasOne(p5 => p5.promocion2)
            //   .WithMany(b => b.promociones_compatibles)
            //   .HasForeignKey(p5 => p5.id_promocion_2)
            //   .HasConstraintName("ForeignKey_5promocompatibles2_promocion2");

            modelBuilder.Entity<productos_excluidos>()
                .HasOne(p5 => p5.promocion)
                .WithMany(b => b.productos_excluidos)
                .HasForeignKey(p5 => p5.id_promocion)
                .HasConstraintName("ForeignKey_5productos_excluidos_promocion2");

            modelBuilder.Entity<beneficios_promocion>()
                .HasOne(p6 => p6.promocion)
                .WithMany(b => b.beneficios_promocion)
                .HasForeignKey(p6 => p6.id_promocion)
                .HasConstraintName("ForeignKey_6beneficiospromocion_promocion2");

            modelBuilder.Entity<beneficio_desc>()
               .HasOne(p7 => p7.promocion)
               .WithMany(b => b.beneficio_desc)
               .HasForeignKey(p7 => p7.id_promocion)
               .HasConstraintName("ForeignKey_7beneficiodesc_promocion2");

            modelBuilder.Entity<beneficio_productos>()
               .HasOne(p8 => p8.promocion)
               .WithMany(b => b.beneficio_productos)
               .HasForeignKey(p8 => p8.id_promocion)
               .HasConstraintName("ForeignKey_8beneficioproductos_promocion2");

            modelBuilder.Entity<afectacion_cc>()
              .HasOne(p9 => p9.promocion)
              .WithMany(b => b.afectacion_cc)
              .HasForeignKey(p9 => p9.id_promocion)
              .HasConstraintName("ForeignKey_9afectacion_cc_promocion2");

            modelBuilder.Entity<entidades_participantes>()
              .HasOne(p => p.cat_tipo_entidades)
              .WithMany(b => b.entidades_participantes)
              .HasForeignKey(p => p.id_tipo_entidad)
              .HasConstraintName("ForeignKey_entidadesparticipantes_cattipoentidades2");

            modelBuilder.Entity<entidades_excluidas>()
             .HasOne(p => p.cat_tipo_entidades)
             .WithMany(b => b.entidades_excluidas)
             .HasForeignKey(p => p.id_tipo_entidad)
             .HasConstraintName("ForeignKey_entidades_excluidas_cattipoentidades2");

            modelBuilder.Entity<entidades_obligatorias>()
             .HasOne(p => p.cat_tipo_entidades)
             .WithMany(b => b.entidades_obligatorias)
             .HasForeignKey(p => p.id_tipo_entidad)
             .HasConstraintName("ForeignKey_entidades_obligatorias_cattipoentidades2");

            modelBuilder.Entity<beneficios_promocion>()
             .HasOne(p => p.cat_beneficios)
             .WithMany(b => b.beneficios_promocion)
             .HasForeignKey(p => p.id_cat_beneficios)
             .HasConstraintName("ForeignKey_beneficiospromocion_cattipoentidades2");

            modelBuilder.Entity<beneficio_msi>()
             .HasOne(p => p.cat_msi)
             .WithMany(b => b.beneficio_msi)
             .HasForeignKey(p => p.id_cat_msi)
             .HasConstraintName("ForeignKey_beneficiomsi_catmsi2");

            modelBuilder.Entity<beneficio_msi>()
             .HasOne(p => p.promocion)
             .WithMany(b => b.beneficio_msi)
             .HasForeignKey(p => p.id_promocion)
             .HasConstraintName("ForeignKey_beneficiomsi_promo52");

            modelBuilder.Entity<afectacion_cc>()
             .HasOne(p => p.condiones_comerciales_sucursal)
             .WithMany(b => b.afectacion_cc)
             .HasForeignKey(p => p.id_condiones_comerciales_sucursal)
             .HasConstraintName("ForeignKey_afectacioncc_condiones_comerciales_sucursal2");

            modelBuilder.Entity<Tecnicos>()
                .HasOne(p => p.users)
                .WithMany(b => b.tecnicos)
                .HasForeignKey(p => p.id)
                .HasConstraintName("ForeignKey_Users_Tecnicos");

            modelBuilder.Entity<comisiones_vendedores>()
               .HasOne(p => p.Users)
               .WithMany(b => b.comisiones_vendedores)
               .HasForeignKey(p => p.id_quienpago)
               .HasConstraintName("ForeignKey_Users_comisiones_vendedoress");


            modelBuilder.Entity<Tecnicos>()
                .HasOne(p => p.tecnicos_Tipo)
                .WithMany(b => b.tecnicos)
                .HasForeignKey(p => p.id_tipo_tecnico)
                .HasConstraintName("ForeignKey_Users_Tecnicos_Tipo");


            modelBuilder.Entity<Users>()
                .HasOne(p => p.App)
                .WithMany(b => b.users)
                .HasForeignKey(p => p.id_app)
                .HasConstraintName("ForeignKey_Users_App");

            modelBuilder.Entity<Users>()
                .HasOne(p => p.Rol)
                .WithMany(b => b.users)
                .HasForeignKey(p => p.id_rol)
                .HasConstraintName("ForeignKey_Users_Rol");

            modelBuilder.Entity<Tecnicos_Actividad>()
                .HasOne(p => p.users)
                .WithMany(b => b.tecnicos_actividad)
                .HasForeignKey(p => p.id_user)
                .HasConstraintName("ForeignKey_Users_Tecnicos_Actividad");

            modelBuilder.Entity<Prediagnostico_Refacciones>()
                .HasOne(p => p.prediagnostico)
                .WithMany(b => b.refacciones)
                .HasForeignKey(p => p.id_prediagnostico)
                .HasConstraintName("ForeignKey_Prediagnostico_Refacciones");

            //modelBuilder.Entity<Tecnicos_Actividad>()
            //    .HasOne(p => p.actividad)
            //    .WithMany(b => b.tecnicos_actividad)
            //    .HasForeignKey(p => p.id_actividad)
            //    .HasConstraintName("ForeignKey_Users_Tecnicos_Actividad_Cat");

            modelBuilder.Entity<Tecnicos_Cobertura>()
                .HasOne(p => p.users)
                .WithMany(b => b.tecnicos_cobertura)
                .HasForeignKey(p => p.id_user)
                .HasConstraintName("ForeignKey_Users_Tecnicos_Cobertura");

            modelBuilder.Entity<Tecnicos_Cobertura>()
                .HasOne(p => p.cobertura)
                .WithMany(b => b.tecnicos_cobertura)
                .HasForeignKey(p => p.id_cobertura)
                .HasConstraintName("ForeignKey_Users_Tecnicos_Cobertura_Cat");

            modelBuilder.Entity<Tecnicos_Producto>()
                .HasOne(p => p.users)
                .WithMany(b => b.tecnicos_producto)
                .HasForeignKey(p => p.id_user)
                .HasConstraintName("ForeignKey_Users_Tecnicos_Productos");

            modelBuilder.Entity<Tecnicos_Producto>()
                .HasOne(p => p.producto)
                .WithMany(b => b.tecnicos_producto)
                .HasForeignKey(p => p.id_categoria_producto)
                .HasConstraintName("ForeignKey_Users_Tecnicos_Producto_Cat");

            modelBuilder.Entity<Cat_Productos>()
                .HasOne(p => p.linea)
                .WithMany(b => b.productos)
                .HasForeignKey(p => p.id_linea)
                .HasConstraintName("ForeignKey_Linea_Producto");

            modelBuilder.Entity<Cat_Productos>()
                .HasOne(p => p.sublinea)
                .WithMany(b => b.productos)
                .HasForeignKey(p => p.id_sublinea)
                .HasConstraintName("ForeignKey_Sublinea_Producto");

            modelBuilder.Entity<Rel_Categoria_Producto_Tipo_Producto>()
                .HasOne(p => p.categoria)
                .WithMany(b => b.rel_categoria)
                .HasForeignKey(p => p.id_categoria)
                .HasConstraintName("ForeignKey_Categoria_Producto_Rel");

            modelBuilder.Entity<Rel_Categoria_Producto_Tipo_Producto>()
              .HasOne(p => p.tipo_servicio)
              .WithMany(b => b.rel_categoria)
              .HasForeignKey(p => p.id_tipo_servicio)
              .HasConstraintName("ForeignKey_Categoria_Producto_Rel_Tipo_Servicio");

            modelBuilder.Entity<Cat_Imagenes_Producto>()
                .HasOne(p => p.productos)
                .WithMany(b => b.cat_imagenes_producto)
                .HasForeignKey(p => p.id_producto)
                .HasConstraintName("ForeignKey_Imagen_Producto");

            modelBuilder.Entity<Cat_Sugeridos_Producto>()
               .HasOne(p => p.productos)
               .WithMany(b => b.cat_sugeridos_producto)
               .HasForeignKey(p => p.id_producto)
               .HasConstraintName("ForeignKey_Sugeridos_Producto");

            modelBuilder.Entity<Servicio>()
                .HasOne(p => p.cliente)
                .WithMany(b => b.servicio)
                .HasForeignKey(p => p.id_cliente)
                .HasConstraintName("ForeignKey_Servicio_Cliente");

            modelBuilder.Entity<Servicio>()
                .HasOne(p => p.tipo_servicio)
                .WithMany(b => b.servicio)
                .HasForeignKey(p => p.id_tipo_servicio)
                .HasConstraintName("ForeignKey_Servicio_TipoServicio");

            modelBuilder.Entity<Sub_cat_tipo_servicio>()
                .HasOne(p => p.tipo_servicio)
                .WithMany(b => b.Sub_cat_tipo_servicio)
                .HasForeignKey(p => p.id_tipo_servicio)
                .HasConstraintName("ForeignKey_Servicio_SubTipoServicio");

            modelBuilder.Entity<Servicio>()
                .HasOne(p => p.solicitado_por)
                .WithMany(b => b.servicio)
                .HasForeignKey(p => p.id_solicitado_por)
                .HasConstraintName("ForeignKey_Servicio_Solicitado_Por");

            modelBuilder.Entity<Servicio>()
                .HasOne(p => p.solicitud_via)
                .WithMany(b => b.servicio)
                .HasForeignKey(p => p.id_solicitud_via)
                .HasConstraintName("ForeignKey_Servicio_Solicitud_Via");

            modelBuilder.Entity<rel_tecnico_visita>()
                .HasOne(p => p.tecnico)
                .WithMany(b => b.rel_tecnico_visita)
                .HasForeignKey(p => p.id_tecnico)
                .HasConstraintName("ForeignKey_Tecnico_Visita");

            modelBuilder.Entity<rel_tecnico_visita>()
                .HasOne(p => p.visita)
                .WithMany(b => b.rel_tecnico_visita)
                .HasForeignKey(p => p.id_vista)
                .HasConstraintName("ForeignKey_Visita_Tecnico");

            modelBuilder.Entity<Visita>()
                .HasOne(p => p.servicio)
                .WithMany(b => b.visita)
                .HasForeignKey(p => p.id_servicio)
                .HasConstraintName("ForeignKey_Servicio_Visita");

            modelBuilder.Entity<Rel_servicio_visita_producto>()
               .HasOne(p => p.visita)
               .WithMany(b => b.servicio_producto)
               .HasForeignKey(p => p.id_vista)
               .HasConstraintName("ForeignKey_Rel_Servicio_Producto");

            modelBuilder.Entity<Rel_servicio_visita_Refaccion>()
               .HasOne(p => p.visita)
               .WithMany(b => b.servicio_refaccion)
               .HasForeignKey(p => p.id_vista)
               .HasConstraintName("ForeignKey_Rel_Servicio_Refaccion");

            modelBuilder.Entity<Producto_Check_List_Respuestas>()
               .HasOne(p => p.visita)
               .WithMany(b => b.producto_check_List_respuestas)
               .HasForeignKey(p => p.id_vista)
               .HasConstraintName("ForeignKey_Producto_Check_List_Respuestas");

            modelBuilder.Entity<Check_List_Respuestas>()
               .HasOne(p => p.producto_check_list_respuestas)
               .WithMany(b => b.check_list_respuestas)
               .HasForeignKey(p => p.id_producto_check_list_respuestas)
               .HasConstraintName("ForeignKey_Check_List_Respuestas");

            modelBuilder.Entity<Piezas_Repuesto>()
                .HasOne(p => p.refacciones)
                .WithMany(b => b.piezas_repuesto)
                .HasForeignKey(p => p.id_rel_servicio_refaccion)
                .HasConstraintName("ForeignKey_Piezas_Repuesto_Rel_Servicio_Producto");

            modelBuilder.Entity<Piezas_Repuesto_Tecnico>()
                .HasOne(p => p.refacciones)
                .WithMany(b => b.piezas_repuesto_tecnico)
                .HasForeignKey(p => p.id_rel_servicio_refaccion)
                .HasConstraintName("ForeignKey_Piezas_Repuesto_Tecnico_Rel_Servicio_Producto");

            modelBuilder.Entity<Cat_Direccion>()
                .HasOne(p => p.cliente)
                .WithMany(b => b.direccion)
                .HasForeignKey(p => p.id_cliente)
                .HasConstraintName("ForeignKey_Direccion_Cliente");

            modelBuilder.Entity<Direcciones_Cliente>()
                .HasOne(p => p.cliente)
                .WithMany(b => b.direcciones_Clientes)
                .HasForeignKey(p => p.id_cliente)
                .HasConstraintName("ForeignKey_Direcciones_Clientes");

            modelBuilder.Entity<DatosFiscales>()
            .HasOne(p => p.cliente)
            .WithMany(b => b.datos_fiscales)
            .HasForeignKey(p => p.id_cliente)
            .HasConstraintName("ForeignKey_DatosFiscales_Cliente");

            modelBuilder.Entity<Cat_Materiales>()
                .HasOne(p => p.grupo_precio)
                .WithMany(b => b.materiales)
                .HasForeignKey(p => p.id_grupo_precio)
                .HasConstraintName("ForeignKey_Material_Grupo_Precio");

            //modelBuilder.Entity<Cat_Materiales_Tecnico>()
            //    .HasOne(p => p.grupo_precio)
            //    .WithMany(b => b.materiales_tecnico)
            //    .HasForeignKey(p => p.id_grupo_precio)
            //    .HasConstraintName("ForeignKey_Material_Grupo_Precio_Tecnico");

            //modelBuilder.Entity<Cat_Materiales_Tecnico>()
            //    .HasOne(p => p.material)
            //    .WithMany(b => b.materiales_tecnico)
            //    .HasForeignKey(p => p.id_material)
            //    .HasConstraintName("ForeignKey_Material_Material_Tecnico");

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////// PARTNERS //////////////////////////////////////////////////////////////////

            //modelBuilder.Entity<Cotizaciones>()
            //    .HasOne(p => p.Cliente)
            //    .WithMany(b => b.Id_Cliente_Cotizacion)
            //    .HasForeignKey(p => p.Id_Cliente)
            //    .HasConstraintName("ForeignKey_Cotizacion_Cliente");

            //modelBuilder.Entity<Cotizaciones>()
            //    .HasOne(p => p.Vendedor)
            //    .WithMany(b => b.Id_Vendedor_Cotizacion)
            //    .HasForeignKey(p => p.Id_Vendedor)
            //    .HasConstraintName("ForeignKey_Cotizacion_Vendedor");

            modelBuilder.Entity<Cotizaciones>()
                .HasOne(p => p.Canal)
                .WithMany(b => b.Id_Canal_Cotizacion)
                .HasForeignKey(p => p.Id_Canal)
                .HasConstraintName("ForeignKey_Cotizacion_Canal");

            //modelBuilder.Entity<Cotizaciones>()
            //    .HasOne(p => p.Cuenta)
            //    .WithMany(b => b.Id_Cuenta_Cotizacion)
            //    .HasForeignKey(p => p.Id_Cuenta)
            //    .HasConstraintName("ForeignKey_Cotizacion_Cuenta");

            modelBuilder.Entity<Cat_Cuentas>()
                .HasOne(p => p.Canal)
                .WithMany(b => b.Id_Canal_Cuenta)
                .HasForeignKey(p => p.Id_Canal)
                .HasConstraintName("ForeignKey_Canal_cuenta");

            modelBuilder.Entity<Cat_Sucursales>()
                .HasOne(p => p.Cuenta)
                .WithMany(b => b.Id_Cuenta_Sucursal)
                .HasForeignKey(p => p.Id_Cuenta)
                .HasConstraintName("ForeignKey_cuenta_Sucursal");

            modelBuilder.Entity<Cotizacion_Producto>()
                .HasOne(p => p.Cotizacion)
                .WithMany(b => b.Id_Cotizacion_Producto)
                .HasForeignKey(p => p.Id_Cotizacion)
                .HasConstraintName("ForeignKey_Cotizacion_Producto_cotiza");

            modelBuilder.Entity<Cotizacion_Producto>()
                .HasOne(p => p.Producto)
                .WithMany(b => b.Id_Cotizacion_Producto)
                .HasForeignKey(p => p.Id_Producto)
                .HasConstraintName("ForeignKey_Cotizacion_Producto_prod");

            modelBuilder.Entity<Vendedores>()
                .HasOne(p => p.estado)
                .WithMany(b => b.Id_Estado_Vendedores)
                .HasForeignKey(p => p.id_estado)
                .HasConstraintName("ForeignKey_Vendedores_Estado");

            modelBuilder.Entity<Vendedores>()
                .HasOne(p => p.municipio)
                .WithMany(b => b.Id_Municipio_Vendedores)
                .HasForeignKey(p => p.id_municipio)
                .HasConstraintName("ForeignKey_Vendedores_Mun");
            modelBuilder.Entity<His_Servicio_Estatus>()
               .HasOne(p => p.servicio)
               .WithMany(b => b.historial)
               .HasForeignKey(p => p.id_servicio)
               .HasConstraintName("ForeignKey_Historial_Servicio");

            //modelBuilder.Entity<Check_List_Preguntas>()
            //    .HasOne(p => p.categoria)
            //    .WithMany(b => b.Preguntas)
            //    .HasForeignKey(p => p.id_categoria)
            //    .HasConstraintName("ForeignKey_Ckl_Categoria_Producto");

            //modelBuilder.Entity<Check_List_Respuestas>()
            //    .HasOne(p => p.pregunta)
            //    .WithMany(b => b.Respuestas)
            //    .HasForeignKey(p => p.id_pregunta)
            //    .HasConstraintName("ForeignKey_Ckl_Pregunta_Respuesta");

            modelBuilder.Entity<Cliente_Productos>()
                .HasOne(p => p.Cliente)
                .WithMany(b => b.Id_Cliente_Productos)
                .HasForeignKey(p => p.Id_Cliente)
                .HasConstraintName("ForeignKey_Cliente_Producto");

            //modelBuilder.Entity<Cliente_Productos>()
            //    .HasOne(p => p.Producto)
            //    .WithMany(b => b.Id_Cliente_Productos)
            //    .HasForeignKey(p => p.Id_Producto)
            //    .HasConstraintName("ForeignKey_Cliente_Producto_Producto");

            //modelBuilder.Entity<Cliente_Productos>()
            //    .HasOne(p => p.EstatusCompra)
            //    .WithMany(b => b.Id_Cliente_Productos)
            //    .HasForeignKey(p => p.Id_EsatusCompra)
            //    .HasConstraintName("ForeignKey_Cliente_Producto_Estatus");

            modelBuilder.Entity<Cat_Imagenes_Accesosrios>()
                .HasOne(p => p.accesorios)
                .WithMany(b => b.cat_imagenes_accesorio)
                .HasForeignKey(p => p.id_Accesorio)
                .HasConstraintName("ForeignKey_Imagen_Accesorios");

            modelBuilder.Entity<Cat_Accesorios_Relacionados>()
               .HasOne(p => p.accesorios)
               .WithMany(b => b.Cat_Accesorios_Relacionados)
               .HasForeignKey(p => p.id_Accesorio)
               .HasConstraintName("ForeignKey_Accesorios_relacionados");

            modelBuilder.Entity<Productos_Carrito>()
               .HasOne(p => p.Productos)
               .WithMany(b => b.Id_Carritos_Productos)
               .HasForeignKey(p => p.Id_Producto)
               .HasConstraintName("ForeignKey_Carritos_productos");

            //modelBuilder.Entity<Cat_Productos_Preguntas_Troubleshooting>()
            //   .HasOne(p => p.producto)
            //   .WithMany(b => b.cat_productos_preguntas_troubleshooting)
            //   .HasForeignKey(p => p.id_producto)
            //   .HasConstraintName("ForeignKey_cat_productos_preguntas_troubleshooting");

            //modelBuilder.Entity<Cat_Productos_Respuestas_Troubleshooting>()
            //   .HasOne(p => p.troubleshooting)
            //   .WithMany(b => b.cat_Productos_respuestas_troubleshooting)
            //   .HasForeignKey(p => p.id_troubleshooting)
            //   .HasConstraintName("ForeignKey_Cat_Productos_Respuestas_Troubleshooting");

            modelBuilder.Entity<Servicio_Troubleshooting>()
              .HasOne(p => p.estatus_troubleshooting)
              .WithMany(b => b.servicio_troubleshooting)
              .HasForeignKey(p => p.id_estatus_troubleshooting)
              .HasConstraintName("ForeignKey_Servicio_Troubleshooting");

            modelBuilder.Entity<Servicio_Troubleshooting>()
              .HasOne(p => p.servicio)
              .WithMany(b => b.servicio_troubleshooting)
              .HasForeignKey(p => p.id_servicio)
              .HasConstraintName("ForeignKey_Servicio_Troubleshooting_Servivo");

            modelBuilder.Entity<quejas_servicios>()
              .HasOne(p => p.Servicio)
              .WithMany(b => b.quejas_servicios)
              .HasForeignKey(p => p.id_servicio)
              .OnDelete(DeleteBehavior.Cascade)
              .HasConstraintName("ForeignKey_Servicio_Queja");

            modelBuilder.Entity<Mensaje>()
                .HasOne(p => p.cat_Motivo)
                .WithMany(b => b.mensajes)
                .HasForeignKey(p => p.motivo_id)
               .HasConstraintName("ForeignKey_Motivo_Mensajes");

            // Para revisar
            //modelBuilder.Entity<Cat_Motivo>()
            //    .HasOne(p => p.mensajes)
            //    .WithMany(b => b.cat_Motivo)
            //    .HasForeignKey(p => p.motivo_id)
            //    .HasConstraintName("ForeignKey_Motivo_Mensajes");

            modelBuilder.Entity<Direccion_Cotizacion>()
                .HasOne(p => p.cotizacion)
                .WithMany(b => b.direcciones_cotizacion)
                .HasForeignKey(p => p.id_cotizacion)
                .HasConstraintName("ForeignKey_Direcciones_Cotizaciones");

            modelBuilder.Entity<config_comision>()
                .HasOne(p => p.cat_tipo_condicion)
                .WithMany(b => b.config_comisiones)
                .HasForeignKey(p => p.id_cat_tipo_condicion)
                .HasConstraintName("ForeignKey_config_comision_condicion");

            modelBuilder.Entity<config_comision>()
              .HasOne(p => p.cat_tipos_herencia)
              .WithMany(b => b.config_comisiones)
              .HasForeignKey(p => p.id_tipos_herencia_promo)
              .HasConstraintName("ForeignKey_config_comision_herencia");

            modelBuilder.Entity<com_entidades_participantes>()
              .HasOne(p => p.config_comisiones)
              .WithMany(b => b.entidades_participantes)
              .HasForeignKey(p => p.id_comisionv)
              .HasConstraintName("ForeignKey_config_comision_participantes");

            modelBuilder.Entity<com_entidades_participantes>()
            .HasOne(p => p.cat_tipo_entidades)
            .WithMany(b => b.com_entidades_participantes)
            .HasForeignKey(p => p.id_tipo_entidad)
            .HasConstraintName("ForeignKey_comentidadesparticipantes_comcattipoentidades2");


            modelBuilder.Entity<com_afectacion_cc>()
           .HasOne(p => p.config_comisiones)
           .WithMany(b => b.afectacion_cc)
           .HasForeignKey(p => p.id_comisionv)
           .HasConstraintName("ForeignKey_config_comision_afectacion");


            modelBuilder.Entity<com_afectacion_cc>()
             .HasOne(p => p.condiones_comerciales_sucursal)
             .WithMany(b => b.com_afectacion_cc)
             .HasForeignKey(p => p.id_condiones_comerciales_sucursal)
             .HasConstraintName("ForeignKey_comafectacioncc_condiones_comerciales_sucursal2");




            modelBuilder.Entity<com_producto_promocion>()
            .HasOne(p => p.config_comisiones)
            .WithMany(b => b.producto_promocion)
            .HasForeignKey(p => p.id_comisionv)
            .HasConstraintName("ForeignKey_config_comision_productos_misiones_prom");



            modelBuilder.Entity<com_productos_condicion>()
            .HasOne(p => p.config_comisiones)
            .WithMany(b => b.productos_condicion)
            .HasForeignKey(p => p.id_comisionv)
            .HasConstraintName("ForeignKey_config_comision_productos_condicion");

            #region Encuestas
            modelBuilder.Entity<encuesta_general>()
               .HasOne(p => p.Servicio)
               .WithMany(b => b.e_general)
               .HasForeignKey(p => p.id_servicio)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("ForeignKey_e_general_servicio");

            #endregion








            #region Canales
            modelBuilder.Entity<Canales>(entity =>
            {
                entity.Property(e => e.Nombre).IsRequired();
            });

            modelBuilder.Entity<ProductosQuejas>(entity =>
            {
                entity.HasKey(e => new { e.ProductoId, e.QuejaId });

                entity.HasIndex(e => e.QuejaId);

                entity.HasOne(d => d.Queja)
                    .WithMany(p => p.ProductosQuejas)
                    .HasForeignKey(d => d.QuejaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductosQuejas_Quejas");
            });

            modelBuilder.Entity<Propuestas>(entity =>
            {
                entity.Property(e => e.Solucion).IsRequired();

                entity.HasOne(d => d.Queja)
                    .WithMany(p => p.Propuestas)
                    .HasForeignKey(d => d.QuejaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Propuestas_Quejas");
            });

            modelBuilder.Entity<Quejas>(entity =>
            {
                entity.Property(e => e.Folio).IsRequired();
            });
            #endregion

            //modelBuilder.Entity<Home_producto_cliente>()
            //    .HasOne(p => p.clientes)
            //    .WithMany(b => b.home_producto_cliente)
            //    .HasForeignKey(p => p.id_cliente)
            //    .HasConstraintName("ForeignKey_Cliente_Home_Program");

            modelBuilder.Entity<Home_Producto_Estado>()
                .HasOne(b => b.Estado)
                .WithMany(c => c.Home_Productos)
                .HasForeignKey(b => b.id_estado)
                .HasConstraintName("ForeignKey_Estados_Home_Program");

            modelBuilder.Entity<Home_Producto_Estado>()
                .HasOne(p => p.Producto)
                .WithMany(c => c.Productos_Estados)
                .HasForeignKey(p => p.id_producto_home)
                .HasConstraintName("ForeignKey_Productos_Home_Estados");


            #region Certificado de mantenimiento
            //modelBuilder.Entity<Cer_producto_cliente>()
            //    .HasOne(p => p.clientes)
            //    .WithMany(b => b.cer_producto_cliente)
            //    .HasForeignKey(p => p.id_cliente)
            //    .HasConstraintName("ForeignKey_Cliente_Mantenimiento");

            modelBuilder.Entity<rel_certificado_producto>()
                .HasOne(p => p.certificado)
                .WithMany(b => b.rel_certificado_producto)
                .HasForeignKey(p => p.id_certificado)
                .HasConstraintName("ForeignKey_Producto_Cer_Mantenimiento");

            modelBuilder.Entity<rel_homep_producto>()
                .HasOne(p => p.homep)
                .WithMany(b => b.homep_Productos)
                .HasForeignKey(p => p.id_homep)
                .HasConstraintName("ForeignKey_rel_hpp_productos_cliente");

            modelBuilder.Entity<Cer_producto_cliente>()
                .HasOne(p => p.viaticos)
                .WithMany(b => b.cer_producto_cliente)
                .HasForeignKey(p => p.id_viaticos)
                .HasConstraintName("ForeignKey_Viaticos_Mantenimiento");

            modelBuilder.Entity<Cer_producto_cliente>()
                .HasOne(p => p.labor)
                .WithMany(b => b.cer_producto_cliente)
                .HasForeignKey(p => p.id_labor)
                .HasConstraintName("ForeignKey_Labor_Mantenimiento");

            modelBuilder.Entity<rel_certificado_producto_consumibles>()
                .HasOne(p => p.consumible)
                .WithMany(b => b.cer_producto_cliente)
                .HasForeignKey(p => p.id_consumible)
                .HasConstraintName("ForeignKey_Consumible_Mantenimiento");

            modelBuilder.Entity<rel_certificado_producto_consumibles>()
                .HasOne(p => p.rel_certificado_producto)
                .WithMany(b => b.rel_certificado_producto_consumibles)
                .HasForeignKey(p => p.id_rel_certificado_producto)
                .HasConstraintName("ForeignKey_Prodcuto_cer_Mantenimiento");

            modelBuilder.Entity<rel_consumible_sublinea>()
                .HasOne(p => p.consumible)
                .WithMany(b => b.rel_consumible_sublinea)
                .HasForeignKey(p => p.id_consumible)
                .HasConstraintName("ForeignKey_rel_consumible_sublinea");
            #endregion

        }
    }
}
