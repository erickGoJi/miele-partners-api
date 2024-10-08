USE [db_miele]
GO
/****** Object:  StoredProcedure [dbo].[GraficaTiempoRespuesta]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP PROCEDURE IF EXISTS [dbo].[GraficaTiempoRespuesta]
GO
/****** Object:  StoredProcedure [dbo].[GraficaTiempoRespuesta]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Antonio Acosta Murillo
-- Create date: Dic 2018
-- Description:	Tiempos de respuesta de servicio agrupados por días (1-3, 4-6, +6)
-- =============================================
CREATE PROCEDURE [dbo].[GraficaTiempoRespuesta]
@Garantia BIT = NULL,
@PeriodoTiempo NVARCHAR(15) = NULL,
@TipoServicio INT = NULL,
@SubTipoServicio INT = NULL,
@TecnicoId INT = NULL,
@TipoTecnico INT = NULL,
@SubTipoTecnico INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @Fecha DATETIME = NULL;
	SELECT @Fecha = dbo.PeriodoTiempo(@PeriodoTiempo);

	DECLARE @DiasAgrupados TABLE ( DiasRespuesta INT NULL);

	INSERT INTO @DiasAgrupados ( DiasRespuesta )
    SELECT DATEDIFF(DAY, creado, fecha_agendado)
    FROM dbo.View_Servicio_Vista_Producto_Cliente_Tecnico
	WHERE Cat_estatus_servicioid NOT IN (13, 16)
	AND (@Garantia IS NULL OR garantia = @Garantia)
	AND (@Fecha IS NULL OR creado BETWEEN CONVERT(DATE, @Fecha) AND CONVERT(DATE, GETDATE()))
	AND (@TipoServicio IS NULL OR id_tipo_servicio = @TipoServicio)
	AND (@SubTipoServicio IS NULL OR id_sub_tipo_servicio = @SubTipoServicio)
	AND (@TecnicoId IS NULL OR Tecnicosid = @TecnicoId)
	AND (@TipoTecnico IS NULL OR id_tipo_tecnico = @TipoTecnico)
	AND (@SubTipoTecnico IS NULL OR id_cat_tecnicos_sub_Tipo = @SubTipoTecnico);

	SELECT a.Rango, COUNT(*) AS Total
	FROM
	(SELECT CASE 
	WHEN DiasRespuesta BETWEEN 1 AND 3 THEN '01-03'
	WHEN DiasRespuesta BETWEEN 4 AND 6 THEN '04-06'
	WHEN DiasRespuesta > 6 THEN '+6'
	ELSE NULL END AS Rango
	FROM @DiasAgrupados
	WHERE DiasRespuesta IS NOT NULL) a
	GROUP BY a.Rango

END;
GO
