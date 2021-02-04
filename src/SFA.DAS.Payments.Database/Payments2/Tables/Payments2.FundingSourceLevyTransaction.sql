CREATE TABLE [Payments2].[FundingSourceLevyTransaction]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_FundingSourceLevyTransaction PRIMARY KEY CLUSTERED,
	[Ukprn] BIGINT NOT NULL,
	[CollectionPeriod] TINYINT NOT NULL,
	[AcademicYear] SMALLINT NOT NULL,
	[DeliveryPeriod] TINYINT NOT NULL,
	[JobId]  BIGINT NOT NULL,
	[AccountId] BIGINT NOT NULL, 
	[TransferSenderAccountId] BIGINT NULL, 
	[RequiredPaymentEventId] UNIQUEIDENTIFIER NOT NULL,
	[EarningEventId] UNIQUEIDENTIFIER NOT NULL,
	[CreationDate] DATETIMEOFFSET NOT NULL CONSTRAINT DF_FundingSourceLevyTransaction__CreationDate DEFAULT (SYSDATETIMEOFFSET()),
	[Amount] DECIMAL(15,5) NOT NULL,
	[MessagePayload] nvarchar(max) not null,
	[MessageType] nvarchar(max) not null, 
    [IlrSubmissionDateTime] DATETIME NOT NULL, 
    [FundingAccountId] BIGINT NOT NULL,
	[TransactionType] TINYINT NULL, 
	[SfaContributionPercentage] DECIMAL(15,5) NULL,
    [LearnerUln]  BIGINT NULL,
    [LearnerReferenceNumber] NVARCHAR(50) NULL,
	[LearningAimReference] NVARCHAR(8) NULL,
	[LearningAimProgrammeType] INT NULL ,
	[LearningAimStandardCode] INT NULL,
	[LearningAimFrameworkCode] INT NULL,
	[LearningAimPathwayCode] INT NULL,
	[LearningAimFundingLineType] NVARCHAR(100) NULL,
	[LearningStartDate] DATETIME2 NULL,
    [ApprenticeshipId] BIGINT NULL,
	[ApprenticeshipEmployerType] TINYINT NULL,
	[DuplicateNumber] INT NULL
)
GO

CREATE NONCLUSTERED INDEX [IX_FundingSourceLevyTransaction__PeriodEnd] ON [Payments2].[FundingSourceLevyTransaction] 
([FundingAccountId] ASC,[AcademicYear] ASC,[CollectionPeriod] ASC)
WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_FundingSourceLevyTransaction__SubmissionCleanup]
ON [Payments2].[FundingSourceLevyTransaction] ([Ukprn],[CollectionPeriod],[AcademicYear],[JobId],[IlrSubmissionDateTime])
GO

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