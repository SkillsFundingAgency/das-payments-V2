Create TABLE [Payments2].[ProvidersRequiringReprocessing]
( 
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_ProvidersRequiringReprocessing PRIMARY KEY CLUSTERED,
	Ukprn BIGINT NULL, 
)
GO

CREATE INDEX [UX_ProvidersRequiringReprocessing__Ukprn] ON [Payments2].[ProvidersRequiringReprocessing]
(
	Ukprn
)
GO

