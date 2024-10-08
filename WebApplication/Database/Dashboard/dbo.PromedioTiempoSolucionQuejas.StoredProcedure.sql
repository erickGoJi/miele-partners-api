USE [db_miele]
GO
/****** Object:  StoredProcedure [dbo].[PromedioTiempoSolucionQuejas]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP PROCEDURE IF EXISTS [dbo].[PromedioTiempoSolucionQuejas]
GO
/****** Object:  StoredProcedure [dbo].[PromedioTiempoSolucionQuejas]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Antonio Acosta Murillo
-- Create date: Nov 2018
-- Description:	Tiempo promedio desde la fecha de creación de la queja, hasta su fecha de cierre
-- =============================================
CREATE PROCEDURE [dbo].[PromedioTiempoSolucionQuejas] 
	@ProductoId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Quejas TABLE
	(
		QuejaId INT NOT NULL,
		FechaCreacion DATE NOT NULL
	);

	INSERT INTO @Quejas ( QuejaId, FechaCreacion )
	SELECT a.QuejaId, MAX(b.Fecha) FechaCreacion
	FROM dbo.ProductosQuejas a
	INNER JOIN dbo.Quejas b
	ON b.Id = a.QuejaId
	WHERE b.Estatus = 0
	AND (@ProductoId IS NULL OR a.ProductoId = @ProductoId)
	GROUP BY a.QuejaId;

	DECLARE @FechaCierreQueja TABLE
	(
			QuejaId INT NOT NULL,
			FechaCierre DATE NOT NULL
	);

	INSERT INTO @FechaCierreQueja ( QuejaId, FechaCierre )
	SELECT QuejaId, MAX(FechaCierre) FechaCierra FROM dbo.Propuestas
	WHERE FechaCierre IS NOT NULL
	GROUP BY QuejaId;

	DECLARE @QuejaDias TABLE ( QuejaId INT NOT NULL, FechaCreacion DATE NOT NULL, FechaCierre DATE NOT NULL, Dias INT NOT NULL );

	INSERT INTO @QuejaDias ( QuejaId, FechaCreacion, FechaCierre, Dias )
	SELECT a.QuejaId,
		   a.FechaCreacion,
		   b.FechaCierre,
		   DATEDIFF(DAY, a.FechaCreacion, b.FechaCierre)  AS Dias
	FROM @Quejas a
	INNER JOIN @FechaCierreQueja b
	ON a.QuejaId = b.QuejaId;

	SELECT AVG(Dias) Resultado
	FROM @QuejaDias;

END
GO
