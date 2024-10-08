USE [db_miele]
GO
/****** Object:  StoredProcedure [dbo].[PorcentajePrimeraInstalacion]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP PROCEDURE IF EXISTS [dbo].[PorcentajePrimeraInstalacion]
GO
/****** Object:  StoredProcedure [dbo].[PorcentajePrimeraInstalacion]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Antonio Acosta Murillo
-- Create date: Nov 2018
-- Description:	Ordenes de servicio Instalados en primera visita (Fecha Primera Agenda = Fecha Completado) / Ordenes de Servicio Completadas de Instalacion
-- =============================================
CREATE PROCEDURE [dbo].[PorcentajePrimeraInstalacion]
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

	DECLARE @OrdenesInstaladasPrimeraVisita FLOAT = NULL;

	SELECT @OrdenesInstaladasPrimeraVisita = COUNT(a.id)
	FROM dbo.Servicio a 
	INNER JOIN dbo.Visita b
	ON b.id_servicio = a.id
	INNER JOIN dbo.Rel_servicio_producto c
	ON b.id = c.id_vista
	INNER JOIN dbo.Cliente_Productos d
	ON d.Id_Cliente = a.id_cliente
    LEFT JOIN dbo.Tecnicos AS e
        ON e.id = b.Tecnicosid
    LEFT JOIN dbo.Cat_Tecnicos_Tipo AS f
        ON f.id = e.id_tipo_tecnico
    LEFT JOIN dbo.Cat_Tecnicos_Sub_Tipo AS g
        ON g.id_tipo = e.id_cat_tecnicos_sub_Tipo
	WHERE (CONVERT(DATE, b.fecha_agendado) = CONVERT(DATE, b.fecha_fin_visita))
	AND (@Garantia IS NULL OR b.garantia = @Garantia)
	AND (@Fecha IS NULL OR a.creado BETWEEN CONVERT(DATE, @Fecha) AND CONVERT(DATE, GETDATE()))
	AND (@TipoServicio IS NULL OR a.id_tipo_servicio = @TipoServicio)
	AND (@SubTipoServicio IS NULL OR a.id_sub_tipo_servicio = @SubTipoServicio)
	AND (@TecnicoId IS NULL OR e.id = @TecnicoId)
	AND (@TipoTecnico IS NULL OR f.id = @TipoTecnico)
	AND (@SubTipoTecnico IS NULL OR g.id_tipo = @SubTipoTecnico)
	GROUP BY a.id

	DECLARE @OrdenesInstaladas FLOAT = NULL;

	SELECT @OrdenesInstaladas = COUNT(a.id)
	FROM dbo.Servicio a 
	INNER JOIN dbo.Visita b
	ON b.id_servicio = a.id
	INNER JOIN dbo.Rel_servicio_producto c
	ON b.id = c.id_vista
	INNER JOIN dbo.Cliente_Productos d
	ON d.Id_Cliente = a.id_cliente
    LEFT JOIN dbo.Tecnicos AS e
        ON e.id = b.Tecnicosid
    LEFT JOIN dbo.Cat_Tecnicos_Tipo AS f
        ON f.id = e.id_tipo_tecnico
    LEFT JOIN dbo.Cat_Tecnicos_Sub_Tipo AS g
        ON g.id_tipo = e.id_cat_tecnicos_sub_Tipo
	WHERE b.fecha_fin_visita IS NOT NULL
	AND (@Garantia IS NULL OR b.garantia = @Garantia)
	AND (@Fecha IS NULL OR a.creado BETWEEN CONVERT(DATE, @Fecha) AND CONVERT(DATE, GETDATE()))
	AND (@TipoServicio IS NULL OR a.id_tipo_servicio = @TipoServicio)
	AND (@SubTipoServicio IS NULL OR a.id_sub_tipo_servicio = @SubTipoServicio)
	AND (@TecnicoId IS NULL OR e.id = @TecnicoId)
	AND (@TipoTecnico IS NULL OR f.id = @TipoTecnico)
	AND (@SubTipoTecnico IS NULL OR g.id_tipo = @SubTipoTecnico)
	GROUP BY a.id

	IF( @OrdenesInstaladasPrimeraVisita IS NOT NULL
	AND @OrdenesInstaladasPrimeraVisita > 0
	AND @OrdenesInstaladas IS NOT NULL
	AND @OrdenesInstaladas > 0 )
	BEGIN
		SELECT CONVERT(DECIMAL(4,2), (@OrdenesInstaladasPrimeraVisita / @OrdenesInstaladas * 100)) 
		AS PorcentajePrimeraInstalacion
	END
	ELSE
	BEGIN
		SELECT NULL AS Resultado;
	END;

END;
GO
