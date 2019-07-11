CREATE TABLE [Payments2].[ApprenticeshipPause]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	ApprenticeshipId BIGINT NOT NULL,
	PauseDate DATE NOT NULL,
	ResumeDate DATE NULL
)

GO

CREATE INDEX [IX_ApprenticeshipPause_ApprenticeshipId] ON [Payments2].[ApprenticeshipPause] (ApprenticeshipId)
