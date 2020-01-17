/*
	You must restart SQL server after running this script.

	This script MUST only be used on dev environments as it has a password hardcoded.
*/


/*
	Configure the server
*/
sp_configure 'contained database authentication', 1;  
GO  
RECONFIGURE;  
GO  

USE [master]
GO
EXEC xp_instance_regwrite N'HKEY_LOCAL_MACHINE', N'Software\Microsoft\MSSQLServer\MSSQLServer', N'LoginMode', REG_DWORD, 2
GO



/*
Create Logging DB
*/
CREATE DATABASE [AppLog]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'AppLog', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\AppLog.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'AppLog_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\AppLog_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [AppLog].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [AppLog] SET  READ_WRITE 
GO

USE [AppLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Message] [nvarchar](max) NULL,
    [MessageTemplate] [nvarchar](max) NULL,
    [Level] [nvarchar](128) NULL,
    [TimeStampUTC] [datetime] NOT NULL,
    [Exception] [nvarchar](max) NULL,
    [MachineName] [nvarchar](max) NULL,
    [ProcessName] [nvarchar](max) NULL,
    [ThreadId] [nvarchar](max) NULL,
    [CallerName] [nvarchar](max) NULL,
    [SourceFile] [nvarchar](max) NULL,
    [LineNumber] [int] NULL,
    [JobId] [nvarchar](max) NULL,
    [TaskKey] [nvarchar](max) NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


/*
	Create Payments DB
*/
CREATE DATABASE [SFA.DAS.Payments.Database]
 CONTAINMENT = PARTIAL
 ON  PRIMARY 
( NAME = N'SFA.DAS.Payments.Database', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SFA.DAS.Payments.Database_Primary.mdf' , SIZE = 204800KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SFA.DAS.Payments.Database_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SFA.DAS.Payments.Database_Primary.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SFA.DAS.Payments.Database].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

USE [SFA.DAS.Payments.Database]
GO
ALTER DATABASE [SFA.DAS.Payments.Database] SET  READ_WRITE 
GO



/*
	Create databases from Payments v1 
*/
CREATE DATABASE [SFA.DAS.Payments.Database.Legacy]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SFA.DAS.Payments.Database.Legacy', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SFA.DAS.Payments.Database.Legacy_Primary.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SFA.DAS.Payments.Database.Legacy_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SFA.DAS.Payments.Database.Legacy_Primary.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SFA.DAS.Payments.Database.Legacy].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO


ALTER DATABASE [SFA.DAS.Payments.Database.Legacy] SET  READ_WRITE 
GO
USE [master]
GO
CREATE DATABASE [SFA.DAS.Payments.Database.Legacy.Payments]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SFA.DAS.Payments.Database.Legacy.Payments', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SFA.DAS.Payments.Database.Legacy.Payments_Primary.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SFA.DAS.Payments.Database.Legacy.Payments_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SFA.DAS.Payments.Database.Legacy.Payments_Primary.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SFA.DAS.Payments.Database.Legacy.Payments].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [SFA.DAS.Payments.Database.Legacy.Payments] SET  READ_WRITE 
GO



/*
	Create default user and give appropriate permissions
*/
CREATE LOGIN [SFActor] WITH PASSWORD=N'SFActor', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

USE [AppLog]

CREATE USER [SFActor] FOR LOGIN [SFActor]

USE [AppLog]

ALTER ROLE [db_owner] ADD MEMBER [SFActor]

USE [SFA.DAS.Payments.Database]

CREATE USER [SFActor] FOR LOGIN [SFActor]

USE [SFA.DAS.Payments.Database]

ALTER ROLE [db_owner] ADD MEMBER [SFActor]

USE [SFA.DAS.Payments.Database.Legacy]

CREATE USER [SFActor] FOR LOGIN [SFActor]

USE [SFA.DAS.Payments.Database.Legacy]

ALTER ROLE [db_owner] ADD MEMBER [SFActor]

USE [SFA.DAS.Payments.Database.Legacy.Payments]

CREATE USER [SFActor] FOR LOGIN [SFActor]

USE [SFA.DAS.Payments.Database.Legacy.Payments]

ALTER ROLE [db_owner] ADD MEMBER [SFActor]

