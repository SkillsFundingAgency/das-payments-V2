CREATE TABLE [Payments2].[TestingProvider]
(
	[Ukprn] INT NOT NULL PRIMARY KEY, 
    [LastUsed] DATETIME NOT NULL DEFAULT GetUtcDate()
)
