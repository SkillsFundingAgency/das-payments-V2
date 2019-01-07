CREATE TABLE [Payments2].[EarningEventPeriod]
(
	Id BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT PK_EarningEventPeriod PRIMARY KEY CLUSTERED,	
	EarningEventId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_EarningEventPeriod__EarningEvent FOREIGN KEY REFERENCES [Payments2].[EarningEvent] (EventId), 
	PriceEpisodeIdentifier NVARCHAR(50) NULL,
	TransactionType TINYINT NOT NULL, 
	DeliveryPeriod TINYINT NOT NULL, 
	Amount DECIMAL(15,5) NOT NULL,
	CreationDate DATETIME2 NOT NULL CONSTRAINT DF_EarningEventPeriod__CreationDate DEFAULT (SYSDATETIME()),	
)
