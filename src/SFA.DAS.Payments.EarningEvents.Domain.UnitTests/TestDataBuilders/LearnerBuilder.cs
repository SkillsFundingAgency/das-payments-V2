using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using System;
using AutoFixture;

namespace SFA.DAS.Payments.EarningEvents.Domain.UnitTests
{
    public class LearnerBuilder
    {
        readonly Fixture fixture = new Fixture();
        private int numberOfDeliveries = 1;

        public static LearnerBuilder LearnerWithBasicLevyAim() => new LearnerBuilder();

        public LearnerBuilder WithMultipleDeliveries(int number = 3)
        {
            numberOfDeliveries = number;
            return this;
        }

        public ProcessLearnerCommand Build()
        {
            fixture.Customize<FM36Learner>(c => c
                .Without(x => x.PriceEpisodes)
                .Without(x => x.LearningDeliveries)
                .Do(x =>
                {
                    x.PriceEpisodes = fixture.Build<PriceEpisode>()
                        .With(y => y.PriceEpisodeValues, fixture.Build<PriceEpisodeValues>().With(z => z.EpisodeStartDate, DateTime.Now).Create())
                        .CreateMany(1).ToList();

                    x.LearningDeliveries = fixture.Build<LearningDelivery>()
                        .With(y => y.AimSeqNumber, Convert.ToInt32(x.PriceEpisodes.First().PriceEpisodeValues.PriceEpisodeAimSeqNumber))
                        .With(y => y.LearningDeliveryValues, fixture.Build<LearningDeliveryValues>().With(z => z.LearnAimRef, "ZPROG001").Create())
                        .CreateMany(numberOfDeliveries).ToList();
                }));

            return fixture.Build<ProcessLearnerCommand>()
                .With(x => x.CollectionYear, 1920)
                .With(x => x.CollectionPeriod, 1)
                .Create();
        }
    }
}