using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests.Builders
{
    public class TestFunctionalSkillsEarningEventBuilder<T> where T : FunctionalSkillEarningsEvent, new()
    {
        
        private readonly T functionalSkillEarningsEvent;
        Random rand = new Random();

        public TestFunctionalSkillsEarningEventBuilder()
        {

            functionalSkillEarningsEvent = new T
            {
                Ukprn = rand.Next(100000, 200000),
                CollectionPeriod = new CollectionPeriod() {AcademicYear = 1920, Period = 1},
                CollectionYear = 1920,
                EventId = Guid.NewGuid(),
                EventTime = DateTime.UtcNow,
                JobId = rand.Next(100000, 200000),
                IlrFileName = "SampleIlr",
                IlrSubmissionDateTime = DateTime.UtcNow,
                Learner = new Learner()
                {
                    ReferenceNumber = rand.Next(10000000, 20000000).ToString(),
                    Uln = rand.Next(10000000, 20000000)
                },
                Earnings = CreateEarnings()
            };
        }


        public TestFunctionalSkillsEarningEventBuilder<T> WithLearner(string learnerRefNo, long uln)
        {
            functionalSkillEarningsEvent.Learner = new Learner()
                {ReferenceNumber = learnerRefNo,Uln = uln};

            return this;
        }

        public TestFunctionalSkillsEarningEventBuilder<T> WithUkPrn( long ukprn)
        {
            functionalSkillEarningsEvent.Ukprn = ukprn;
            return this;
        }

        public TestFunctionalSkillsEarningEventBuilder<T> WithCollectionPeriod(short academicYear, byte period)
        {
            functionalSkillEarningsEvent.CollectionPeriod = new CollectionPeriod()
                {AcademicYear = academicYear, Period = period};
            functionalSkillEarningsEvent.CollectionYear = academicYear;

            return this;
        }


        public T Build()
        {
            return functionalSkillEarningsEvent;
        }



        private ReadOnlyCollection<FunctionalSkillEarning> CreateEarnings()
        {
            var earnings = new List<FunctionalSkillEarning>();

            earnings.Add(CreateFunctionalSkillEarning(FunctionalSkillType.BalancingMathsAndEnglish));
            earnings.Add(CreateFunctionalSkillEarning(FunctionalSkillType.LearningSupport));
            earnings.Add(CreateFunctionalSkillEarning(FunctionalSkillType.OnProgrammeMathsAndEnglish, 39.95m));

            return earnings.AsReadOnly();
        }

        private FunctionalSkillEarning CreateFunctionalSkillEarning(FunctionalSkillType type, decimal installmentAmount = 0m)
        {
            return new FunctionalSkillEarning()
            {
                Periods = CreatePeriods(12, installmentAmount),
                Type = type
            };
        }

        private ReadOnlyCollection<EarningPeriod> CreatePeriods(int count, decimal value)
        {
            var periods = new List<EarningPeriod>();
            for (int i = 0; i < count; i++)
            {
                periods.Add(new EarningPeriod(){Period = (byte) (i + 1), Amount = value});
            }

            return periods.AsReadOnly();
        }
    }
}