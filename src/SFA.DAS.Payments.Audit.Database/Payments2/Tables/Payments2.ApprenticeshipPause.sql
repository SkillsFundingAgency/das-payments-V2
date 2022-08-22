CREATE TABLE [Payments2].[ApprenticeshipPause] (
    [Id]               BIGINT NOT NULL CONSTRAINT PK_ApprenticeshipPause PRIMARY KEY CLUSTERED,
    [ApprenticeshipId] BIGINT NOT NULL,
    [PauseDate]        DATE   NOT NULL,
    [ResumeDate]       DATE   NULL
);
GO

CREATE INDEX [IX_ApprenticeshipPause__ApprenticeshipId] ON [Payments2].[ApprenticeshipPriceEpisode]
(
  ApprenticeshipId
) 
GO


