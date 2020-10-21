CREATE TABLE [Metrics].[CollectionPeriodTolerance]
(
	[Id]								INT				IDENTITY(1,1) NOT NULL		  CONSTRAINT PK_CollectionPeriodTolerance PRIMARY KEY CLUSTERED,
	[AcademicYear]						SMALLINT		NOT NULL,
	[CollectionPeriod]					TINYINT			NOT NULL,
	[SubmissionToleranceLower]			DECIMAL(15,5)	NOT NULL,
	[SubmissionToleranceUpper]			DECIMAL(15,5)	NOT NULL,
	[PeriodEndToleranceLower]			DECIMAL(15,5)	NOT NULL,
	[PeriodEndToleranceUpper]			DECIMAL(15,5)	NOT NULL,
	
	INDEX IX_CollectionPeriodTolerance__AcademicYear_CollectionPeriod (AcademicYear, CollectionPeriod),
	CONSTRAINT UQ_CollectionPeriodTolerance UNIQUE (AcademicYear, CollectionPeriod)
);
GO
