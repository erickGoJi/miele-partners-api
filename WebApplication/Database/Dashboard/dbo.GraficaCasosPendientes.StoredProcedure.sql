USE [db_miele]
GO
/****** Object:  StoredProcedure [dbo].[GraficaCasosPendientes]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP PROCEDURE IF EXISTS [dbo].[GraficaCasosPendientes]
GO
/****** Object:  StoredProcedure [dbo].[GraficaCasosPendientes]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Antonio Acosta Murillo
-- Create date: Nov 2018
-- Description:	Suma de todos los servicios con estatus diferente a 
-- completado y cancelado agrados por días (1-5, 6-10, 11-15, 16-20, 21-25, +26)
-- =============================================
CREATE PROCEDURE [dbo].[GraficaCasosPendientes]
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

	DECLARE @CasosPendientes TABLE ( DiasPendientes INT NULL);

	INSERT INTO @CasosPendientes ( DiasPendientes )
	SELECT DATEDIFF(DAY, fecha_inicio_visita, fecha_agendado)
    FROM dbo.View_Servicio_Vista_Producto_Cliente_Tecnico 
	WHERE id_estatus_servicio NOT IN (15, 13, 16)
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
	WHEN DiasPendientes BETWEEN 1 AND 5 THEN '01-05'
	WHEN DiasPendientes BETWEEN 6 AND 10 THEN '06-10'
	WHEN DiasPendientes BETWEEN 11 AND 15 THEN '11-15'
	WHEN DiasPendientes BETWEEN 16 AND 20 THEN '16-20'
	WHEN DiasPendientes BETWEEN 21 AND 26 THEN '21-26'
	WHEN DiasPendientes > 26 THEN '+26'
	ELSE NULL END AS Rango
	FROM @CasosPendientes
	WHERE DiasPendientes IS NOT NULL) a
	GROUP BY a.Rango

END;
GO
