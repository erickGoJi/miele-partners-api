USE [db_miele]
GO
/****** Object:  StoredProcedure [dbo].[PorcentajeServiciosResultosPrimeraVisita]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP PROCEDURE IF EXISTS [dbo].[PorcentajeServiciosResultosPrimeraVisita]
GO
/****** Object:  StoredProcedure [dbo].[PorcentajeServiciosResultosPrimeraVisita]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Antonio Acosta Murillo
-- Create date: Nov 2018
-- Description:	Porcentaje de servicios resueltos en primera visita
-- =============================================
CREATE PROCEDURE [dbo].[PorcentajeServiciosResultosPrimeraVisita]
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

	DECLARE @ServiciosPrimeraVisita FLOAT;
	
	DECLARE @Fecha DATETIME = NULL;
	SELECT @Fecha = dbo.PeriodoTiempo(@PeriodoTiempo);

	SELECT @ServiciosPrimeraVisita = COUNT(*)
	FROM dbo.View_Servicio_Vista_Producto_Cliente_Tecnico
	WHERE CONVERT(DATE, fecha_agendado) = CONVERT(DATE, fecha_fin_visita)
	AND (@Garantia IS NULL OR garantia = @Garantia)
	AND (@Fecha IS NULL OR creado BETWEEN CONVERT(DATE, @Fecha) AND CONVERT(DATE, GETDATE()))
	AND (@TipoServicio IS NULL OR id_tipo_servicio = @TipoServicio)
	AND (@SubTipoServicio IS NULL OR id_sub_tipo_servicio = @SubTipoServicio)
	AND (@TecnicoId IS NULL OR Tecnicosid = @TecnicoId)
	AND (@TipoTecnico IS NULL OR id_tipo_tecnico = @TipoTecnico)
	AND (@SubTipoTecnico IS NULL OR id_cat_tecnicos_sub_Tipo = @SubTipoTecnico);

	DECLARE @ServiciosResultos FLOAT;

	SELECT @ServiciosResultos = COUNT(*)
	FROM dbo.View_Servicio_Vista_Producto_Cliente_Tecnico
	WHERE fecha_fin_visita IS NOT NULL;

	SELECT CONVERT(DECIMAL(4,2), (@ServiciosPrimeraVisita/@ServiciosResultos * 100)) 
	AS Resultado;

END;



GO
