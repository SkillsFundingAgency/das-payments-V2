using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.PaymentsDue.Application.Services;
using SFA.DAS.Payments.PaymentsDue.Domain;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Application.UnitTests.Services
{
    [TestFixture]
    public class FunctionalSkillsEarningServiceTest
    {
        [Test]
        public void TestCreatePaymentDue()
        {
            // arrange
            var processorMock = new Mock<IFunctionalSkillsEarningProcessor>(MockBehavior.Strict);
            var incentives = new [] { new IncentivePaymentDueEvent() };

            processorMock.Setup(p => p.HandleEarning(It.IsAny<Submission>(), It.IsAny<FunctionalSkillEarning>(), It.IsAny<Learner>(), It.IsAny<LearningAim>()))
                .Returns(incentives)
                .Verifiable();

            var service = new FunctionalSkillsEarningService(processorMock.Object);

            var message = new FunctionalSkillEarningsEvent
            {
                CollectionPeriod = new CalendarPeriod("1819-R01"),
                CollectionYear = "1819",
                EventTime = DateTimeOffset.Now,
                IlrSubmissionDateTime = DateTime.Today,
                JobId = 1,
                Ukprn = 2,
                Learner = new Learner
                {
                    Uln = 3,
                    ReferenceNumber = "4"
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = 5,
                    FundingLineType = "6",
                    PathwayCode = 7,
                    StandardCode = 8,
                    ProgrammeType = 9,
                    Reference = "10"
                },
                PriceEpisodes = new ReadOnlyCollection<PriceEpisode>(new List<PriceEpisode>
                {
                    new PriceEpisode{ Identifier = "1"},
                    new PriceEpisode{ Identifier = "11"}
                }),
                Earnings = new ReadOnlyCollection<FunctionalSkillEarning>(new List<FunctionalSkillEarning>
                {
                    new FunctionalSkillEarning
                    {                        
                        //Type = FunctionalSkillType.OnProgrammeMathsAndEnglish,
                        //Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        //{
                        //    new EarningPeriod {PriceEpisodeIdentifier = "1", Period = 1, Amount = 1},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "1", Period = 2, Amount = 2},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "1", Period = 3, Amount = 3},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "1", Period = 4, Amount = 4},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "1", Period = 5, Amount = 5},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "1", Period = 6, Amount = 6},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "1", Period = 7, Amount = 7},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "1", Period = 8, Amount = 8},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "1", Period = 9, Amount = 9},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "11", Period = 10, Amount = 10},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "11", Period = 11, Amount = 11},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "11", Period = 12, Amount = 12}
                        //})
                    },
                    new FunctionalSkillEarning
                    {
                        //Type = FunctionalSkillType.BalancingMathsAndEnglish,
                        //Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        //{
                        //    new EarningPeriod {PriceEpisodeIdentifier = null, Period = 1, Amount = 0},
                        //    new EarningPeriod {PriceEpisodeIdentifier = null, Period = 2, Amount = 0},
                        //    new EarningPeriod {PriceEpisodeIdentifier = null, Period = 3, Amount = 0},
                        //    new EarningPeriod {PriceEpisodeIdentifier = null, Period = 4, Amount = 0},
                        //    new EarningPeriod {PriceEpisodeIdentifier = null, Period = 5, Amount = 0},
                        //    new EarningPeriod {PriceEpisodeIdentifier = null, Period = 6, Amount = 0},
                        //    new EarningPeriod {PriceEpisodeIdentifier = null, Period = 7, Amount = 0},
                        //    new EarningPeriod {PriceEpisodeIdentifier = null, Period = 8, Amount = 0},
                        //    new EarningPeriod {PriceEpisodeIdentifier = null, Period = 9, Amount = 0},
                        //    new EarningPeriod {PriceEpisodeIdentifier = null, Period = 10, Amount = 10},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "1", Period = 11, Amount = 1},
                        //    new EarningPeriod {PriceEpisodeIdentifier = "11", Period = 12, Amount = 12}
                        //})
                    }
                })
            };

            // act 
            var actual = service.CreatePaymentsDue(message);

            // assert
            Assert.IsNotNull(actual);
        }
    }
}
