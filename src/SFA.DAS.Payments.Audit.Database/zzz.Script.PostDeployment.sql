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

RAISERROR('		   Update User Account Passwords',10,1) WITH NOWAIT;
GO

RAISERROR('			     RO User',10,1) WITH NOWAIT;
ALTER USER [DASPaymentAuditROUser] WITH PASSWORD = N'$(AuditROUserPassword)';
GO

RAISERROR('			     RW User',10,1) WITH NOWAIT;
ALTER USER [DASPaymentAuditRWUser] WITH PASSWORD = N'$(AuditRWUserPassword)';
GO

RAISERROR('Completed',10,1) WITH NOWAIT;
GO