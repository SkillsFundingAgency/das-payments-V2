using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Model;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.PeriodEnd.Summary
{
    class TestHelper
    {
        public static ProviderContractTypeAmounts DefaultYearToDateAmounts => new ProviderContractTypeAmounts{ContractType1 = 16300, ContractType2 = 16300};
        public static PeriodEndProviderSummary DefaultPeriodEndProviderSummary => new PeriodEndProviderSummary(1234, 123, 1, 1920);
        public static decimal DefaultDataLockedTotal => 4000;
        public static decimal AlreadyPaidDataLockedEarnings => 1000;

        public static List<TransactionTypeAmountsByContractType> GetDefaultDcEarnings =>
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
                    TransactionType16 = 200,                }
            };

    }
}
