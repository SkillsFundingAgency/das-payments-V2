CREATE TABLE [Payments2].[AuditMigrationJob]
(
	[Id] INT NOT NULL IDENTITY(1,1) CONSTRAINT PK_AuditArchiveJob PRIMARY KEY CLUSTERED,
	[AcademicYear] SMALLINT NOT NULL,
	[Period] TINYINT NOT NULL,
	[StartDate] DATETIME2 NULL,
	[CompletionDate] DATETIME2 NULL,
	[Failed] BIT NOT NULL CONSTRAINT DF_AuditMigrationJob__Failed DEFAULT (0),
	[CreationDate] DATETIME2 NOT NULL CONSTRAINT DF_AuditMigrationJob__CreationDate DEFAULT sysutcdatetime()
)
GO

CREATE NONCLUSTERED INDEX [IX_AuditMigrationJob__AcademicYear_Period] on [Payments2].[AuditMigrationJob] 
(
	[AcademicYear],
	[Period]
)
WITH (ONLINE = ON)
GO
