CREATE TABLE [Payments2].[Payment]
(
	[Id] [uniqueidentifier] NOT NULL CONSTRAINT PK_Payment_Id PRIMARY KEY,
	[Ukprn] [bigint] NOT NULL,
	ContractType INT NOT NULL,
	[TransactionType] [int] NOT NULL,
	[PriceEpisodeIdentifier] [nvarchar](50) NOT NULL,
	FundingSource INT NOT NULL,
	[Amount] [decimal](15, 5) NOT NULL, 
	DeliveryPeriodMonth INT NOT NULL,
	DeliveryPeriodYear INT NOT NULL,
	CollectionPeriodName [char](8) NOT NULL,
	CollectionPeriodMonth INT NOT NULL,
	CollectionPeriodYear INT NOT NULL,
	[LearnerReferenceNumber] [nvarchar](50) NOT NULL,
	[LearnAimReference] [nvarchar](50) NOT NULL,
	FrameworkCode INT NOT NULL,
	PathwayCode INT NOT NULL,
	StandardCode INT NOT NULL,
    [ProgrammeType] INT NOT NULL
)

GO

CREATE INDEX [IX_Payment_ApprenticeshipKey] ON [Payments2].[Payment]
(
[Ukprn],
[LearnerReferenceNumber],
FrameworkCode,
PathwayCode,
[ProgrammeType],
StandardCode,
[LearnAimReference]
)
