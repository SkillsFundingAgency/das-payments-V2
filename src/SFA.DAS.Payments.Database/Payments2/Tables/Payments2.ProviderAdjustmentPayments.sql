CREATE TABLE [Payments2].[ProviderAdjustmentPayments]
(
	[Ukprn] [bigint] NOT NULL,
	[SubmissionId] [uniqueidentifier] NOT NULL,
	[SubmissionCollectionPeriod] [int] NOT NULL,
	[SubmissionAcademicYear] [int] NOT NULL,
	[PaymentType] [int] NOT NULL,
	[PaymentTypeName] [nvarchar](250) NOT NULL,
	[Amount] [decimal](15, 5) NOT NULL,
	[CollectionPeriodName] [varchar](8) NOT NULL,
	[CollectionPeriodMonth] [int] NOT NULL,
	[CollectionPeriodYear] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Ukprn] ASC,
	[SubmissionId] ASC,
	[SubmissionCollectionPeriod] ASC,
	[SubmissionAcademicYear] ASC,
	[PaymentType] ASC,
	[CollectionPeriodName] ASC
	)
)
