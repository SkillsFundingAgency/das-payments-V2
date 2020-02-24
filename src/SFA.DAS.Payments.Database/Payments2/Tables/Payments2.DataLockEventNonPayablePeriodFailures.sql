CREATE TABLE [Payments2].[DataLockEventNonPayablePeriodFailures]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_DataLockEventNonPayablePeriodFailures PRIMARY KEY CLUSTERED,	
	DataLockEventNonPayablePeriodId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_DataLockEventNonPayablePeriodFailures__DataLockEventNonPayablePeriod FOREIGN KEY REFERENCES [Payments2].[DataLockEventNonPayablePeriod] (DataLockEventNonPayablePeriodId) ON DELETE CASCADE, 
	DataLockFailureId TINYINT NOT NULL,
	CreationDate DATETIMEOFFSET NOT NULL CONSTRAINT DF_DataLockEventNonPayablePeriodFailures__CreationDate DEFAULT (SYSDATETIMEOFFSET()), 
    [ApprenticeshipId] BIGINT NULL,	

)
GO
CREATE NONCLUSTERED INDEX [IX_DataLockEventNonPayablePeriodFailures__DataLockEventNonPayablePeriodId] ON [Payments2].[DataLockEventNonPayablePeriodFailures] ([DataLockEventNonPayablePeriodId]) INCLUDE ([DataLockFailureId]) WITH (ONLINE = ON);