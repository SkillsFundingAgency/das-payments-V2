/* 
PreDeployment script to cater for existing duplicates prior to adding unique constraints to prevent future duplicates
*/
--initial test to see if this script has ran before - if we have the duplicate number column on one of the tables then the predeployment script must have ran
--or the main table scripts which also include the indexes 				
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.Payment'))
	SET NOEXEC ON

--DataLocks
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.DataLockEvent'))
AND EXISTS (SELECT COUNT(1) FROM Payments2.DataLockEvent WHERE DuplicateNumber IS NULL)
	WITH DataLockEventCte
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
	WHERE DataLockEventCte.RN > 1;

	GO

--FundingSourceEvent
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.FundingSourceEvent')) 
AND EXISTS (SELECT COUNT(1) FROM Payments2.FundingSourceEvent WHERE DuplicateNumber IS NULL)
	WITH FundingSourceEventCte
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
	WHERE FundingSourceEventCte.RN > 1;

	GO

--Required Payment event
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent'))
AND EXISTS (SELECT COUNT(1) FROM Payments2.RequiredPaymentEvent WHERE DuplicateNumber IS NULL)

	WITH RequiredPaymentEventCte
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
	WHERE RequiredPaymentEventCte.RN > 1;

	GO

--payments
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.Payment'))
AND EXISTS (SELECT COUNT(1) FROM Payments2.Payment WHERE DuplicateNumber IS NULL)

	WITH PaymentCte
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
	WHERE PaymentCte.RN > 1;

	GO

SET NOEXEC OFF;