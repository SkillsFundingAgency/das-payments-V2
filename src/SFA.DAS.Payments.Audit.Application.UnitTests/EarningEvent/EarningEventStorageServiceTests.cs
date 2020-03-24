using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMoqCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.EarningEvent
{
    [TestFixture]
    public class EarningEventStorageServiceTests
    {
        private AutoMoqCore.AutoMoqer moqer;

        [SetUp]
        public void SetUp()
        {
            moqer = new AutoMoqer();
            moqer.SetInstance((IEarningEventMapper)new EarningEventMapper());
        }

        [Test]
        public async Task Saves_Batch_Of_Act1_Earning_Events()
        {
            var earnings = new List<EarningEvents.Messages.Events.EarningEvent>
            {
                new ApprenticeshipContractType1EarningEvent
                {
                    JobId = 123,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 1920, Period = 1},
                    Ukprn = 1234,
                    EventId = Guid.NewGuid(),
                    Learner = new Learner { Uln = 123456, ReferenceNumber = "learner ref"},
                    EventTime = DateTimeOffset.Now,
                    IlrSubmissionDateTime = DateTime.Now,
                    SfaContributionPercentage = .95M,
                    AgreementId = null,
                    CollectionYear = 2020,
                    IlrFileName = "somefile.ilr",
                    IncentiveEarnings = new List<IncentiveEarning>
                    {
                        new IncentiveEarning
                        {
                            Type = IncentiveEarningType.First16To18EmployerIncentive,
                            Periods = new List<EarningPeriod>
                            {
                                new EarningPeriod
                                {
                                    Period = 1,
                                    Amount = 100,
                                }
                            }.AsReadOnly()
                        }
                    },
                    LearningAim = new LearningAim(),
                    OnProgrammeEarnings = new List<OnProgrammeEarning>(),
                    PriceEpisodes = new List<PriceEpisode>(),
                    StartDate = DateTime.Now
                }
            };
            var service = moqer.Create<EarningEventStorageService>();
            await service.StoreEarnings(earnings, CancellationToken.None);
            moqer.GetMock<IEarningEventRepository>()
                .Verify(x => x.SaveEarningEvents(It.Is<List<EarningEventModel>>(lst => lst.All(item => earnings.Any(earning => earning.EventId == item.EventId))), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}