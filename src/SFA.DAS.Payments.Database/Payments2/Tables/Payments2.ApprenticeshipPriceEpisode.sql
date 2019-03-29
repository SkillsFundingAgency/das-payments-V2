CREATE TABLE [Payments2].[ApprenticeshipPriceEpisode]
(
	Id BIGINT NOT NULL IDENTITY CONSTRAINT PK_ApprenticeshipPriceEpisode PRIMARY KEY,
	ApprenticeshipId BIGINT NOT NULL CONSTRAINT FK_ApprenticeshipPriceEpisode__Apprenticeship FOREIGN KEY REFERENCES [Payments2].[Apprenticeship] (Id),
	StartDate Date NOT NULL,
	EndDate Date NULL,
	Cost DECIMAL NOT NULL,
	Removed BIT NOT NULL CONSTRAINT DF_ApprenticeshipPriceEpisode__Removed DEFAULT (0)
)
