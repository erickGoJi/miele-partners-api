USE [db_miele]
GO
/****** Object:  StoredProcedure [dbo].[CantidadCasos]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP PROCEDURE IF EXISTS [dbo].[CantidadCasos]
GO
/****** Object:  StoredProcedure [dbo].[CantidadCasos]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Antonio Acosta Murillo
-- Create date: Nov 2018
-- Description:	Cantidad de casos por producto, tipos de servicio, etc
-- =============================================
CREATE PROCEDURE [dbo].[CantidadCasos]
@Garantia BIT = NULL,
@PeriodoTiempo NVARCHAR(15) = NULL,
@TipoServicio INT = NULL,
@SubTipoServicio INT = NULL,
@TecnicoId INT = NULL,
@TipoTecnico INT = NULL,
@SubTipoTecnico INT = NULL,
@ProductoId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @TotalCasos TABLE
	(
		id_producto INT NOT NULL,
		TotalCasosPorProducto INT NOT NULL
	);

	DECLARE @Fecha DATETIME = NULL;
	SELECT @Fecha = dbo.PeriodoTiempo(@PeriodoTiempo);

	INSERT INTO @TotalCasos (id_producto, TotalCasosPorProducto)
	SELECT id_producto, COUNT(*) AS TotalCasosPorProducto
	FROM dbo.View_Servicio_Vista_Producto_Cliente_Tecnico
	WHERE (@Garantia IS NULL OR garantia = @Garantia)
	AND (@Fecha IS NULL OR creado BETWEEN CONVERT(DATE, @Fecha) AND CONVERT(DATE, GETDATE()))
	AND (@TipoServicio IS NULL OR id_tipo_servicio = @TipoServicio)
	AND (@SubTipoServicio IS NULL OR id_sub_tipo_servicio = @SubTipoServicio)
	AND (@TecnicoId IS NULL OR Tecnicosid = @TecnicoId)
	AND (@TipoTecnico IS NULL OR id_tipo_tecnico = @TipoTecnico)
	AND (@SubTipoTecnico IS NULL OR id_cat_tecnicos_sub_Tipo = @SubTipoTecnico)
	AND (@ProductoId IS NULL OR id_producto = @ProductoId)
	GROUP BY id_producto;

	SELECT SUM(TotalCasosPorProducto) AS Resultado 
	FROM @TotalCasos;

END;
GO
