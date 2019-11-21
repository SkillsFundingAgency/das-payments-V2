CREATE TABLE [PaymentsDue].[RequiredPayments]
(
	[Id] [uniqueidentifier] NOT NULL,
	[CommitmentId] [bigint] NULL,
	[CommitmentVersionId] [varchar](25) NULL,
	[AccountId] [bigint] NULL,
	[AccountVersionId] [varchar](50) NULL,
	[Uln] [bigint] NULL,
	[LearnRefNumber] [varchar](12) NULL,
	[AimSeqNumber] [int] NULL,
	[Ukprn] [bigint] NULL,
	[IlrSubmissionDateTime] [datetime] NULL,
	[PriceEpisodeIdentifier] [varchar](25) NULL,
	[StandardCode] [bigint] NULL,
	[ProgrammeType] [int] NULL,
	[FrameworkCode] [int] NULL,
	[PathwayCode] [int] NULL,
	[ApprenticeshipContractType] [int] NULL,
	[DeliveryMonth] [int] NULL,
	[DeliveryYear] [int] NULL,
	[CollectionPeriodName] [varchar](8) NOT NULL,
	[CollectionPeriodMonth] [int] NOT NULL,
	[CollectionPeriodYear] [int] NOT NULL,
	[TransactionType] [int] NULL,
	[AmountDue] [decimal](15, 5) NULL,
	[SfaContributionPercentage] [decimal](15, 5) NULL,
	[FundingLineType] [varchar](100) NULL,
	[UseLevyBalance] [bit] NULL,
	[LearnAimRef] [varchar](8) NULL,
	[LearningStartDate] [datetime] NULL,
)
GO

ALTER TABLE [PaymentsDue].[RequiredPayments] ADD  DEFAULT (newid()) FOR [Id]
GO