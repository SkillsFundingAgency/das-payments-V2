using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Application.Mapping;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Mapping
{
    [TestFixture]
    public class MapperConfigurationTest
    {
        private ApprenticeshipContractType1EarningEvent earningEventPayment;

        [OneTimeSetUp]
        public void Initialise()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<DataLocksProfile>();
            });
            Mapper.AssertConfigurationIsValid();
        }

        [SetUp]
        public void Setup()
        {
            var earnings = new List<EarningPeriod>();
            for (byte i = 1; i < 13; i++)
            {
                earnings.Add(new EarningPeriod
                {
                    Amount = 100,
                    Period = i,
                    PriceEpisodeIdentifier = "p-1",
                    SfaContributionPercentage = 0.9m
                });
            }

            var incentiveEarnings = new List<IncentiveEarning>();
            foreach (var incentiveEarningType in Enum.GetValues(typeof(IncentiveEarningType)))
            {
                incentiveEarnings.Add(new IncentiveEarning
                {
                    CensusDate = DateTime.MaxValue,
                    Periods = earnings.AsReadOnly(),
                    Type = (IncentiveEarningType)incentiveEarningType
                });
            }

            earningEventPayment = new ApprenticeshipContractType1EarningEvent()
            {
                Ukprn = 12345,
                SfaContributionPercentage = 0.9m,
                JobId = 123,
                CollectionPeriod = new CollectionPeriod { Period = 12, AcademicYear = 1819 },
                Learner = new Learner { ReferenceNumber = "1234-ref", Uln = 123456 },
                CollectionYear = 2019,
                LearningAim = new LearningAim
                {
                    PathwayCode = 12,
                    FrameworkCode = 1245,
                    FundingLineType = "Non-DAS 16-18 Learner",
                    StandardCode = 1209,
                    ProgrammeType = 7890,
                    Reference = "ZPROG001"
                },
                IlrSubmissionDateTime = DateTime.MaxValue,
                EventTime = DateTimeOffset.MaxValue,
                AgreementId = "12120002",
                IncentiveEarnings = incentiveEarnings,
                OnProgrammeEarnings = (new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        CensusDate = DateTime.MaxValue,
                        Type = OnProgrammeEarningType.Learning,
                        Periods = earnings.AsReadOnly()
                    },
                     new OnProgrammeEarning
                    {
                        CensusDate = DateTime.MaxValue,
                        Type = OnProgrammeEarningType.Balancing,
                        Periods = earnings.AsReadOnly()
                    },
                      new OnProgrammeEarning
                    {
                        CensusDate = DateTime.MaxValue,
                        Type = OnProgrammeEarningType.Completion,
                        Periods = earnings.AsReadOnly()
                    }

                }),
                PriceEpisodes = (new List<PriceEpisode>
                {
                    new PriceEpisode
                    {
                        ActualEndDate = DateTime.MaxValue,
                        Completed = true,
                        CompletionAmount= 300m,
                        Identifier = "p-1",
                        EffectiveTotalNegotiatedPriceStartDate = DateTime.MinValue,
                        InstalmentAmount = 100m,
                        NumberOfInstalments = 12,
                        PlannedEndDate = DateTime.MaxValue,
                        TotalNegotiatedPrice1 = 25.0m,
                        TotalNegotiatedPrice2 = 25.0m,
                        TotalNegotiatedPrice3 = 25.0m,
                        TotalNegotiatedPrice4 = 25.0m
                    }

                })
            };

        }


        [Test]
        public void CanMapUkprn()
        {
            var payableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>(earningEventPayment);

            payableEarning.Ukprn.Should().Be(earningEventPayment.Ukprn);
        }

        [Test]
        public void CanMapCollectionPeriod()
        {
            var payableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>(earningEventPayment);
            payableEarning.CollectionPeriod.Should().NotBeNull();
            payableEarning.CollectionPeriod.AcademicYear.Should().Be(earningEventPayment.CollectionPeriod.AcademicYear);
            payableEarning.CollectionPeriod.Period.Should().Be(earningEventPayment.CollectionPeriod.Period);
        }

        [Test]
        public void CanMapJobId()
        {
            var payableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>(earningEventPayment);
            payableEarning.JobId.Should().Be(earningEventPayment.JobId);
        }


        [Test]
        public void CanMapIlrSubmissionDateTime()
        {
            var payableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>(earningEventPayment);
            payableEarning.IlrSubmissionDateTime.Should().Be(earningEventPayment.IlrSubmissionDateTime);
        }

        [Test]
        public void CanMapLearner()
        {
            var payableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>(earningEventPayment);
            payableEarning.Learner.Should().NotBeNull();
            payableEarning.Learner.ReferenceNumber.Should().Be(earningEventPayment.Learner.ReferenceNumber);
        }

        [Test]
        [TestCaseSource(nameof(GetIncentives))]
        public void CanMapIncentiveEarnings(IncentiveEarningType type)
        {
            var payableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>(earningEventPayment);
            payableEarning.IncentiveEarnings.Should().NotBeNull();

            var incentivePayment = payableEarning.IncentiveEarnings.Single(o => o.Type == type);

            incentivePayment.Periods.Count().Should().Be(12);
            incentivePayment.Periods.First().Amount.Should().Be(100);
            incentivePayment.Periods.First().PriceEpisodeIdentifier.Should().BeEquivalentTo("p-1");
        }

        [Test]
        [TestCaseSource(nameof(GetOnProgrammeEarning))]
        public void CanMapOnProgrammeEarnings(OnProgrammeEarningType type)
        {
            var payableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>(earningEventPayment);
            payableEarning.OnProgrammeEarnings.Should().NotBeNull();

            var payment = payableEarning.OnProgrammeEarnings.Single(o => o.Type == type);

            payment.Periods.Count().Should().Be(12);
            payment.Periods.First().Amount.Should().Be(100);
            payment.Periods.First().PriceEpisodeIdentifier.Should().BeEquivalentTo("p-1");
        }

        [Test]
        public void CanMapPriceEpisode()
        {
            var payableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>(earningEventPayment);
            payableEarning.PriceEpisodes.Should().NotBeNull();

            var priceEpisode = payableEarning.PriceEpisodes.First();
            priceEpisode.ActualEndDate.Should().Be(earningEventPayment.PriceEpisodes.First().ActualEndDate);
            priceEpisode.Completed.Should().Be(earningEventPayment.PriceEpisodes.First().Completed);
            priceEpisode.CompletionAmount.Should().Be(earningEventPayment.PriceEpisodes.First().CompletionAmount);
            priceEpisode.Identifier.Should().Be(earningEventPayment.PriceEpisodes.First().Identifier);
            priceEpisode.EffectiveTotalNegotiatedPriceStartDate.Should().Be(earningEventPayment.PriceEpisodes.First().EffectiveTotalNegotiatedPriceStartDate);
            priceEpisode.InstalmentAmount.Should().Be(earningEventPayment.PriceEpisodes.First().InstalmentAmount);
            priceEpisode.NumberOfInstalments.Should().Be(earningEventPayment.PriceEpisodes.First().NumberOfInstalments);
            priceEpisode.PlannedEndDate.Should().Be(earningEventPayment.PriceEpisodes.First().PlannedEndDate);
            priceEpisode.TotalNegotiatedPrice1.Should().Be(earningEventPayment.PriceEpisodes.First().TotalNegotiatedPrice1);
            priceEpisode.TotalNegotiatedPrice2.Should().Be(earningEventPayment.PriceEpisodes.First().TotalNegotiatedPrice2);
            priceEpisode.TotalNegotiatedPrice3.Should().Be(earningEventPayment.PriceEpisodes.First().TotalNegotiatedPrice3);
            priceEpisode.TotalNegotiatedPrice4.Should().Be(earningEventPayment.PriceEpisodes.First().TotalNegotiatedPrice4);

        }

        private static Array GetIncentives()
        {
            return Enum.GetValues(typeof(IncentiveEarningType));
        }

        private static Array GetOnProgrammeEarning()
        {
            return Enum.GetValues(typeof(OnProgrammeEarningType));
        }

    }
}
