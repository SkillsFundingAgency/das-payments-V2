CREATE TABLE [Jobs].[JobMessageFinished]
(
	JobMessageFinishedId BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_JobMessageFinished PRIMARY KEY CLUSTERED,
	JobId BIGINT NOT NULL CONSTRAINT FK_JobMessageFinished__Job FOREIGN KEY REFERENCES [Jobs].[Job] (JobId),
	MessageId UNIQUEIDENTIFIER NOT NULL,
	[Status] TINYINT NOT NULL CONSTRAINT FK_JobMessagefinished__JobMessageStatus FOREIGN KEY REFERENCES [Jobs].[JobMessageStatus] (Id) CONSTRAINT DF_JobMessageFinished__Status DEFAULT (1),
	EndTime DATETIMEOFFSET NOT NULL, 
    MessageName NVARCHAR(250) NULL,
	CreationDate DATETIMEOFFSET NOT NULL CONSTRAINT DF_JobMessageFinished__CreationDate DEFAULT (SYSUTCDATETIME()),
)
GO

CREATE INDEX [IX_JobMessageFinished__MessageId_JobId_EndTime_Status] ON [Jobs].[JobMessageFinished](
	JobId,
	MessageId,
	[Status],
	EndTime
)
GO
