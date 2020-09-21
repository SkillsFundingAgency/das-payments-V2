using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Model;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.Submission.Summary
{
    public class TestsHelper
    {
        public static SubmissionSummary DefaultSubmissionSummary => new SubmissionSummary(1234, 123, 1, 1920);
        
        public static List<TransactionTypeAmounts> DefaultDcEarnings =>
            new List<TransactionTypeAmounts>
            {
                new TransactionTypeAmounts
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
                new TransactionTypeAmounts
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

        public static List<TransactionTypeAmounts> DefaultDasEarnings => new List<TransactionTypeAmounts>
        {
            new TransactionTypeAmounts
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
                TransactionType16 = 200,            },
            new TransactionTypeAmounts
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
                TransactionType16 = 200,            }
        };
        
        public static decimal DefaultDataLockedTotal => 4000;
        public static decimal AlreadyPaidDataLockedEarnings => 1000;

        public static DataLockTypeCounts DefaultDataLockedEarnings => new DataLockTypeCounts
        {
            DataLock2 = 1000,
            DataLock7 = 2000,
            DataLock4 = 1000
        };

        public static ContractTypeAmounts DefaultHeldBackCompletionPayments => new ContractTypeAmounts { ContractType1 = 2000, ContractType2 = 1000 };

        public static List<TransactionTypeAmounts> DefaultRequiredPayments => new List<TransactionTypeAmounts>
            {
                new TransactionTypeAmounts
                {
                    ContractType = ContractType.Act1,
                    TransactionType1 = 9000,
                    TransactionType2 = 0,
                    TransactionType3 = 1000,
                    TransactionType4 = 100,
                    TransactionType5 = 100,
                    TransactionType6 = 100,
                    TransactionType7 = 100,
                    TransactionType8 = 100,
                    TransactionType9 = 100,
                    TransactionType10 = 100,
                    TransactionType11 = 100,
                    TransactionType12 = 100,
                    TransactionType13 = 100,
                    TransactionType14 = 100,
                    TransactionType15 = 100,
                    TransactionType16 = 100,
                },
                new TransactionTypeAmounts
                {
                    ContractType = ContractType.Act2,
                    TransactionType1 = 12000,
                    TransactionType2 = 0,
                    TransactionType3 = 2000,
                    TransactionType4 = 100,
                    TransactionType5 = 100,
                    TransactionType6 = 100,
                    TransactionType7 = 100,
                    TransactionType8 = 100,
                    TransactionType9 = 100,
                    TransactionType10 = 100,
                    TransactionType11 = 100,
                    TransactionType12 = 100,
                    TransactionType13 = 100,
                    TransactionType14 = 100,
                    TransactionType15 = 100,
                    TransactionType16 = 100,
                }
            };

        public static ContractTypeAmounts DefaultYearToDateAmounts => new ContractTypeAmounts{ContractType1 = 16300, ContractType2 = 16300};
    }
}