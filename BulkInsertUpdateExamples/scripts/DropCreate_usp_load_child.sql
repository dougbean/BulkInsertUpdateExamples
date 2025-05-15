USE [SandboxDB]
GO

DROP PROCEDURE [dbo].[usp_load_child]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_load_child] @batch AS dbo.udt_child readonly
AS
BEGIN
	
	SET NOCOUNT ON;
	    
	INSERT INTO [dbo].[Child]
  	(
		[MockId],
		[Name]
	)
	select
		b.MockId,
		b.Name
		FROM @batch AS b
	LEFT JOIN Child o WITH (NOLOCK)
		ON b.Id = o.Id
	WHERE o.Id IS NULL
END
GO


