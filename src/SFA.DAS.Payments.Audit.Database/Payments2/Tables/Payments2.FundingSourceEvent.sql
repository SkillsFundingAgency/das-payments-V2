CREATE TABLE [Payments2].[FundingSourceEvent]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_FundingSourceEvent PRIMARY KEY CLUSTERED,	
	EventId UNIQUEIDENTIFIER NOT NULL,
	EarningEventId UNIQUEIDENTIFIER NOT NULL,
	RequiredPaymentEventId UNIQUEIDENTIFIER NOT NULL, 
	ClawbackSourcePaymentEventId UNIQUEIDENTIFIER NULL,
	EventTime DATETIMEOFFSET NOT NULL,
	JobId BIGINT NOT NULL,
	DeliveryPeriod TINYINT NOT NULL,
	CollectionPeriod TINYINT NOT NULL,
	AcademicYear SMALLINT NOT NULL,
	Ukprn BIGINT NOT NULL,
	LearnerReferenceNumber NVARCHAR(50) NOT NULL,
	LearnerUln BIGINT NOT NULL,
	PriceEpisodeIdentifier NVARCHAR(50) NOT NULL,
	Amount DECIMAL(15,5) NOT NULL,
	LearningAimReference NVARCHAR(8) NOT NULL,
	LearningAimProgrammeType INT NOT NULL ,
	LearningAimStandardCode INT NOT NULL,
	LearningAimFrameworkCode INT NOT NULL,
	LearningAimPathwayCode INT NOT NULL,
	LearningAimFundingLineType NVARCHAR(100) NOT NULL,
	ContractType TINYINT NOT NULL,
	TransactionType TINYINT NOT NULL, 
	FundingSourceType TINYINT NOT NULL,
	IlrSubmissionDateTime DATETIME2 NOT NULL,
	SfaContributionPercentage DECIMAL(15,5) NOT NULL,
	AgreementId NVARCHAR(255) NULL, 
	AccountId BIGINT NULL, 
	TransferSenderAccountId BIGINT NULL, 
	CreationDate DATETIMEOFFSET NOT NULL,
	EarningsStartDate DATETIME NOT NULL,
	EarningsPlannedEndDate DATETIME NULL,
	EarningsActualEndDate DATETIME NULL,
	EarningsCompletionStatus TINYINT NOT NULL,
	EarningsCompletionAmount DECIMAL (15,5),
	EarningsInstalmentAmount DECIMAL (15,5),
	EarningsNumberOfInstalments SMALLINT NOT NULL,
	LearningStartDate DATETIME2 NULL,
	ApprenticeshipId BIGINT NULL,
	ApprenticeshipPriceEpisodeId BIGINT NULL,
	ApprenticeshipEmployerType TINYINT NULL, 
	NonPaymentReason TINYINT NULL,
	DuplicateNumber INT NULL,
	AgeAtStartOfLearning TINYINT NULL
)
GO

CREATE NONCLUSTERED INDEX [IX_FundingSourceEvent__AuditDataFactory] ON [Payments2].[FundingSourceEvent]
(
	[AcademicYear],
	[CollectionPeriod],
	[EventId]
)
WITH (ONLINE = ON)
GO
