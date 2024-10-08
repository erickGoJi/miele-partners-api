USE [db_miele]
GO
/****** Object:  View [dbo].[View_Servicio_Vista_Producto_Cliente_Tecnico]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP VIEW IF EXISTS [dbo].[View_Servicio_Vista_Producto_Cliente_Tecnico]
GO
/****** Object:  View [dbo].[View_Servicio_Vista_Producto_Cliente_Tecnico]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_Servicio_Vista_Producto_Cliente_Tecnico] AS

SELECT a.id,
       a.Cat_Categoria_Servicioid,
       a.Cat_estatus_servicioid,
       a.IBS,
       a.activar_credito,
       a.actualizado,
       a.actualizadopor,
       a.contacto,
       a.creado,
       a.creadopor,
       a.descripcion_actividades,
       a.fecha_servicio,
       a.id_categoria_servicio,
       a.id_cliente,
       a.id_distribuidor_autorizado,
       a.id_estatus_servicio,
       a.id_motivo_cierre,
       a.id_solicitado_por,
       a.id_solicitud_via,
       a.id_sub_tipo_servicio,
       a.id_tipo_servicio,
       a.no_servicio,
       a.sub_tipo_servicioid,
       b.id AS VisitaId,
       b.Tecnicosid,
       b.actividades_realizar,
       b.asignacion_refacciones,
       b.cantidad,
       b.comprobante,
       b.concepto,
       b.entrega_refacciones,
       b.estatus,
       b.factura,
       b.fecha_deposito,
       b.fecha_entrega_refaccion,
       b.fecha_fin_visita,
       b.fecha_inicio_visita,
       b.fecha_visita,
       b.garantia,
       b.hora,
       b.hora_fin,
       b.id_direccion,
       b.id_servicio,
       b.imagen_firma,
       b.imagen_pago_referenciado,
       b.latitud_fin,
       b.latitud_inicio,
       b.longitud_fin,
       b.longitud_inicio,
       b.no_operacion,
       b.pagado,
       b.pago_pendiente,
       b.pre_diagnostico,
       b.si_acepto_tecnico_refaccion,
       b.terminos_condiciones,
       b.persona_recibe,
       b.fecha_agendado,
       b.url_ppdf_reporte,
       c.id AS Relid,
       c.descripcion_cierre,
       c.estatus AS EstatusRel,
       c.garantia AS Garantiarel,
       c.id_producto,
       c.id_vista,
       c.no_serie,
       c.primera_visita,
       c.reparacion,
	   d.Id AS IdClienteProducto,
       d.Cat_Estatus_CompraId,
       d.Cat_Productosid,
       d.FechaCompra,
       d.FinGarantia,
       d.Id_Cliente AS IdCP,
       d.Id_EsatusCompra,
       d.Id_Producto AS IdProductoCP,
       d.NoOrdenCompra,
       d.NoPoliza,
       d.actualizado AS ActualizadoCP,
       d.actualizadopor AS ActualizadoPorCP,
       d.creado AS CreadoCP,
       d.creadopor AS CreadoPorCP,
       e.id AS TecnicoId,
       e.actualizado AS ActualizadoTecnico,
       e.actualizadopor AS ActualizadoPorTecnico,
       e.color,
       e.creado AS CreadoTecnico,
       e.creadopor AS CreadoPorTecnico,
       e.id_tipo_tecnico,
       e.noalmacen,
       e.id_cat_tecnicos_sub_Tipo,
       f.id AS IdTipoTecnico,
       f.desc_tipo,
       g.id AS IdSubTipoTecnico,
       g.Sub_desc_tipo,
       g.id_tipo
FROM dbo.Servicio AS a
    JOIN dbo.Visita AS b
        ON b.id_servicio = a.id
    JOIN dbo.Rel_servicio_producto AS c
        ON c.id_vista = b.id
	JOIN dbo.Cliente_Productos d
	ON d.Id_Cliente =
	( SELECT TOP (1) id_cliente FROM dbo.Servicio WHERE a.id_cliente = d.Id_Cliente ORDER BY creado DESC)
    JOIN dbo.Tecnicos AS e
        ON e.id = b.Tecnicosid
    LEFT OUTER JOIN dbo.Cat_Tecnicos_Tipo AS f
        ON f.id = e.id_tipo_tecnico
    LEFT OUTER JOIN dbo.Cat_Tecnicos_Sub_Tipo AS g
        ON g.id_tipo = e.id_cat_tecnicos_sub_Tipo;


GO
