CREATE TABLE [Payments2].[EmployerProviderPriority]
(
	Id bigint not null identity Constraint Pk_EmployerProviderPriority Primary Key,
	EmployerAccountId bigint not null,
	Ukprn bigint not null,
	[Order] int not null
)
