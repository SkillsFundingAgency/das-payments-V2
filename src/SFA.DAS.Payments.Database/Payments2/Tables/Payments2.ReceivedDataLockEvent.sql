CREATE TABLE Payments2.ReceivedDataLockEvent
(
	[Id] BIGINT NOT NULL CONSTRAINT PK_ReceivedDataLockEvent PRIMARY KEY IDENTITY, 
    [Ukprn] BIGINT NOT NULL, 
    [JobId]  BIGINT NOT NULL, 
	[MessageType] NVARCHAR(2000) NOT NULL,
	[Message] NVARCHAR(MAX) NOT NULL,
    [CreationDate] DATETIME2 NOT NULL DEFAULT sysutcdatetime()
)


GO

CREATE INDEX [IX_ReceivedDataLockEvent_Ukprn_JobId] ON [Payments2].[ReceivedDataLockEvent] 
([Ukprn], [JobId])
