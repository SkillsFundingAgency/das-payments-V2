CREATE TABLE [Payments2].[Payment]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_Payment PRIMARY KEY,
	EventId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Payment__EventId DEFAULT(NEWID()), CONSTRAINT UQ_Payment__EventId UNIQUE([EventId]), 
	EarningEventId UNIQUEIDENTIFIER NOT NULL,
	FundingSourceEventId UNIQUEIDENTIFIER NOT NULL,
	RequiredPaymentEventId UNIQUEIDENTIFIER NULL,
	EventTime DATETIMEOFFSET NOT NULL,
	JobId BIGINT NOT NULL,
	DeliveryPeriod TINYINT NOT NULL,
	CollectionPeriod TINYINT NOT NULL,
	AcademicYear SMALLINT CONSTRAINT DF_Payment__AcademicYear DEFAULT ((0)) NOT NULL,
	Ukprn BIGINT NOT NULL,
	LearnerReferenceNumber NVARCHAR(50) NOT NULL,
	LearnerUln BIGINT NOT NULL,
	PriceEpisodeIdentifier NVARCHAR(50) NOT NULL,
	Amount DECIMAL(15,5) NOT NULL,
	LearningAimReference  NVARCHAR(8) NOT NULL,
	LearningAimProgrammeType INT NOT NULL ,
	LearningAimStandardCode INT NOT NULL,
	LearningAimFrameworkCode INT NOT NULL,
	LearningAimPathwayCode INT NOT NULL,
	LearningAimFundingLineType NVARCHAR(100) NOT NULL,
	ContractType TINYINT NOT NULL,
	TransactionType TINYINT NOT NULL,
	FundingSource TINYINT NOT NULL,
	IlrSubmissionDateTime DATETIME2 NOT NULL,
	SfaContributionPercentage DECIMAL(15,5) NOT NULL,
	AgreementId NVARCHAR(255) NULL, 
	[AccountId] BIGINT NULL , 
	TransferSenderAccountId BIGINT NULL , 
	CreationDate DATETIMEOFFSET NOT NULL CONSTRAINT DF_Payment__CreationDate DEFAULT (SYSDATETIMEOFFSET()),
	EarningsStartDate DATETIME NOT NULL,
	EarningsPlannedEndDate DATETIME NULL,
	EarningsActualEndDate DATETIME NULL,
	EarningsCompletionStatus TINYINT NULL,
	EarningsCompletionAmount DECIMAL (15,5) NULL,
	EarningsInstalmentAmount DECIMAL (15,5) NULL,
	EarningsNumberOfInstalments SMALLINT NULL,
	LearningStartDate DATETIME2 NULL,
	ApprenticeshipId BIGINT NULL,
	ApprenticeshipPriceEpisodeId BIGINT NULL,
	ApprenticeshipEmployerType TINYINT NULL,
	ReportingAimFundingLineType NVARCHAR(120) NULL, 
	[NonPaymentReason] TINYINT NULL,
	[DuplicateNumber] INT NULL
);
GO

CREATE UNIQUE INDEX UX_Payment_LogicalDuplicates ON Payments2.Payment 
(
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

CREATE NONCLUSTERED INDEX [IX_Payment__ApprenticeshipKey] ON [Payments2].[Payment]
(
	[Ukprn] ASC,
  [AcademicYear] ASC,
  [LearnerReferenceNumber] ASC,
  [LearningAimReference] ASC,
  [LearningAimProgrammeType] ASC,
  [LearningAimStandardCode] ASC,
  [LearningAimFrameworkCode] ASC,
  [LearningAimPathwayCode] ASC,
  [ContractType] ASC,
  [LearnerUln] ASC
)

GO

CREATE INDEX [IX_Payment__UkprnPeriodSearch] ON [Payments2].[Payment]
(
 [Ukprn],
 CollectionPeriod,
 AcademicYear,
 DeliveryPeriod,
 JobId
) 

GO

CREATE INDEX [IX_Payment__Audit] ON [Payments2].[Payment]
(
 [EarningEventId],
 FundingSourceEventId
) 

GO

CREATE NONCLUSTERED INDEX [IX_Payment__Metrics_Paid_DataLocks] ON [Payments2].[Payment]
(
	[Ukprn],
  [LearnerReferenceNumber] ASC,
  [LearningAimReference] ASC,
  [LearningAimProgrammeType] ASC,
  [LearningAimStandardCode] ASC,
  [LearningAimFrameworkCode] ASC,
  [LearningAimPathwayCode] ASC,
	[CollectionPeriod]
)
include(Amount, [TransactionType], [DeliveryPeriod])

GO

CREATE NONCLUSTERED INDEX [IX_Payment__CollectionPeriodCompletionPayments] on [Payments2].[Payment] 
(
	[Ukprn],
	[ContractType],
	[TransactionType]
)

GO
