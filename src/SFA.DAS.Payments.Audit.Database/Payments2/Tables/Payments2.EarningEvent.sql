﻿CREATE TABLE [Payments2].[EarningEvent]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_EarningEvent PRIMARY KEY CLUSTERED,
	EventId UNIQUEIDENTIFIER NOT NULL CONSTRAINT UQ_EarningEvent_EventId UNIQUE, 
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
	IlrSubmissionDateTime DATETIME2 NOT NULL,
	JobId  BIGINT NOT NULL,
	EventTime DATETIMEOFFSET NOT NULL,
	CreationDate DATETIMEOFFSET NOT NULL CONSTRAINT DF_EarningEvent__CreationDate DEFAULT (SYSDATETIMEOFFSET()), 
    [LearningAimSequenceNumber] BIGINT NULL,
	[SfaContributionPercentage] [decimal](15, 5) NULL,
	IlrFileName  NVARCHAR(400) NULL,
	EventType NVARCHAR(4000) NULL,
)
GO

CREATE UNIQUE INDEX UQ_EarningEvent ON [Payments2].[EarningEvent] 
(
[JobId], 
[Ukprn], 
[AcademicYear], 
[CollectionPeriod], 
[ContractType], 
[LearnerUln], 
[LearnerReferenceNumber], 
[LearningAimReference], 
[LearningAimProgrammeType], 
[LearningAimStandardCode], 
[LearningAimFrameworkCode], 
[LearningAimPathwayCode], 
[LearningAimFundingLineType],
[LearningAimSequenceNumber], 
[LearningStartDate], 
[EventType]
)
GO

CREATE INDEX IX_EarningEvent_Audit ON [Payments2].[EarningEvent]
(
	[AcademicYear],
	[CollectionPeriod],
	[Ukprn],
	[LearnerUln],
	[JobId]
)
WITH (ONLINE = ON)
GO
