CREATE TABLE [Payments2].[DataLockEvent]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_DataLockEvent PRIMARY KEY CLUSTERED,
	EventId UNIQUEIDENTIFIER NOT NULL, 
	EarningEventId UNIQUEIDENTIFIER NOT NULL,
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
	IsPayable BIT NOT NULL,
	DataLockSourceId TINYINT NOT NULL,
	JobId  BIGINT NOT NULL,
	EventTime DATETIMEOFFSET NOT NULL,
	CreationDate DATETIMEOFFSET NOT NULL,
	DuplicateNumber INT NULL
)
GO

CREATE NONCLUSTERED INDEX [IX_DataLockEvent__AuditDataFactory] ON [Payments2].[DataLockEvent]
(
	[AcademicYear],
	[CollectionPeriod],
	[EventId]
)
WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_DataLockEvent_IdentifyDataLocksTool] ON [Payments2].[DataLockEvent] 
(
	[LearnerUln], 
	[AcademicYear]
) WITH (ONLINE = ON)
