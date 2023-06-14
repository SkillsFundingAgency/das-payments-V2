﻿CREATE TABLE [Payments2].[EarningEvent]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_EarningEvent PRIMARY KEY CLUSTERED,
	EventId UNIQUEIDENTIFIER NOT NULL, 
	Ukprn BIGINT NOT NULL,
	ContractType  TINYINT NOT NULL,
	CollectionPeriod TINYINT NOT NULL,
	AcademicYear SMALLINT NOT NULL,
	LearnerReferenceNumber  NVARCHAR(50) NOT NULL,
	LearnerUln  BIGINT NOT NULL,
	LearningAimReference   NVARCHAR(8) NOT NULL,
	LearningAimProgrammeType INT NOT NULL ,
	LearningAimStandardCode INT NOT NULL,
	LearningAimFrameworkCode INT NOT NULL,
	LearningAimPathwayCode INT NOT NULL,
	LearningAimFundingLineType  NVARCHAR(100) NULL,
	LearningStartDate DATETIME2 NULL,
	AgreementId NVARCHAR(255) NULL, 
	IlrSubmissionDateTime DATETIME2 NULL,
	JobId  BIGINT NOT NULL,
	EventTime DATETIMEOFFSET NOT NULL,
	CreationDate DATETIMEOFFSET NOT NULL, 
    LearningAimSequenceNumber BIGINT NULL,
	SfaContributionPercentage DECIMAL(15, 5) NULL,
	IlrFileName  NVARCHAR(400) NULL,
	EventType NVARCHAR(4000) NULL,
)
GO

CREATE NONCLUSTERED INDEX [IX_EarningEvent__AuditDataFactory] ON [Payments2].[EarningEvent]
(
	[AcademicYear],
	[CollectionPeriod],
	[EventId]
)
WITH (ONLINE = ON)
GO
