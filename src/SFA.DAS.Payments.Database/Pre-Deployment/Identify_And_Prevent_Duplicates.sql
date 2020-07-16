/* 
PreDeployment script to cater for existing duplicates prior to adding unique constraints to prevent future duplicates
*/

--DataLocks
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.DataLockEvent'))
BEGIN
	DECLARE @DataLockDupCte AS INT;

	;WITH DataLockDupCte
	AS (
		SELECT DuplicateNumber
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
	SELECT @DataLockDupCte = COUNT(1) 
	FROM DataLockDupCte 
	WHERE DuplicateNumber IS NULL AND RN > 1;
	
	SELECT @DataLockDupCte as DataLockDup

	IF (@DataLockDupCte >= 1)
	BEGIN
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
		WHERE DataLockEventCte.RN > 1;
	END
END
GO

--FundingSourceEvent

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.FundingSourceEvent')) 
BEGIN
	DECLARE @FundingSourceDupCte AS INT;

	WITH FundingSourceDupCte
	AS (
		SELECT 
			DuplicateNumber,
			Row_Number() OVER (
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
	SELECT @FundingSourceDupCte = COUNT(1) 
	FROM FundingSourceDupCte 
	WHERE DuplicateNumber IS NULL AND RN > 1;
	
	SELECT @FundingSourceDupCte as FundingSourceDup

	IF(@FundingSourceDupCte >= 1)
	BEGIN 
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
	END
END
GO

--Required Payment event
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent'))
BEGIN
	
	DECLARE @RequiredPaymentDupCte AS INT;
	
	WITH RequiredPaymentDupCte
	AS (
		SELECT 
			DuplicateNumber,
			Row_Number() OVER (
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
	SELECT @RequiredPaymentDupCte = COUNT(1) 
	FROM RequiredPaymentDupCte 
	WHERE DuplicateNumber IS NULL AND RN > 1;
	
	SELECT @RequiredPaymentDupCte AS RequiredPaymentDup

	IF(@RequiredPaymentDupCte >= 1)
	BEGIN

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
	END
END
GO

--payments
IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.Payment'))
BEGIN
	
	DECLARE @PaymentDupCte AS INT;
	
	WITH PaymentDupCte
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
	SELECT @PaymentDupCte = COUNT(1) 
	FROM PaymentDupCte 
	WHERE DuplicateNumber IS NULL AND RN > 1;

	SELECT @PaymentDupCte AS PaymentDup

	IF(@PaymentDupCte >= 1)
	BEGIN

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
	END
END
GO