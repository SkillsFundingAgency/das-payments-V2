using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Payments.DataLocks.Application.Mapping;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Mapping
{
    [TestFixture]
    public class MapperConfigurationTest
    {
        private ApprenticeshipContractType1EarningEvent earningEventPayment;
        private Act1FunctionalSkillEarningsEvent functionalSkillEarningEvent;

        [OneTimeSetUp]
        public void Initialise()
        {
            Mapper.Reset();
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
                        TotalNegotiatedPrice4 = 25.0m,
                        FundingLineType = "19+ Apprenticeship Levy Contract (procured)"
                    }

                }),
                EventId =  Guid.NewGuid(),
                StartDate = DateTime.Today.AddDays(-5)
            };

            var functionalSkillEarnings = new List<FunctionalSkillEarning>
            {
                new FunctionalSkillEarning
                {
                    Type = FunctionalSkillType.OnProgrammeMathsAndEnglish,
                    Periods = earnings.AsReadOnly()
                }
            };

            functionalSkillEarningEvent = new Act1FunctionalSkillEarningsEvent
            {
                EventId = Guid.NewGuid(),
                Ukprn = 12345,
                JobId = 123,
                CollectionPeriod = new CollectionPeriod {Period = 12, AcademicYear = 1819},
                Learner = new Learner {ReferenceNumber = "1234-ref", Uln = 123456},
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
                Earnings = functionalSkillEarnings.AsReadOnly(),
                PriceEpisodes = new List<PriceEpisode>
                {
                    new PriceEpisode
                    {
                        ActualEndDate = DateTime.MaxValue,
                        Completed = true,
                        CompletionAmount = 300m,
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

                }
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
        public void CanMapLearningStartDate()
        {
            var payableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>(earningEventPayment);
            payableEarning.StartDate.Should().Be(earningEventPayment.StartDate);
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
            priceEpisode.FundingLineType.Should().Be(earningEventPayment.PriceEpisodes.First().FundingLineType);
        }

        [Test]
        public void TestDataLockEventToDataLockStatusChangedEventMap()
        {
            var dataLockEvent = new PayableEarningEvent
            {
                Learner = new Learner { Uln = 1, ReferenceNumber = "2"},
                LearningAim = new LearningAim{FrameworkCode = 3},
                AgreementId = "4",
                CollectionPeriod = new CollectionPeriod{AcademicYear = 1, Period = 2},
                EventId = Guid.NewGuid()                
            };

            var changedToFailEvent = new DataLockStatusChangedToFailed
            {
                TransactionTypesAndPeriods = new Dictionary<TransactionType, List<EarningPeriod>> {{TransactionType.Balancing, new List<EarningPeriod> {new EarningPeriod {Period = 1}, new EarningPeriod {Period = 2}}}},
                EventId = Guid.NewGuid()
            };

            Mapper.Map(dataLockEvent, changedToFailEvent);

            changedToFailEvent.Learner.Should().NotBeNull();
            changedToFailEvent.Learner.Uln.Should().Be(1);
            changedToFailEvent.LearningAim.Should().NotBeNull();
            changedToFailEvent.LearningAim.FrameworkCode.Should().Be(3);
            changedToFailEvent.CollectionPeriod.Should().NotBeNull();
            changedToFailEvent.CollectionPeriod.AcademicYear.Should().Be(1);
            changedToFailEvent.EventId.Should().Be(changedToFailEvent.EventId);
            changedToFailEvent.TransactionTypesAndPeriods.Should().NotBeNull();
            changedToFailEvent.TransactionTypesAndPeriods.Should().NotBeEmpty();
        }

        [Test]
        public void CanMapFunctionalSkillEarningToFunctionalSkillDataLockEvent()
        {
            var dataLockEvent =
                Mapper.Map<Act1FunctionalSkillEarningsEvent, PayableFunctionalSkillEarningEvent>(
                    functionalSkillEarningEvent);

            dataLockEvent.Should().NotBeNull();

            dataLockEvent.CollectionYear.Should().Be(functionalSkillEarningEvent.CollectionYear);
            dataLockEvent.Earnings.Should().NotBeNull();
        }

        [Test]
        public void CanMapFunctionalSkillEarningToFailedFunctionalSkillDataLockEvent()
        {
            var dataLockEvent =
                Mapper.Map<Act1FunctionalSkillEarningsEvent, FunctionalSkillEarningFailedDataLockMatching>(
                    functionalSkillEarningEvent);

            dataLockEvent.Should().NotBeNull();

            dataLockEvent.CollectionYear.Should().Be(functionalSkillEarningEvent.CollectionYear);
            dataLockEvent.Earnings.Should().NotBeNull();
        }

        [Test]
        public void CanMapEarningEventModelToApprenticeshipContractType1EarningEvent()
        {
            var earningEventModel = new EarningEventModel
            {
                AgreementId = "1",
                Ukprn = 100,
                AcademicYear = 1819,
                EventId = Guid.NewGuid(),
                EventTime = DateTime.Today,
                IlrSubmissionDateTime = DateTime.Today,
                JobId = 1,
                IlrFileName = "100.xml",
                SfaContributionPercentage = 0.5m,
                CollectionPeriod = 1,
                PriceEpisodes = new List<EarningEventPriceEpisodeModel>
                {
                    new EarningEventPriceEpisodeModel
                    {
                        PriceEpisodeIdentifier = "1",
                        TotalNegotiatedPrice1= 10m,
                        TotalNegotiatedPrice2 = 20m,
                        TotalNegotiatedPrice3 = 30m,
                        TotalNegotiatedPrice4 = 40m,
                        AgreedPrice = 100m,
                        CourseStartDate = DateTime.Today,
                        StartDate = DateTime.Today,
                        PlannedEndDate = DateTime.Today,
                        ActualEndDate = DateTime.Today,
                        NumberOfInstalments = 1,
                        InstalmentAmount = 100m,
                        CompletionAmount = 20m,
                        Completed = true,
                        EmployerContribution = 10m,
                        CompletionHoldBackExemptionCode = 10
                    }
                },
                LearnerReferenceNumber = "20",
                LearnerUln = 1000,
                LearningAimReference = "30",
                LearningAimProgrammeType = (int)ProgrammeType.Standard,
                LearningAimStandardCode = 25,
                LearningAimFrameworkCode = 402,
                LearningAimPathwayCode = 103,
                LearningAimFundingLineType = "18-19",
                LearningAimSequenceNumber = 1
            };

            var act1EarningEvent = Mapper.Map<ApprenticeshipContractType1EarningEvent>(earningEventModel);
            act1EarningEvent.Learner = Mapper.Map<Learner>(earningEventModel);
            act1EarningEvent.LearningAim = Mapper.Map<LearningAim>(earningEventModel);

            act1EarningEvent.AgreementId.Should().Be(earningEventModel.AgreementId);
            act1EarningEvent.Ukprn.Should().Be(earningEventModel.Ukprn);
            act1EarningEvent.CollectionPeriod.Period.Should().Be(earningEventModel.CollectionPeriod);
            act1EarningEvent.CollectionPeriod.AcademicYear.Should().Be(earningEventModel.AcademicYear);
            act1EarningEvent.CollectionYear.Should().Be(earningEventModel.AcademicYear);
            act1EarningEvent.EventId.Should().Be(earningEventModel.EventId);
            act1EarningEvent.EventTime.Should().Be(earningEventModel.EventTime);
            act1EarningEvent.IlrSubmissionDateTime.Should().Be(earningEventModel.IlrSubmissionDateTime);
            act1EarningEvent.JobId.Should().Be(earningEventModel.JobId);
            act1EarningEvent.IlrFileName.Should().Be(earningEventModel.IlrFileName);
            act1EarningEvent.SfaContributionPercentage.Should().Be(earningEventModel.SfaContributionPercentage);

            act1EarningEvent.PriceEpisodes[0].Identifier.Should().Be(earningEventModel.PriceEpisodes[0].PriceEpisodeIdentifier);
            act1EarningEvent.PriceEpisodes[0].TotalNegotiatedPrice1.Should().Be(earningEventModel.PriceEpisodes[0].TotalNegotiatedPrice1);
            act1EarningEvent.PriceEpisodes[0].TotalNegotiatedPrice2.Should().Be(earningEventModel.PriceEpisodes[0].TotalNegotiatedPrice2);
            act1EarningEvent.PriceEpisodes[0].TotalNegotiatedPrice3.Should().Be(earningEventModel.PriceEpisodes[0].TotalNegotiatedPrice3);
            act1EarningEvent.PriceEpisodes[0].TotalNegotiatedPrice4.Should().Be(earningEventModel.PriceEpisodes[0].TotalNegotiatedPrice4);
            act1EarningEvent.PriceEpisodes[0].AgreedPrice.Should().Be(earningEventModel.PriceEpisodes[0].AgreedPrice);
            act1EarningEvent.PriceEpisodes[0].CourseStartDate.Should().Be(earningEventModel.PriceEpisodes[0].CourseStartDate);
            act1EarningEvent.PriceEpisodes[0].PlannedEndDate.Should().Be(earningEventModel.PriceEpisodes[0].PlannedEndDate);
            act1EarningEvent.PriceEpisodes[0].ActualEndDate.Should().Be(earningEventModel.PriceEpisodes[0].ActualEndDate);
            act1EarningEvent.PriceEpisodes[0].NumberOfInstalments.Should().Be(earningEventModel.PriceEpisodes[0].NumberOfInstalments);
            act1EarningEvent.PriceEpisodes[0].InstalmentAmount.Should().Be(earningEventModel.PriceEpisodes[0].InstalmentAmount);
            act1EarningEvent.PriceEpisodes[0].CompletionAmount.Should().Be(earningEventModel.PriceEpisodes[0].CompletionAmount);
            act1EarningEvent.PriceEpisodes[0].Completed.Should().Be(earningEventModel.PriceEpisodes[0].Completed);
            act1EarningEvent.PriceEpisodes[0].EmployerContribution.Should().Be(earningEventModel.PriceEpisodes[0].EmployerContribution);
            act1EarningEvent.PriceEpisodes[0].CompletionHoldBackExemptionCode.Should().Be(earningEventModel.PriceEpisodes[0].CompletionHoldBackExemptionCode);
            
            act1EarningEvent.Learner.Uln.Should().Be(earningEventModel.LearnerUln);
            act1EarningEvent.Learner.ReferenceNumber.Should().Be(earningEventModel.LearnerReferenceNumber);
            
            act1EarningEvent.LearningAim.Reference.Should().Be(earningEventModel.LearningAimReference);
            act1EarningEvent.LearningAim.ProgrammeType.Should().Be(earningEventModel.LearningAimProgrammeType);
            act1EarningEvent.LearningAim.StandardCode.Should().Be(earningEventModel.LearningAimStandardCode );
            act1EarningEvent.LearningAim.FrameworkCode.Should().Be(earningEventModel.LearningAimFrameworkCode);
            act1EarningEvent.LearningAim.PathwayCode.Should().Be(earningEventModel.LearningAimPathwayCode);
            act1EarningEvent.LearningAim.FundingLineType.Should().Be(earningEventModel.LearningAimFundingLineType);
            act1EarningEvent.LearningAim.SequenceNumber.Should().Be(earningEventModel.LearningAimSequenceNumber);
         
        }


        [Test]
        public void CanMapEarningEventModelToFunctionalSkillAct1EarningEvent()
        {
            var earningEventModel = new EarningEventModel
            {
                AgreementId = "1",
                Ukprn = 100,
                AcademicYear = 1819,
                EventId = Guid.NewGuid(),
                EventTime = DateTime.Today,
                IlrSubmissionDateTime = DateTime.Today,
                JobId = 1,
                IlrFileName = "100.xml",
                SfaContributionPercentage = 0.5m,
                CollectionPeriod = 1,
                LearnerReferenceNumber = "20",
                LearnerUln = 1000,
                LearningAimReference = "30",
                LearningAimProgrammeType = (int)ProgrammeType.Standard,
                LearningAimStandardCode = 25,
                LearningAimFrameworkCode = 402,
                LearningAimPathwayCode = 103,
                LearningAimFundingLineType = "18-19",
                LearningAimSequenceNumber = 1,
                StartDate = DateTime.Today
            };

            var act1EarningEvent = Mapper.Map<Act1FunctionalSkillEarningsEvent>(earningEventModel);
            act1EarningEvent.Learner = Mapper.Map<Learner>(earningEventModel);
            act1EarningEvent.LearningAim = Mapper.Map<LearningAim>(earningEventModel);
            
            act1EarningEvent.Ukprn.Should().Be(earningEventModel.Ukprn);
            act1EarningEvent.CollectionPeriod.Period.Should().Be(earningEventModel.CollectionPeriod);
            act1EarningEvent.CollectionPeriod.AcademicYear.Should().Be(earningEventModel.AcademicYear);
            act1EarningEvent.CollectionYear.Should().Be(earningEventModel.AcademicYear);
            act1EarningEvent.EventId.Should().Be(earningEventModel.EventId);
            act1EarningEvent.EventTime.Should().Be(earningEventModel.EventTime);
            act1EarningEvent.IlrSubmissionDateTime.Should().Be(earningEventModel.IlrSubmissionDateTime);
            act1EarningEvent.JobId.Should().Be(earningEventModel.JobId);
            act1EarningEvent.IlrFileName.Should().Be(earningEventModel.IlrFileName);
            act1EarningEvent.StartDate.Should().Be(earningEventModel.StartDate);

            act1EarningEvent.Learner.Uln.Should().Be(earningEventModel.LearnerUln);
            act1EarningEvent.Learner.ReferenceNumber.Should().Be(earningEventModel.LearnerReferenceNumber);

            act1EarningEvent.LearningAim.Reference.Should().Be(earningEventModel.LearningAimReference);
            act1EarningEvent.LearningAim.ProgrammeType.Should().Be(earningEventModel.LearningAimProgrammeType);
            act1EarningEvent.LearningAim.StandardCode.Should().Be(earningEventModel.LearningAimStandardCode);
            act1EarningEvent.LearningAim.FrameworkCode.Should().Be(earningEventModel.LearningAimFrameworkCode);
            act1EarningEvent.LearningAim.PathwayCode.Should().Be(earningEventModel.LearningAimPathwayCode);
            act1EarningEvent.LearningAim.FundingLineType.Should().Be(earningEventModel.LearningAimFundingLineType);
            act1EarningEvent.LearningAim.SequenceNumber.Should().Be(earningEventModel.LearningAimSequenceNumber);

        }

        [Test]
        public void CanMapAct1PayableEarningEventIdAndDataLockEventIdCorrectly()
        {
            var payableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>(earningEventPayment);
            payableEarning.EarningEventId.Should().Be(earningEventPayment.EventId);
            payableEarning.EventId.Should().NotBe(default(Guid));
        }

        [Test]
        public void CanMapAct1NonPayableEarningEventIdAndDataLockEventIdCorrectly()
        {
            var nonPayableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, EarningFailedDataLockMatching>(earningEventPayment);
            nonPayableEarning.EarningEventId.Should().Be(earningEventPayment.EventId);
            nonPayableEarning.EventId.Should().NotBe(default(Guid));
        }

        [Test]
        public void CanMapAct1NonPayableEarningStarttDateCorrectly()
        {
            var nonPayableEarning = Mapper.Map<ApprenticeshipContractType1EarningEvent, EarningFailedDataLockMatching>(earningEventPayment);
            nonPayableEarning.StartDate.Should().Be(earningEventPayment.StartDate);
        }

        [Test]
        public void CanMapAct1PayableFunctionalSkillEarningEventEarningEventIdAndDataLockEventIdCorrectly()
        {
            var payableFunctionalSkillEarning = Mapper.Map<Act1FunctionalSkillEarningsEvent, PayableFunctionalSkillEarningEvent>(functionalSkillEarningEvent);
            payableFunctionalSkillEarning.EarningEventId.Should().Be(functionalSkillEarningEvent.EventId);
            payableFunctionalSkillEarning.EventId.Should().NotBe(default(Guid));
        }

        [Test]
        public void CanMapAct1FunctionalSkillEarningFailedDataLockMatchingEventIdAndDataLockEventIdCorrectly()
        {
            var nonFunctionalSkillPayableEarning = Mapper.Map<Act1FunctionalSkillEarningsEvent, FunctionalSkillEarningFailedDataLockMatching>(functionalSkillEarningEvent);
            nonFunctionalSkillPayableEarning.EarningEventId.Should().Be(functionalSkillEarningEvent.EventId);
            nonFunctionalSkillPayableEarning.EventId.Should().NotBe(default(Guid));
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
