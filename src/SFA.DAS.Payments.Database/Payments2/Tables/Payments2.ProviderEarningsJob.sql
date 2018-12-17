Create TABLE [Payments2].[ProviderEarningsJob]( 
	JobId BIGINT NOT NULL CONSTRAINT PK_ProviderEarningsJob PRIMARY KEY CLUSTERED CONSTRAINT FK_ProviderEarningsJob__Job FOREIGN KEY REFERENCES [Payments2].[Job] (JobId),
	DCJobId BIGINT NOT NULL,
	Ukprn BIGINT NOT NULL, 
	IlrSubmissionTime DATETIME NOT NULL,
	CollectionYear SMALLINT NOT NULL,
	CollectionPeriod TINYINT NOT NULL,
	INDEX IX_ProviderEarningsJob__DCJobId_Ukprn NONCLUSTERED (DCJobId, Ukprn)
)
