/*Create schemas*/
CREATE SCHEMA [Reservation]
GO

CREATE SCHEMA [Member]
GO

CREATE SCHEMA [Book]
GO

/*Create Tables*/
CREATE TABLE [Reservation].[Reservation](
	[CorrelationID] [uniqueidentifier] NULL,
	[MemberID] [int] NULL,
	[ISBN] [nvarchar](50) NULL,
	[Status] [nvarchar](20) NULL
) ON [PRIMARY]
GO

CREATE TABLE [Member].[Member](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[CanBorrow] [bit] NOT NULL,
	[ISBN] [nvarchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [Book].[Book](
	[ISBN] [nvarchar](50) NULL,
	[Name] [nvarchar](max) NULL,
	[Author] [nvarchar](max) NULL,
	[Lock] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


