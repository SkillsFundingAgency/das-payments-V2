CREATE TABLE [Payments2].[Payment]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_Payment PRIMARY KEY,
	EventId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Payment__EventId DEFAULT(NEWID()), CONSTRAINT UQ_Payment__EventId UNIQUE([EventId]), 
	EventTime DATETIMEOFFSET NOT NULL,
	FundingSourceEventId UNIQUEIDENTIFIER NOT NULL,
	JobId BIGINT NOT NULL,
    DeliveryPeriod TINYINT NOT NULL,
	CollectionPeriod TINYINT NOT NULL,
    AcademicYear SMALLINT CONSTRAINT DF_Payment__AcademicYear DEFAULT ((0)) NOT NULL,
	Ukprn BIGINT NOT NULL,
	LearnerReferenceNumber  NVARCHAR(50) NOT NULL,
	LearnerUln  BIGINT NOT NULL,
	PriceEpisodeIdentifier NVARCHAR(50) NOT NULL,
    Amount DECIMAL(15,5) NOT NULL,
	LearningAimReference   NVARCHAR(8) NOT NULL,
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
	CreationDate DATETIMEOFFSET NOT NULL CONSTRAINT DF_Payment__CreationDate DEFAULT (SYSDATETIMEOFFSET()),
);
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