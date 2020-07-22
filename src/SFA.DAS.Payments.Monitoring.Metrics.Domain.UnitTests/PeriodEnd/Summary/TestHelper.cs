using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Model;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.PeriodEnd.Summary
{
    class TestHelper
    {
        public static ProviderContractTypeAmounts DefaultYearToDateAmounts => new ProviderContractTypeAmounts
            {Ukprn = DefaultPeriodEndProviderSummary.Ukprn, ContractType1 = 16300, ContractType2 = 16300};

        public static PeriodEndProviderSummary DefaultPeriodEndProviderSummary =>
            new PeriodEndProviderSummary(1234, 123, 1, 1920);

        public static PeriodEndSummary DefaultPeriodEndSummary =>
            new PeriodEndSummary(123, 1, 1920);

        public static decimal DefaultDataLockedTotal => 4000;
        public static decimal AlreadyPaidDataLockedEarnings => 1000;

        public static List<TransactionTypeAmountsByContractType> GetDefaultDcEarnings =>
            new List<TransactionTypeAmountsByContractType>
            {
                new TransactionTypeAmountsByContractType
                {
                    ContractType = ContractType.Act1,
                    TransactionType1 = 49000,
                    TransactionType2 = 0,
                    TransactionType3 = 6000,
                    TransactionType4 = 200,
                    TransactionType5 = 900,
                    TransactionType6 = 200,
                    TransactionType7 = 200,
                    TransactionType8 = 200,
                    TransactionType9 = 200,
                    TransactionType10 = 200,
                    TransactionType11 = 200,
                    TransactionType12 = 200,
                    TransactionType13 = 200,
                    TransactionType14 = 200,
                    TransactionType15 = 200,
                    TransactionType16 = 200,
                },
                new TransactionTypeAmountsByContractType
                {
                    ContractType = ContractType.Act2,
                    TransactionType1 = 48000,
                    TransactionType2 = 0,
                    TransactionType3 = 6000,
                    TransactionType4 = 200,
                    TransactionType5 = 200,
                    TransactionType6 = 200,
                    TransactionType7 = 900,
                    TransactionType8 = 200,
                    TransactionType9 = 200,
                    TransactionType10 = 200,
                    TransactionType11 = 200,
                    TransactionType12 = 200,
                    TransactionType13 = 200,
                    TransactionType14 = 200,
                    TransactionType15 = 200,
                    TransactionType16 = 200,
                }
            };

        public static ProviderContractTypeAmounts DefaultHeldBackCompletionPayments => new ProviderContractTypeAmounts
            {Ukprn = DefaultPeriodEndProviderSummary.Ukprn, ContractType1 = 2000, ContractType2 = 1000};

        public static List<TransactionTypeAmountsByContractType> GetPaymentTransactionTypes =>
            new List<TransactionTypeAmountsByContractType>
            {
                new TransactionTypeAmountsByContractType
                {
                    ContractType = ContractType.Act1,
                    TransactionType1 = 24000,
                    TransactionType2 = 0,
                    TransactionType3 = 6000,
                    TransactionType4 = 200,
                    TransactionType5 = 200,
                    TransactionType6 = 200,
                    TransactionType7 = 200,
                    TransactionType8 = 200,
                    TransactionType9 = 200,
                    TransactionType10 = 200,
                    TransactionType11 = 200,
                    TransactionType12 = 200,
                    TransactionType13 = 200,
                    TransactionType14 = 200,
                    TransactionType15 = 200,
                    TransactionType16 = 200,
                },
                new TransactionTypeAmountsByContractType
                {
                    ContractType = ContractType.Act2,
                    TransactionType1 = 24000,
                    TransactionType2 = 0,
                    TransactionType3 = 6000,
                    TransactionType4 = 200,
                    TransactionType5 = 200,
                    TransactionType6 = 200,
                    TransactionType7 = 200,
                    TransactionType8 = 200,
                    TransactionType9 = 200,
                    TransactionType10 = 200,
                    TransactionType11 = 200,
                    TransactionType12 = 200,
                    TransactionType13 = 200,
                    TransactionType14 = 200,
                    TransactionType15 = 200,
                    TransactionType16 = 200,
                }
            };
    


    public static List<ProviderFundingSourceAmounts> GetPaymentFundingSourceAmounts =>
            new List<ProviderFundingSourceAmounts>
            {
                new ProviderFundingSourceAmounts
                {
                    
                    Ukprn = DefaultPeriodEndProviderSummary.Ukprn,
                    ContractType = ContractType.Act1,
                    FundingSource1 = 9900m,
                    FundingSource2 = 2600m,
                    FundingSource3 = 2500m,
                    FundingSource4 = 1000m,
                    FundingSource5 = 300m,
                    
                },
                new ProviderFundingSourceAmounts
                {
                    Ukprn = DefaultPeriodEndProviderSummary.Ukprn,
                    ContractType = ContractType.Act2,
                    FundingSource1 = 300,
                    FundingSource2 = 9900,
                    FundingSource3 = 2500m,
                    FundingSource4 = 1000m,
                    FundingSource5 = 2600m
                }
            };

    public static ProviderContractTypeAmounts GetYearToDatePayments => new ProviderContractTypeAmounts() {ContractType1 = 16300, ContractType2 = 16300, Ukprn = DefaultPeriodEndProviderSummary.Ukprn };
    }
}
