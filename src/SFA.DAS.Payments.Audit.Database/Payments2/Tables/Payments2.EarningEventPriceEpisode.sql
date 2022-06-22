CREATE TABLE [Payments2].[EarningEventPriceEpisode]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_EarningEventPriceEpisode PRIMARY KEY CLUSTERED,
	EarningEventId UNIQUEIDENTIFIER NOT NULL, 
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
	AgreedPrice DECIMAL(15, 5) NULL,
	CourseStartDate DATETIME2  NULL,
	AcademicYear SMALLINT NULL,
	CollectionPeriod TINYINT NULL,
)
GO

CREATE NONCLUSTERED INDEX [IX_EarningEventPriceEpisode__AuditDataFactory] ON [Payments2].[EarningEventPriceEpisode]
(
	[AcademicYear],
	[CollectionPeriod],
	[EarningEventId]
)
WITH (ONLINE = ON)
GO
