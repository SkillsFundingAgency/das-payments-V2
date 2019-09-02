CREATE TABLE [Payments2].[DeferredApprovalsEvent]
(
	[Id] BIGINT NOT NULL CONSTRAINT [PK_DeferredApprovalsEvent] PRIMARY KEY IDENTITY(1,1), 
    [EventTime] DATETIME2 NOT NULL, 
    [EventType] NVARCHAR(1024) NOT NULL, 
    [EventBody] NVARCHAR(MAX) NOT NULL
)

GO

CREATE INDEX [IX_DeferredApprovalsEvent_EventTime] ON [Payments2].[DeferredApprovalsEvent] ([EventTime])
