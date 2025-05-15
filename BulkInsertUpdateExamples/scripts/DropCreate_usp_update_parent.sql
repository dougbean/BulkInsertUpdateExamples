USE [SandboxDB]
GO

DROP PROCEDURE [dbo].[usp_update_parent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

	CREATE PROCEDURE [dbo].[usp_update_parent] @batch AS dbo.udt_parent readonly
	
	AS
	BEGIN
	
		update m
			 SET m.Name = b.Name
			 FROM [dbo].[Parent] m
			 left join @batch as b
			 on b.ID = m.ID
			 where b.ID = m.ID	
	END
GO


