using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Core.Internal;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.EarningEvent
{
    [TestFixture]
    public class EarningEventMapperTests
    {
        protected IMapper Mapper { get; private set; }

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<EarningEventProfile>());
            Mapper = new Mapper(config);
        }

        [Test]
        public void Maps_Act1_Earnings()
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
                LearningAim = new LearningAim
                {
                    StartDate = DateTime.Now,
                    FrameworkCode = 1,
                    FundingLineType = "Levy 18+",
                    Reference = "aim ref",
                    SequenceNumber = 112
                },
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new List<EarningPeriod>
                        {
                            new EarningPeriod
                            {
                                Period = 1,
                                Amount = 1000
                            },
                            new EarningPeriod
                            {
                                Period = 2,
                                Amount = 1000
                            },
                        }.AsReadOnly()
                    }
                },
                PriceEpisodes = new List<PriceEpisode>
                {
                    new PriceEpisode
                    {
                        Identifier = "01-01-29/1234",
                        FundingLineType = "Levy 16-18",
                        LearningAimSequenceNumber = 112
                    },
                    new PriceEpisode
                    {
                        Identifier = "02-02-29/1234"
                    }
                },
                StartDate = DateTime.Now
            };



            var mapper = new EarningEventMapper(Mapper);
            var earningEventModel = mapper.Map(earningEvent);
            earningEventModel.Should().NotBeNull("Earning event model was null");
            CompareCommonProperties(earningEvent, earningEventModel);
            //ComparePriceEpisodes(earningEvent, earningEventModel); //TODO: 
        }

        private void CompareCommonProperties(EarningEvents.Messages.Events.EarningEvent earningEvent, EarningEventModel earningEventModel)
        {
            earningEventModel.EventId.Should().Be(earningEvent.EventId);
            earningEventModel.EventTime.Should().Be(earningEvent.EventTime);
            earningEventModel.JobId.Should().Be(earningEvent.JobId);
            earningEventModel.IlrFileName.Should().Be(earningEvent.IlrFileName);
            earningEventModel.IlrSubmissionDateTime.Should().Be(earningEvent.IlrSubmissionDateTime);
            earningEventModel.AcademicYear.Should().Be(earningEvent.CollectionPeriod.AcademicYear);
            earningEventModel.CollectionPeriod.Should().Be(earningEvent.CollectionPeriod.Period);
            CompareCourseDetails(earningEvent, earningEventModel);
            CompareLearnerDetails(earningEvent, earningEventModel);
        }

        private void CompareCourseDetails(EarningEvents.Messages.Events.EarningEvent earningEvent,
            EarningEventModel earningEventModel)
        {
            earningEventModel.LearningStartDate.Should().Be(earningEvent.LearningAim.StartDate);
            earningEventModel.LearningAimSequenceNumber.Should().Be(earningEvent.LearningAim?.SequenceNumber);
            earningEventModel.LearningAimFrameworkCode.Should().Be(earningEvent.LearningAim.FrameworkCode);
            earningEventModel.LearningAimPathwayCode.Should().Be(earningEvent.LearningAim.PathwayCode);
            earningEventModel.LearningAimProgrammeType.Should().Be(earningEvent.LearningAim.ProgrammeType);
            earningEventModel.LearningAimStandardCode.Should().Be(earningEvent.LearningAim.StandardCode);
            earningEventModel.LearningAimReference.Should().Be(earningEvent.LearningAim.Reference);
            earningEventModel.LearningAimFundingLineType.Should().Be(earningEvent.PriceEpisodes.Single(x => x.LearningAimSequenceNumber == earningEvent.LearningAim.SequenceNumber).FundingLineType);
        }

        private void CompareLearnerDetails(EarningEvents.Messages.Events.EarningEvent earningEvent, EarningEventModel earningEventModel)
        {
            earningEventModel.LearnerReferenceNumber.Should().Be(earningEvent.Learner.ReferenceNumber);
            earningEventModel.LearnerUln.Should().Be(earningEvent.Learner.Uln);
        }
    }
}