﻿using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public static  class DataExtensions
    {
        public static Model.Core.Learner ToLearner(this Learner learner)
        {
            return new Model.Core.Learner
            {
                ReferenceNumber = learner.LearnRefNumber,
                Uln = learner.Uln
            };
        }

        public static LearningAim ToLearningAim(this Course course)
        {
            return new LearningAim
            {
                PathwayCode = course.PathwayCode,
                FrameworkCode = course.FrameworkCode,
                FundingLineType = course.FundingLineType,
                StandardCode = course.StandardCode,
                ProgrammeType = course.ProgrammeType,
                Reference = course.LearnAimRef
            };
        }
    }
}