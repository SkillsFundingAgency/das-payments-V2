﻿using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Model;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests
{
    public class TestsHelper
    {
        public static SubmissionSummary DefaultSubmissionSummary => new SubmissionSummary(1234, 123, 1, 1920);
        public static (List<TransactionTypeAmounts> DcEarnings, List<TransactionTypeAmounts> DasEarnings)
            DefaultEarnings()
        {
            return (DcEarnings: DefaultDcEarnings, DasEarnings: DefaultDasEarnings);
        }


        public static List<TransactionTypeAmounts> DefaultDcEarnings =>
            new List<TransactionTypeAmounts>
            {
                new TransactionTypeAmounts
                {
                    ContractType = ContractType.Act1,
                    TransactionType1 = 12000,
                    TransactionType2 = 0,
                    TransactionType3 = 3000,
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
                    TransactionType3 = 3000,
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

        public static List<TransactionTypeAmounts> DefaultDasEarnings => new List<TransactionTypeAmounts>
        {
            new TransactionTypeAmounts
            {
                ContractType = ContractType.Act1,
                TransactionType1 = 12000,
                TransactionType2 = 0,
                TransactionType3 = 3000,
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
                TransactionType3 = 3000,
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

        public static DataLockTypeAmounts DefaultDataLockedEarnings => new DataLockTypeAmounts
        {
            DataLock2 = 1000,
            DataLock7 = 2000,
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


    }
}