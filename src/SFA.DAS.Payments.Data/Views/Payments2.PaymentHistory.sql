create view [Payments2].[PaymentHistory] 
as
-- payments genrated by v1
select 
	p.[PaymentId] as [Id],
    rp.Ukprn,
    rp.LearnRefNumber as [LearnerReferenceNumber],
    rp.LearnAimRef as [LearnAimReference],
    rp.TransactionType,
    rp.PriceEpisodeIdentifier,
    case when rp.DeliveryMonth < 8 
		then concat(rp.DeliveryYear - 2001, rp.DeliveryYear - 2000, '-R', format(rp.DeliveryMonth + 5, 'd2'))
		else concat(rp.DeliveryYear - 2000, rp.DeliveryYear - 1999, '-R', format(rp.DeliveryMonth - 7, 'd2')) 
	end as [DeliveryPeriod],
    p.CollectionPeriodName as [CollectionPeriod],
    p.Amount,
	concat(
		rp.Ukprn, '-',
		rp.LearnRefNumber, '-',
		rp.FrameworkCode, '-',
		rp.PathwayCode, '-',
		rp.ProgrammeType, '-',
		rp.StandardCode, '-',
		rp.LearnAimRef
	) as [ApprenticeshipKey]
from 
	[Payments].[Payments] p
	inner join [PaymentsDue].[RequiredPayments] rp on rp.Id = p.RequiredPaymentId

union all
-- payments genrated by v2
select 
	[Id],
    [Ukprn],
    [LearnerReferenceNumber],
    [LearnAimReference],
    [TransactionType],
    [ApprenticeshipKey],
    [PriceEpisodeIdentifier],
    [DeliveryPeriod],
    [CollectionPeriod],
    [Amount],
	[ApprenticeshipKey]
from 
	[Payments].[Payments] p
