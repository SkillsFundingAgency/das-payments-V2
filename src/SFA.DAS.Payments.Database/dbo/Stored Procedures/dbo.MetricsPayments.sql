create procedure [dbo].[Metrics_Payments]
	@academicYear smallint,
	@collectionPeriod tinyint,
	@populateEarnings bit = 0
as
begin
	with ukprns as ( -- remove ukprns where there was no submissions and where there were failures
		select distinct ukprn from Payments2.Job j1 where AcademicYear = @academicYear and CollectionPeriod = @collectionPeriod
		--and not exists(select 1 from Payments2.Job j2 where j2.Ukprn = j1.Ukprn and j2.Status <> 2)
		--and not exists(select 1 from Payments2.Payment p where p.Ukprn = j1.Ukprn and AcademicYear = @academicYear and EarningEventId = '00000000-0000-0000-0000-000000000000')
	)
	select
		TransactionTypes.TransactionType as [Transaction Type],
		isnull(Earnings.EarningsYTD, 0) as [Earnings YTD],
		isnull(Earnings.EarningsACT1, 0) as [Earnings ACT1],
		isnull(Earnings.EarningsACT2, 0) as [Earnings ACT2],
		isnull(Earnings.NegativeEarningsYTD, 0) as [Negative Earnings],
		isnull(Earnings.NegativeEarningsACT1, 0) as [Negative Earnings ACT1],
		isnull(Earnings.NegativeEarningsACT2, 0) as [Negative Earnings ACT2],
		isnull(ActualPayments.ActualPaymentYTD, 0) as [Payments YTD],
		isnull(ActualPayments.ActualPaymentACT1, 0) as [Payments ACT1],
		isnull(ActualPayments.ActualPaymentACT2, 0) as [Payments ACT2],
		isnull([DataLockErrors], 0) as [Data Lock Errors],
		isnull([HeldBackCompletionPayments], 0) as [Held Back Completion],
		isnull([HeldBackCompletionPaymentsAct1], 0) as [HBCP ACT1],
		isnull([HeldBackCompletionPaymentsAct2], 0) as [HBCP ACT2]
	from (
			select n as TransactionType from (values (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14),(15),(16)) v(n)
		) as TransactionTypes
	
		left join (
			select
				TransactionType, 
				sum(case when Amount > 0 then Amount end) [EarningsYTD],
				sum(case when ContractType = 1 and Amount > 0 then Amount end) [EarningsACT1],
				sum(case when ContractType = 2 and Amount > 0 then Amount end) [EarningsACT2],
				sum(case when Amount < 0 then Amount end) [NegativeEarningsYTD],
				sum(case when ContractType = 1 and Amount < 0 then Amount end) [NegativeEarningsACT1],
				sum(case when ContractType = 2 and Amount < 0 then Amount end) [NegativeEarningsACT2]
			from
				Payments2.EarningEvent e with(nolock)
				join Payments2.EarningEventPeriod p with(nolock) on p.EarningEventId = e.EventId
				join ukprns on ukprns.Ukprn = e.Ukprn
			where
				e.AcademicYear = @academicYear
				and e.CollectionPeriod = @collectionPeriod
				and p.DeliveryPeriod <= @collectionPeriod
				and @populateEarnings = 1
			group by
				p.TransactionType
		) as Earnings
		on Earnings.TransactionType = TransactionTypes.TransactionType

		left join (
			select 
				TransactionType, 
				sum(Amount) [ActualPaymentYTD],
				sum(case when ContractType = 1 then Amount end) [ActualPaymentACT1],
				sum(case when ContractType = 2 then Amount end) [ActualPaymentACT2]
			from 
				Payments2.Payment with(nolock) 
				join ukprns on ukprns.Ukprn = Payments2.Payment.Ukprn
			where 
				AcademicYear = @academicYear
				and CollectionPeriod <= @collectionPeriod
			group by 
				TransactionType
		) as ActualPayments
		on ActualPayments.TransactionType = TransactionTypes.TransactionType

		left join (
			select 
				TransactionType, 
				sum(cast(json_value(EarningPeriod, '$.Amount') as decimal(15,5))) [DataLockErrors]
			from 
				Payments2.DataLockFailure with(nolock) 
				join ukprns on ukprns.Ukprn = Payments2.DataLockFailure.Ukprn
			where 
				AcademicYear = @academicYear
				and CollectionPeriod = @collectionPeriod
				and DeliveryPeriod <= CollectionPeriod
			group by 
				TransactionType
		) as [DataLockErrors]
		on [DataLockErrors].TransactionType = TransactionTypes.TransactionType

		left join ( -- we're not recording HBCP yet so working out all TT2 earnings that were not paid and had no DL error
			select
				ep.TransactionType,
				sum(case when p.Id is null and f.Id is null then ep.Amount end) as HeldBackCompletionPayments,
				sum(case when p.Id is null and f.Id is null and e.ContractType = 1 then ep.Amount end) as HeldBackCompletionPaymentsAct1,
				sum(case when p.Id is null and f.Id is null and e.ContractType = 2 then ep.Amount end) as HeldBackCompletionPaymentsAct2
			from
				Payments2.EarningEvent e with(nolock)
				join Payments2.EarningEventPeriod ep with(nolock) on ep.EarningEventId = e.EventId
				join ukprns on ukprns.Ukprn = e.Ukprn
				left join Payments2.Payment p with(nolock) on p.EarningEventId = ep.EarningEventId
					and p.DeliveryPeriod = ep.DeliveryPeriod
					and p.TransactionType = ep.TransactionType
				left join Payments2.DataLockFailure f with(nolock) on f.EarningEventId = e.EventId
					and f.DeliveryPeriod = ep.DeliveryPeriod
					and f.TransactionType = ep.TransactionType
			where
				ep.TransactionType = 2
				and ep.Amount <> 0
				and e.AcademicYear = @academicYear
				and e.CollectionPeriod = @collectionPeriod
				and ep.DeliveryPeriod <= @collectionPeriod
			group by
				ep.TransactionType
		) as HeldBackCompletionPayments
		on HeldBackCompletionPayments.TransactionType = TransactionTypes.TransactionType

	order by
		1
end
GO 
GRANT EXECUTE ON [dbo].[Metrics_Payments] to [DataViewer]
GO
