CREATE TABLE [Payments2].[Payment]
(
	[Id] [uniqueidentifier] NOT NULL CONSTRAINT PK_Payment_Id PRIMARY KEY,
	 PriceEpisodeIdentifier NVARCHAR(50) NOT NULL,
         Amount DECIMAL(15,5) NOT NULL,
         CollectionPeriodName CHAR(8) NOT NULL,
         CollectionPeriodMonth INT NOT NULL,
         CollectionPeriodYear INT NOT NULL,
         DeliveryPeriodMonth INT NOT NULL,
         DeliveryPeriodYear INT NOT NULL,
         Ukprn BIGINT NOT NULL,
         LearnerReferenceNumber  NVARCHAR(50) NOT NULL,
         LearnerUln  BIGINT NOT NULL,
          LearningAimReference   NVARCHAR(8) NOT NULL,
          LearningAimProgrammeType INT NOT NULL ,
          LearningAimStandardCode INT NOT NULL,
          LearningAimFrameworkCode INT NOT NULL,
          LearningAimPathwayCode INT NOT NULL,
          LearningAimAgreedPrice  DECIMAL(15,5) NOT NULL,
          LearningAimFundingLineType  NVARCHAR(100) NOT NULL,
          JobId  BIGINT NOT NULL,
          ContractType  INT NOT NULL,
          TransactionType  INT NOT NULL,
          FundingSource  INT NOT NULL,
          [IlrSubmissionDateTime] DateTime NOT NULL,
          SfaContributionPercentage DECIMAL(15,5)
)
GO

CREATE INDEX [IX_Payment_ApprenticeshipKey] ON [Payments2].[Payment]
(
[Ukprn],
[LearnerReferenceNumber],
 LearningAimReference ,
 LearningAimProgrammeType,
 LearningAimStandardCode,
 LearningAimFrameworkCode ,
 LearningAimPathwayCode 
)
