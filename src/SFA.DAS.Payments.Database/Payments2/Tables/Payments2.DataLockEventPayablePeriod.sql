CREATE TABLE [Payments2].[DataLockEventPayablePeriod]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_DataLockEventPayablePeriod PRIMARY KEY CLUSTERED,	
	DataLockEventId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_DataLockEventPayablePeriod__DataLockEvent FOREIGN KEY REFERENCES [Payments2].[DataLockEvent] (EventId) ON DELETE CASCADE, 
	PriceEpisodeIdentifier NVARCHAR(50) NULL,
	TransactionType TINYINT NOT NULL, 
	DeliveryPeriod TINYINT NOT NULL, 
	Amount DECIMAL(15,5) NOT NULL,
	SfaContributionPercentage DECIMAL(15,5) NULL,
	CreationDate DATETIMEOFFSET NOT NULL CONSTRAINT DF_DataLockEventPayablePeriod__CreationDate DEFAULT (SYSDATETIMEOFFSET()),
	LearningStartDate DATETIME2 NULL,
    ApprenticeshipId BIGINT NULL,
    ApprenticeshipPriceEpisodeId BIGINT NULL,
	ApprenticeshipEmployerType TINYINT NULL,
)
GO;

CREATE NONCLUSTERED INDEX [IX_DataLockEventPayablePeriod__DataLockEventId] ON [Payments2].[DataLockEventPayablePeriod] ([DataLockEventId]) 
WITH (ONLINE = ON);
GO;

CREATE NONCLUSTERED INDEX [DataLockEventPayablePeriod_MatchedLearner_Import] ON [Payments2].[DataLockEventPayablePeriod] ([PriceEpisodeIdentifier],[TransactionType],[Amount])
INCLUDE ([DataLockEventId],[DeliveryPeriod],[SfaContributionPercentage],[LearningStartDate],[ApprenticeshipId],[ApprenticeshipPriceEpisodeId],[ApprenticeshipEmployerType])
WITH (ONLINE = ON);