--FundingSourceLevyTransaction

IF EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_FundingSourceLevyTransaction_LogicalDuplicates' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	DROP INDEX UX_FundingSourceLevyTransaction_LogicalDuplicates ON Payments2.FundingSourceLevyTransaction
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN DuplicateNumber
GO