USE [db_miele]
GO
/****** Object:  StoredProcedure [dbo].[PromedioTiempoReaccionQuejas]    Script Date: 10-Dec-18 12:37:50 AM ******/
DROP PROCEDURE IF EXISTS [dbo].[PromedioTiempoReaccionQuejas]
GO
/****** Object:  StoredProcedure [dbo].[PromedioTiempoReaccionQuejas]    Script Date: 10-Dec-18 12:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Antonio Acosta Murillo
-- Create date: Nov 2018
-- Description:	Tiempo promedio desde la fecha de creación de la queja, hasta la fecha de primer contacto con cliente
-- =============================================
CREATE PROCEDURE [dbo].[PromedioTiempoReaccionQuejas]
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

	DECLARE @SolucionPrimerContacto TABLE
	(
			QuejaId INT NOT NULL,
			FechaPrimerContacto DATE NOT NULL
	);

	INSERT INTO @SolucionPrimerContacto ( QuejaId, FechaPrimerContacto )
	SELECT QuejaId, MIN(Fecha) FechaPrimerContacto FROM dbo.Propuestas
	GROUP BY QuejaId;

	DECLARE @QuejaDias TABLE ( QuejaId INT NOT NULL, FechaCreacion DATE NOT NULL, FechaPrimerContacto DATE NOT NULL, Dias INT NOT NULL );

	INSERT INTO @QuejaDias ( QuejaId, FechaCreacion, FechaPrimerContacto, Dias )
	SELECT a.QuejaId,
		   a.FechaCreacion,
		   b.FechaPrimerContacto,
		   DATEDIFF(day, a.FechaCreacion, b.FechaPrimerContacto)  AS Dias
	FROM @Quejas a
	INNER JOIN @SolucionPrimerContacto b
	ON a.QuejaId = b.QuejaId;

	SELECT AVG(Dias) Resultado
	FROM @QuejaDias;

END
GO
