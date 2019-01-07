/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS (SELECT * FROM [Payments2].[JobStatus]  WHERE [Id] = 1)
	INSERT INTO [Payments2].[JobStatus]  values (1,'In progress')
GO	

IF NOT EXISTS (SELECT * FROM [Payments2].[JobStatus]  WHERE [Id] = 2)
	INSERT INTO [Payments2].[JobStatus]  VALUES (2,'Completed')
GO	

IF NOT EXISTS (SELECT * FROM [Payments2].[JobStatus]  WHERE [Id] = 3)
	INSERT INTO [Payments2].[JobStatus]  values (3,'completed with errors')
GO	

IF NOT EXISTS (SELECT * FROM [Payments2].[JobEventStatus]  WHERE [Id] = 1)
	INSERT INTO [Payments2].[JobEventStatus] values (1,'Queued')
GO 

IF NOT EXISTS (SELECT * FROM [Payments2].[JobEventStatus]  WHERE [Id] = 2)
	INSERT INTO [Payments2].[JobEventStatus] values (2,'Processing')
GO 
IF NOT EXISTS (SELECT * FROM [Payments2].[JobEventStatus]  WHERE [Id] = 3)
	INSERT INTO [Payments2].[JobEventStatus] values (3,'Completed')
GO 
IF NOT EXISTS (SELECT * FROM [Payments2].[JobEventStatus]  WHERE [Id] = 4)
	INSERT INTO [Payments2].[JobEventStatus] values (4,'Failed')
GO 

IF NOT EXISTS (SELECT * FROM [Payments2].[JobType]  WHERE [Id] = 1)
	INSERT INTO [Payments2].[JobType] values (1,'Earnings job')
GO 

IF NOT EXISTS (SELECT * FROM [Payments2].[JobType]  WHERE [Id] = 2)
	INSERT INTO [Payments2].[JobType] values (2,'Month end job')
GO 
