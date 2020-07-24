begin tran

;with cte as (
select 
	*, 
	Row_Number() over (
	partition by [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [ContractType], [LearnerUln], [LearnerReferenceNumber], 
        [LearningAimReference], [LearningAimProgrammeType], [LearningAimStandardCode], [LearningAimFrameworkCode], [LearningAimPathwayCode], [LearningAimFundingLineType],
        [LearningAimSequenceNumber], [LearningStartDate], [EventType] order by [Ukprn], [AcademicYear], [CollectionPeriod]) as RN 
		from Payments2.EarningEvent 
		)
Delete From Payments2.EarningEventPeriod 
	where EarningEventId in 
	(Select EventId as EarningEventId from cte where RN > 1)

;with cte as (
select 
	*, 
	Row_Number() over (
	partition by [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [ContractType], [LearnerUln], [LearnerReferenceNumber], 
        [LearningAimReference], [LearningAimProgrammeType], [LearningAimStandardCode], [LearningAimFrameworkCode], [LearningAimPathwayCode], [LearningAimFundingLineType],
        [LearningAimSequenceNumber], [LearningStartDate], [EventType] order by [Ukprn], [AcademicYear], [CollectionPeriod]) as RN 
		from Payments2.EarningEvent 
		)
Delete From Payments2.EarningEvent
	where EventId in 
	(Select EventId from cte where RN > 1)
	
rollback