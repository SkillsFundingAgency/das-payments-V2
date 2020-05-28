begin tran

if not exists (select * from sys.columns where name = N'DuplicateNumber' and object_id = OBJECT_ID(N'Payments2.DataLockEvent'))
	Alter table Payments2.DataLockEvent  add DuplicateNumber INT null 
go

;with DataLockEventCte as (
select 
	*, 
	Row_Number() over (
	partition by [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [ContractType], [LearnerUln], [LearnerReferenceNumber], 
        [LearningAimReference], [LearningAimProgrammeType], [LearningAimStandardCode], [LearningAimFrameworkCode], [LearningAimPathwayCode], [LearningAimFundingLineType],
        [LearningStartDate] order by [JobId], [Ukprn], [AcademicYear], [CollectionPeriod]) as RN 
		from Payments2.DataLockEvent 
		)
UPDATE rp
	SET DuplicateNumber = RN - 1
	FROM Payments2.DataLockEvent rp
	JOIN DataLockEventCte ON DataLockEventCte.Id = rp.Id
	WHERE DataLockEventCte.RN > 1

Go
if not exists (select * from sys.indexes where name = N'UX_DataLockEvent_LogicalDuplicates' and object_id = OBJECT_ID(N'Payments2.DataLockEvent'))
	Create Unique Index UX_DataLockEvent_LogicalDuplicates on Payments2.DataLockEvent( [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [ContractType], 
		[LearnerUln], [LearnerReferenceNumber], [LearningAimReference], [LearningAimProgrammeType], [LearningAimStandardCode], [LearningAimFrameworkCode], 
		[LearningAimPathwayCode], [LearningAimFundingLineType], [LearningStartDate], DuplicateNumber)

Select 
	* 
	from Payments2.DataLockEvent
	where DuplicateNUmber is not null

	
Commit