CREATE TABLE [Payments2].[LevyAccountAudit]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_LevyAccountAudit PRIMARY KEY CLUSTERED,
	[AccountId] BIGINT NOT NULL,
	[AcademicYear] SMALLINT NOT NULL,
	[CollectionPeriod] TINYINT NOT NULL,
	[SourceLevyAccountBalance] DECIMAL(18, 4) NOT NULL,
	[AdjustedLevyAccountBalance] DECIMAL(18, 4) NOT NULL,
	[SourceTransferAllowance] DECIMAL(18, 4) NOT NULL,
	[AdjustedTransferAllowance] DECIMAL(18, 4) NOT NULL,
	[IsLevyPayer] BIT NOT NULL,
	[CreationDate] DATETIMEOFFSET NOT NULL CONSTRAINT DF_LevyAccountAudit__CreationDate DEFAULT (SYSDATETIMEOFFSET())
)
