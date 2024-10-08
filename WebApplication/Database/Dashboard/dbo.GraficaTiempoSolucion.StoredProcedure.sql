USE [db_miele]
GO
/****** Object:  StoredProcedure [dbo].[GraficaTiempoSolucion]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP PROCEDURE IF EXISTS [dbo].[GraficaTiempoSolucion]
GO
/****** Object:  StoredProcedure [dbo].[GraficaTiempoSolucion]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Antonio Acosta Murillo
-- Create date: Dic 2018
-- Description:	Tiempos de solucion de servicio agrupados por días (1-5, 6-10, 11-15, 16-20, 21-25, +26)
-- =============================================
CREATE PROCEDURE [dbo].[GraficaTiempoSolucion]
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
	SELECT DATEDIFF(DAY, creado, fecha_agendado) AS DiasRespuesta
    FROM dbo.View_Servicio_Vista_Producto_Cliente_Tecnico
	WHERE (@Garantia IS NULL OR garantia = @Garantia)
	AND (@Fecha IS NULL OR creado BETWEEN CONVERT(DATE, @Fecha) AND CONVERT(DATE, GETDATE()))
	AND (@TipoServicio IS NULL OR id_tipo_servicio = @TipoServicio)
	AND (@SubTipoServicio IS NULL OR id_sub_tipo_servicio = @SubTipoServicio)
	AND (@TecnicoId IS NULL OR Tecnicosid = @TecnicoId)
	AND (@TipoTecnico IS NULL OR id_tipo_tecnico = @TipoTecnico)
	AND (@SubTipoTecnico IS NULL OR id_cat_tecnicos_sub_Tipo = @SubTipoTecnico)

	SELECT a.Rango, COUNT(*) AS Total
	FROM
	(SELECT CASE 
	WHEN DiasRespuesta BETWEEN 1 AND 5 THEN '01-05'
	WHEN DiasRespuesta BETWEEN 6 AND 10 THEN '06-10'
	WHEN DiasRespuesta BETWEEN 11 AND 15 THEN '11-15'
	WHEN DiasRespuesta BETWEEN 16 AND 20 THEN '16-20'
	WHEN DiasRespuesta BETWEEN 21 AND 26 THEN '21-26'
	WHEN DiasRespuesta > 26 THEN '+26'
	ELSE NULL END AS Rango
	FROM @DiasAgrupados
	WHERE DiasRespuesta IS NOT NULL) a
	GROUP BY a.Rango

END;

--(1-5, 6-10, 11-15, 16-20, 21-25, +26)

--  select case  
--    when score between 0 and 9 then ' 0- 9'
--    when score between 10 and 19 then '10-19'
--    else '20-99' end as range
--  from scores
GO
