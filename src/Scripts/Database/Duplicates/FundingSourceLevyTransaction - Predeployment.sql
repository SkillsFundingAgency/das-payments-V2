	--FundingSourceLevyTransaction

	IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction ADD DuplicateNumber INT NULL;
	GO

		;WITH FundingSourceLevyTransactionCte
	AS (
		SELECT *
			,Row_Number() OVER (
				PARTITION BY 
				[JobId],
				[Ukprn],
				[AcademicYear],
				[CollectionPeriod],
				[DeliveryPeriod],
				[TransactionType],
				[Amount],
				[SfaContributionPercentage],
				[LearnerUln],
				[LearnerReferenceNumber],
				[LearningAimReference],
				[LearningAimProgrammeType],
				[LearningAimStandardCode],
				[LearningAimFrameworkCode],
				[LearningAimPathwayCode],
				[LearningAimFundingLineType],
				[LearningStartDate],
				[ApprenticeshipId],
				[AccountId],
				[TransferSenderAccountId],
				[ApprenticeshipEmployerType] ORDER BY [JobId]
					,[Ukprn]
					,[AcademicYear]
					,[CollectionPeriod]
					,[DeliveryPeriod]
				) AS RN
		FROM Payments2.FundingSourceLevyTransaction
		)

	UPDATE fslt
	SET DuplicateNumber = RN - 1
	FROM Payments2.FundingSourceLevyTransaction fslt
	JOIN FundingSourceLevyTransactionCte ON FundingSourceLevyTransactionCte.Id = fslt.Id
	WHERE FundingSourceLevyTransactionCte.RN > 1
	GO 

	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_FundingSourceLevyTransaction_LogicalDuplicates' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
		CREATE UNIQUE INDEX [UX_FundingSourceLevyTransaction_LogicalDuplicates] ON [Payments2].[FundingSourceLevyTransaction]
			(
			[JobId],
			[Ukprn],
			[AcademicYear],
			[CollectionPeriod],
			[DeliveryPeriod],
			[TransactionType],
			[Amount],
			[SfaContributionPercentage],
			[LearnerUln],
			[LearnerReferenceNumber],
			[LearningAimReference],
			[LearningAimProgrammeType],
			[LearningAimStandardCode],
			[LearningAimFrameworkCode],
			[LearningAimPathwayCode],
			[LearningAimFundingLineType],
			[LearningStartDate],
			[ApprenticeshipId],
			[AccountId],
			[TransferSenderAccountId],
			[ApprenticeshipEmployerType],
			[DuplicateNumber]
			)
	GO