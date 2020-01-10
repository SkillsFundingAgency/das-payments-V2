create table Metrics.ProviderEarning(
	Id bigint not null identity(1,1) constraint PK_ProviderEarning primary key  clustered
	,SubmissionSummaryId  bigint not null constraint FK_ProviderEarning__SubmissionSummary_Id foreign key references [Metrics].[SubmissionSummary] (Id) on delete cascade
	,EarningsType tinyint not null constraint FK_ProviderEarning__SubmissionEarningType_Id foreign key references [Metrics].[SubmissionEarningType] (Id)
	,ContractType tinyint not null
	,TransactionType1 decimal(15,5) not null
	,TransactionType2 decimal(15,5) not null
	,TransactionType3 decimal(15,5) not null
	,TransactionType4 decimal(15,5) not null
	,TransactionType5 decimal(15,5) not null
	,TransactionType6 decimal(15,5) not null
	,TransactionType7 decimal(15,5) not null
	,TransactionType8 decimal(15,5) not null
	,TransactionType9 decimal(15,5) not null
	,TransactionType10 decimal(15,5) not null
	,TransactionType11 decimal(15,5) not null
	,TransactionType12 decimal(15,5) not null
	,TransactionType13 decimal(15,5) not null
	,TransactionType14 decimal(15,5) not null
	,TransactionType15 decimal(15,5) not null
	,TransactionType16 decimal(15,5) not null
	,CreationDate datetimeoffset not null Constraint DF_ProviderEarning__CreationDate Default (sysdatetimeoffset())
	,Index IX_ProviderEarning (SubmissionSummaryId, EarningsType, ContractType, CreationDate)
)
