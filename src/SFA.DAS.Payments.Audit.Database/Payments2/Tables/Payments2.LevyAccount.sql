CREATE TABLE [Payments2].[LevyAccount]
(
	[AccountId] 		BIGINT 			NOT NULL CONSTRAINT PK_LevyAccount PRIMARY KEY CLUSTERED, 
	[AccountName] 		VARCHAR(255) 	NOT NULL, 
	[Balance] 			DECIMAL(18, 4) 	NOT NULL, 
	[IsLevyPayer] 		BIT 			NOT NULL, 
	[TransferAllowance] DECIMAL(18, 4) 	NOT NULL
)
