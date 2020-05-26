begin tran

if not exists (select * from sys.columns where name = N'DuplicateNumber' and object_id = OBJECT_ID(N'Payments2.Payment'))
	Alter table Payments2.Payment  add DuplicateNumber INT null 
go

;with cte as (
select 
	*, 
	Row_Number() over (
	partition by [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [DeliveryPeriod], [ContractType], [TransactionType], [Amount], [SfaContributionPercentage], [LearnerUln], [LearnerReferenceNumber], 
        [LearningAimReference], [LearningAimProgrammeType], [LearningAimStandardCode], [LearningAimFrameworkCode], [LearningAimPathwayCode], [LearningAimFundingLineType],
        [LearningStartDate],  [FundingSource], [ApprenticeshipId], [AccountId], [TransferSenderAccountId], [ApprenticeshipEmployerType] order by [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [DeliveryPeriod]) as RN 
		from Payments2.Payment 
		)
update Payments2.Payment 
	Set DuplicateNumber = RN-1
	from Payments2.Payment rp
	join cte on rp.Id = rp.Id
	where cte.RN > 1

Go
if not exists (select * from sys.indexes where name = N'UX_Payment_LogicalDuplicates' and object_id = OBJECT_ID(N'Payments2.Payment'))
	Create Unique Index UX_Payment_LogicalDuplicates on Payments2.Payment( [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [DeliveryPeriod], [ContractType], [TransactionType], 
		[Amount], [SfaContributionPercentage], [LearnerUln], [LearnerReferenceNumber], [LearningAimReference], [LearningAimProgrammeType], [LearningAimStandardCode], [LearningAimFrameworkCode], 
		[LearningAimPathwayCode], [LearningAimFundingLineType], [LearningStartDate],  [FundingSource], [ApprenticeshipId], [AccountId], [TransferSenderAccountId], [ApprenticeshipEmployerType], DuplicateNumber)

Select 
	* 
	from Payments2.Payment
	where DuplicateNUmber is not null

	
Commit