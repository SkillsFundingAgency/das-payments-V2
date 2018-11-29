CREATE TABLE [Payments2].[EarningEventPriceEpisode]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_EarningEventPriceEpisode PRIMARY KEY CLUSTERED,
	EarningEventId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_EarningEventPriceEpisode__EarningEvent FOREIGN KEY REFERENCES [Payments2].[EarningEvent] (EarningEventId), 
	PriceEpisodeIdentifier NVARCHAR(50) NOT NULL,
	SfaContributionPercentage DECIMAL(15,5) NOT NULL,
	TotalNegotiatedPrice1 DECIMAL(15,5) NOT NULL,
	TotalNegotiatedPrice2 DECIMAL(15,5) NULL,
	TotalNegotiatedPrice3 DECIMAL(15,5) NULL,
	TotalNegotiatedPrice4 DECIMAL(15,5) NULL,
	StartDate DATETIME NOT NULL,
	PlannedEndDate DATETIME NOT NULL,
	ActualEndDate DATETIME NULL,
	NumberOfInstalments INT NOT NULL, 
	InstalmentAmount DECIMAL(15,5) NOT NULL,
	CompletionAmount DECIMAL(15,5) NOT NULL,
	Completed BIT NOT NULL,
	CreationDate DATETIME2 NOT NULL CONSTRAINT DF_EarningEventPriceEpisode__CreationDate DEFAULT (SYSDATETIME())	
)
