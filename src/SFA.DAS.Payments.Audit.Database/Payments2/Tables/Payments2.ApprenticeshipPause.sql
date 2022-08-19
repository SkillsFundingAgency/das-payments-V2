CREATE TABLE [Payments2].[ApprenticeshipPause] (
    [Id]               BIGINT NOT NULL IDENTITY (1, 1) CONSTRAINT PK_ApprenticeshipPause PRIMARY KEY CLUSTERED,
    [ApprenticeshipId] BIGINT NOT NULL,
    [PauseDate]        DATE   NOT NULL,
    [ResumeDate]       DATE   NULL
);



GO


