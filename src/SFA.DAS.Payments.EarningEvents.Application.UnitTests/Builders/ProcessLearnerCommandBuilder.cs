using System;
using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests.Builders
{
    public class ProcessLearnerCommandBuilder
    {
        private readonly ProcessLearnerCommand learnerCommand;

        public ProcessLearnerCommandBuilder()
        {
            learnerCommand = new ProcessLearnerCommand
            {
                Ukprn = 1,
                JobId = 1,
                CollectionPeriod = 1,
                CollectionYear = 1920,
                IlrSubmissionDateTime = DateTime.Today,
                SubmissionDate = DateTime.Today,
                Learner = new FM36Learner
                {
                    LearnRefNumber = "learner-a",
                    ULN = 1234678,
                    PriceEpisodes = new List<PriceEpisode>
                    {
                        new PriceEpisode
                        {
                            PriceEpisodeIdentifier = "PE-1",
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = new DateTime(2019,8,1),
                                PriceEpisodeActualEndDate = new DateTime(2020,7,31),
                                PriceEpisodeAimSeqNumber = 1,
                                PriceEpisodeContractType = "Contract for services with the employer"
                            },
                            PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>()
                        }
                    },
                    LearningDeliveries = new List<LearningDelivery>
                    {
                        new LearningDelivery
                        {
                            AimSeqNumber = 1,
                            LearningDeliveryValues = new LearningDeliveryValues
                            {
                                LearnAimRef = "ZPROG001",
                                StdCode = 50,
                                ProgType = 25
                            },
                            LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>(),
                            LearningDeliveryPeriodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>()
                        }
                    }
                }
            };
        }

        public ProcessLearnerCommand Build()
        {
            return learnerCommand;
        }

        public ProcessLearnerCommandBuilder WithExtendedLearningSupport()
        {
            var learningDelivery = new LearningDelivery
            {
                AimSeqNumber = 2,
                LearningDeliveryValues = new LearningDeliveryValues {LearnAimRef = "M&E"},
                LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>
                {
                    new LearningDeliveryPeriodisedValues
                    {
                        AttributeName = "MathEngOnProgPayment",
                        Period1 = 39.25m,
                        Period2 = 39.25m,
                        Period3 = 39.25m,
                        Period4 = 39.25m,
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
                        AttributeName = "LearnSuppFundCash",
                        Period1 = 0,
                        Period2 = 0,
                        Period3 = 150,
                        Period4 = 150,
                        Period5 = 0,
                        Period6 = 0,
                        Period7 = 0,
                        Period8 = 0,
                        Period9 = 0,
                        Period10 = 0,
                        Period11 = 0,
                        Period12 = 0,
                    }
                },
                LearningDeliveryPeriodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>
                {
                    new LearningDeliveryPeriodisedTextValues
                    {
                        AttributeName = "LearnDelContType",
                        Period1 = "Contract for services with the employer",
                        Period2 = "Contract for services with the employer",
                        Period3 = "Contract for services with the employer",
                        Period4 = "Contract for services with the employer",
                        Period5 = "Contract for services with the employer",
                        Period6 = "Contract for services with the employer",
                        Period7 = "Contract for services with the employer",
                        Period8 = "Contract for services with the employer",
                        Period9 = "Contract for services with the employer",
                        Period10 = "Contract for services with the employer",
                        Period11 = "Contract for services with the employer",
                        Period12 = "Contract for services with the employer"
                    },
                    new LearningDeliveryPeriodisedTextValues
                    {
                        AttributeName = "FundLineType",
                        Period1 = "19+ Apprenticeship (Employer on App Service)",
                        Period2 = "19+ Apprenticeship (Employer on App Service)",
                        Period3 = "19+ Apprenticeship (Employer on App Service)",
                        Period4 = "19+ Apprenticeship (Employer on App Service)",
                        Period5 = "19+ Apprenticeship (Employer on App Service)",
                        Period6 = "19+ Apprenticeship (Employer on App Service)",
                        Period7 = "19+ Apprenticeship (Employer on App Service)",
                        Period8 = "19+ Apprenticeship (Employer on App Service)",
                        Period9 = "19+ Apprenticeship (Employer on App Service)",
                        Period10 = "19+ Apprenticeship (Employer on App Service)",
                        Period11 = "19+ Apprenticeship (Employer on App Service)",
                        Period12 = "19+ Apprenticeship (Employer on App Service)"
                    }
                }
            };
            learnerCommand.Learner.LearningDeliveries.Add(learningDelivery);
            return this;
        }
    }
}
