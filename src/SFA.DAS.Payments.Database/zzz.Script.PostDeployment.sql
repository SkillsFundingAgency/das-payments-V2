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
	INSERT INTO [Payments2].[JobStatus]  values (3,'Completed with errors')
GO	

IF NOT EXISTS (SELECT * FROM [Payments2].[JobStatus]  WHERE [Id] = 4)
	INSERT INTO [Payments2].[JobStatus]  values (4,'Timed out due to idle job')
GO

IF NOT EXISTS (SELECT * FROM [Payments2].[JobStatus]  WHERE [Id] = 5)
	INSERT INTO [Payments2].[JobStatus]  values (5,'DC Tasks Failed')
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
	INSERT INTO [Payments2].[JobType] values (2,'Period end start job')
else 
	UPDATE [Payments2].[JobType] SET [Description] = 'Period end start job' WHERE [Id] = 2
GO 

IF NOT EXISTS (SELECT * FROM [Payments2].[JobType]  WHERE [Id] = 3)
	INSERT INTO [Payments2].[JobType] VALUES (3,'Component test earnings job')
GO 

IF NOT EXISTS (SELECT * FROM [Payments2].[JobType]  WHERE [Id] = 4)
	INSERT INTO [Payments2].[JobType] VALUES (4,'Component test month end job')
GO 

IF NOT EXISTS (SELECT * FROM [Payments2].[JobType]  WHERE [Id] = 5)
	INSERT INTO [Payments2].[JobType] values (5,'Period end run job')
GO 

IF NOT EXISTS (SELECT * FROM [Payments2].[JobType]  WHERE [Id] = 6)
	INSERT INTO [Payments2].[JobType] values (6,'Period end stop job')
GO 

IF NOT EXISTS (SELECT * FROM [Payments2].[JobType]  WHERE [Id] = 7)
	INSERT INTO [Payments2].[JobType] values (7,'Period end submission window validation job')
GO 

MERGE INTO [Payments2].[ApprenticeshipStatus]	 AS Target
USING (VALUES
(1	, N'Active'),
(2	, N'Paused'),
(3 , N'Stopped'),
(4 , N'Inactive')
) AS Source ([Id],[Description])
ON (Target.[Id] = Source.[Id])
WHEN MATCHED AND
  ( NULLIF(Source.[Description], Target.[Description]) IS NOT NULL) THEN
 UPDATE SET [Description] = Source.[Description]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([Id],[Description]) VALUES(Source.[Id],Source.[Description])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;
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

/* ---- Well-known UKPRNs for DC integration testing ---- */
;with KnownUkprns as (
	select '10000833' as Ukprn union
	select '10001144' union
	select '10001326' union
	select '10001436' union
	select '10001548' union
	select '10001971' union
	select '10002005' union
	select '10002815' union
	select '10002843' union
	select '10002919' union
	select '10002935' union
	select '10003538' union
	select '10004180' union
	select '10004760' union
	select '10005583' union
	select '10005926' union
	select '10006432' union
	select '10006519' union
	select '10006574' union
	select '10006847' union
	select '10007419' union
	select '10007635' union
	select '10007643' union
	select '10008227' union
	select '10010178' union
	select '10010939' union
	select '10013222' union
	select '10018361' union
	select '10020395' union
	select '10027518' union
	select '10028120' union
	select '10030102' union
	select '10030571' union
	select '10030758' union
	select '10031093' union
	select '10031408' union
	select '10032250' union
	select '10033904' union
	select '10034279' union
	select '10036176' union
	select '10037830' union
	select '10038872' union
	select '10044985' union
	select '10045119' union
	select '10046078' union
	select '10046354' union
	select '10052903' union
	select '10057010' union
	select '10058123' union
	select '10063506'
	)
insert into Payments2.TestingProvider (Ukprn)
    select distinct Ukprn
    from KnownUkprns t
    where not exists (select 1 from Payments2.TestingProvider t2 where t2.Ukprn = t.Ukprn);
GO

MERGE INTO [Metrics].[SubmissionEarningType]	 AS Target
USING (VALUES
(1	, N'DC Earnings'),
(2	, N'DAS Earnings')
) AS Source ([Id],[Description])
ON (Target.[Id] = Source.[Id])
WHEN MATCHED AND
  ( NULLIF(Source.[Description], Target.[Description]) IS NOT NULL) THEN
 UPDATE SET [Description] = Source.[Description]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([Id],[Description]) VALUES(Source.[Id],Source.[Description])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;
 GO
