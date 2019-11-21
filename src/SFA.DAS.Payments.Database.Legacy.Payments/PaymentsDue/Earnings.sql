CREATE TABLE [PaymentsDue].[Earnings](
	[RequiredPaymentId] [uniqueidentifier] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[PlannedEndDate] [datetime] NOT NULL,
	[ActualEnddate] [datetime] NULL,
	[CompletionStatus] [int] NULL,
	[CompletionAmount] [decimal](15, 5) NULL,
	[MonthlyInstallment] [decimal](15, 5) NOT NULL,
	[TotalInstallments] [int] NOT NULL,
	[EndpointAssessorId] [varchar](7) NULL
) 