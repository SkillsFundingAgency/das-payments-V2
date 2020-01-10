create table Metrics.ProviderDataLockedEarning(
	Id bigint not null identity(1,1) constraint PK_ProviderDataLockedEarning primary key  clustered
	,SubmissionSummaryId bigint not null constraint FK_ProviderDataLockedEarning__SubmissionSummary_Id foreign key references [Metrics].[SubmissionSummary] (Id) on delete cascade
	,DataLock1 decimal(15,5) not null
	,DataLock2 decimal(15,5) not null
	,DataLock3 decimal(15,5) not null
	,DataLock4 decimal(15,5) not null
	,DataLock5 decimal(15,5) not null
	,DataLock6 decimal(15,5) not null
	,DataLock7 decimal(15,5) not null
	,DataLock8 decimal(15,5) not null
	,DataLock9 decimal(15,5) not null
	,DataLock10 decimal(15,5) not null
	,DataLock11 decimal(15,5) not null
	,DataLock12 decimal(15,5) not null
	,CreationDate datetimeoffset not null Constraint DF_ProviderDataLockedEarning__CreationDate Default (sysdatetimeoffset())
	,Index IX_ProviderDataLockedEarning (SubmissionSummaryId, CreationDate)
)
