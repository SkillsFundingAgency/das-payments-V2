CREATE TABLE TransferPayments.AccountTransfers
	(
		TransferId bigint PRIMARY KEY identity(1,1),
		SendingAccountId bigint NOT NULL,
		ReceivingAccountId bigint NOT NULL,
		RequiredPaymentId uniqueidentifier NOT NULL,
		CommitmentId bigint NOT NULL,
		Amount decimal(15,5) NOT NULL,
		TransferType int NOT NULL,
		CollectionPeriodName varchar(8) NOT NULL,
		CollectionPeriodMonth int NOT NULL,
		CollectionPeriodYear int NOT NULL
	)
	GO
	CREATE INDEX IX_TransferPayments_AccountTransfers_RequiredPaymentId ON TransferPayments.AccountTransfers (RequiredPaymentId)