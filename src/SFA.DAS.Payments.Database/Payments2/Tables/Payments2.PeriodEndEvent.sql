CREATE TABLE [Payments2].[PeriodEndEvent]
(
    [EventId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_PeriodEndEvent] PRIMARY KEY,
    [JobId] BIGINT NOT NULL, 
    [EventTime] DATETIMEOFFSET NOT NULL, 
    [AcademicYear] SMALLINT NOT NULL, 
    [Period] TINYINT NOT NULL, 
    [EventType] NVARCHAR(255) NOT NULL
)

GO

CREATE INDEX [IX_PeriodEndEvent_EventTime] ON [Payments2].[PeriodEndEvent] ([EventTime] DESC)
