create table Metrics.ProviderRequiredPayment(
	Id bigint not null identity(1,1) constraint PK_ProviderRequiredPayment primary key clustered
	,SubmissionSummaryId  bigint not null constraint FK_ProviderRequiredPayment__SubmissionSummary_Id foreign key references [Metrics].[SubmissionSummary] (Id) on delete cascade
	,ContractType tinyint not null
	,TransctionType1 decimal(15,5) not null
	,TransctionType2 decimal(15,5) not null
	,TransctionType3 decimal(15,5) not null
	,TransctionType4 decimal(15,5) not null
	,TransctionType5 decimal(15,5) not null
	,TransctionType6 decimal(15,5) not null
	,TransctionType7 decimal(15,5) not null
	,TransctionType8 decimal(15,5) not null
	,TransctionType9 decimal(15,5) not null
	,TransctionType10 decimal(15,5) not null
	,TransctionType11 decimal(15,5) not null
	,TransctionType12 decimal(15,5) not null
	,TransctionType13 decimal(15,5) not null
	,TransctionType14 decimal(15,5) not null
	,TransctionType15 decimal(15,5) not null
	,TransctionType16 decimal(15,5) not null
	,CreationDate datetimeoffset not null Constraint DF_ProviderRequiredPayment__CreationDate Default (sysdatetimeoffset())
	,Index IX_ProviderRequiredPayment (SubmissionSummaryId, ContractType, CreationDate)
)