USE [SandboxDB]
GO

DROP TYPE [dbo].[udt_child]
GO

CREATE TYPE [dbo].[udt_child] AS TABLE(
	[Id] [int] NULL,
	[ParentId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL --Sql Server defauts to nvarchar(1) if size is not specified
)
GO


