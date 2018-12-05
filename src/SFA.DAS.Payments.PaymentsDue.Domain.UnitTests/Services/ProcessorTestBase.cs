using System;
using System.Collections.ObjectModel;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PaymentsDue.Domain.UnitTests.Services
{
    public class ProcessorTestBase
    {
        protected const long JobId = 12345;
        protected const long Ukprn = 12;

        protected ApprenticeshipContractTypeEarningsEvent GetEarning()
        {
            var priceEpisode = new PriceEpisode
            {
                TotalNegotiatedPrice1 = 120,
                StartDate = new DateTime(2018, 8, 1),
                PlannedEndDate = new DateTime(2019, 7, 31),
                Identifier = "13"
            };

            var earning = new ApprenticeshipContractType2EarningEvent
            {
                EarningYear = 2018,
                EventTime = DateTimeOffset.UtcNow,
                Learner = new Learner
                {
                    ReferenceNumber = "1",
                    Uln = 3
                },
                Ukprn = Ukprn,
                JobId = JobId,
                LearningAim = new LearningAim
                {
                    FrameworkCode = 5,
                    FundingLineType = "6",
                    PathwayCode = 7,
                    ProgrammeType = 8,
                    Reference = "9",
                    StandardCode = 10
                },
                PriceEpisodes = new ReadOnlyCollection<PriceEpisode>(new[]
                {
                    priceEpisode
                }),
                SfaContributionPercentage = 100
            };
            return earning;
        }
    }
}