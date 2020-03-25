using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMoqCore;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.EarningEvent
{
    [TestFixture]
    public class EarningsDuplicateEliminatorTests
    {
        private AutoMoqCore.AutoMoqer moqer;

        [SetUp]
        public void SetUp()
        {
            moqer = new AutoMoqer();
            moqer.GetMock<IEarningEventMapper>()
                .Setup(mapper => mapper.Map(It.IsAny<EarningEvents.Messages.Events.EarningEvent>()))
                .Returns<EarningEvents.Messages.Events.EarningEvent>(earningEvent =>
                    new EarningEventModel { EventId = earningEvent.EventId });
        }


        private EarningEvents.Messages.Events.EarningEvent CreateEarningEvent(
            Action<EarningEvents.Messages.Events.EarningEvent> action)
        {
            var earningEvent = new ApprenticeshipContractType1EarningEvent
            {
                JobId = 123,
                CollectionPeriod = new CollectionPeriod { AcademicYear = 1920, Period = 1 },
                Ukprn = 1234,
                EventId = Guid.NewGuid(),
                Learner = new Learner { Uln = 123456, ReferenceNumber = "learner ref" },
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
                StartDate = DateTime.Today
            };
            action?.Invoke(earningEvent);
            return earningEvent;
        }

        [Test]
        public async Task Removes_Duplicates_In_The_Batch()
        {
            var earnings = new List<EarningEvents.Messages.Events.EarningEvent>
            {
                CreateEarningEvent(null),
                CreateEarningEvent(null),
                CreateEarningEvent(model => model.Ukprn = 4321),
            };
            var service = moqer.Create<EarningsDuplicateEliminator>();
            var deDuplicatedEvents = service.RemoveDuplicates(earnings);
            deDuplicatedEvents.Count.Should().Be(2);
        }
    }
}