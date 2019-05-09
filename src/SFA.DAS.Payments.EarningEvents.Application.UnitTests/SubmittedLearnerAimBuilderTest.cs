using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests
{
    [TestFixture]
    public class SubmittedLearnerAimBuilderTest
    {
        [Test]
        public void TestAimsAreCreatedFromCommand()
        {
            var processLearnerCommand = new ProcessLearnerCommand
            {
                CollectionPeriod = 2,
                CollectionYear = 2021,
                IlrSubmissionDateTime = DateTime.Now,
                JobId            = 3,
                Ukprn = 4,
                Learner = new FM36Learner
                {
                    ULN = 5,
                    LearnRefNumber = "6",
                    LearningDeliveries = new List<LearningDelivery>
                    {
                        new LearningDelivery {AimSeqNumber = 1, LearningDeliveryValues = new LearningDeliveryValues {LearnAimRef = "ZPROG001"}},
                        new LearningDelivery
                        {
                            AimSeqNumber = 2, 
                            LearningDeliveryValues = new LearningDeliveryValues {LearnAimRef = "M&E"},
                            LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngBalPayment",
                                    Period1 = 1,
                                    Period2 = 2
                                }
                            }
                        }
                    },
                    PriceEpisodes = new List<PriceEpisode>
                    {
                        new PriceEpisode
                        {
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                PriceEpisodeAimSeqNumber = 1,
                                PriceEpisodeContractType = "Levy Contract",
                            },
                            PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                            {
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeOnProgPayment",
                                    Period1 = 10,
                                    Period2 = 20
                                }
                            }
                        }
                    }

                }
            };

            var builder = new SubmittedLearnerAimBuilder();
            var submittedAims = builder.Build(processLearnerCommand);
            submittedAims.Should().HaveCount(2);

            var mainAim = submittedAims.Single(aim => aim.LearningAimReference == "ZPROG001");
            var mathsAndEnglishAim = submittedAims.Single(aim => aim.LearningAimReference != "ZPROG001");

            mainAim.SfaContributionPercentage.Should().Be(processLearnerCommand.Learner.PriceEpisodes[0].PriceEpisodeValues.PriceEpisodeSFAContribPct);
            mainAim.AcademicYear.Should().Be(processLearnerCommand.CollectionYear);
            mainAim.CollectionPeriod.Should().Be((byte)processLearnerCommand.CollectionPeriod);
            mainAim.IlrSubmissionDateTime.Should().Be(processLearnerCommand.IlrSubmissionDateTime);
            mainAim.JobId.Should().Be(processLearnerCommand.JobId);
            mainAim.LearnerReferenceNumber.Should().Be(processLearnerCommand.Learner.LearnRefNumber);
            mainAim.LearnerUln.Should().Be(processLearnerCommand.Learner.ULN);
            mainAim.LearningAimFrameworkCode.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0].LearningDeliveryValues.FworkCode);
            mainAim.LearningAimPathwayCode.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0].LearningDeliveryValues.PwayCode);
            mainAim.LearningAimProgrammeType.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0].LearningDeliveryValues.ProgType);
            mainAim.LearningAimStandardCode.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0].LearningDeliveryValues.StdCode);
            mainAim.Ukprn.Should().Be(processLearnerCommand.Ukprn);

            mathsAndEnglishAim.AcademicYear.Should().Be(processLearnerCommand.CollectionYear);
            mathsAndEnglishAim.CollectionPeriod.Should().Be((byte)processLearnerCommand.CollectionPeriod);
            mathsAndEnglishAim.IlrSubmissionDateTime.Should().Be(processLearnerCommand.IlrSubmissionDateTime);
            mathsAndEnglishAim.JobId.Should().Be(processLearnerCommand.JobId);
            mathsAndEnglishAim.LearnerReferenceNumber.Should().Be(processLearnerCommand.Learner.LearnRefNumber);
            mathsAndEnglishAim.LearnerUln.Should().Be(processLearnerCommand.Learner.ULN);
            mathsAndEnglishAim.LearningAimFrameworkCode.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0].LearningDeliveryValues.FworkCode);
            mathsAndEnglishAim.LearningAimPathwayCode.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0].LearningDeliveryValues.PwayCode);
            mathsAndEnglishAim.LearningAimProgrammeType.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0].LearningDeliveryValues.ProgType);
            mathsAndEnglishAim.LearningAimStandardCode.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0].LearningDeliveryValues.StdCode);
            mathsAndEnglishAim.Ukprn.Should().Be(processLearnerCommand.Ukprn);

        }
    }
}
