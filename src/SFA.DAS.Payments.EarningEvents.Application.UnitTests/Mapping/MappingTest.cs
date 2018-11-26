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

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
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
                            LearnAimRef = "ZPROG001"
                        }
                    }
                },
                PriceEpisodes = new List<PriceEpisode>
                {
                    new PriceEpisode
                    {
                        PriceEpisodeIdentifier = "pe-1",
                        PriceEpisodeValues = new PriceEpisodeValues
                        {

                            EpisodeStartDate = DateTime.Today.AddMonths(-12),
                            PriceEpisodePlannedEndDate = DateTime.Today,
                            PriceEpisodeActualEndDate = DateTime.Today,
                            PriceEpisodePlannedInstalments = 12,
                            PriceEpisodeCompletionElement = 3000,
                            PriceEpisodeInstalmentValue = 1000,
                            TNP1 = 15000,
                            TNP2 = 15000,
                            PriceEpisodeCompleted = true,
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
                    }
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
                CollectionYear = "1819",
                Ukprn = 12345,
                JobId = 69,
                CollectionPeriod = 1,
                IlrSubmissionDateTime = DateTime.UtcNow,
                SubmissionDate = DateTime.UtcNow
            };
        }

        [Test]
        public void Maps_Ukprn()
        {
            var earningEvent =
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
            earningEvent.Ukprn.Should().Be(processLearnerCommand.Ukprn);
        }

        [Test]
        public void Maps_JobId()
        {
            var earningEvent =
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
            earningEvent.JobId.Should().Be(processLearnerCommand.JobId);
        }

        [Test]
        public void Maps_Collection_Year()
        {
            var earningEvent =
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
            earningEvent.CollectionYear.Should().Be("1819");
        }

        [Test]
        public void Maps_Collection_Period()
        {
            var earningEvent =
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
            earningEvent.CollectionPeriod.Should().NotBeNull();
            earningEvent.CollectionPeriod.Period.Should().Be(1);
        }

        [Test]
        public void Maps_IlrSubmissionTime()
        {
            var earningEvent =
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
            earningEvent.IlrSubmissionDateTime.Should().Be(processLearnerCommand.IlrSubmissionDateTime);
        }

        [Test]
        public void Maps_Learner()
        {
            var earningEvent =
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
            earningEvent.Learner.Should().NotBeNull();
            earningEvent.Learner.ReferenceNumber.Should().Be(fm36Learner.LearnRefNumber);
        }

        [Test]
        public void Maps_Price_Episodes()
        {
            var earningEvent =
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
            earningEvent.PriceEpisodes.Should().NotBeEmpty();
            earningEvent.PriceEpisodes.First().Identifier.Should().Be("pe-1");
            earningEvent.PriceEpisodes.First().TotalNegotiatedPrice1.Should().Be(15000);
            earningEvent.PriceEpisodes.First().TotalNegotiatedPrice2.Should().Be(15000);
            earningEvent.PriceEpisodes.First().CompletionAmount.Should().Be(3000);
            earningEvent.PriceEpisodes.First().InstalmentAmount.Should().Be(1000);
            earningEvent.PriceEpisodes.First().NumberOfInstalments.Should().Be(12);
            earningEvent.PriceEpisodes.First().TotalNegotiatedPrice3.Should().BeNull();
        }

        [Test]
        public void Maps_Price_Episode_Periodised_Values_To_Earning_Periods()
        {
            var earningEvent =
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
            earningEvent.OnProgrammeEarnings.Should().NotBeNullOrEmpty();
            earningEvent.OnProgrammeEarnings.Should().HaveCount(3);
            var learnings =
                earningEvent.OnProgrammeEarnings.FirstOrDefault(x => x.Type == OnProgrammeEarningType.Learning);
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
            var earningEvent =
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
            var learning =
                earningEvent.OnProgrammeEarnings.FirstOrDefault(earnings =>
                    earnings.Type == OnProgrammeEarningType.Learning);
            learning.Should().NotBeNull();
            learning.Periods.Should().HaveCount(12);
            learning.Periods.All(period => period.Amount == 1000).Should().BeTrue();
        }

        [Test]
        public void Maps_Completion_Earnings()
        {
            var earningEvent =
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
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
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
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
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
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
            var earningEvent =
                Mapper.Instance.Map<ProcessLearnerCommand, ApprenticeshipContractType2EarningEvent>(
                    processLearnerCommand);
            earningEvent.Should().NotBeNull();
            earningEvent.LearningAim.Reference.Should().Be("ZPROG001");

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

        private static string MapIncentiveType(IncentiveType incentiveType)
        {
            switch (incentiveType)
            {
                case IncentiveType.First16To18EmployerIncentive:
                    return "PriceEpisodeFirstEmp1618Pay";
                case IncentiveType.First16To18ProviderIncentive:
                    return "PriceEpisodeFirstProv1618Pay";
                case IncentiveType.Second16To18EmployerIncentive:
                    return "PriceEpisodeSecondEmp1618Pay";
                case IncentiveType.Second16To18ProviderIncentive:
                    return "PriceEpisodeSecondProv1618Pay";
                case IncentiveType.OnProgramme16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftOnProgPayment";
                case IncentiveType.Completion16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftCompletionPayment";
                case IncentiveType.Balancing16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftBalancing";
                case IncentiveType.FirstDisadvantagePayment:
                    return "PriceEpisodeFirstDisadvantagePayment";
                case IncentiveType.SecondDisadvantagePayment:
                    return "PriceEpisodeSecondDisadvantagePayment";
                case IncentiveType.LearningSupport:
                    return "PriceEpisodeLSFCash"; 
                 default:
                     return string.Empty;
            }
        }
    }
}