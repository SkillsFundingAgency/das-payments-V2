create table Metrics.SubmissionsSummaryDataLockCounts(
	Id bigint not null identity(1,1) constraint PK_SubmissionsSummaryDataLockCounts primary key  clustered
	,SubmissionsSummaryId bigint not null constraint FK_SubmissionsSummaryDataLockCounts__SubmissionsSummary_Id foreign key references [Metrics].[SubmissionsSummary] (Id) on delete cascade
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
	,CreationDate datetimeoffset not null Constraint DF_SubmissionsSummaryDataLockCounts__CreationDate Default (sysdatetimeoffset())
	,Index IX_SubmissionsSummaryDataLockCounts (SubmissionsSummaryId, CreationDate)
)
