CREATE TABLE [Metrics].[ProviderPaymentTransaction] (
    [Id]                         BIGINT             IDENTITY (1, 1) NOT NULL,
    [ProviderPeriodEndSummaryId] BIGINT             NOT NULL,
    [ContractType]               TINYINT            NOT NULL,
    [TransactionType1]           DECIMAL (15, 5)    NOT NULL,
    [TransactionType2]           DECIMAL (15, 5)    NOT NULL,
    [TransactionType3]           DECIMAL (15, 5)    NOT NULL,
    [TransactionType4]           DECIMAL (15, 5)    NOT NULL,
    [TransactionType5]           DECIMAL (15, 5)    NOT NULL,
    [TransactionType6]           DECIMAL (15, 5)    NOT NULL,
    [TransactionType7]           DECIMAL (15, 5)    NOT NULL,
    [TransactionType8]           DECIMAL (15, 5)    NOT NULL,
    [TransactionType9]           DECIMAL (15, 5)    NOT NULL,
    [TransactionType10]          DECIMAL (15, 5)    NOT NULL,
    [TransactionType11]          DECIMAL (15, 5)    NOT NULL,
    [TransactionType12]          DECIMAL (15, 5)    NOT NULL,
    [TransactionType13]          DECIMAL (15, 5)    NOT NULL,
    [TransactionType14]          DECIMAL (15, 5)    NOT NULL,
    [TransactionType15]          DECIMAL (15, 5)    NOT NULL,
    [TransactionType16]          DECIMAL (15, 5)    NOT NULL,
    [CreationDate]               DATETIMEOFFSET (7) CONSTRAINT [DF_ProviderPaymentTransaction__CreationDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    CONSTRAINT [PK_ProviderPaymentTransaction] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProviderPaymentTransaction__ProviderPeriodEndSummary_Id] FOREIGN KEY ([ProviderPeriodEndSummaryId]) REFERENCES [Metrics].[ProviderPeriodEndSummary] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ProviderPaymentTransaction]
    ON [Metrics].[ProviderPaymentTransaction]([ProviderPeriodEndSummaryId] ASC, [ContractType] ASC, [CreationDate] ASC);

