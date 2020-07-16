/* 
PreDeployment script to cater for existing duplicates prior to adding unique constraints to prevent future duplicates

this covers most of the scenarios but it will require deploying twice on any environment where DuplicateNumber doesn't exists 

If this script was executed as standard script and DuplicateNumber doesn't exists at all in database the dacPack will throw compiler error during pre-deployment stage
that is the reason this pre-Deploy script needs to be executed using sp_executesql, this error only occures for existing environments and does not affects fresh new installs

after using sp_executesql Deployment will still fail for existing environments because of create UX_..._LogicalDuplicates but importantly the DuplicateNumber column will be created during the first run 
therefore running same release again will cause this PreDeploy script to execute correcly Which in turn means create UX_..._LogicalDuplicates will also be successful

*/

--DataLocks
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.DataLockEvent'))
BEGIN
	DECLARE @DataLock AS NVARCHAR(4000) = 
	'
		;WITH DataLockEventCte
		AS (
			SELECT Id, Row_Number() OVER (
					PARTITION BY [JobId]
					,[Ukprn]
					,[AcademicYear]
					,[CollectionPeriod]
					,[IsPayable]
					,[ContractType]
					,[LearnerUln]
					,[LearnerReferenceNumber]
					,[LearningAimReference]
					,[LearningAimProgrammeType]
					,[LearningAimStandardCode]
					,[LearningAimFrameworkCode]
					,[LearningAimPathwayCode]
					,[LearningAimFundingLineType]
					,[LearningStartDate] ORDER BY [JobId]
					,[Ukprn]
					,[AcademicYear]
					,[CollectionPeriod]
					) AS RN
			FROM Payments2.DataLockEvent
			)
		UPDATE rp
		SET DuplicateNumber = RN - 1
		FROM Payments2.DataLockEvent rp
		JOIN DataLockEventCte ON DataLockEventCte.Id = rp.Id
		WHERE DataLockEventCte.RN > 1;
	'
	EXECUTE sp_executesql @DataLock
END
GO

--FundingSourceEvent
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.FundingSourceEvent')) 
BEGIN
	DECLARE @FundingSource AS NVARCHAR(4000) = 
	'
		;WITH FundingSourceEventCte
		AS (
			SELECT Id, Row_Number() OVER (
					PARTITION BY [JobId]
					,[Ukprn]
					,[AcademicYear]
					,[CollectionPeriod]
					,[DeliveryPeriod]
					,[ContractType]
					,[TransactionType]
					,[Amount]
					,[SfaContributionPercentage]
					,[LearnerUln]
					,[LearnerReferenceNumber]
					,[LearningAimReference]
					,[LearningAimProgrammeType]
					,[LearningAimStandardCode]
					,[LearningAimFrameworkCode]
					,[LearningAimPathwayCode]
					,[LearningAimFundingLineType]
					,[LearningStartDate]
					,[FundingSourceType]
					,[ApprenticeshipId]
					,[AccountId]
					,[TransferSenderAccountId]
					,[ApprenticeshipEmployerType] ORDER BY [JobId]
						,[Ukprn]
						,[AcademicYear]
						,[CollectionPeriod]
						,[DeliveryPeriod]
					) AS RN
			FROM Payments2.FundingSourceEvent
			)
		UPDATE fse
		SET DuplicateNumber = RN - 1
		FROM Payments2.FundingSourceEvent fse
		JOIN FundingSourceEventCte ON FundingSourceEventCte.Id = fse.Id
		WHERE FundingSourceEventCte.RN > 1;
	'
	EXECUTE sp_executesql @FundingSource

END
GO

--Required Payment event
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent'))
BEGIN
	
	DECLARE @RequiredPayment AS NVARCHAR(4000) = 
	'
		;WITH RequiredPaymentEventCte
		AS (
			SELECT Id, Row_Number() OVER (
					PARTITION BY [JobId]
					,[Ukprn]
					,[AcademicYear]
					,[CollectionPeriod]
					,[DeliveryPeriod]
					,[ContractType]
					,[TransactionType]
					,[Amount]
					,[SfaContributionPercentage]
					,[LearnerUln]
					,[LearnerReferenceNumber]
					,[LearningAimReference]
					,[LearningAimProgrammeType]
					,[LearningAimStandardCode]
					,[LearningAimFrameworkCode]
					,[LearningAimPathwayCode]
					,[LearningAimFundingLineType]
					,[LearningStartDate]
					,[EventType]
					,[ApprenticeshipId]
					,[AccountId]
					,[TransferSenderAccountId]
					,[ApprenticeshipEmployerType] ORDER BY [JobId]
						,[Ukprn]
						,[AcademicYear]
						,[CollectionPeriod]
						,[DeliveryPeriod]
					) AS RN
			FROM Payments2.RequiredPaymentEvent
			)
		UPDATE rpe
		SET DuplicateNumber = RN - 1
		FROM Payments2.RequiredPaymentEvent rpe
		JOIN RequiredPaymentEventCte ON RequiredPaymentEventCte.Id = rpe.Id
		WHERE RequiredPaymentEventCte.RN > 1;
	'
	EXECUTE sp_executesql @RequiredPayment

END
GO

--payments
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.Payment'))
BEGIN
	
	DECLARE @Payment AS NVARCHAR(4000) = 
	'
		;WITH PaymentCte
		AS (
			SELECT Id, Row_Number() OVER (
					PARTITION BY [JobId]
					,[Ukprn]
					,[AcademicYear]
					,[CollectionPeriod]
					,[DeliveryPeriod]
					,[ContractType]
					,[TransactionType]
					,[Amount]
					,[SfaContributionPercentage]
					,[LearnerUln]
					,[LearnerReferenceNumber]
					,[LearningAimReference]
					,[LearningAimProgrammeType]
					,[LearningAimStandardCode]
					,[LearningAimFrameworkCode]
					,[LearningAimPathwayCode]
					,[LearningAimFundingLineType]
					,[LearningStartDate]
					,[FundingSource]
					,[ApprenticeshipId]
					,[AccountId]
					,[TransferSenderAccountId]
					,[ApprenticeshipEmployerType] 
					ORDER BY [JobId]
						,[Ukprn]
						,[AcademicYear]
						,[CollectionPeriod]
						,[DeliveryPeriod]
					) AS RN
			FROM Payments2.Payment
			)
		UPDATE p
		SET DuplicateNumber = RN - 1
		FROM Payments2.Payment p
		JOIN PaymentCte ON PaymentCte.Id = p.Id
		WHERE PaymentCte.RN > 1;
	'
	EXECUTE sp_executesql @Payment

END
GO