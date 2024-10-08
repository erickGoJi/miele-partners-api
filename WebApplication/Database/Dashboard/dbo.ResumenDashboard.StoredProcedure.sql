USE [db_miele]
GO
/****** Object:  StoredProcedure [dbo].[ResumenDashboard]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP PROCEDURE IF EXISTS [dbo].[ResumenDashboard]
GO
/****** Object:  StoredProcedure [dbo].[ResumenDashboard]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Antonio Acosta Murillo
-- Create date: Nov 2018
-- Description:	
-- total pendientes: suma de todos los servicios con estatus diferente a completado y cancelado
-- porcentaje reparacion primera visita: Toatal días / Total servicio (te paso un aimagen con un ejemplo que hicimos con el cliente)
-- promedio tiempos de respuesta: suma del total de tiempo de respuesta por servicio / total de servicios
-- Promedio tiempos de solucion:  suma del total de tiempo de solucion por servicio / total de servicios
-- =============================================
CREATE PROCEDURE [dbo].[ResumenDashboard]
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

	DECLARE @Resultado TABLE ( 
	TotalPendientes INT NULL, 
	PorcentajeServiciosResultosPrimeraVisita DECIMAL(4,2) NULL, 
	PromedioTiempoRespuesta INT NULL,
	PromedioTiempoSolucion INT NULL
	);

	INSERT INTO @Resultado ( TotalPendientes )
	EXEC dbo.CantidadCasos 
	@Garantia = @Garantia,
	@PeriodoTiempo = @Fecha,
	@TipoServicio = @TipoServicio,
	@SubTipoServicio = @SubTipoServicio,
	@TecnicoId = @TecnicoId,
	@TipoTecnico = @TipoTecnico,
	@SubTipoTecnico = @SubTipoTecnico
	
	INSERT INTO @Resultado ( PorcentajeServiciosResultosPrimeraVisita )
	EXEC [dbo].[PorcentajeServiciosResultosPrimeraVisita] 
	@Garantia = @Garantia,
	@PeriodoTiempo = @Fecha,
	@TipoServicio = @TipoServicio,
	@SubTipoServicio = @SubTipoServicio,
	@TecnicoId = @TecnicoId,
	@TipoTecnico = @TipoTecnico,
	@SubTipoTecnico = @SubTipoTecnico

	INSERT INTO @Resultado ( PromedioTiempoRespuesta )
	EXEC [dbo].[PromedioDiasRespuesta] 
	@Garantia = @Garantia,
	@PeriodoTiempo = @Fecha,
	@TipoServicio = @TipoServicio,
	@SubTipoServicio = @SubTipoServicio,
	@TecnicoId = @TecnicoId,
	@TipoTecnico = @TipoTecnico,
	@SubTipoTecnico = @SubTipoTecnico

	INSERT INTO @Resultado ( PromedioTiempoSolucion )
	EXEC [dbo].[PromedioDiasSolucion] 
	@Garantia = @Garantia,
	@PeriodoTiempo = @Fecha,
	@TipoServicio = @TipoServicio,
	@SubTipoServicio = @SubTipoServicio,
	@TecnicoId = @TecnicoId,
	@TipoTecnico = @TipoTecnico,
	@SubTipoTecnico = @SubTipoTecnico

	SELECT MAX(TotalPendientes) AS TotalPendientes,
           MAX(PorcentajeServiciosResultosPrimeraVisita) AS PorcentajeServiciosResultosPrimeraVisita,
           MAX(PromedioTiempoRespuesta) AS PromedioTiempoRespuesta,
           MAX(PromedioTiempoSolucion) AS PromedioTiempoSolucion
	FROM @Resultado

END;
GO
