using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Model;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.PeriodEnd
{
    internal class PeriodEndTestHelper
    {
        public static List<ProviderTransactionTypeAmounts> SingleProviderDcEarnings => new List<ProviderTransactionTypeAmounts>
        {
            new ProviderTransactionTypeAmounts
            {
                Ukprn = 12312,
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
                TransactionType16 = 200
            },
            new ProviderTransactionTypeAmounts
            {
                Ukprn = 12312,
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
                TransactionType16 = 200
            }
        };

        public static List<ProviderTransactionTypeAmounts> MultipleProviderDcEarnings(int providerEarningsToCreate)
        {
            var results = new List<ProviderTransactionTypeAmounts>();

            for (int i = 0; i < providerEarningsToCreate; i++)
            {
                results.Add(new ProviderTransactionTypeAmounts
                {
                    Ukprn = 10000 + i,
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
                    TransactionType16 = 200
                });
            }

            return results;
        }
    }
}
