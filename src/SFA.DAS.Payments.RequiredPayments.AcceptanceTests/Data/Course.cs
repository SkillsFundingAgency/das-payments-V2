﻿using System;
using SFA.DAS.Payments.Tests.Core.Data;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data
{
    public class Course : Learner
    {
        public int AimSeqNumber { get; set; }

        public int ProgrammeType { get; set; }

        public int? FrameworkCode { get; set; }

        public int? PathwayCode { get; set; }

        public int? StandardCode { get; set; }

        public decimal SfaContributionPercentage { get; set; }

        public string FundingLineType { get; set; }

        public string LearnAimRef { get; set; }

        public DateTime LearningStartTime { get; set; }
    }
}