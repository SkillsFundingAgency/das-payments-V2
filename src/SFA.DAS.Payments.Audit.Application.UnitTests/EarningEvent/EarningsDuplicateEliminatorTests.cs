using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMoqCore;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
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

        private EarningEventModel CreateEarningEventModel(
            Action<EarningEventModel> action)
        {
            var earningEvent = new EarningEventModel
            {
                JobId = 123,
                CollectionPeriod = 1,
                AcademicYear = 1920,
                Ukprn = 1234,
                EventId = Guid.NewGuid(),
                LearnerUln = 123456,
                LearnerReferenceNumber = "learner ref",
                EventTime = DateTimeOffset.Now,
                IlrSubmissionDateTime = DateTime.Now,
                SfaContributionPercentage = .95M,
                AgreementId = null,
                IlrFileName = "somefile.ilr",
                Periods = new List<EarningEventPeriodModel>(),
                LearningAimFundingLineType = "Funding line type",
                LearningAimSequenceNumber = 1,
                LearningAimFrameworkCode = 10,
                LearningAimPathwayCode = 11,
                LearningAimReference = "learn ref",
                LearningStartDate = DateTime.Today.AddYears(-1),
                LearningAimProgrammeType = 12,
                LearningAimStandardCode = 13,
                ContractType = ContractType.Act1,
                PriceEpisodes = new List<EarningEventPriceEpisodeModel>(),
                StartDate = DateTime.Today,
                ActualEndDate = null,
                PlannedEndDate = DateTime.Today.AddYears(1),
                InstalmentAmount = 500,
                NumberOfInstalments = 24,
                CompletionAmount = 1500,
                CompletionStatus = 1,
                EventType = typeof(ApprenticeshipContractType1EarningEvent).FullName,
            };
            action?.Invoke(earningEvent);
            return earningEvent;
        }


        [Test]
        public void Removes_Duplicates_In_The_Batch()
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

        [Test]
        public async Task Removes_Duplicate_Earnings_Already_Stored_In_Db()
        {
            var earnings = new List<EarningEventModel>
            {
                CreateEarningEventModel(null),
                CreateEarningEventModel(null),
                CreateEarningEventModel(model => model.EventType = typeof(ApprenticeshipContractType2EarningEvent).FullName),
            };

            moqer.GetMock<IEarningEventRepository>()
                .Setup(x => x.GetDuplicateEarnings(It.IsAny<List<EarningEventModel>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(earnings.Take(2).ToList());
            var service = moqer.Create<EarningsDuplicateEliminator>();
            var deDuplicatedEvents = await service.RemoveDuplicates(earnings, CancellationToken.None).ConfigureAwait(false);
            deDuplicatedEvents.Count.Should().Be(1);
        }
    }
}