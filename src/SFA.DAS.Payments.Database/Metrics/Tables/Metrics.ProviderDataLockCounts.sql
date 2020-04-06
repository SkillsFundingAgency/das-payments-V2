create table Metrics.ProviderDataLockCounts(
	Id bigint not null identity(1,1) constraint PK_ProviderDataLockCounts primary key  clustered
	,SubmissionSummaryId bigint not null constraint FK_ProviderDataLockCounts__SubmissionSummary_Id foreign key references [Metrics].[SubmissionSummary] (Id) on delete cascade
	,DataLock1 int not null
	,DataLock2 int not null
	,DataLock3 int not null
	,DataLock4 int not null
	,DataLock5 int not null
	,DataLock6 int not null
	,DataLock7 int not null
	,DataLock8 int not null
	,DataLock9 int not null
	,DataLock10 int not null
	,DataLock11 int not null
	,DataLock12 int not null
	,CreationDate datetimeoffset not null Constraint DF_ProviderDataLockCounts__CreationDate Default (sysdatetimeoffset())
	,Index IX_ProviderDataLockCounts (SubmissionSummaryId, CreationDate)
)
