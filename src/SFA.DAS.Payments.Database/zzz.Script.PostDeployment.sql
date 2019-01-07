/*
--------------------------------------------------------------------------------------
Post-Deployment Script 
--------------------------------------------------------------------------------------
*/

SET NOCOUNT ON;
GO

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

RAISERROR('		   Extended Property',10,1) WITH NOWAIT;
GO

RAISERROR('		         %s - %s',10,1,'BuildNumber','$(BUILD_BUILDNUMBER)') WITH NOWAIT;
IF NOT EXISTS (SELECT name, value FROM fn_listextendedproperty('BuildNumber', default, default, default, default, default, default))
	EXEC sp_addextendedproperty @name = N'BuildNumber', @value = '$(BUILD_BUILDNUMBER)';  
ELSE
	EXEC sp_updateextendedproperty @name = N'BuildNumber', @value = '$(BUILD_BUILDNUMBER)';  
	
GO

RAISERROR('		         %s - %s',10,1,'BuildBranch','$(BUILD_BRANCHNAME)') WITH NOWAIT;
IF NOT EXISTS (SELECT name, value FROM fn_listextendedproperty('BuildBranch', default, default, default, default, default, default))
	EXEC sp_addextendedproperty @name = N'BuildBranch', @value = '$(BUILD_BRANCHNAME)';  
ELSE
	EXEC sp_updateextendedproperty @name = N'BuildBranch', @value = '$(BUILD_BRANCHNAME)';  
GO

DECLARE @DeploymentTime VARCHAR(35) = CONVERT(VARCHAR(35),GETUTCDATE(),113);
RAISERROR('		         %s - %s',10,1,'DeploymentDatetime',@DeploymentTime) WITH NOWAIT;
IF NOT EXISTS (SELECT name, value FROM fn_listextendedproperty('DeploymentDatetime', default, default, default, default, default, default))
	EXEC sp_addextendedproperty @name = N'DeploymentDatetime', @value = @DeploymentTime;  
ELSE
	EXEC sp_updateextendedproperty @name = N'DeploymentDatetime', @value = @DeploymentTime;  
GO

RAISERROR('		         %s - %s',10,1,'ReleaseName','$(RELEASE_RELEASENAME)') WITH NOWAIT;
IF NOT EXISTS (SELECT name, value FROM fn_listextendedproperty('ReleaseName', default, default, default, default, default, default))
	EXEC sp_addextendedproperty @name = N'ReleaseName', @value = '$(RELEASE_RELEASENAME)';  
ELSE
	EXEC sp_updateextendedproperty @name = N'ReleaseName', @value = '$(BUILD_BRANCHNAME)';  
GO

RAISERROR('		         Drop DisplayDeploymentProperties View.',10,1,'ReleaseName','$(RELEASE_RELEASENAME)') WITH NOWAIT;
IF EXISTS (SELECT * FROM [sys].[objects] WHERE [type] = 'V' AND Name = 'DisplayDeploymentProperties_VW')
BEGIN 
	DROP VIEW [dbo].[DisplayDeploymentProperties_VW];
END
GO

RAISERROR('		        Create DisplayDeploymentProperties View.',10,1,'ReleaseName','$(RELEASE_RELEASENAME)') WITH NOWAIT;
EXEC ('CREATE VIEW [dbo].[DisplayDeploymentProperties_VW]
AS
	SELECT name, value 
	FROM fn_listextendedproperty(default, default, default, default, default, default, default);  
	');

GO

RAISERROR('		   Update User Account Passwords',10,1) WITH NOWAIT;
GO

RAISERROR('			     RO User',10,1) WITH NOWAIT;
ALTER USER [DASPaymentROUser] WITH PASSWORD = N'$(ROUserPassword)';
GO

RAISERROR('			     RW User',10,1) WITH NOWAIT;
ALTER USER [DASPaymentRWUser] WITH PASSWORD = N'$(RWUserPassword)';
GO

RAISERROR('			     DSCI User',10,1) WITH NOWAIT;
ALTER USER [User_DSCI] WITH PASSWORD = N'$(DsciUserPassword)';
GO

RAISERROR('Completed',10,1) WITH NOWAIT;
GO
