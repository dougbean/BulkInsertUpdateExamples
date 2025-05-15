USE [SandboxDB]
GO

DROP PROCEDURE [dbo].[usp_load_product]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

	CREATE PROCEDURE [dbo].[usp_load_product] @batch AS dbo.udt_product readonly
	
	AS
	BEGIN

		DECLARE @MyTableVar TABLE
		(
			[Id] [int] NOT NULL, 
			[Name] [nvarchar](255) NOT NULL, 
			[Price] [money] NOT NULL 
		)
    
		SET NOCOUNT ON;
	    
		INSERT INTO [dbo].[Product]
		(      
			[Name],
			[Price]
		)          
			OUTPUT  
			INSERTED.Id,
			INSERTED.Name,
			INSERTED.Price         
			INTO @MyTableVar 
				select        
				mb.Name,
				mb.Price
		FROM @batch AS mb
		LEFT JOIN Product m WITH (NOLOCK)
		ON mb.Id = m.Id 
		WHERE m.Id IS NULL

		select Id, Name, Price from @MyTableVar
	END
	
GO


