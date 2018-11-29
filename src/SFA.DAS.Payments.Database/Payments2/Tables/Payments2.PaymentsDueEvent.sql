CREATE TABLE [Payments2].[PaymentsDueEvent]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_PaymentsDueEvent PRIMARY KEY CLUSTERED,
	PaymentsDueId UNIQUEIDENTIFIER NOT NULL ,
	EarningEventId UNIQUEIDENTIFIER NOT NULL,  --NOT guaranteed the earning event will be stored before the PaymentDueEvent
	PriceEpisodeIdentifier NVARCHAR(50) NOT NULL,
	Ukprn BIGINT NOT NULL,
	ContractType  TINYINT NOT NULL,
	TransactionType TINYINT NOT NULL, 
	Amount DECIMAL(15,5) NOT NULL,
	CollectionPeriod TINYINT NOT NULL,
	CollectionYear NVARCHAR(4) NOT NULL,
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
	EventTime DATETIME NOT NULL,
	CreationDate DATETIME2 NOT NULL CONSTRAINT DF_PaymentDueEvent__CreationDate DEFAULT (SYSDATETIME())
)
