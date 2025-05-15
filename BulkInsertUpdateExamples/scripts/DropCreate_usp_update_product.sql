USE [SandboxDB]
GO

DROP PROCEDURE [dbo].[usp_update_product]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

	CREATE PROCEDURE [dbo].[usp_update_product] @batch AS dbo.udt_product readonly
			
	AS
	BEGIN
	
		SET NOCOUNT ON;    
	       
			update m 
			SET      
			m.Name = b.Name,
			m.Price = b.Price
		FROM [dbo].[Product] m 
		LEFT JOIN @batch as b	
		ON b.Id = m.Id 
	
	END		
	
GO


