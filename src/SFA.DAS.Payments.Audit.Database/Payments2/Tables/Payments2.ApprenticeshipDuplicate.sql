﻿CREATE TABLE [Payments2].[ApprenticeshipDuplicate]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_ApprenticeshipDuplicate PRIMARY KEY CLUSTERED,
	ApprenticeshipId BIGINT NOT NULL,
	Ukprn BIGINT NOT NULL ,
	Uln BIGINT NOT NULL
)
GO