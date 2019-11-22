CREATE TABLE [Payments2].[CurrentPriceEpisode]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_CurrentPriceEpisode PRIMARY KEY CLUSTERED,	
	[PriceEpisodeIdentifier] NVARCHAR(100) NOT NULL,
	AgreedPrice DECIMAL(15, 5)  NOT NULL,
    [Ukprn] BIGINT NOT NULL, 
    [Uln] BIGINT NOT NULL, 
    [JobId]  BIGINT NOT NULL, 
	[MessageType] NVARCHAR(2000) NOT NULL,
	[Message] NVARCHAR(MAX) NOT NULL,
    [CreationDate] DATETIME2 NOT NULL DEFAULT sysutcdatetime()
)

GO

CREATE INDEX [IX_CurrentPriceEpisode_Ukprn_JobId] ON [Payments2].[CurrentPriceEpisode] 
([Ukprn], [JobId])
