create table Metrics.SubmissionSummary (
	Id bigint not null identity(1,1) constraint PK_Submission primary key  clustered
	,Ukprn bigint not null
	,AcademicYear smallint not null
	,CollectionPeriod tinyint not null
	,JobId bigint not null
	,[Percentage] decimal(15,5) not null
	,[Difference] decimal(15,5) not  null
	,PercentageContractType1 decimal(15,5) not null
	,PercentageContractType2 decimal(15,5) not null
	,DifferenceContractType1 decimal(15,5) not  null
	,DifferenceContractType2 decimal(15,5) not  null
	,EarningsDCContractType1 decimal(15,5) not null
	,EarningsDCContractType2 decimal(15,5) not null
	,EarningsDASContractType1 decimal(15,5) not null
	,EarningsDASContractType2 decimal(15,5) not null
	,EaringsDifferenceContractType1 decimal(15,5) not  null
	,EaringsDifferenceContractType2 decimal(15,5) not  null
	,EarningsPercentageContractType1 decimal(15,5) not null
	,EarningsPercentageContractType2 decimal(15,5) not null
	,RequiredPaymentsContractType1 decimal(15,5) not  null
	,RequiredPaymentsContractType2 decimal(15,5) not  null
	,RequiredPaymentsNonLevy decimal(15,5) not null
	,DataLockedEarnings decimal(15,5) not null
	,HeldBackCompletionPaymentsContractType1 decimal(15,5) not null
	,HeldBackCompletionPaymentsContractType2 decimal(15,5) not null
	,CreationDate datetimeoffset not null Constraint DF_Submission__CreationDate Default (sysdatetimeoffset())
	,Index IX_Submission (Ukprn, JobId, AcademicYear, CollectionPeriod, CreationDate)
	,CONSTRAINT UQ_Submission Unique (Ukprn, AcademicYear, CollectionPeriod)
)