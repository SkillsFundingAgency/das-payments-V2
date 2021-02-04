--FundingSourceLevyTransaction

IF EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_FundingSourceLevyTransaction_LogicalDuplicates' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	DROP INDEX UX_FundingSourceLevyTransaction_LogicalDuplicates ON Payments2.FundingSourceLevyTransaction
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN DuplicateNumber
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'TransactionType' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN TransactionType
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'SfaContributionPercentage' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN SfaContributionPercentage
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'LearnerUln' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN LearnerUln
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'LearningAimReference' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN LearningAimReference
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'LearnerReferenceNumber' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN LearnerReferenceNumber
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'LearningAimProgrammeType' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN LearningAimProgrammeType
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'LearningAimStandardCode' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN LearningAimStandardCode
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'LearningAimFrameworkCode' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN LearningAimFrameworkCode
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'LearningAimPathwayCode' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN LearningAimPathwayCode
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'LearningAimFundingLineType' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN LearningAimFundingLineType
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'LearningStartDate' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN LearningStartDate
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'ApprenticeshipId' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN ApprenticeshipId
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'ApprenticeshipEmployerType' AND object_id = OBJECT_ID(N'Payments2.FundingSourceLevyTransaction'))
	ALTER TABLE Payments2.FundingSourceLevyTransaction DROP COLUMN ApprenticeshipEmployerType
GO



	