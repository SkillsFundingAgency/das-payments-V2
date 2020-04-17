using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests.Builders
{
    public class TestContractTypeEarningEventBuilder<T> where T : ApprenticeshipContractTypeEarningsEvent, new()
    {
        private readonly T contractTypeEarningsEvent;
        Random rand = new Random();

        public TestContractTypeEarningEventBuilder()
        {
            contractTypeEarningsEvent = new T
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
                OnProgrammeEarnings = new List<OnProgrammeEarning>()
                {
                    BuildLearningEarnings(), BuildCompletionEarning(), BuildBalancingEarnings()
                },
                IncentiveEarnings = BuildIncentiveEarnings()
            };

        }

        private List<IncentiveEarning> BuildIncentiveEarnings()
        {
            var incentiveEarnings = new List<IncentiveEarning>();
            foreach (var value in Enum.GetValues(typeof(IncentiveEarningType)))
            {
                incentiveEarnings.Add(new IncentiveEarning()
                {
                    Periods = CreatePeriods(12, 0m),
                    Type =(IncentiveEarningType) value
                });
            }

            return incentiveEarnings;
        }

        private OnProgrammeEarning BuildBalancingEarnings()
        {
            return CreateOnProgramEarning(OnProgrammeEarningType.Balancing);
        }

        private OnProgrammeEarning BuildCompletionEarning()
        {
            return CreateOnProgramEarning(OnProgrammeEarningType.Completion);
        }

        private OnProgrammeEarning BuildLearningEarnings()
        {
            return CreateOnProgramEarning(OnProgrammeEarningType.Learning);
        }

        private OnProgrammeEarning CreateOnProgramEarning(OnProgrammeEarningType type)
        {
            var onProgrammeEarning = new OnProgrammeEarning()
            {
                Type = type,
                Periods = CreatePeriods(12, 1000m)
            };
            return onProgrammeEarning;
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

        public TestContractTypeEarningEventBuilder<T> WithLearner(string learnerRefNo, long uln)
        {
            contractTypeEarningsEvent.Learner = new Learner()
                {ReferenceNumber = learnerRefNo,Uln = uln};

            return this;
        }

        public TestContractTypeEarningEventBuilder<T> WithUkPrn( long ukprn)
        {
            contractTypeEarningsEvent.Ukprn = ukprn;
            return this;
        }

        public TestContractTypeEarningEventBuilder<T> WithCollectionPeriod(short academicYear, byte period)
        {
            contractTypeEarningsEvent.CollectionPeriod = new CollectionPeriod()
                {AcademicYear = academicYear, Period = period};
            contractTypeEarningsEvent.CollectionYear = academicYear;

            return this;
        }


        public T Build()
        {
            return contractTypeEarningsEvent;
        }
    }
}