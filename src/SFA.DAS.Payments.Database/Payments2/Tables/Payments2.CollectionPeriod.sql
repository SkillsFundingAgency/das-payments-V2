CREATE TABLE [Payments2].[CollectionPeriod]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_CollectionPeriod PRIMARY KEY CLUSTERED,
	[AcademicYear] SMALLINT NOT NULL,
	[Period] TINYINT NOT NULL,
	[ReferenceDataValidationDate] DATETIME2 NULL,
	[CompletionDate] DATETIME2 NOT NULL,
	[AuditMigrationStartDate] DATETIME2 NULL,
	[AuditMigrationCompletionDate] DATETIME2 NULL,
	[CreationDate] DATETIME2 NOT NULL DEFAULT sysutcdatetime()
)
