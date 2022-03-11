CREATE TABLE [Payments2].[DataLockEvent]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_DataLockEvent PRIMARY KEY CLUSTERED,
	EventId UNIQUEIDENTIFIER NOT NULL CONSTRAINT UQ_DataLockEvent_EventId UNIQUE, 
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
	DataLockSourceId TINYINT NOT NULL CONSTRAINT DF_DataLockEvent__DataLockSourceId DEFAULT (1),
	JobId  BIGINT NOT NULL,
	EventTime DATETIMEOFFSET NOT NULL,
	CreationDate DATETIMEOFFSET NOT NULL CONSTRAINT DF_DataLockEvent__CreationDate DEFAULT (SYSDATETIMEOFFSET()),
	[DuplicateNumber] INT NULL

)
GO

Create Unique Index UX_DataLockEvent_LogicalDuplicates on Payments2.DataLockEvent
(
	[JobId], 
	[Ukprn], 
	[AcademicYear], 
	[CollectionPeriod], 
	[IsPayable], 
	[ContractType], 
	[LearnerUln],
	[LearnerReferenceNumber], 
	[LearningAimReference], 
	[LearningAimProgrammeType], 
	[LearningAimStandardCode], 
	[LearningAimFrameworkCode], 
	[LearningAimPathwayCode],
	[LearningAimFundingLineType], 
	[LearningStartDate], 
	[DuplicateNumber]
)
GO

CREATE INDEX [IX_DataLockEvent_Audit] ON [Payments2].[DataLockEvent] 
(
	[Ukprn],
	[LearnerUln],
	[CollectionPeriod],
	[AcademicYear],
	[JobId]
) 
WITH (ONLINE = ON)
GO