CREATE TABLE [Payments].[Periods](
	[PeriodName] [char](8) NOT NULL,
	[CalendarMonth] [int] NOT NULL,
	[CalendarYear] [int] NOT NULL,
	[AccountDataValidAt] [datetime] NULL,
	[CommitmentDataValidAt] [datetime] NULL,
	[CompletionDateTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PeriodName] ASC
)
) 