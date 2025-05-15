
--- CREATE TABLES ----

IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Parent]') AND type in (N'U'))
BEGIN
	
	SET ANSI_NULLS ON

	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[Parent](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	 CONSTRAINT [PK_Parent] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] 

	Print N'Created Parent Table';
END
ELSE
	Print N'Parent Table already exists';
GO


IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Child]') AND type in (N'U'))
BEGIN
	SET ANSI_NULLS ON

	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[Child](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	 CONSTRAINT [PK_Child] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] 

	ALTER TABLE [dbo].[Child]  WITH CHECK ADD  CONSTRAINT [FK_Child_Mock] FOREIGN KEY([ParentId])
	REFERENCES [dbo].[Parent] ([ID])
	ON DELETE CASCADE

	ALTER TABLE [dbo].[Child] CHECK CONSTRAINT [FK_Child_Mock]	

	Print N'Created Child Table';
END
ELSE
	Print N'Child Table already exists';
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND type in (N'U'))
BEGIN
	SET ANSI_NULLS ON

	SET QUOTED_IDENTIFIER ON

	CREATE TABLE [dbo].[Product](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](255) NOT NULL,
		[Price] [money] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	Print N'Created Product Table';
END
ELSE
	Print N'Product Table already exists';
GO

--- CREATE USER DEFINED TABLE TYPES ----

IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'udt_parent')
BEGIN
	
	CREATE TYPE [dbo].[udt_parent] AS TABLE(
		[Id] [int] NULL,
		[Name] [nvarchar](255) NOT NULL --Sql Server defauts to nvarchar(1) if size is not specified
	)
	Print N'Created udt_parent';
END
ELSE
	Print N'udt_parent already exists';
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'udt_child')
BEGIN
	
	CREATE TYPE [dbo].[udt_child] AS TABLE(
		[Id] [int] NULL,
		[ParentId] [int] NOT NULL,
		[Name] [nvarchar](255) NOT NULL --Sql Server defauts to nvarchar(1) if size is not specified
	)
	Print N'Created udt_child';
END
ELSE
	Print N'udt_child already exists';
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'udt_product')
BEGIN
	
	CREATE TYPE [dbo].[udt_product] AS TABLE(
		[Id] [int] NOT NULL,
		[Name] [nvarchar](255) NOT NULL, --Sql Server defauts to nvarchar(1) if size is not specified
		[Price] [money] NOT NULL
	)

	Print N'Created udt_product';
END
ELSE
	Print N'udt_product already exists';
GO


--- STORED PROCEDURES ----

IF NOT EXISTS (SELECT type_desc, type FROM sys.procedures WITH(NOLOCK) WHERE NAME = 'usp_load_parent' AND type = 'P')
BEGIN
execute ('	

		CREATE PROCEDURE [dbo].[usp_load_parent] @batch AS dbo.udt_parent readonly
			
		AS
		BEGIN
			
			DECLARE @MyTableVar TABLE
			(
				[Id] [int]  NOT NULL,
				[Name] [nvarchar](max) NOT NULL
			); 
			
			-- SET NOCOUNT ON added to prevent extra result sets from
			-- interfering with SELECT statements.
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

	')
   Print N'usp_load_parent';
END
ELSE
	Print N'usp_load_parent already exists';
GO


IF NOT EXISTS (SELECT type_desc, type FROM sys.procedures WITH(NOLOCK) WHERE NAME = 'usp_load_child' AND type = 'P')
BEGIN
execute ('
		CREATE PROCEDURE [dbo].[usp_load_child] @batch AS dbo.udt_child readonly
		AS
		BEGIN
			
			SET NOCOUNT ON;
				
			INSERT INTO [dbo].[Child]
			(
				[ParentId],
				[Name]
			)
			select
				b.ParentId,
				b.Name
				FROM @batch AS b
			LEFT JOIN Child o WITH (NOLOCK)
				ON b.Id = o.Id
			WHERE o.Id IS NULL
		END
	')
   Print N'usp_load_child created';
END
ELSE
	Print N'usp_load_child already exists';
GO


IF NOT EXISTS (SELECT type_desc, type FROM sys.procedures WITH(NOLOCK) WHERE NAME = 'usp_update_parent' AND type = 'P')
BEGIN
execute ('
		CREATE PROCEDURE [dbo].[usp_update_parent] @batch AS dbo.udt_parent readonly
		-- Add the parameters for the stored procedure here
		AS
		BEGIN
			
			update m
				 SET m.Name = b.Name
				 FROM [dbo].[Parent] m
				 left join @batch as b
				 on b.ID = m.ID
				 where b.ID = m.ID	
		END
	')
   Print N'usp_update_parent created';
END
ELSE
	Print N'usp_update_parent already exists';
GO     
	 
IF NOT EXISTS (SELECT type_desc, type FROM sys.procedures WITH(NOLOCK) WHERE NAME = 'usp_load_product' AND type = 'P')
BEGIN
execute ('

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
	')
   Print N'usp_load_product created';
END
ELSE
	Print N'usp_load_product already exists';
GO

IF NOT EXISTS (SELECT type_desc, type FROM sys.procedures WITH(NOLOCK) WHERE NAME = 'usp_update_product' AND type = 'P')
BEGIN
execute ('

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
	')
   Print N'usp_update_product  created';
END
ELSE
	Print N'usp_update_product already exists';
GO