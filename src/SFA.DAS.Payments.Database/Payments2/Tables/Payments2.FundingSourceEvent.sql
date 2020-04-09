CREATE TABLE [Payments2].[FundingSourceEvent]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_FundingSourceEvent PRIMARY KEY CLUSTERED,	
	EventId UNIQUEIDENTIFIER NOT NULL,
	EarningEventId UNIQUEIDENTIFIER NOT NULL,
	RequiredPaymentEventId UNIQUEIDENTIFIER NOT NULL, 
	PriceEpisodeIdentifier NVARCHAR(50) NOT NULL,
	ContractType  TINYINT NOT NULL,
	TransactionType TINYINT NOT NULL, 
	FundingSourceType TINYINT NOT NULL,
	SfaContributionPercentage DECIMAL(15,5) NOT NULL,
	Amount DECIMAL(15,5) NOT NULL,
	CollectionPeriod TINYINT NOT NULL,
	AcademicYear SMALLINT NOT NULL,
	DeliveryPeriod TINYINT NOT NULL,
	LearnerReferenceNumber  NVARCHAR(50) NOT NULL,
	LearnerUln  BIGINT NOT NULL,
	LearningAimReference   NVARCHAR(8) NOT NULL,
	LearningAimProgrammeType INT NOT NULL ,
	LearningAimStandardCode INT NOT NULL,
	LearningAimFrameworkCode INT NOT NULL,
	LearningAimPathwayCode INT NOT NULL,
	LearningAimFundingLineType  NVARCHAR(100) NOT NULL,
	AgreementId NVARCHAR(255) NULL, 
	Ukprn  BIGINT NOT NULL,
	IlrSubmissionDateTime DATETIME2 NOT NULL,
	JobId  BIGINT NOT NULL,
	EventTime DATETIMEOFFSET NOT NULL,
	[AccountId] BIGINT  NULL , 
	TransferSenderAccountId BIGINT NULL , 
	CreationDate DATETIMEOFFSET NOT NULL CONSTRAINT DF_FundingSourceEvent__CreationDate DEFAULT (SYSDATETIMEOFFSET()),
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
    [NonPaymentReason] TINYINT NULL,
)

GO

CREATE INDEX [IX_FundingSourceEvent__Audit] ON [Payments2].[FundingSourceEvent]
(
  [EarningEventId],
  RequiredPaymentEventId
)  

GO

CREATE NONCLUSTERED INDEX [IX_FundingSourceEvent__Submission] ON [Payments2].[FundingSourceEvent] 
([AcademicYear], [CollectionPeriod], [Ukprn], [IlrSubmissionDateTime]) 
WITH (ONLINE = ON)

GO

CREATE NONCLUSTERED INDEX [IX_FundingSourceEvent__JobId] ON [Payments2].[FundingSourceEvent] ([JobId]) WITH (ONLINE = ON)

GO