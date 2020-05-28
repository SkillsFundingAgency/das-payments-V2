begin tran

if not exists (select * from sys.columns where name = N'DuplicateNumber' and object_id = OBJECT_ID(N'Payments2.FundingSourceEvent'))
	Alter table Payments2.FundingSourceEvent  add DuplicateNumber INT null 
go

;with FundingSourceEventCte as (
select 
	*, 
	Row_Number() over (
	partition by [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [DeliveryPeriod], [ContractType], [TransactionType], [Amount], [SfaContributionPercentage], [LearnerUln], [LearnerReferenceNumber], 
        [LearningAimReference], [LearningAimProgrammeType], [LearningAimStandardCode], [LearningAimFrameworkCode], [LearningAimPathwayCode], [LearningAimFundingLineType],
        [LearningStartDate],  [FundingSourceType], [ApprenticeshipId], [AccountId], [TransferSenderAccountId], [ApprenticeshipEmployerType] order by [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [DeliveryPeriod]) as RN 
		from Payments2.FundingSourceEvent 
		)
UPDATE fse
	SET DuplicateNumber = RN - 1
	FROM Payments2.FundingSourceEvent fse
	JOIN FundingSourceEventCte ON FundingSourceEventCte.Id = fse.Id
	WHERE FundingSourceEventCte.RN > 1

Go
if not exists (select * from sys.indexes where name = N'UX_FundingSourceEvent_LogicalDuplicates' and object_id = OBJECT_ID(N'Payments2.FundingSourceEvent'))
	Create Unique Index UX_FundingSourceEvent_LogicalDuplicates on Payments2.FundingSourceEvent( [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [DeliveryPeriod], [ContractType], [TransactionType], 
		[Amount], [SfaContributionPercentage], [LearnerUln], [LearnerReferenceNumber], [LearningAimReference], [LearningAimProgrammeType], [LearningAimStandardCode], [LearningAimFrameworkCode], 
		[LearningAimPathwayCode], [LearningAimFundingLineType], [LearningStartDate],  [FundingSourceType], [ApprenticeshipId], [AccountId], [TransferSenderAccountId], [ApprenticeshipEmployerType], DuplicateNumber)

Select 
	* 
	from Payments2.FundingSourceEvent
	where DuplicateNUmber is not null

	
Commit