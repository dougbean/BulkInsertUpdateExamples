USE [SandboxDB]
GO

DROP PROCEDURE [dbo].[usp_load_parent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_load_parent] @batch AS dbo.udt_parent readonly
	
AS
BEGIN
	
	DECLARE @MyTableVar TABLE
	(
        [Id] [int]  NOT NULL,
	    [Name] [nvarchar](255) NOT NULL
	); 	
	
	SET NOCOUNT ON;
	    
	INSERT INTO [dbo].[Parent]
           ([Name])
        OUTPUT INSERTED.Id, INSERTED.Name  
        INTO @MyTableVar  
	select mb.Name
		FROM @batch AS mb
	LEFT JOIN Parent m WITH (NOLOCK)
		ON mb.Id = m.Id
	WHERE m.Id IS NULL

	select Id, Name from @MyTableVar
END


GO


