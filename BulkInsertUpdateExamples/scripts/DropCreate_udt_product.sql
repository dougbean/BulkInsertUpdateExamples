USE [SandboxDB]
GO

DROP TYPE [dbo].[udt_product]
GO

CREATE TYPE [dbo].[udt_product] AS TABLE(
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL, --Sql Server defauts to nvarchar(1) if size is not specified
	[Price] [money] NOT NULL
)
GO

