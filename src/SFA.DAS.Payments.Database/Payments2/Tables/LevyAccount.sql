CREATE TABLE [Payments2].[LevyAccount]
(
	[AccountId] BIGINT NOT NULL CONSTRAINT PK_LevyAccount PRIMARY KEY, 
    [SequenceId] BIGINT NULL, 
    [AccountHashId] VARCHAR(125) NULL, 
    [AccountName] VARCHAR(255) NOT NULL, 
    [Balance] DECIMAL(18, 4) NOT NULL, 
    [IsLevyPayer] BIT NOT NULL, 
    [TransferAllowance] DECIMAL(18, 4) NOT NULL
)
