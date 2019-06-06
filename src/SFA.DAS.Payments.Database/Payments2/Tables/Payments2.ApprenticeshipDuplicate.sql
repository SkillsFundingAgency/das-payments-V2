CREATE TABLE [Payments2].[ApprenticeshipDuplicate]
(
	Id BIGINT NOT NULL IDENTITY CONSTRAINT PK_ApprenticeshipDuplicate PRIMARY KEY,
	ApprenticeshipId BIGINT NOT NULL CONSTRAINT FK_ApprenticeshipDuplicate__Apprenticeship FOREIGN KEY REFERENCES [Payments2].[Apprenticeship] (Id),
	Ukprn BIGINT NOT NULL ,
	Uln BIGINT NOT NULL
)
