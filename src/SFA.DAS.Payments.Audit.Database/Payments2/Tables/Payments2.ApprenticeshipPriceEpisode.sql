CREATE TABLE [Payments2].[ApprenticeshipPriceEpisode]
(
	Id 					BIGINT 			NOT NULL CONSTRAINT PK_ApprenticeshipPriceEpisode PRIMARY KEY CLUSTERED,
	ApprenticeshipId 	BIGINT 			NOT NULL,
	StartDate 			Date 			NOT NULL,
	EndDate 			Date 			NULL,
	Cost 				DECIMAL(15,5) 	NOT NULL,
	Removed 			BIT 			NOT NULL,
	CreationDate 		DATETIMEOFFSET 	NOT NULL,
)
GO

CREATE INDEX [IX_ApprenticeshipPriceEpisode__ApprenticeshipId] ON [Payments2].[ApprenticeshipPriceEpisode]
(
	[ApprenticeshipId]
) 
GO
