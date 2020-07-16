/* 
PreDeployment script to cater for existing duplicates prior to adding unique constraints to prevent future duplicates

NOTE:the reason this is executed using sp_executesql is because in the scenario when DuplicateNumber doesn't exists at all in database this script will thwow compiler error during deployment
Deployment will still fail when it tries to create UX_..._LogicalDuplicates but the DuplicateNumber will be created and therefore running same release again should cause this PreDeploy script 
Which in turn means create UX_..._LogicalDuplicates will also be successful
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