/* 
PreDeployment script to cater for existing duplicates prior to adding unique constraints to prevent future duplicates
*/
--initial test to see if this script has ran before - if we have the duplicate number column on one of the tables then the predeployment script must have ran
--or the main table scripts which also include the indexes 				
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.Payment'))
SET NOEXEC ON

	--DataLocks
	IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.DataLockEvent'))
		ALTER TABLE Payments2.DataLockEvent ADD DuplicateNumber INT NULL
	GO

	;WITH DataLockEventCte
	AS (
		SELECT *
			,Row_Number() OVER (
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
	WHERE DataLockEventCte.RN > 1
	GO

	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_DataLockEvent_LogicalDuplicates'	AND object_id = OBJECT_ID(N'Payments2.DataLockEvent'))
		CREATE UNIQUE INDEX UX_DataLockEvent_LogicalDuplicates ON Payments2.DataLockEvent (
			[JobId]
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
			,[LearningStartDate]
			,DuplicateNumber
			)
	GO
	--FundingSourceEvent
	IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.FundingSourceEvent'))
		ALTER TABLE Payments2.FundingSourceEvent ADD DuplicateNumber INT NULL;
	GO

	;WITH FundingSourceEventCte
	AS (
		SELECT *
			,Row_Number() OVER (
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
	WHERE FundingSourceEventCte.RN > 1
	GO

	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_FundingSourceEvent_LogicalDuplicates' AND object_id = OBJECT_ID(N'Payments2.FundingSourceEvent'))
		CREATE UNIQUE INDEX UX_FundingSourceEvent_LogicalDuplicates ON Payments2.FundingSourceEvent (
			[JobId]
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
			,[ApprenticeshipEmployerType]
			,DuplicateNumber
			)
	GO

	--Required Payment event
	IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent'))
		ALTER TABLE Payments2.RequiredPaymentEvent ADD DuplicateNumber INT NULL;
	GO

	IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = N'EventType' AND object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent'))
		ALTER TABLE Payments2.RequiredPaymentEvent ADD EventType NVARCHAR(4000) NULL;
	GO
	
	;WITH RequiredPaymentEventCte
	AS (
		SELECT *
			,Row_Number() OVER (
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
	WHERE RequiredPaymentEventCte.RN > 1
	GO 

	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_RequiredPaymentEvent_LogicalDuplicates' AND object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent'))
		CREATE UNIQUE INDEX UX_RequiredPaymentEvent_LogicalDuplicates ON Payments2.RequiredPaymentEvent (
			[JobId]
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
			,[ApprenticeshipEmployerType]
			,DuplicateNumber
			)
	GO
	--payments
	IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.Payment'))
		ALTER TABLE Payments2.Payment ADD DuplicateNumber INT NULL;
	GO

	;WITH PaymentCte
	AS (
		SELECT *
			,Row_Number() OVER (
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
	WHERE PaymentCte.RN > 1
	GO 

	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_Payment_LogicalDuplicates' AND object_id = OBJECT_ID(N'Payments2.Payment'))
		CREATE UNIQUE INDEX UX_Payment_LogicalDuplicates ON Payments2.Payment (
			[JobId]
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
			,DuplicateNumber
			)
	GO

SET NOEXEC OFF