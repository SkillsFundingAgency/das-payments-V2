CREATE TABLE [Payments2].[Payment]
(
	[PaymentId] BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_Payment PRIMARY KEY,
	[ExternalId] [UNIQUEIDENTIFIER] NOT NULL CONSTRAINT UQ_Payment__ExternalId UNIQUE, 
	Ukprn BIGINT NOT NULL,
	LearnerReferenceNumber  NVARCHAR(50) NOT NULL,
	LearnerUln  BIGINT NOT NULL,
	PriceEpisodeIdentifier NVARCHAR(50) NOT NULL,
    Amount DECIMAL(15,5) NOT NULL,
	CollectionPeriodName CHAR(8) NOT NULL,
	CollectionPeriodMonth INT NOT NULL,
	CollectionPeriodYear INT NOT NULL,
	DeliveryPeriodMonth INT NOT NULL,
	DeliveryPeriodYear INT NOT NULL,
	LearningAimReference   NVARCHAR(8) NOT NULL,
	LearningAimProgrammeType INT NOT NULL ,
	LearningAimStandardCode INT NOT NULL,
	LearningAimFrameworkCode INT NOT NULL,
	LearningAimPathwayCode INT NOT NULL,
	LearningAimFundingLineType  NVARCHAR(100) NOT NULL,
	ContractType  INT NOT NULL,
	TransactionType  INT NOT NULL,
	FundingSource  INT NOT NULL,
	IlrSubmissionDateTime DATETIME NOT NULL,
	SfaContributionPercentage DECIMAL(15,5),
	EarningsStartDate DATETIME NOT NULL, 
	EarningsPlannedEndDate DATETIME NOT NULL, 
	EarningsActualEndDate DATETIME NULL,
	EarningsCompletionStatus INT NOT NULL,
	EarningsCompletionAmount DECIMAL (15,5),
	EarningsInstalmentAmount DECIMAL (15,5),
	EarningsNumberOfInstalments INT NOT NULL,
	JobId  BIGINT NOT NULL,
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