/*
	You must restart SQL server after running this script.

	Run this script after deploying the dacpacs from the solution
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

