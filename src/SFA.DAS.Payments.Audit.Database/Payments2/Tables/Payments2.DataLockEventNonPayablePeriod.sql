CREATE TABLE [Payments2].[DataLockEventNonPayablePeriod]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_DataLockEventNonPayablePeriod PRIMARY KEY CLUSTERED,	
	DataLockEventId UNIQUEIDENTIFIER NOT NULL, 
	DataLockEventNonPayablePeriodId UNIQUEIDENTIFIER NOT NULL,
	PriceEpisodeIdentifier NVARCHAR(50) NULL,
	TransactionType TINYINT NOT NULL,	
	DeliveryPeriod TINYINT NOT NULL, 
	Amount DECIMAL(15,5) NOT NULL,
	SfaContributionPercentage DECIMAL(15,5) NULL,
	CreationDate DATETIMEOFFSET NOT NULL,
	LearningStartDate DATETIME2 NULL,
	AcademicYear SMALLINT NULL,
	CollectionPeriod TINYINT NULL,
)
GO

CREATE NONCLUSTERED INDEX [IX_DataLockEventNonPayablePeriod__AuditDataFactory] ON [Payments2].[DataLockEventNonPayablePeriod]
(
	[AcademicYear],
	[CollectionPeriod],
	[DataLockEventId]
)
WITH (ONLINE = ON)
GO
