/*
--------------------------------------------------------------------------------------
Post-Deployment Script 
--------------------------------------------------------------------------------------
*/

SET NOCOUNT ON;
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
