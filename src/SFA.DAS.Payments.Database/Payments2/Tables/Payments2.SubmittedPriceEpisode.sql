CREATE TABLE [Payments2].[SubmittedPriceEpisode]
(
	[Id] INT NOT NULL CONSTRAINT PK_SubmittedPriceEpisode PRIMARY KEY IDENTITY, 
    [Ukprn] BIGINT NOT NULL, 
    [LearnerReferenceNumber] NVARCHAR(12) NOT NULL, 
    [PriceEpisodeIdentifier] VARCHAR(25) NOT NULL, 
    [IlrDetails] NVARCHAR(MAX) NOT NULL,
)

GO

CREATE UNIQUE INDEX [IX_SubmittedPriceEpisode_Key] ON [Payments2].[SubmittedPriceEpisode] ([Ukprn], [LearnerReferenceNumber])
