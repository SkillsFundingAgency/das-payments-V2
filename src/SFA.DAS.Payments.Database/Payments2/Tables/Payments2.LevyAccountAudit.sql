CREATE TABLE [Payments2].[LevyAccountAudit]
(
	[AccountId] BIGINT NOT NULL PRIMARY KEY,
	[AcademicYear] SMALLINT NOT NULL,
	[CollectionPeriod] TINYINT NOT NULL,
	[LevyAccountBalance] DECIMAL(18, 4) NOT NULL,
	[RemainingTransferAllowance] DECIMAL(18, 4) NOT NULL,
	[IsLevyPayer] BIT NOT NULL
)
