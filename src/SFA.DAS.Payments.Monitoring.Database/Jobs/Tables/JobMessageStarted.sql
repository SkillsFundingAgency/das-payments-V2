CREATE TABLE [Jobs].[JobMessageStarted]
(
	JobMessageStartedId BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_JobMessageStarted PRIMARY KEY CLUSTERED,
	JobId BIGINT NOT NULL CONSTRAINT FK_JobMessageStarted__Job FOREIGN KEY REFERENCES [Jobs].[Job] (JobId),
	MessageId UNIQUEIDENTIFIER NOT NULL,
	ParentMessageId UNIQUEIDENTIFIER NULL,
	StartTime DATETIMEOFFSET NULL,
    MessageName NVARCHAR(250) NOT NULL,
	CreationDate DATETIMEOFFSET NOT NULL CONSTRAINT DF_JobMessageStarted__CreationDate DEFAULT (SYSUTCDATETIME()),
)
GO

CREATE INDEX [IX_JobMessageStarted__MessageId_JobId_StartTime] ON [Jobs].[JobMessageStarted](
	JobId,
	MessageId,
	StartTime
)
GO
