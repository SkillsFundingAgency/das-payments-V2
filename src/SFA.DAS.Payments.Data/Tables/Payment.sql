CREATE TABLE [Payments2].[Payment]
(
	[Id] [uniqueidentifier] NOT NULL CONSTRAINT PK_Payment_Id PRIMARY KEY,
	[Ukprn] [bigint] NOT NULL,
	[LearnerReferenceNumber] [nvarchar](50) NOT NULL,
	[LearnAimReference] [nvarchar](50) NOT NULL,
	[TransactionType] [int] NOT NULL,
	[ApprenticeshipKey] [nvarchar](250) NOT NULL,
	[PriceEpisodeIdentifier] [nvarchar](50) NOT NULL,
	[DeliveryPeriod] [char](8) NOT NULL,
	[CollectionPeriod] [char](8) NOT NULL,
	[Amount] [decimal](15, 5) NOT NULL
)

GO

CREATE INDEX [IX_Payment_ApprenticeshipKey] ON [Payments2].[Payment] ([ApprenticeshipKey])
