CREATE TABLE [Metrics].[ProviderPaymentFundingSource] (
    [Id]                         BIGINT             IDENTITY (1, 1) NOT NULL,
    [ProviderPeriodEndSummaryId] BIGINT             NOT NULL,
    [ContractType]               TINYINT            NOT NULL,
    [FundingSource1]             DECIMAL (15, 5)    NOT NULL,
    [FundingSource2]             DECIMAL (15, 5)    NOT NULL,
    [FundingSource3]             DECIMAL (15, 5)    NOT NULL,
    [FundingSource4]             DECIMAL (15, 5)    NOT NULL,
    [FundingSource5]             DECIMAL (15, 5)    NOT NULL,
    [CreationDate]               DATETIMEOFFSET (7) CONSTRAINT [DF_ProviderPaymentFundingSource__CreationDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    CONSTRAINT [PK_ProviderPaymentFundingSource] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProviderPaymentFundingSource__ProviderPeriodEndSummary_Id] FOREIGN KEY ([ProviderPeriodEndSummaryId]) REFERENCES [Metrics].[ProviderPeriodEndSummary] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ProviderPaymentFundingSource]
    ON [Metrics].[ProviderPaymentFundingSource]([ProviderPeriodEndSummaryId] ASC, [ContractType] ASC, [CreationDate] ASC);

