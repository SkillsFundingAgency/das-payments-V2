CREATE TABLE [Payments2].[EarningEventPeriod]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_EarningEventPeriod PRIMARY KEY CLUSTERED,	
	EarningEventId UNIQUEIDENTIFIER NOT NULL,  
	PriceEpisodeIdentifier NVARCHAR(50) NULL,
	TransactionType TINYINT NOT NULL,
	DeliveryPeriod TINYINT NOT NULL, 
	Amount DECIMAL(15,5) NOT NULL,
	SfaContributionPercentage DECIMAL(15,5) NULL,
	CreationDate DATETIMEOFFSET NOT NULL,	
	CensusDate DATETIME2  NULL,
	AcademicYear SMALLINT NULL,
	CollectionPeriod TINYINT NULL
)
GO

--CREATE NONCLUSTERED INDEX [IX_EarningEventPeriod__EarningEventId] ON [Payments2].[EarningEventPeriod]
--(
--	[EarningEventId]
--)
--WITH (ONLINE = ON)
--GO

--CREATE NONCLUSTERED INDEX [IX_EarningEventPeriod__Metrics] ON [Payments2].[EarningEventPeriod]
--(
--	[EarningEventId]
--)
--INCLUDE 
--(
--	[TransactionType],
--	[Amount]
--)
--WITH (ONLINE = ON)
--GO

CREATE INDEX [IX_EarningEventPeriod__AuditDataFactory] ON [Payments2].[EarningEventPeriod]
(
	[AcademicYear],
	[CollectionPeriod],
	[EarningEventId]
)
WITH (ONLINE = ON)
GO