﻿CREATE TABLE [Payments2].[DataLockEventNonPayablePeriodFailures]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_DataLockEventNonPayablePeriodFailures PRIMARY KEY CLUSTERED,	
	DataLockEventNonPayablePeriodId UNIQUEIDENTIFIER NOT NULL, 
	DataLockFailureId TINYINT NOT NULL,
	CreationDate DATETIMEOFFSET NOT NULL, 
    ApprenticeshipId BIGINT NULL,
	AcademicYear SMALLINT NULL,
	CollectionPeriod TINYINT NULL,
)
GO

CREATE NONCLUSTERED INDEX [IX_DataLockEventNonPayablePeriodFailures__AuditDataFactory] ON [Payments2].[DataLockEventNonPayablePeriodFailures]
(
	[AcademicYear],
	[CollectionPeriod],
	[DataLockEventNonPayablePeriodId]
)
WITH (ONLINE = ON)
GO