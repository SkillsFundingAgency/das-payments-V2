begin tran

if not exists (select * from sys.columns where name = N'DuplicateNumber' and object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent'))
	Alter table Payments2.RequiredPaymentEvent  add DuplicateNumber INT null 
go

IF NOT EXISTS (
		SELECT *
		FROM sys.columns
		WHERE name = N'EventType'
			AND object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent')
		)
		ALTER TABLE Payments2.RequiredPaymentEvent ADD EventType NVARCHAR(4000) NULL;
go

;with RequiredPaymentEventCte as (
select 
	*, 
	Row_Number() over (
	partition by [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [DeliveryPeriod], [ContractType], [TransactionType], [Amount], [SfaContributionPercentage], [LearnerUln], [LearnerReferenceNumber], 
        [LearningAimReference], [LearningAimProgrammeType], [LearningAimStandardCode], [LearningAimFrameworkCode], [LearningAimPathwayCode], [LearningAimFundingLineType],
        [LearningStartDate],  [EventType], [ApprenticeshipId], [AccountId], [TransferSenderAccountId], [ApprenticeshipEmployerType] order by [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [DeliveryPeriod]) as RN 
		from Payments2.RequiredPaymentEvent 
		)
UPDATE rpe
	SET DuplicateNumber = RN - 1
	FROM Payments2.RequiredPaymentEvent rpe
	JOIN RequiredPaymentEventCte ON RequiredPaymentEventCte.Id = rpe.Id
	WHERE RequiredPaymentEventCte.RN > 1

Go
if not exists (select * from sys.indexes where name = N'UX_RequiredPaymentEvent_LogicalDuplicates' and object_id = OBJECT_ID(N'Payments2.RequiredPaymentEvent'))
	Create Unique Index UX_RequiredPaymentEvent_LogicalDuplicates on Payments2.RequiredPaymentEvent( [JobId], [Ukprn], [AcademicYear], [CollectionPeriod], [DeliveryPeriod], [ContractType], [TransactionType], 
		[Amount], [SfaContributionPercentage], [LearnerUln], [LearnerReferenceNumber], [LearningAimReference], [LearningAimProgrammeType], [LearningAimStandardCode], [LearningAimFrameworkCode], 
		[LearningAimPathwayCode], [LearningAimFundingLineType], [LearningStartDate],  [EventType], [ApprenticeshipId], [AccountId], [TransferSenderAccountId], [ApprenticeshipEmployerType], DuplicateNumber)

Select 
	* 
	from Payments2.RequiredPaymentEvent
	where DuplicateNUmber is not null

	
Commit