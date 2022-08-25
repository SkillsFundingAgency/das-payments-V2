Create TABLE [Payments2].[Job]
( 
	[JobId] 					BIGINT 			NOT NULL CONSTRAINT PK_Job PRIMARY KEY CLUSTERED,
	[JobType] 					TINYINT 		NOT NULL,
	[StartTime] 				DATETIMEOFFSET 	NOT NULL,	
	[EndTime] 					DATETIMEOFFSET 	NULL, 
	[Status] 					TINYINT 		NOT NULL,
	[CreationDate] 				DATETIMEOFFSET 	NOT NULL,
	[DCJobId] 					BIGINT 			NULL,
	[Ukprn] 					BIGINT 			NULL, 
	[IlrSubmissionTime] 		DATETIME 		NULL,
	[LearnerCount] 				INT 			NULL,
	[AcademicYear] 				SMALLINT 		NOT NULL,
	[CollectionPeriod] 			TINYINT 		NOT NULL,
	[DataLocksCompletionTime] 	DATETIMEOFFSET 	NULL,
	[DCJobSucceeded] 			BIT 			NULL,
	[DCJobEndTime] 				DATETIMEOFFSET 	NULL
)
GO

CREATE INDEX [IX_Job__Search] ON [Payments2].[Job]
(
	[JobId],
	[JobType],
	[DCJobId],
	[Ukprn],
	[Status],
	[StartTime],
	[EndTime],	
	[DataLocksCompletionTime],
	[DCJobSucceeded],
	[DCJobEndTime]
)
WITH (ONLINE = ON)
GO

CREATE INDEX [IX_Payments2_Job__IlrSubmissionTime] ON Payments2.Job 
(
	[IlrSubmissionTime]
)
WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_Payments2_Job_Search] ON [Payments2].[Job] 
(
	[DCJobId], 
	[AcademicYear], 
	[DCJobSucceeded], 
	[JobType], 
	[Status]
) 
INCLUDE 
(
	[IlrSubmissionTime], 
	[Ukprn]
) 
WITH (ONLINE = ON)
GO
