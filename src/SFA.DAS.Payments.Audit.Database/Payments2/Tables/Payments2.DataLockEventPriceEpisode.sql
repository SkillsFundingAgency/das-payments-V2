CREATE TABLE [Payments2].[DataLockEventPriceEpisode]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_DataLockEventPriceEpisode PRIMARY KEY CLUSTERED,
	DataLockEventId UNIQUEIDENTIFIER NOT NULL, 
	PriceEpisodeIdentifier NVARCHAR(50) NOT NULL,
	SfaContributionPercentage DECIMAL(15,5) NOT NULL,
	TotalNegotiatedPrice1 DECIMAL(15,5) NOT NULL,
	TotalNegotiatedPrice2 DECIMAL(15,5) NULL,
	TotalNegotiatedPrice3 DECIMAL(15,5) NULL,
	TotalNegotiatedPrice4 DECIMAL(15,5) NULL,
	StartDate DATETIME2 NOT NULL,
	EffectiveTotalNegotiatedPriceStartDate DATETIME2 NULL,
	PlannedEndDate DATETIME2 NOT NULL,
	ActualEndDate DATETIME2 NULL,
	NumberOfInstalments INT NOT NULL, 
	InstalmentAmount DECIMAL(15,5) NOT NULL,
	CompletionAmount DECIMAL(15,5) NOT NULL,
	Completed BIT NOT NULL,
    EmployerContribution DECIMAL(15,5) NULL,
    CompletionHoldBackExemptionCode INT NULL,
	CreationDate DATETIMEOFFSET NOT NULL,
	AcademicYear SMALLINT NULL,
	CollectionPeriod TINYINT NULL
)
GO

--CREATE NONCLUSTERED INDEX [IX_DataLockEventPriceEpisode__DataLockEventId] ON [Payments2].[DataLockEventPriceEpisode] 
--(	
--	[DataLockEventId]
--) 
--WITH (ONLINE = ON)
--GO

CREATE INDEX [IX_DataLockEventPriceEpisode_AuditDataFactory] ON [Payments2].[DataLockEventPriceEpisode] 
(
	[DataLockEventId]
)
INCLUDE
(
	[AcademicYear],
	[CollectionPeriod]
) 
WITH (ONLINE = ON)
GO