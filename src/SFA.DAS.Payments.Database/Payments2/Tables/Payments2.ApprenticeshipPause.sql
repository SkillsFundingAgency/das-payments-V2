CREATE TABLE [Payments2].[ApprenticeshipPause] (
    [Id]               BIGINT IDENTITY (1, 1) NOT NULL,
    [ApprenticeshipId] BIGINT NOT NULL,
    [PauseDate]        DATE   NOT NULL,
    [ResumeDate]       DATE   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApprenticeshipPause_Apprenticeship] FOREIGN KEY ([ApprenticeshipId]) REFERENCES [Payments2].[Apprenticeship] ([Id])
);



GO


