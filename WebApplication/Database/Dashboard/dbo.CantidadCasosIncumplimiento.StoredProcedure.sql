USE [db_miele]
GO
/****** Object:  StoredProcedure [dbo].[CantidadCasosIncumplimiento]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP PROCEDURE IF EXISTS [dbo].[CantidadCasosIncumplimiento]
GO
/****** Object:  StoredProcedure [dbo].[CantidadCasosIncumplimiento]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Antonio Acosta Murillo
-- Create date: Nov 2018
-- Description:	Cantidad de Casos con incumplimiento
-- =============================================
CREATE PROCEDURE [dbo].[CantidadCasosIncumplimiento]
@Garantia BIT = NULL,
@TipoServicio INT = NULL,
@SubTipoServicio INT = NULL,
@TecnicoId INT = NULL,
@TipoTecnico INT = NULL,
@SubTipoTecnico INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

	SELECT COUNT(*) AS Resultado
    FROM dbo.View_Servicio_Vista_Producto_Cliente_Tecnico 
	WHERE (DATEDIFF(MINUTE, fecha_inicio_visita, fecha_agendado) / 60) > 30
	AND (@Garantia IS NULL OR garantia = @Garantia)
	AND (@TipoServicio IS NULL OR id_tipo_servicio = @TipoServicio)
	AND (@SubTipoServicio IS NULL OR id_sub_tipo_servicio = @SubTipoServicio)
	AND (@TecnicoId IS NULL OR Tecnicosid = @TecnicoId)
	AND (@TipoTecnico IS NULL OR id_tipo_tecnico = @TipoTecnico)
	AND (@SubTipoTecnico IS NULL OR id_cat_tecnicos_sub_Tipo = @SubTipoTecnico);

END;

GO
