create table Metrics.ProviderDataLockedEarning(
	Id bigint not null identity(1,1) constraint PK_ProviderDataLockedEarning primary key  clustered
	,SubmissionSummaryId bigint not null constraint FK_ProviderDataLockedEarning__SubmissionSummary_Id foreign key references [Metrics].[SubmissionSummary] (Id) on delete cascade
	,DalaLock1 decimal(15,5) not null
	,DalaLock2 decimal(15,5) not null
	,DalaLock3 decimal(15,5) not null
	,DalaLock4 decimal(15,5) not null
	,DalaLock5 decimal(15,5) not null
	,DalaLock6 decimal(15,5) not null
	,DalaLock7 decimal(15,5) not null
	,DalaLock8 decimal(15,5) not null
	,DalaLock9 decimal(15,5) not null
	,DalaLock10 decimal(15,5) not null
	,DalaLock11 decimal(15,5) not null
	,DalaLock12 decimal(15,5) not null
	,CreationDate datetimeoffset not null Constraint DF_ProviderDataLockedEarning__CreationDate Default (sysdatetimeoffset())
	,Index IX_ProviderDataLockedEarning (SubmissionSummaryId, CreationDate)
)
