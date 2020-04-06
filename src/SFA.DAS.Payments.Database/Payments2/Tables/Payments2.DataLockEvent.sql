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

)

GO

CREATE INDEX [IX_DataLockEvent_Submission] ON [Payments2].[DataLockEvent] ([Ukprn], [AcademicYear], [CollectionPeriod], [IlrSubmissionDateTime])
GO
Create index IX_DataLockEvent__Metrics ON [Payments2].[DataLockEvent] ([AcademicYear], [CollectionPeriod], [IsPayable], [LearningAimReference], [Ukprn], [JobId]) 
	INCLUDE ([DataLockSourceId], [EarningEventId], [EventId], [IlrSubmissionDateTime], [LearnerReferenceNumber], [LearnerUln]) WITH (ONLINE = ON)
Go

CREATE NONCLUSTERED INDEX [IX_DataLockEvent__Metrics_Paid_DataLocks] ON [Payments2].[DataLockEvent] 
( 
[JobId],
[Ukprn],
[IsPayable] 
) 
INCLUDE
(
[CollectionPeriod], 
[EventId], 
[LearnerReferenceNumber], 
[LearningAimFrameworkCode], 
[LearningAimPathwayCode], 
[LearningAimProgrammeType], 
[LearningAimReference], 
[LearningAimStandardCode]
) 
WITH (ONLINE = ON)

GO

CREATE NONCLUSTERED INDEX [IX_DataLockEvent__Manual_Metrics_Paid_DataLocks] ON [Payments2].[DataLockEvent]
(
	[Ukprn] ,
	[CollectionPeriod],
	[JobId],
	[IsPayable]
)
INCLUDE([EventId],[LearnerReferenceNumber], [LearningAimReference],
    [LearningAimProgrammeType] ,
    [LearningAimStandardCode] ,
    [LearningAimFrameworkCode],
    [LearningAimPathwayCode] ) 
go