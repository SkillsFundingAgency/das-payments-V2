CREATE TABLE [Payments2].[DataLockFailure]
(
	[Id] BIGINT NOT NULL CONSTRAINT PK_DataLockFailure PRIMARY KEY IDENTITY, 
	[DataLockEventId] UNIQUEIDENTIFIER,
	[EarningEventId] UNIQUEIDENTIFIER NOT NULL,
    [Ukprn] BIGINT NOT NULL, 
    [LearnerUln] BIGINT NOT NULL, 
    [LearnerReferenceNumber] NVARCHAR(50) NOT NULL, 
    [LearningAimReference] NVARCHAR(8) NOT NULL, 
    [LearningAimProgrammeType] INT NOT NULL, 
    [LearningAimStandardCode] INT NOT NULL, 
    [LearningAimFrameworkCode] INT NOT NULL, 
    [LearningAimPathwayCode] INT NOT NULL, 
    [AcademicYear] SMALLINT NOT NULL, 
    [TransactionType] TINYINT NOT NULL,
    [DeliveryPeriod] TINYINT NOT NULL, 
    [CollectionPeriod] TINYINT NOT NULL, 
    [EarningPeriod] NVARCHAR(MAX) NOT NULL, 
    [Amount] DECIMAL(15, 5) NOT NULL,
    [CreationDate] DATETIME2 NOT NULL DEFAULT sysutcdatetime()
)

GO

CREATE UNIQUE INDEX [IX_DataLockFailure_Key] ON [Payments2].[DataLockFailure] ([Ukprn], [LearnerReferenceNumber], [LearningAimFrameworkCode], [LearningAimPathwayCode], [LearningAimProgrammeType], [LearningAimReference], [LearningAimStandardCode], [AcademicYear], [DeliveryPeriod], [TransactionType])
