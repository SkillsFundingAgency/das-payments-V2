CREATE TABLE [Payments2].[RequiredPaymentEvent]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_RequiredPaymentEvent PRIMARY KEY CLUSTERED,	
	EventId UNIQUEIDENTIFIER NOT NULL,
	EarningEventId UNIQUEIDENTIFIER NOT NULL,
	PriceEpisodeIdentifier NVARCHAR(50) NOT NULL,
	Ukprn BIGINT NOT NULL,
	ContractType  TINYINT NOT NULL,
	TransactionType TINYINT NOT NULL, 
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
	IlrSubmissionDateTime DATETIME2 NOT NULL,
	JobId  BIGINT NOT NULL,
	EventTime DATETIMEOFFSET NOT NULL,
	[AccountId] BIGINT NULL , 
	TransferSenderAccountId BIGINT NULL , 
	CreationDate DATETIMEOFFSET NOT NULL CONSTRAINT DF_RequiredPaymentEvent__CreationDate DEFAULT (SYSDATETIMEOFFSET()),
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

CREATE INDEX [IX_RequiredPaymentEvent__Audit] ON [Payments2].[RequiredPaymentEvent]
(
  [EarningEventId]
)  

GO

CREATE NONCLUSTERED INDEX [IX_RequiredPaymentEvent_Submission] ON [Payments2].[RequiredPaymentEvent] 
([AcademicYear], [CollectionPeriod], [Ukprn], [IlrSubmissionDateTime]) 
WITH (ONLINE = ON)