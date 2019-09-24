create view [Payments2].[DataMatchReport]
	as 

select 
	(case when dle.DataLockSourceId = 1 then 'ILR' else 'DAS_PE' end) as CollectionType
	, dle.Ukprn
	, dle.LearnerReferenceNumber
	, dle.LearnerUln
	, dlenppf.DataLockFailureId
	, ee.LearningAimSequenceNumber
	, dle.CollectionPeriod
	, dle.AcademicYear
	, dle.IlrSubmissionDateTime
	, dlenpp.DeliveryPeriod
	, dle.DataLockSourceId
	, dle.IsPayable
	, dle.LearningAimReference

from 
	Payments2.DataLockEvent as dle with (nolock) 
	inner join Payments2.EarningEvent as ee with (nolock) on dle.EarningEventId = ee.EventId 
	inner join Payments2.DataLockEventNonPayablePeriod as dlenpp with (nolock) on dle.EventId = dlenpp.DataLockEventId 
	inner join Payments2.DataLockEventNonPayablePeriodFailures as dlenppf with (nolock) on dlenpp.DataLockEventNonPayablePeriodId = dlenppf.DataLockEventNonPayablePeriodId

where
	(dle.IsPayable = 0) and (dle.LearningAimReference = 'ZPROG001')