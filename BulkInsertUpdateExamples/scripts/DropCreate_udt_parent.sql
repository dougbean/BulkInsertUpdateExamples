USE [SandboxDB]
GO

DROP TYPE [dbo].[udt_parent]
GO

CREATE TYPE [dbo].[udt_parent] AS TABLE(
	[Id] [int] NULL,
	[Name] [nvarchar](255) NOT NULL --Sql Server defauts to nvarchar(1) if size is not specified
)
GO


