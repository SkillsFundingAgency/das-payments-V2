CREATE TABLE [Payments2].[ApprenticeshipDuplicate]
(
	[Id] 				BIGINT NOT NULL CONSTRAINT PK_ApprenticeshipDuplicate PRIMARY KEY CLUSTERED,
	[ApprenticeshipId] 	BIGINT NOT NULL,
	[Ukprn] 			BIGINT NOT NULL,
	[Uln] 				BIGINT NOT NULL
)
GO

CREATE INDEX [IX_Payments2_ApprenticeshipDuplicate__Uln] ON [Payments2].[ApprenticeshipDuplicate] 
(
	[Uln]
)
GO