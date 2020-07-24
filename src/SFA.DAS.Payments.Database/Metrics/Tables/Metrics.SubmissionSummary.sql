create table Metrics.SubmissionSummary (
	Id bigint not null identity(1,1) constraint PK_SubmissionSummary primary key  clustered
	,Ukprn bigint not null
	,AcademicYear smallint not null
	,CollectionPeriod tinyint not null
	,JobId bigint not null
	,[Percentage] decimal(15,5) not null
	,ContractType1 decimal(15,5) not null
	,ContractType2 decimal(15,5) not null
	,DifferenceContractType1 decimal(15,5) not  null
	,DifferenceContractType2 decimal(15,5) not  null
	,PercentageContractType1 decimal(15,5) not null
	,PercentageContractType2 decimal(15,5) not null
	,EarningsDCContractType1 decimal(15,5) not null
	,EarningsDCContractType2 decimal(15,5) not null
	,EarningsDASContractType1 decimal(15,5) not null
	,EarningsDASContractType2 decimal(15,5) not null
	,EarningsDifferenceContractType1 decimal(15,5) not  null
	,EarningsDifferenceContractType2 decimal(15,5) not  null
	,EarningsPercentageContractType1 decimal(15,5) not null
	,EarningsPercentageContractType2 decimal(15,5) not null
	,RequiredPaymentsContractType1 decimal(15,5) not  null
	,RequiredPaymentsContractType2 decimal(15,5) not  null
	,RequiredPaymentsNonLevy decimal(15,5) not null
	,DataLockedEarnings decimal(15,5) not null
	,AlreadyPaidDataLockedEarnings decimal(15,5) not null Constraint DF_SubmissionSummary__AlreadyPaidDataLockedEarnings default(0)
	,TotalDataLockedEarnings decimal(15,5) not null Constraint DF_SubmissionSummary__TotalDataLockedEarnings default(0)
	,HeldBackCompletionPaymentsContractType1 decimal(15,5) not null
	,HeldBackCompletionPaymentsContractType2 decimal(15,5) not null
	,PaymentsYearToDateContractType1 decimal(15,5) not null
	,PaymentsYearToDateContractType2 decimal(15,5) not null
	,CreationDate datetimeoffset not null Constraint DF_SubmissionSummary__CreationDate Default (sysdatetimeoffset())
	,Index IX_SubmissionSummary (Ukprn, JobId, AcademicYear, CollectionPeriod, CreationDate)
	,CONSTRAINT UQ_SubmissionSummary Unique (Ukprn, AcademicYear, CollectionPeriod)
)