CREATE TABLE [Payments2].[EmployerProviderPriority]
(
	Id bigint not null identity Constraint Pk_EmployerProviderPriority Primary Key,
	EmployerAccountId bigint not null,
	Ukprn bigint not null,
	[Order] int not null
)
GO


CREATE INDEX [IX_EmployerProviderPriority_EmployerAccountId] ON [Payments2].[EmployerProviderPriority] 
(
  EmployerAccountId
)
GO