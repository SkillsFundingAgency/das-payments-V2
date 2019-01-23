using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers
{
    internal static class EarningEventDataHelper
    {


        public static LearningAim CreateLearningAim()
        {
            return new LearningAim
            {
                ProgrammeType = 5,
                PathwayCode = 6,
                StandardCode = 7,
                FrameworkCode = 8,
                Reference = "9",
                FundingLineType = "11"
            };
        }

        public static Learner CreateLearner()
        {
            return new Learner
            {
                ReferenceNumber = "3",
                Uln = 4
            };
        }
        
    }
}
