Create TABLE [Payments2].[JobProviderEarnings]( 
	JobId BIGINT NOT NULL CONSTRAINT PK_JobProviderEarnings PRIMARY KEY CLUSTERED CONSTRAINT FK_JobProviderEarnings__Job FOREIGN KEY REFERENCES [Payments2].[Job] (JobId),
	DCJobId BIGINT NOT NULL,
	Ukprn BIGINT NOT NULL, 
	IlrSubmissionTime DATETIME2 NOT NULL,
	CollectionYear VARCHAR(4) NOT NULL,
	CollectionPeriod TINYINT NOT NULL,
	INDEX IX_JobProviderEarnings__DCJobId_Ukprn NONCLUSTERED (DCJobId, Ukprn)
)
