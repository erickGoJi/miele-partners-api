USE [db_miele]
GO
/****** Object:  StoredProcedure [dbo].[PorcentajeDiasRespuestaMayor]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP PROCEDURE IF EXISTS [dbo].[PorcentajeDiasRespuestaMayor]
GO
/****** Object:  StoredProcedure [dbo].[PorcentajeDiasRespuestaMayor]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Antonio Acosta Murillo
-- Create date: Nov 2018
-- Description:	Porcentaje Dias de Respuesta Mayor 6
-- =============================================
CREATE PROCEDURE [dbo].[PorcentajeDiasRespuestaMayor]
@Garantia BIT = NULL,
@TipoServicio INT = NULL,
@SubTipoServicio INT = NULL,
@TecnicoId INT = NULL,
@TipoTecnico INT = NULL,
@SubTipoTecnico INT = NULL,
@Dia INT = 6
AS
BEGIN
    SET NOCOUNT ON;

	IF (@Dia IS NULL)
	BEGIN
		SET @Dia = 6;
	END;

	DECLARE @DiasDeRespuestaMayor FLOAT = NULL;

	SELECT @DiasDeRespuestaMayor = COUNT(*)
    FROM dbo.View_Servicio_Vista_Producto_Cliente_Tecnico
	WHERE Cat_estatus_servicioid NOT IN (13, 16) 
	AND DATEDIFF(DAY, creado, fecha_agendado) > @Dia
	AND (@Garantia IS NULL OR garantia = @Garantia)
	AND (@TipoServicio IS NULL OR id_tipo_servicio = @TipoServicio)
	AND (@SubTipoServicio IS NULL OR id_sub_tipo_servicio = @SubTipoServicio)
	AND (@TecnicoId IS NULL OR Tecnicosid = @TecnicoId)
	AND (@TipoTecnico IS NULL OR id_tipo_tecnico = @TipoTecnico)
	AND (@SubTipoTecnico IS NULL OR id_cat_tecnicos_sub_Tipo = @SubTipoTecnico);

	DECLARE @Total FLOAT = NULL;

	SELECT @Total = COUNT(*)
	FROM dbo.View_Servicio_Vista_Producto_Cliente_Tecnico
	WHERE Cat_estatus_servicioid NOT IN (13, 16) 
	AND DATEDIFF(DAY, creado, fecha_agendado) IS NOT NULL;

	IF( @DiasDeRespuestaMayor IS NOT NULL
	AND @DiasDeRespuestaMayor > 0
	AND @Total IS NOT NULL
	AND @Total > 0 )
	BEGIN
		SELECT CONVERT(DECIMAL(4,2), (@DiasDeRespuestaMayor/@Total) * 100)
		AS Resultado; 
	END
	ELSE
	BEGIN
		SELECT NULL AS Resultado;
	END;

END;
GO
