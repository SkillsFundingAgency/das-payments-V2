CREATE TABLE [Payments2].[Payment]
(
	[PaymentId] BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_Payment PRIMARY KEY,
	[ExternalId] [UNIQUEIDENTIFIER] NOT NULL CONSTRAINT DF_Payment__ExternalId DEFAULT(NEWID()), CONSTRAINT UQ_Payment__ExternalId UNIQUE([ExternalId]), 
	Ukprn BIGINT NOT NULL,
	LearnerReferenceNumber  NVARCHAR(50) NOT NULL,
	LearnerUln  BIGINT NOT NULL,
	PriceEpisodeIdentifier NVARCHAR(50) NOT NULL,
    Amount DECIMAL(15,5) NOT NULL,
	CollectionPeriodName CHAR(8) NOT NULL,
	CollectionPeriodMonth TINYINT NOT NULL,
	CollectionPeriodYear SMALLINT NOT NULL,
	DeliveryPeriodMonth TINYINT NOT NULL,
	DeliveryPeriodYear SMALLINT NOT NULL,
	LearningAimReference   NVARCHAR(8) NOT NULL,
	LearningAimProgrammeType INT NOT NULL ,
	LearningAimStandardCode INT NOT NULL,
	LearningAimFrameworkCode INT NOT NULL,
	LearningAimPathwayCode INT NOT NULL,
	LearningAimFundingLineType  NVARCHAR(100) NOT NULL,
	ContractType  TINYINT NOT NULL,
	TransactionType  TINYINT NOT NULL,
	FundingSource  TINYINT NOT NULL,
	IlrSubmissionDateTime DATETIME2 NOT NULL,
	SfaContributionPercentage DECIMAL(15,5) NOT NULL,
	JobId  BIGINT NOT NULL,
	CreationDate DATETIME NOT NULL CONSTRAINT DF_Payment__CreationDate DEFAULT (GETDATE())
)
GO

CREATE INDEX [IX_Payment__ApprenticeshipKey] ON [Payments2].[Payment]
(
	Ukprn,
	LearnerUln, 
	LearnerReferenceNumber,
	LearningAimReference ,
	LearningAimProgrammeType,
	LearningAimStandardCode,
	LearningAimFrameworkCode ,
	LearningAimPathwayCode 
)