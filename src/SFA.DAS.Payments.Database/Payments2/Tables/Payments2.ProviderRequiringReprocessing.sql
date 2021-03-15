Create TABLE [Payments2].[ProviderRequiringReprocessing]
( 
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_ProviderRequiringReprocessing PRIMARY KEY CLUSTERED,
	Ukprn BIGINT NOT NULL, 
	CreationDate DateTimeOffset(7) DEFAULT SYSDATETIMEOFFSET(),
)
GO

CREATE UNIQUE INDEX [UX_ProviderRequiringReprocessing__Ukprn] ON [Payments2].[ProviderRequiringReprocessing]
(
	Ukprn
) 
GO

