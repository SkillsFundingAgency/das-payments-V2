CREATE TABLE [Payments].[Payments]
(
	[PaymentId] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[RequiredPaymentId] [uniqueidentifier] NOT NULL,
	[DeliveryMonth] [int] NOT NULL,
	[DeliveryYear] [int] NOT NULL,
	[CollectionPeriodName] [varchar](8) NOT NULL,
	[CollectionPeriodMonth] [int] NOT NULL,
	[CollectionPeriodYear] [int] NOT NULL,
	[FundingSource] [int] NOT NULL,
	[TransactionType] [int] NOT NULL,
	[Amount] [decimal](15, 5) NULL,
)

GO


ALTER TABLE [Payments].[Payments] ADD  DEFAULT (newid()) FOR [PaymentId]
GO

