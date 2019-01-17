CREATE TABLE [Payments2].[Payment] (
    [PaymentId]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [ExternalId]                 UNIQUEIDENTIFIER CONSTRAINT [DF_Payment__ExternalId] DEFAULT (newid()) NOT NULL,
    [Ukprn]                      BIGINT           NOT NULL,
    [LearnerReferenceNumber]     NVARCHAR (50)    NOT NULL,
    [LearnerUln]                 BIGINT           NOT NULL,
    [PriceEpisodeIdentifier]     NVARCHAR (50)    NOT NULL,
    [Amount]                     DECIMAL (15, 5)  NOT NULL,
    [CollectionPeriodName]       CHAR (8)         NOT NULL,
    [DeliveryPeriod]             TINYINT          NOT NULL,
    [LearningAimReference]       NVARCHAR (8)     NOT NULL,
    [LearningAimProgrammeType]   INT              NOT NULL,
    [LearningAimStandardCode]    INT              NOT NULL,
    [LearningAimFrameworkCode]   INT              NOT NULL,
    [LearningAimPathwayCode]     INT              NOT NULL,
    [LearningAimFundingLineType] NVARCHAR (100)   NOT NULL,
    [ContractType]               TINYINT          NOT NULL,
    [TransactionType]            TINYINT          NOT NULL,
    [FundingSource]              TINYINT          NOT NULL,
    [IlrSubmissionDateTime]      DATETIME2 (7)    NOT NULL,
    [SfaContributionPercentage]  DECIMAL (15, 5)  NOT NULL,
    [JobId]                      BIGINT           NOT NULL,
    [CreationDate]               DATETIME         CONSTRAINT [DF_Payment__CreationDate] DEFAULT (getdate()) NOT NULL,
    [AcademicYear]               SMALLINT         CONSTRAINT [DF_Payment_CollectionPeriodAcademicYear] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED ([PaymentId] ASC),
    CONSTRAINT [UQ_Payment__ExternalId] UNIQUE NONCLUSTERED ([ExternalId] ASC)
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
  [CollectionPeriodName],
  [JobId]
) 
 

GO