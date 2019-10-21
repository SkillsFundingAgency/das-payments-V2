using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests.Mapping
{
    [TestFixture]
    public class MappingTest
    {
        private FM36Learner fm36Learner;
        private ProcessLearnerCommand processLearnerCommand;
        private IntermediateLearningAim learningAim;
        private DateTime currentYearStart = new DateTime(2018, 8, 1);
        private DateTime currentYearEnd = new DateTime(2019, 8, 1).AddSeconds(-1);

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => { cfg.AddProfile<EarningsEventProfile>(); });
            Mapper.AssertConfigurationIsValid();
        }

        [SetUp]
        public void SetUp()
        {
            
            fm36Learner = new FM36Learner
            {
                LearnRefNumber = "learner-a",
                LearningDeliveries = new List<LearningDelivery>
                {
                    new LearningDelivery
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryValues = new LearningDeliveryValues
                        {
                            LearnAimRef = "ZPROG001",
                            StdCode = 100,
                            FworkCode = 200,
                            ProgType = 300,
                            PwayCode = 400,
                            LearnDelInitialFundLineType = "Funding Line Type",
                            LearnStartDate = DateTime.Today.AddDays(-5)
                        }
                    },
                    new LearningDelivery
                    {
                        AimSeqNumber = 2,
                        LearningDeliveryValues = new LearningDeliveryValues
                        {
                            LearnAimRef = "M&E",
                            StdCode = 100,
                            FworkCode = 200,
                            ProgType = 300,
                            PwayCode = 400,
                            LearnDelInitialFundLineType = "Funding Line Type",
                            LearnStartDate = DateTime.Today.AddDays(-10)
                        },
                        LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>
                        {
                            new LearningDeliveryPeriodisedValues
                            {
                                AttributeName = "MathEngOnProgPayment",
                                Period1 = 100,
                                Period2 = 100,
                                Period3 = 100,
                                Period4 = 100,
                                Period5 = 100,
                                Period6 = 100,
                                Period7 = 100,
                                Period8 = 100,
                                Period9 = 100,
                                Period10 = 100,
                                Period11 = 100,
                                Period12 = 100,
                            },
                            new LearningDeliveryPeriodisedValues
                            {
                                AttributeName = "MathEngBalPayment",
                                Period1 = 0,
                                Period2 = 0,
                                Period3 = 0,
                                Period4 = 0,
                                Period5 = 0,
                                Period6 = 0,
                                Period7 = 0,
                                Period8 = 0,
                                Period9 = 0,
                                Period10 = 0,
                                Period11 = 0,
                                Period12 = 300,
                            },
                            new LearningDeliveryPeriodisedValues
                            {
                                AttributeName = "PriceEpisodeLSFCash",
                                Period1 = 0,
                                Period2 = 150,
                                Period3 = 0,
                                Period4 = 0,
                                Period5 = 0,
                                Period6 = 0,
                                Period7 = 0,
                                Period8 = 0,
                                Period9 = 0,
                                Period10 = 0,
                                Period11 = 0,
                                Period12 = 0,
                            },
                        },
                    }
                },
                PriceEpisodes = new List<PriceEpisode>
                {
                    new PriceEpisode
                    {
                        PriceEpisodeIdentifier = "pe-1",
                        PriceEpisodeValues = new PriceEpisodeValues
                        {
                            PriceEpisodeAimSeqNumber = 1,
                            EpisodeStartDate = currentYearStart,
                            EpisodeEffectiveTNPStartDate = currentYearStart.AddMonths(1), 
                            PriceEpisodePlannedEndDate = DateTime.Today,
                            PriceEpisodeActualEndDate = DateTime.Today,
                            PriceEpisodePlannedInstalments = 12,
                            PriceEpisodeCompletionElement = 3000,
                            PriceEpisodeInstalmentValue = 1000,
                            TNP1 = 15000,
                            TNP2 = 15000,
                            PriceEpisodeCompleted = true,
                            PriceEpisodeCumulativePMRs = 13,
                            PriceEpisodeCompExemCode = 14,
                            PriceEpisodeTotalTNPPrice = 30000
                        },
                        PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                        {
                            new PriceEpisodePeriodisedValues
                            {
                                AttributeName = "PriceEpisodeOnProgPayment",
                                Period1 = 1000,
                                Period2 = 1000,
                                Period3 = 1000,
                                Period4 = 1000,
                                Period5 = 1000,
                                Period6 = 1000,
                                Period7 = 1000,
                                Period8 = 1000,
                                Period9 = 1000,
                                Period10 = 1000,
                                Period11 = 1000,
                                Period12 = 1000,
                            },
                            new PriceEpisodePeriodisedValues
                            {
                                AttributeName = "PriceEpisodeCompletionPayment",
                                Period1 = 0,
                                Period2 = 0,
                                Period3 = 0,
                                Period4 = 0,
                                Period5 = 0,
                                Period6 = 0,
                                Period7 = 0,
                                Period8 = 0,
                                Period9 = 0,
                                Period10 = 0,
                                Period11 = 0,
                                Period12 = 3000,
                            },
                            new PriceEpisodePeriodisedValues
                            {
                                AttributeName = "PriceEpisodeBalancePayment",
                                Period1 = 0,
                                Period2 = 0,
                                Period3 = 0,
                                Period4 = 0,
                                Period5 = 0,
                                Period6 = 0,
                                Period7 = 0,
                                Period8 = 0,
                                Period9 = 0,
                                Period10 = 0,
                                Period11 = 0,
                                Period12 = 3000,
                            },
                        }
                    },
                   

                }
            };

            var incentiveTypes = GetIncentiveTypes();

            foreach (var incentiveType in incentiveTypes)
            {
                fm36Learner.PriceEpisodes.First().PriceEpisodePeriodisedValues.Add(new PriceEpisodePeriodisedValues
                {
                    AttributeName = incentiveType,
                    Period1 = 1000,
                    Period2 = 1000,
                    Period3 = 1000,
                    Period4 = 1000,
                    Period5 = 1000,
                    Period6 = 1000,
                    Period7 = 1000,
                    Period8 = 1000,
                    Period9 = 1000,
                    Period10 = 1000,
                    Period11 = 1000,
                    Period12 = 1000,
                });
            }

            processLearnerCommand = new ProcessLearnerCommand
            {
                Learner = fm36Learner,
                CollectionYear = 1819,
                Ukprn = 12345,
                JobId = 69,
                CollectionPeriod = 1,
                IlrSubmissionDateTime = DateTime.UtcNow,
                SubmissionDate = DateTime.UtcNow
            };

            learningAim = new IntermediateLearningAim(processLearnerCommand, fm36Learner.PriceEpisodes, fm36Learner.LearningDeliveries[0]);
        }

        [Test]
        public void Maps_Ukprn()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.Ukprn.Should().Be(processLearnerCommand.Ukprn);
        }

        [Test]
        public void Maps_JobId()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.JobId.Should().Be(processLearnerCommand.JobId);
        }

        [Test]
        public void Maps_Collection_Year()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.CollectionYear.Should().Be(1819);
        }

        [Test]
        public void Maps_Collection_Period()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.CollectionPeriod.Should().NotBeNull();
            earningEvent.CollectionPeriod.Period.Should().Be(1);
        }

        [Test]
        public void Maps_IlrSubmissionTime()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.IlrSubmissionDateTime.Should().Be(processLearnerCommand.IlrSubmissionDateTime);
        }

        [Test]
        public void Maps_Learner()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.Learner.Should().NotBeNull();
            earningEvent.Learner.ReferenceNumber.Should().Be(fm36Learner.LearnRefNumber);
        }

        [Test]
        public void Maps_Price_Episodes()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.PriceEpisodes.Should().NotBeEmpty();
            earningEvent.PriceEpisodes.First().Identifier.Should().Be("pe-1");
            earningEvent.PriceEpisodes.First().CourseStartDate.Should().Be(currentYearStart);
            earningEvent.PriceEpisodes.First().EffectiveTotalNegotiatedPriceStartDate.Should().Be(currentYearStart.AddMonths(1));
            earningEvent.PriceEpisodes.First().TotalNegotiatedPrice1.Should().Be(15000);
            earningEvent.PriceEpisodes.First().TotalNegotiatedPrice2.Should().Be(15000);
            earningEvent.PriceEpisodes.First().CompletionAmount.Should().Be(3000);
            earningEvent.PriceEpisodes.First().InstalmentAmount.Should().Be(1000);
            earningEvent.PriceEpisodes.First().NumberOfInstalments.Should().Be(12);
            earningEvent.PriceEpisodes.First().TotalNegotiatedPrice3.Should().BeNull();
            earningEvent.PriceEpisodes.First().EmployerContribution.Should().Be(13);
            earningEvent.PriceEpisodes.First().CompletionHoldBackExemptionCode.Should().Be(14);
            earningEvent.PriceEpisodes.First().AgreedPrice.Should().Be(30000);
        }

        [Test]
        public void Maps_Price_Episode_Periodised_Values_To_Earning_Periods()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.OnProgrammeEarnings.Should().NotBeNullOrEmpty();
            earningEvent.OnProgrammeEarnings.Should().HaveCount(3);
            var learnings = earningEvent.OnProgrammeEarnings.FirstOrDefault(x => x.Type == OnProgrammeEarningType.Learning);
            learnings.Should().NotBeNull();
            learnings.Periods.Count.Should().Be(12);
            learnings.Periods.All(period => period.Amount == 1000 && period.PriceEpisodeIdentifier == "pe-1").Should()
                .BeTrue();
            var completion =
                earningEvent.OnProgrammeEarnings.FirstOrDefault(x => x.Type == OnProgrammeEarningType.Completion);
            completion.Should().NotBeNull();
            completion.Periods.Count.Should().Be(12);
            completion.Periods.Any(x => x.Period == 12 && x.Amount == 3000).Should().BeTrue();
        }

        [Test]
        public void Maps_On_Programme_Earnings()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            var learning =
                earningEvent.OnProgrammeEarnings.FirstOrDefault(earnings =>
                    earnings.Type == OnProgrammeEarningType.Learning);
            learning.Should().NotBeNull();
            learning.Periods.Should().HaveCount(12);
            learning.Periods.All(period => period.Amount == 1000).Should().BeTrue();
        }

        [Test]
        public void Maps_SfaContribution_With_Earnings_When_SfaContribution_Present()
        {
            var learningAimWithSfaContribution = learningAim;
            learningAimWithSfaContribution.PriceEpisodes.First().PriceEpisodePeriodisedValues.Add(new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeSFAContribPct",
                Period1 = 0.91m,
                Period2 = 0.91m,
                Period3 = 0.91m,
                Period4 = 0.91m,
                Period5 = 0.91m,
                Period6 = 0.91m,
                Period7 = 0.91m,
                Period8 = 0.91m,
                Period9 = 0.91m,
                Period10 = 0.91m,
                Period11 = 0.91m,
                Period12 = 0.91m,
            });
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            var learning =
                earningEvent.OnProgrammeEarnings.FirstOrDefault(earnings =>
                    earnings.Type == OnProgrammeEarningType.Learning);
            learning.Should().NotBeNull();
            learning.Periods.Should().HaveCount(12);
            learning.Periods.All(period => period.SfaContributionPercentage == 0.91m).Should().BeTrue();
        }

        [Test]
        public void Maps_Completion_Earnings()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            var completion =
                earningEvent.OnProgrammeEarnings.FirstOrDefault(earnings =>
                    earnings.Type == OnProgrammeEarningType.Completion);
            completion.Should().NotBeNull();
            completion.Periods.Should().HaveCount(12);
            var completionPeriod = completion.Periods.FirstOrDefault(p => p.Period == 12);
            completionPeriod.Should().NotBeNull();
            completionPeriod.Amount.Should().Be(3000);
        }

        [Test]
        public void MapsBalancingEarnings()
        {
            var earningEvent =
                Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(
                    learningAim);
            var completion =
                earningEvent.OnProgrammeEarnings.FirstOrDefault(earnings =>
                    earnings.Type == OnProgrammeEarningType.Balancing);
            completion.Should().NotBeNull();
            completion.Periods.Should().HaveCount(12);
            var completionPeriod = completion.Periods.FirstOrDefault(p => p.Period == 12);
            completionPeriod.Should().NotBeNull();
            completionPeriod.Amount.Should().Be(3000);
        }

        [Test]
        [TestCaseSource(nameof(GetIncentiveTypes))]
        public void MapsIncentiveEarnings(string incentiveType)
        {
            var earningEvent =
                Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(
                    learningAim);
            var learning =
                earningEvent.IncentiveEarnings.FirstOrDefault(earnings =>
                    MapIncentiveType(earnings.Type).Equals(incentiveType));
            learning.Should().NotBeNull();
            learning.Periods.Should().HaveCount(12);
            learning.Periods.All(period => period.Amount == 1000).Should().BeTrue();
        }

        [Test]
        public void Maps_LearningAim()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.Should().NotBeNull();
            earningEvent.LearningAim.Reference.Should().Be("ZPROG001");
        }

        [Test]
        public void Maps_LearnStartDate()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.Should().NotBeNull();
            earningEvent.StartDate.Should().Be(DateTime.Today.AddDays(-5));
        }

        [Test]
        public void TestFunctionalSkillsEarningMap()
        {
            learningAim = new IntermediateLearningAim(processLearnerCommand, fm36Learner.PriceEpisodes, fm36Learner.LearningDeliveries[1]);
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, Act2FunctionalSkillEarningsEvent>(learningAim);
            earningEvent.Should().NotBeNull();
            earningEvent.LearningAim.Reference.Should().Be("M&E");
            earningEvent.Earnings.Should().HaveCount(3);
            earningEvent.StartDate.Should().Be(DateTime.Today.AddDays(-10));

            var balancing = earningEvent.Earnings.Where(e => e.Type == FunctionalSkillType.BalancingMathsAndEnglish).ToArray();
            balancing.Should().HaveCount(1);
            balancing[0].Periods.Where(p => p.Amount == 0).Should().HaveCount(11);
            balancing[0].Periods.Where(p => p.Amount == 300).Should().HaveCount(1);
            
            var onProg = earningEvent.Earnings.Where(e => e.Type == FunctionalSkillType.OnProgrammeMathsAndEnglish).ToArray();
            onProg.Should().HaveCount(1);
            onProg[0].Periods.Where(p => p.Amount == 100).Should().HaveCount(12);

            var learningSupport = earningEvent.Earnings.Where(e => e.Type == FunctionalSkillType.LearningSupport).ToArray();
            learningSupport.Should().HaveCount(1);
            learningSupport[0].Periods.Where(p => p.Amount == 0).Should().HaveCount(11);
            learningSupport[0].Periods.Where(p => p.Amount == 150).Should().HaveCount(1);
        }

        [Test]
        public void TestMultiplePriceEpisodes()
        {
            fm36Learner.PriceEpisodes.Clear();

            fm36Learner.PriceEpisodes.Add(new PriceEpisode
            {
                PriceEpisodeIdentifier = "pe-1",
                PriceEpisodeValues = new PriceEpisodeValues
                {
                    EpisodeStartDate = currentYearStart,
                    PriceEpisodePlannedEndDate = currentYearStart.AddMonths(6),
                    PriceEpisodeActualEndDate = currentYearStart.AddMonths(6),
                    PriceEpisodePlannedInstalments = 6,
                    PriceEpisodeCompletionElement = 1500,
                    PriceEpisodeInstalmentValue = 500,
                    TNP1 = 7500,
                    TNP2 = 7500,
                    PriceEpisodeCompleted = true,
                },
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                {
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeOnProgPayment",
                        Period1 = 500,
                        Period2 = 500,
                        Period3 = 500,
                        Period4 = 500,
                        Period5 = 500,
                        Period6 = 500,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = 0
                    },
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeCompletionPayment",
                        Period1 = 0,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 1500,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = 0
                    },
                }
            });
            
            fm36Learner.PriceEpisodes.Add(new PriceEpisode
            {
                PriceEpisodeIdentifier = "pe-2",
                PriceEpisodeValues = new PriceEpisodeValues
                {

                    EpisodeStartDate = currentYearStart.AddMonths(7),
                    PriceEpisodePlannedEndDate = currentYearEnd.Date,
                    PriceEpisodeActualEndDate = currentYearEnd.Date,
                    PriceEpisodePlannedInstalments = 6,
                    PriceEpisodeCompletionElement = 1500,
                    PriceEpisodeInstalmentValue = 500,
                    TNP1 = 7500,
                    TNP2 = 7500,
                    PriceEpisodeCompleted = true
                },
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                {
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeOnProgPayment",
                        Period1 = 0,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 500,
                        Period8 = 500,
                        Period9 = 500,
                        Period10 = 500,
                        Period11 = 500,
                        Period12 = 500
                    },
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeCompletionPayment",
                        Period1 = 0,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = 1500,
                    },
                }
            });

            learningAim = new IntermediateLearningAim(processLearnerCommand, fm36Learner.PriceEpisodes, fm36Learner.LearningDeliveries[0]);
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);

            earningEvent.PriceEpisodes.Should().HaveCount(2);
            earningEvent.OnProgrammeEarnings.Should().HaveCount(3); // we generate 3 earnings even if not present in ILR

            var learning = earningEvent.OnProgrammeEarnings.Single(e => e.Type == OnProgrammeEarningType.Learning);
            learning.Periods.Should().HaveCount(12);
            learning.Periods.Count(p => p.Amount == 500).Should().Be(12);
            learning.Periods.Count(p => p.Period < 7 && p.PriceEpisodeIdentifier == "pe-1").Should().Be(6);
            learning.Periods.Count(p => p.Period > 6 && p.PriceEpisodeIdentifier == "pe-2").Should().Be(6);

            var completion = earningEvent.OnProgrammeEarnings.Single(e => e.Type == OnProgrammeEarningType.Completion);
            completion.Periods.Should().HaveCount(12);
            completion.Periods.Count(p => p.Amount == 0).Should().Be(10);
            completion.Periods.Where(p => p.Period == 8).Single().PriceEpisodeIdentifier.Should().Be("pe-1");
            completion.Periods.Where(p => p.Period == 8).Single().Amount.Should().Be(1500);
            completion.Periods.Where(p => p.Period == 12).Single().PriceEpisodeIdentifier.Should().Be("pe-2");
            completion.Periods.Where(p => p.Period == 12).Single().Amount.Should().Be(1500);            
        }

        [Test]
        public void TestOnlyCurrentYearPriceEpisodesGetCopied()
        {
            
            var lastYearPriceEpisode = new PriceEpisode
            {
                PriceEpisodeIdentifier = "pe-last-year",
                PriceEpisodeValues = new PriceEpisodeValues
                {
                    EpisodeStartDate = currentYearStart.AddYears(-1),
                    PriceEpisodePlannedEndDate = currentYearStart.AddDays(-1),
                    PriceEpisodeActualEndDate = currentYearStart.AddDays(-1),
                    PriceEpisodePlannedInstalments = 6,
                    PriceEpisodeCompletionElement = 1500,
                    PriceEpisodeInstalmentValue = 500,
                    TNP1 = 7500,
                    TNP2 = 7500,
                    PriceEpisodeCompleted = true,
                },
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                {
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeOnProgPayment",
                        Period1 = 0,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = 500
                    },
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeCompletionPayment",
                        Period1 = 0,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = 100
                    },
                }
            };

            var currentYearPriceEpisode = new PriceEpisode
            {
                PriceEpisodeIdentifier = "pe-current-year",
                PriceEpisodeValues = new PriceEpisodeValues
                {
                    EpisodeStartDate = currentYearStart,
                    PriceEpisodePlannedEndDate = currentYearStart.AddMonths(1),
                    PriceEpisodeActualEndDate = currentYearStart.AddMonths(1),
                    PriceEpisodePlannedInstalments = 6,
                    PriceEpisodeCompletionElement = 1500,
                    PriceEpisodeInstalmentValue = 500,
                    TNP1 = 7500,
                    TNP2 = 7500,
                    PriceEpisodeCompleted = true,
                },
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                {
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeOnProgPayment",
                        Period1 = 0,
                        Period2 = 500,
                        Period3 = 500,
                        Period4 = 500,
                        Period5 = 500,
                        Period6 = 500,
                        Period7 = 500,
                        Period8 = 500,
                        Period9 = 500,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = 0
                    },
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeCompletionPayment",
                        Period1 = 0,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 100,
                        Period12 = 0
                    },
                }
            };


            var futureYearPriceEpisode = new PriceEpisode
            {
                PriceEpisodeIdentifier = "pe-next-year",
                PriceEpisodeValues = new PriceEpisodeValues
                {
                    EpisodeStartDate = currentYearEnd.AddDays(1).Date,
                    PriceEpisodePlannedEndDate = currentYearEnd.AddYears(1).Date,
                    PriceEpisodePlannedInstalments = 6,
                    PriceEpisodeCompletionElement = 1500,
                    PriceEpisodeInstalmentValue = 500,
                    TNP1 = 7500,
                    TNP2 = 7500,
                    PriceEpisodeCompleted = true,
                },
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                {
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeOnProgPayment",
                        Period1 = 500,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = 0
                    },
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeCompletionPayment",
                        Period1 = 100,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = 0
                    },
                }
            };

            var incentiveTypes = GetIncentiveTypes();

            foreach (var incentiveType in incentiveTypes)
            {
                var priceEpisodePeriodisedValues = new PriceEpisodePeriodisedValues
                {
                    AttributeName = incentiveType,
                    Period1 = 1000,
                    Period2 = 1000,
                    Period3 = 1000,
                    Period4 = 1000,
                    Period5 = 1000,
                    Period6 = 1000,
                    Period7 = 1000,
                    Period8 = 1000,
                    Period9 = 1000,
                    Period10 = 1000,
                    Period11 = 1000,
                    Period12 = 1000,
                };

                lastYearPriceEpisode.PriceEpisodePeriodisedValues.Add(priceEpisodePeriodisedValues);
                currentYearPriceEpisode.PriceEpisodePeriodisedValues.Add(priceEpisodePeriodisedValues);
                futureYearPriceEpisode.PriceEpisodePeriodisedValues.Add(priceEpisodePeriodisedValues);
            }


            fm36Learner.PriceEpisodes.Clear();
            fm36Learner.PriceEpisodes.Add(lastYearPriceEpisode);
            fm36Learner.PriceEpisodes.Add(currentYearPriceEpisode);
            fm36Learner.PriceEpisodes.Add(futureYearPriceEpisode);

            learningAim = new IntermediateLearningAim(processLearnerCommand, fm36Learner.PriceEpisodes, fm36Learner.LearningDeliveries[0]);
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);

            earningEvent.PriceEpisodes.Should().HaveCount(1);
            earningEvent.PriceEpisodes[0].Identifier.Should().Be("pe-current-year");
            earningEvent.IncentiveEarnings.All(i => i.Periods.Count == 12).Should().BeTrue();
        }

        [TestCase(100.00000900002, 100.00001)]
        [TestCase(100.00000444444, 100.00000)]
        [TestCase(100.00000555555, 100.00001)]
        public void TestRoundsFm36EOnProgEarningPeriodAmountCorrectly(decimal fm36Earning, decimal expectedEarning)
        {
            fm36Learner.PriceEpisodes.Clear();

            fm36Learner.PriceEpisodes.Add(new PriceEpisode
            {
                PriceEpisodeIdentifier = "pe-1",
                PriceEpisodeValues = new PriceEpisodeValues
                {
                    EpisodeStartDate = currentYearStart,
                    PriceEpisodePlannedEndDate = currentYearStart.AddMonths(6),
                    PriceEpisodeActualEndDate = currentYearStart.AddMonths(6),
                    PriceEpisodePlannedInstalments = 6,
                    PriceEpisodeCompletionElement = 1500,
                    PriceEpisodeInstalmentValue = 500,
                    TNP1 = 7500,
                    TNP2 = 7500,
                    PriceEpisodeCompleted = true,
                },
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                {
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeOnProgPayment",
                        Period1 = fm36Earning,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = 0
                    },
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeCompletionPayment",
                        Period1 = 0,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = fm36Earning
                    },
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeBalancePayment",
                        Period1 = 0,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = fm36Earning
                    }
                }
            });

            learningAim = new IntermediateLearningAim(processLearnerCommand, fm36Learner.PriceEpisodes, fm36Learner.LearningDeliveries[0]);
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);

            earningEvent.PriceEpisodes.Should().HaveCount(1);
            earningEvent.OnProgrammeEarnings.Should().HaveCount(3); // we generate 3 earnings even if not present in ILR

            var learning = earningEvent.OnProgrammeEarnings.Single(e => e.Type == OnProgrammeEarningType.Learning);
            learning.Periods.Should().HaveCount(12);
            learning.Periods[0].Amount.Should().Be(expectedEarning);

            var completion = earningEvent.OnProgrammeEarnings.Single(e => e.Type == OnProgrammeEarningType.Completion);
            completion.Periods.Should().HaveCount(12);
            completion.Periods[11].Amount.Should().Be(expectedEarning);

            var balancing = earningEvent.OnProgrammeEarnings.Single(e => e.Type == OnProgrammeEarningType.Balancing);
            balancing.Periods.Should().HaveCount(12);
            balancing.Periods[11].Amount.Should().Be(expectedEarning);
        }

        [TestCase(100.00000900002, 100.00001)]
        [TestCase(100.00000444444, 100.00000)]
        [TestCase(100.00000555555, 100.00001)]
        public void TestRoundsIncentivesFm36EarningPeriodAmountCorrectly(decimal fm36Earning, decimal expectedEarning)
        {
            fm36Learner.PriceEpisodes.Clear();

            fm36Learner.PriceEpisodes.Add(new PriceEpisode
            {
                PriceEpisodeIdentifier = "pe-1",
                PriceEpisodeValues = new PriceEpisodeValues
                {
                    EpisodeStartDate = currentYearStart,
                    PriceEpisodePlannedEndDate = currentYearStart.AddMonths(6),
                    PriceEpisodeActualEndDate = currentYearStart.AddMonths(6),
                    PriceEpisodePlannedInstalments = 6,
                    TNP1 = 7500,
                    TNP2 = 7500,
                    PriceEpisodeCompleted = true,
                },
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                {
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeOnProgPayment",
                        Period1 = fm36Earning,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = 0
                    },
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeCompletionPayment",
                        Period1 = 0,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = fm36Earning
                    },
                    new PriceEpisodePeriodisedValues
                    {
                        AttributeName = "PriceEpisodeBalancePayment",
                        Period1 = 0,
                        Period2 = 0,
                        Period3 = 0,
                        Period4 = 0,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = fm36Earning
                    }
                }
            });

            var incentiveTypes = GetIncentiveTypes().ToList();
            fm36Learner.PriceEpisodes.First().PriceEpisodePeriodisedValues.Add(new PriceEpisodePeriodisedValues
            {
                AttributeName = incentiveTypes[0],
                Period1 = fm36Earning,
                Period2 = 0,
                Period3 = 0,
                Period4 = 0,
                Period5 = 0,
                Period6 = 0,
                Period7 = 0,
                Period8 = 0,
                Period9 = 0,
                Period10 = 0,
                Period11 = 0,
                Period12 = 0,
            });

            learningAim = new IntermediateLearningAim(processLearnerCommand, fm36Learner.PriceEpisodes, fm36Learner.LearningDeliveries[0]);
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            var incentive = earningEvent.IncentiveEarnings.FirstOrDefault(earnings => MapIncentiveType(earnings.Type).Equals(incentiveTypes[0]));
            incentive.Should().NotBeNull();
            incentive.Periods.Should().HaveCount(12);
            incentive.Periods[0].Amount.Should().Be(expectedEarning);
        }

        [TestCase(100.00000900002, 100.00001)]
        [TestCase(100.00000444444, 100.00000)]
        [TestCase(100.00000555555, 100.00001)]
        public void TestRoundsFunctionalSkillsFm36EarningPeriodAmountCorrectly(decimal fm36Earning, decimal expectedEarning)
        {
            fm36Learner.LearningDeliveries.Clear();
            fm36Learner.LearningDeliveries.AddRange(new List<LearningDelivery>
            {
                new LearningDelivery
                {
                    AimSeqNumber = 1,
                    LearningDeliveryValues = new LearningDeliveryValues
                    {
                        LearnAimRef = "M&E",
                        StdCode = 100,
                        FworkCode = 200,
                        ProgType = 300,
                        PwayCode = 400,
                        LearnDelInitialFundLineType = "Funding Line Type",
                        LearnStartDate = DateTime.Today.AddDays(-10)
                    },
                    LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>
                    {
                        new LearningDeliveryPeriodisedValues
                        {
                            AttributeName = "MathEngOnProgPayment",
                            Period1 = fm36Earning,
                            Period2 = 0,
                            Period3 = 0,
                            Period4 = 0,
                            Period5 = 0,
                            Period6 = 0,
                            Period7 = 0,
                            Period8 = 0,
                            Period9 = 0,
                            Period10 = 0,
                            Period11 = 0,
                            Period12 = 0,
                        },
                        new LearningDeliveryPeriodisedValues
                        {
                            AttributeName = "MathEngBalPayment",
                            Period1 = fm36Earning,
                            Period2 = 0,
                            Period3 = 0,
                            Period4 = 0,
                            Period5 = 0,
                            Period6 = 0,
                            Period7 = 0,
                            Period8 = 0,
                            Period9 = 0,
                            Period10 = 0,
                            Period11 = 0,
                            Period12 = 300,
                        },
                        new LearningDeliveryPeriodisedValues
                        {
                            AttributeName = "PriceEpisodeLSFCash",
                            Period1 = fm36Earning,
                            Period2 = 0,
                            Period3 = 0,
                            Period4 = 0,
                            Period5 = 0,
                            Period6 = 0,
                            Period7 = 0,
                            Period8 = 0,
                            Period9 = 0,
                            Period10 = 0,
                            Period11 = 0,
                            Period12 = 0,
                        },
                    },
                }
            });

            learningAim = new IntermediateLearningAim(processLearnerCommand, fm36Learner.PriceEpisodes, fm36Learner.LearningDeliveries[0]);
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, Act2FunctionalSkillEarningsEvent>(learningAim);
            earningEvent.Should().NotBeNull();

            var balancing = earningEvent.Earnings.Where(e => e.Type == FunctionalSkillType.BalancingMathsAndEnglish).ToArray();
            balancing.Should().HaveCount(1);
            balancing[0].Periods[0].Amount.Should().Be(expectedEarning);

            var onProg = earningEvent.Earnings.Where(e => e.Type == FunctionalSkillType.OnProgrammeMathsAndEnglish).ToArray();
            onProg.Should().HaveCount(1);
            onProg[0].Periods[0].Amount.Should().Be(expectedEarning);

            var learningSupport = earningEvent.Earnings.Where(e => e.Type == FunctionalSkillType.LearningSupport).ToArray();
            learningSupport.Should().HaveCount(1);
            learningSupport[0].Periods[0].Amount.Should().Be(expectedEarning);

        }

        private static IEnumerable<string> GetIncentiveTypes()
        {
            yield return "PriceEpisodeFirstEmp1618Pay";
            yield return "PriceEpisodeFirstProv1618Pay";
            yield return "PriceEpisodeSecondEmp1618Pay";
            yield return "PriceEpisodeSecondProv1618Pay";
            yield return "PriceEpisodeApplic1618FrameworkUpliftOnProgPayment";
            yield return "PriceEpisodeApplic1618FrameworkUpliftCompletionPayment";
            yield return "PriceEpisodeApplic1618FrameworkUpliftBalancing";
            yield return "PriceEpisodeFirstDisadvantagePayment";
            yield return "PriceEpisodeSecondDisadvantagePayment";
            yield return "PriceEpisodeLSFCash"; 
        }

        private static string MapIncentiveType(IncentiveEarningType incentiveType)
        {
            switch (incentiveType)
            {
                case IncentiveEarningType.First16To18EmployerIncentive:
                    return "PriceEpisodeFirstEmp1618Pay";
                case IncentiveEarningType.First16To18ProviderIncentive:
                    return "PriceEpisodeFirstProv1618Pay";
                case IncentiveEarningType.Second16To18EmployerIncentive:
                    return "PriceEpisodeSecondEmp1618Pay";
                case IncentiveEarningType.Second16To18ProviderIncentive:
                    return "PriceEpisodeSecondProv1618Pay";
                case IncentiveEarningType.OnProgramme16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftOnProgPayment";
                case IncentiveEarningType.Completion16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftCompletionPayment";
                case IncentiveEarningType.Balancing16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftBalancing";
                case IncentiveEarningType.FirstDisadvantagePayment:
                    return "PriceEpisodeFirstDisadvantagePayment";
                case IncentiveEarningType.SecondDisadvantagePayment:
                    return "PriceEpisodeSecondDisadvantagePayment";
                case IncentiveEarningType.LearningSupport:
                    return "PriceEpisodeLSFCash"; 
                 default:
                     return string.Empty;
            }
        }
    }
}