
IF EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_DataLockEvent_LogicalDuplicates'	AND object_id = OBJECT_ID(N'Payments2.DataLockEvent'))
	DROP INDEX UX_DataLockEvent_LogicalDuplicates ON Payments2.DataLockEvent 
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.DataLockEvent'))
	ALTER TABLE Payments2.DataLockEvent DROP COLUMN DuplicateNumber
GO

--FundingSourceEvent
IF EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_FundingSourceEvent_LogicalDuplicates' AND object_id = OBJECT_ID(N'Payments2.FundingSourceEvent'))
	DROP INDEX UX_FundingSourceEvent_LogicalDuplicates ON Payments2.FundingSourceEvent 
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.FundingSourceEvent'))
	ALTER TABLE Payments2.FundingSourceEvent DROP COLUMN DuplicateNumber
GO

--Required Payment event
IF EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_RequiredPaymentEvent_LogicalDuplicates' AND object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent'))
	DROP INDEX UX_RequiredPaymentEvent_LogicalDuplicates ON Payments2.RequiredPaymentEvent 
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent'))
	ALTER TABLE Payments2.RequiredPaymentEvent DROP COLUMN DuplicateNumber
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'EventType' AND object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent'))
	ALTER TABLE Payments2.RequiredPaymentEvent DROP COLUMN EventType
GO

--payments
IF EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_Payment_LogicalDuplicates' AND object_id = OBJECT_ID(N'Payments2.Payment'))
	DROP INDEX UX_Payment_LogicalDuplicates ON Payments2.Payment
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = N'DuplicateNumber' AND object_id = OBJECT_ID(N'Payments2.Payment'))
	ALTER TABLE Payments2.Payment DROP COLUMN DuplicateNumber
GO