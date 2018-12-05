﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public abstract class EndToEndStepsBase : StepsBase
    {
        protected RequiredPaymentsCacheCleaner RequiredPaymentsCacheCleaner => Container.Resolve<RequiredPaymentsCacheCleaner>();

        protected List<Price> CurrentPriceEpisodes
        {
            get => !Context.TryGetValue<List<Price>>(out var currentPriceEpisodes) ? null : currentPriceEpisodes;
            set => Set(value);
        }

        protected List<Training> CurrentIlr
        {
            get => Get<List<Training>>();
            set => Set(value);
        }

        protected List<Training> PreviousIlr
        {
            get => Get<List<Training>>("previous_training");
            set => Set(value, "previous_training");
        }

        protected List<OnProgrammeEarning> CurrentEarnings
        {
            get => Get<List<OnProgrammeEarning>>("current_earnings");
            set => Set(value, "current_earnings");
        }

        protected List<OnProgrammeEarning> PreviousEarnings
        {
            get => Get<List<OnProgrammeEarning>>("previous_earnings");
            set => Set(value, "previous_earnings");
        }

        public CalendarPeriod CurrentCollectionPeriod
        {
            get => Get<CalendarPeriod>("current_collection_period");
            set => Set(value, "current_collection_period");
        }

        protected EndToEndStepsBase(FeatureContext context) : base(context)
        {
        }

        protected void SetCollectionPeriod(string collectionPeriod)
        {
            Console.WriteLine($"Current collection period is: {collectionPeriod}.");
            var period = collectionPeriod.ToDate().ToCalendarPeriod();
            Console.WriteLine($"Current collection period name is: {period.Name}.");
            CurrentCollectionPeriod = period;
            CollectionPeriod = CurrentCollectionPeriod.Period;
            CollectionYear = CurrentCollectionPeriod.Name.Split('-').FirstOrDefault();
        }

        protected void AddTestLearners(List<Training> training)
        {
            training.ForEach(ilrLearner =>
            {
                var learner = TestSession.Learners.FirstOrDefault(l => l.LearnerIdentifier == ilrLearner.LearnerId);
                if (learner == null)
                {
                    learner = TestSession.GenerateLearner();
                    learner.LearnerIdentifier = ilrLearner.LearnerId;
                    TestSession.Learners.Add(learner);
                }
                learner.Course.AimSeqNumber = (short)ilrLearner.AimSequenceNumber;
                learner.Course.StandardCode = ilrLearner.StandardCode;
                learner.Course.FundingLineType = ilrLearner.FundingLineType;
                learner.Course.LearnAimRef = ilrLearner.AimReference;
                learner.Course.CompletionStatus = ilrLearner.CompletionStatus;
                learner.Course.ProgrammeType = ilrLearner.ProgrammeType;
                learner.Course.FrameworkCode = ilrLearner.FrameworkCode;
                learner.Course.PathwayCode = ilrLearner.PathwayCode;
            });
        }

        protected List<PaymentModel> CreatePayments(ProviderPayment providerPayment, Training learnerTraining, long jobId, DateTime submissionTime, OnProgrammeEarning earning)
        {
            var list = new List<PaymentModel>();
            if (providerPayment.SfaFullyFundedPayments > 0)
                list.Add(CreatePaymentModel(providerPayment, learnerTraining, jobId, submissionTime, earning, providerPayment.SfaFullyFundedPayments, FundingSourceType.FullyFundedSfa));

            if (providerPayment.EmployerCoFundedPayments > 0)
                list.Add(CreatePaymentModel(providerPayment, learnerTraining, jobId, submissionTime, earning, providerPayment.EmployerCoFundedPayments, FundingSourceType.CoInvestedEmployer));

            if (providerPayment.SfaCoFundedPayments > 0)
                list.Add(CreatePaymentModel(providerPayment, learnerTraining, jobId, submissionTime, earning, providerPayment.SfaCoFundedPayments, FundingSourceType.CoInvestedSfa));

            return list;
        }

        private PaymentModel CreatePaymentModel(ProviderPayment providerPayment, Training learnerTraining, long jobId, 
            DateTime submissionTime, OnProgrammeEarning earning, decimal amount, FundingSourceType fundingSourceType)
        {
            var paymentModel = new PaymentModel
            {
                CollectionPeriod = providerPayment.CollectionPeriod.ToDate().ToCalendarPeriod(),
                DeliveryPeriod = providerPayment.DeliveryPeriod.ToDate().ToCalendarPeriod(),
                Ukprn = TestSession.Ukprn,
                JobId = jobId,
                SfaContributionPercentage = (earning.SfaContributionPercentage ?? learnerTraining.SfaContributionPercentage).ToPercent(),
                TransactionType = providerPayment.TransactionType,
                ContractType = learnerTraining.ContractType,
                PriceEpisodeIdentifier = "pe-1",
                FundingSource = fundingSourceType,
                LearnerReferenceNumber = TestSession.GetLearner(learnerTraining.LearnerId).LearnRefNumber,
                LearningAimReference = learnerTraining.AimReference,
                IlrSubmissionDateTime = submissionTime,
                ExternalId = Guid.NewGuid(),
                Amount = amount,
                LearningAimFundingLineType = earning.FundingLineType ?? learnerTraining.FundingLineType,
                LearnerUln = TestSession.Learner.Uln
            };
            return paymentModel;
        }

        protected void PopulateLearnerIncentives(FM36Learner learner, Training training,
            List<IncentiveEarning> incentives)
        {
           


        }

        protected void PopulateLearnerEarnings(FM36Learner learner, Training training, List<OnProgrammeEarning> earnings,
            List<IncentiveEarning> incentives)
        {

            var learningValues = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeOnProgPayment",
            };
            var completionEarnings = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeCompletionPayment",
            };
            var balancingEarnings = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeBalancePayment",
            };

            earnings.ForEach(earning =>
            {
                var period = earning.DeliveryPeriod.ToDate().ToCalendarPeriod().Period;
                SetPeriodValue(period, learningValues, earning.OnProgramme);
                SetPeriodValue(period, completionEarnings, earning.Completion);
                SetPeriodValue(period, balancingEarnings, earning.Balancing);
            });

            var first16To18EmployerIncentive = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeFirstEmp1618Pay",
            };
            var first16To18ProviderIncentive = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeFirstProv1618Pay",
            };
            var second16To18EmployerIncentive = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeSecondEmp1618Pay",
            };
            var second16To18ProviderIncentive = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeSecondProv1618Pay",
            };
            var onProgramme16To18FrameworkUplift = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeApplic1618FrameworkUpliftOnProgPayment",
            };
            var completion16To18FrameworkUplift = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeApplic1618FrameworkUpliftCompletionPayment",
            };
            var balancing16To18FrameworkUplift = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeApplic1618FrameworkUpliftBalancing",
            };
            var firstDisadvantagePayment = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeFirstDisadvantagePayment",
            };
            var secondDisadvantagePayment = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeSecondDisadvantagePayment",
            };
            var learningSupport = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeLSFCash",
            };

            incentives.ForEach(incentive =>
            {
                var period = incentive.DeliveryPeriod.ToDate().ToCalendarPeriod().Period;
                SetPeriodValue(period, first16To18EmployerIncentive, incentive.First16To18EmployerIncentive);
                SetPeriodValue(period, first16To18ProviderIncentive, incentive.First16To18ProviderIncentive);
                SetPeriodValue(period, second16To18EmployerIncentive, incentive.Second16To18EmployerIncentive);
                SetPeriodValue(period, second16To18ProviderIncentive, incentive.Second16To18ProviderIncentive);
                SetPeriodValue(period, onProgramme16To18FrameworkUplift, incentive.OnProgramme16To18FrameworkUplift);
                SetPeriodValue(period, completion16To18FrameworkUplift, incentive.Completion16To18FrameworkUplift);
                SetPeriodValue(period, balancing16To18FrameworkUplift, incentive.Balancing16To18FrameworkUplift);
                SetPeriodValue(period, firstDisadvantagePayment, incentive.FirstDisadvantagePayment);
                SetPeriodValue(period, secondDisadvantagePayment, incentive.SecondDisadvantagePayment);
                SetPeriodValue(period, learningSupport, incentive.LearningSupport);
            });

            learner.PriceEpisodes = GetPriceEpisodes(
                training,
                learningValues,
                completionEarnings,
                balancingEarnings,
                first16To18EmployerIncentive,
                first16To18ProviderIncentive,
                second16To18EmployerIncentive,
                second16To18ProviderIncentive,
                onProgramme16To18FrameworkUplift,
                completion16To18FrameworkUplift,
                balancing16To18FrameworkUplift,
                firstDisadvantagePayment,
                secondDisadvantagePayment,
                learningSupport,
                CurrentPriceEpisodes,
                earnings,
                CollectionYear);

            learner.LearningDeliveries = new List<LearningDelivery>(new[]
            {
                new LearningDelivery
                {
                    AimSeqNumber = training.AimSequenceNumber,
                    LearningDeliveryValues = new LearningDeliveryValues
                    {
                        LearnAimRef = training.AimReference,
                        LearnDelInitialFundLineType = training.FundingLineType,
                        StdCode = training.StandardCode,
                        FworkCode = training.FrameworkCode,
                        ProgType = training.ProgrammeType,
                        PwayCode = training.PathwayCode,
                    }
                }
            });
        }

        private static List<PriceEpisode> GetPriceEpisodes(
            Training learnerEarnings, 
            PriceEpisodePeriodisedValues learningValues, 
            PriceEpisodePeriodisedValues completionEarnings, 
            PriceEpisodePeriodisedValues balancingEarnings, 
            PriceEpisodePeriodisedValues first16To18EmployerIncentive, 
            PriceEpisodePeriodisedValues first16To18ProviderIncentive, 
            PriceEpisodePeriodisedValues second16To18EmployerIncentive, 
            PriceEpisodePeriodisedValues second16To18ProviderIncentive, 
            PriceEpisodePeriodisedValues onProgramme16To18FrameworkUplift, 
            PriceEpisodePeriodisedValues completion16To18FrameworkUplift, 
            PriceEpisodePeriodisedValues balancing16To18FrameworkUplift, 
            PriceEpisodePeriodisedValues firstDisadvantagePayment, 
            PriceEpisodePeriodisedValues secondDisadvantagePayment, 
            PriceEpisodePeriodisedValues learningSupport, 
            List<Price> priceEpisodes, 
            List<OnProgrammeEarning> earnings,
            string collectionYear)
        {
            if (priceEpisodes == null)
            {
                priceEpisodes = new List<Price>
                {
                    new Price
                    {
                        TotalAssessmentPrice = learnerEarnings.TotalAssessmentPrice,
                        TotalTrainingPrice = learnerEarnings.TotalTrainingPrice,
                        TotalAssessmentPriceEffectiveDate = learnerEarnings.StartDate,
                        TotalTrainingPriceEffectiveDate = learnerEarnings.StartDate
                    }
                };
            }

            var result = new List<PriceEpisode>();

            for (var i = 0; i < priceEpisodes.Count; i++)
            {
                var episode = priceEpisodes[i];
                DateTime? actualEndDate = null;

                if (i < priceEpisodes.Count - 1)
                {
                    var nextEpisodeStartDate = priceEpisodes[i + 1].TotalTrainingPriceEffectiveDate.ToDate();
                    actualEndDate = nextEpisodeStartDate.AddMonths(-1);
                }

                var episodeStart = episode.TotalTrainingPriceEffectiveDate.ToCalendarPeriod();
                var episodeLastPeriod = actualEndDate?.ToCalendarPeriod().Period ?? 12;
                var episodeAcademicYear = int.Parse(episodeStart.Name.Substring(0, 4));
                var firstEarningForPriceEpisode = earnings.First(e =>
                {
                    var deliveryPeriod = e.DeliveryCalendarPeriod;
                    var earningAcademicYear = int.Parse(deliveryPeriod.Name.Substring(0, 4));
                    return earningAcademicYear * 100 + deliveryPeriod.Period >= episodeAcademicYear * 100 + episodeStart.Period;
                });

                var sfaPercent = (firstEarningForPriceEpisode.SfaContributionPercentage ?? learnerEarnings.SfaContributionPercentage).ToPercent();

                var priceEpisode = new PriceEpisode
                {
                    PriceEpisodeIdentifier = "pe-" + (i + 1),
                    PriceEpisodeValues = new PriceEpisodeValues
                    {
                        EpisodeStartDate = episode.TotalTrainingPriceEffectiveDate.ToDate(),
                        PriceEpisodeCompletionElement = learnerEarnings.CompletionAmount,
                        PriceEpisodeCompleted = learnerEarnings.CompletionStatus.Equals("completed", StringComparison.OrdinalIgnoreCase),
                        TNP1 = episode.TotalTrainingPrice,
                        TNP2 = episode.TotalAssessmentPrice,
                        PriceEpisodeInstalmentValue = learnerEarnings.InstallmentAmount,
                        PriceEpisodePlannedInstalments = learnerEarnings.NumberOfInstallments,
                        PriceEpisodeActualInstalments = learnerEarnings.ActualInstallments,
                        PriceEpisodeBalancePayment = learnerEarnings.BalancingPayment,
                        PriceEpisodeFundLineType = firstEarningForPriceEpisode.FundingLineType ?? learnerEarnings.FundingLineType,
                        PriceEpisodeBalanceValue = learnerEarnings.BalancingPayment,
                        PriceEpisodeCompletionPayment = learnerEarnings.CompletionAmount,
                        PriceEpisodeContractType = learnerEarnings.ContractType.ToString("G"),
                        PriceEpisodeOnProgPayment = learnerEarnings.InstallmentAmount,
                        PriceEpisodePlannedEndDate = episode.TotalTrainingPriceEffectiveDate.ToDate().AddMonths(learnerEarnings.NumberOfInstallments),
                        PriceEpisodeActualEndDate = actualEndDate,
                        PriceEpisodeSFAContribPct = sfaPercent,
                        PriceEpisodeAimSeqNumber = learnerEarnings.AimSequenceNumber,
                    },
                    PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>()
                };

                PriceEpisodePeriodisedValues episodeLearningValues;
                PriceEpisodePeriodisedValues episodeCompletionEarnings;
                PriceEpisodePeriodisedValues episodeBalancingEarnings;
                PriceEpisodePeriodisedValues episodeFirst16To18EmployerIncentive;
                PriceEpisodePeriodisedValues episodeFirst16To18ProviderIncentive;
                PriceEpisodePeriodisedValues episodeSecond16To18EmployerIncentive;
                PriceEpisodePeriodisedValues episodeSecond16To18ProviderIncentive;
                PriceEpisodePeriodisedValues episodeOnProgramme16To18FrameworkUplift;
                PriceEpisodePeriodisedValues episodeCompletion16To18FrameworkUplift;
                PriceEpisodePeriodisedValues episodeBalancing16To18FrameworkUplift;
                PriceEpisodePeriodisedValues episodeFirstDisadvantagePayment;
                PriceEpisodePeriodisedValues episodeSecondDisadvantagePayment;
                PriceEpisodePeriodisedValues episodeLearningSupport;

                if ((episodeStart.Period > 1 && episodeStart.GetCollectionYear() == collectionYear) || episodeLastPeriod < 12)
                {
                    episodeLearningValues = new PriceEpisodePeriodisedValues {AttributeName = learningValues.AttributeName};
                    episodeCompletionEarnings = new PriceEpisodePeriodisedValues { AttributeName = completionEarnings.AttributeName };
                    episodeBalancingEarnings = new PriceEpisodePeriodisedValues { AttributeName = balancingEarnings.AttributeName };
                    episodeFirst16To18EmployerIncentive = new PriceEpisodePeriodisedValues { AttributeName = first16To18EmployerIncentive.AttributeName };
                    episodeFirst16To18ProviderIncentive = new PriceEpisodePeriodisedValues { AttributeName = first16To18ProviderIncentive.AttributeName };
                    episodeSecond16To18EmployerIncentive = new PriceEpisodePeriodisedValues { AttributeName = second16To18EmployerIncentive.AttributeName };
                    episodeSecond16To18ProviderIncentive = new PriceEpisodePeriodisedValues { AttributeName = second16To18ProviderIncentive.AttributeName };
                    episodeOnProgramme16To18FrameworkUplift = new PriceEpisodePeriodisedValues { AttributeName = onProgramme16To18FrameworkUplift.AttributeName };
                    episodeCompletion16To18FrameworkUplift = new PriceEpisodePeriodisedValues { AttributeName = completion16To18FrameworkUplift.AttributeName };
                    episodeBalancing16To18FrameworkUplift = new PriceEpisodePeriodisedValues { AttributeName = balancing16To18FrameworkUplift.AttributeName };
                    episodeFirstDisadvantagePayment = new PriceEpisodePeriodisedValues { AttributeName = firstDisadvantagePayment.AttributeName };
                    episodeSecondDisadvantagePayment = new PriceEpisodePeriodisedValues { AttributeName = secondDisadvantagePayment.AttributeName };
                    episodeLearningSupport = new PriceEpisodePeriodisedValues { AttributeName = learningSupport.AttributeName };

                    for (var p = 1; p < 13; p++)
                    {
                        var propertyInfo = typeof(PriceEpisodePeriodisedValues).GetProperty("Period" + p);

                        decimal? learningValue = 0,
                            completionValue = 0,
                            balancingValue = 0,
                            first16To18EmployerIncentiveValue = 0,
                            first16To18ProviderIncentiveValue = 0,
                            second16To18EmployerIncentiveValue = 0,
                            second16To18ProviderIncentiveValue = 0,
                            onProgramme16To18FrameworkUpliftValue = 0,
                            completion16To18FrameworkUpliftValue = 0,
                            balancing16To18FrameworkUpliftValue = 0,
                            firstDisadvantagePaymentValue = 0,
                            secondDisadvantagePaymentValue = 0,
                            learningSupportValue = 0;

                        if (p >= episodeStart.Period && p <= episodeLastPeriod)
                        {
                            learningValue = (decimal?)propertyInfo.GetValue(learningValues);
                            completionValue = (decimal?)propertyInfo.GetValue(completionEarnings);
                            balancingValue = (decimal?)propertyInfo.GetValue(balancingEarnings);

                            first16To18EmployerIncentiveValue = (decimal?)propertyInfo.GetValue(episodeFirst16To18EmployerIncentive);
                            first16To18ProviderIncentiveValue = (decimal?)propertyInfo.GetValue(episodeFirst16To18ProviderIncentive);
                            second16To18EmployerIncentiveValue = (decimal?)propertyInfo.GetValue(episodeSecond16To18EmployerIncentive);
                            second16To18ProviderIncentiveValue = (decimal?)propertyInfo.GetValue(episodeSecond16To18ProviderIncentive);
                            onProgramme16To18FrameworkUpliftValue = (decimal?)propertyInfo.GetValue(episodeOnProgramme16To18FrameworkUplift);
                            completion16To18FrameworkUpliftValue = (decimal?)propertyInfo.GetValue(episodeCompletion16To18FrameworkUplift);
                            balancing16To18FrameworkUpliftValue = (decimal?)propertyInfo.GetValue(episodeBalancing16To18FrameworkUplift);
                            firstDisadvantagePaymentValue = (decimal?)propertyInfo.GetValue(episodeFirstDisadvantagePayment);
                            secondDisadvantagePaymentValue = (decimal?)propertyInfo.GetValue(episodeSecondDisadvantagePayment);
                            learningSupportValue = (decimal?)propertyInfo.GetValue(episodeLearningSupport);
                        }

                        propertyInfo.SetValue(episodeLearningValues, learningValue);
                        propertyInfo.SetValue(episodeCompletionEarnings, completionValue);
                        propertyInfo.SetValue(episodeBalancingEarnings, balancingValue);

                        propertyInfo.SetValue(episodeFirst16To18EmployerIncentive, first16To18EmployerIncentiveValue);
                        propertyInfo.SetValue(episodeFirst16To18ProviderIncentive, first16To18ProviderIncentiveValue);
                        propertyInfo.SetValue(episodeSecond16To18EmployerIncentive, second16To18EmployerIncentiveValue);
                        propertyInfo.SetValue(episodeSecond16To18ProviderIncentive, second16To18ProviderIncentiveValue);
                        propertyInfo.SetValue(episodeOnProgramme16To18FrameworkUplift, onProgramme16To18FrameworkUpliftValue);
                        propertyInfo.SetValue(episodeCompletion16To18FrameworkUplift, completion16To18FrameworkUpliftValue);
                        propertyInfo.SetValue(episodeBalancing16To18FrameworkUplift, balancing16To18FrameworkUpliftValue);
                        propertyInfo.SetValue(episodeFirstDisadvantagePayment, firstDisadvantagePaymentValue);
                        propertyInfo.SetValue(episodeSecondDisadvantagePayment, secondDisadvantagePaymentValue);
                        propertyInfo.SetValue(episodeLearningSupport, learningSupportValue);
                    }
                }
                else
                {
                    episodeLearningValues = learningValues;
                    episodeCompletionEarnings = completionEarnings;
                    episodeBalancingEarnings = balancingEarnings;

                    episodeFirst16To18EmployerIncentive = first16To18EmployerIncentive;
                    episodeFirst16To18ProviderIncentive = first16To18ProviderIncentive;
                    episodeSecond16To18EmployerIncentive = second16To18EmployerIncentive;
                    episodeSecond16To18ProviderIncentive = second16To18ProviderIncentive;
                    episodeOnProgramme16To18FrameworkUplift = onProgramme16To18FrameworkUplift;
                    episodeCompletion16To18FrameworkUplift = completion16To18FrameworkUplift;
                    episodeBalancing16To18FrameworkUplift = balancing16To18FrameworkUplift;
                    episodeFirstDisadvantagePayment = firstDisadvantagePayment;
                    episodeSecondDisadvantagePayment = secondDisadvantagePayment;
                    episodeLearningSupport = learningSupport;
                }

                priceEpisode.PriceEpisodePeriodisedValues.Add(episodeLearningValues);
                priceEpisode.PriceEpisodePeriodisedValues.Add(episodeCompletionEarnings);
                priceEpisode.PriceEpisodePeriodisedValues.Add(episodeBalancingEarnings);

                if (!NoValuesSet(episodeFirst16To18EmployerIncentive)) priceEpisode.PriceEpisodePeriodisedValues.Add(episodeFirst16To18EmployerIncentive);
                if (!NoValuesSet(episodeFirst16To18ProviderIncentive)) priceEpisode.PriceEpisodePeriodisedValues.Add(episodeFirst16To18ProviderIncentive);
                if (!NoValuesSet(episodeSecond16To18EmployerIncentive)) priceEpisode.PriceEpisodePeriodisedValues.Add(episodeSecond16To18EmployerIncentive);
                if (!NoValuesSet(episodeSecond16To18ProviderIncentive)) priceEpisode.PriceEpisodePeriodisedValues.Add(episodeSecond16To18ProviderIncentive);
                if (!NoValuesSet(episodeOnProgramme16To18FrameworkUplift)) priceEpisode.PriceEpisodePeriodisedValues.Add(episodeOnProgramme16To18FrameworkUplift);
                if (!NoValuesSet(episodeCompletion16To18FrameworkUplift)) priceEpisode.PriceEpisodePeriodisedValues.Add(episodeCompletion16To18FrameworkUplift);
                if (!NoValuesSet(episodeBalancing16To18FrameworkUplift)) priceEpisode.PriceEpisodePeriodisedValues.Add(episodeBalancing16To18FrameworkUplift);
                if (!NoValuesSet(episodeFirstDisadvantagePayment)) priceEpisode.PriceEpisodePeriodisedValues.Add(episodeFirstDisadvantagePayment);
                if (!NoValuesSet(episodeSecondDisadvantagePayment)) priceEpisode.PriceEpisodePeriodisedValues.Add(episodeSecondDisadvantagePayment);
                if (!NoValuesSet(episodeLearningSupport)) priceEpisode.PriceEpisodePeriodisedValues.Add(episodeLearningSupport);

                result.Add(priceEpisode);
            }

            return result;
        }

        private static bool NoValuesSet(PriceEpisodePeriodisedValues values)
        {
            return NullableDecimalNotSet(values.Period1) &&
                   NullableDecimalNotSet(values.Period2) &&
                   NullableDecimalNotSet(values.Period3) &&
                   NullableDecimalNotSet(values.Period4) &&
                   NullableDecimalNotSet(values.Period5) &&
                   NullableDecimalNotSet(values.Period6) &&
                   NullableDecimalNotSet(values.Period7) &&
                   NullableDecimalNotSet(values.Period8) &&
                   NullableDecimalNotSet(values.Period9) &&
                   NullableDecimalNotSet(values.Period10) &&
                   NullableDecimalNotSet(values.Period11) &&
                   NullableDecimalNotSet(values.Period12);
        }

        private static bool NullableDecimalNotSet(decimal? value)
        {
            return !value.HasValue || value.Value == decimal.Zero;
        }

        private static void SetPeriodValue(int period, PriceEpisodePeriodisedValues periodisedValues, decimal amount)
        {
            var periodProperty = periodisedValues.GetType().GetProperty("Period" + period);
            periodProperty?.SetValue(periodisedValues, amount);
        }
    }
}