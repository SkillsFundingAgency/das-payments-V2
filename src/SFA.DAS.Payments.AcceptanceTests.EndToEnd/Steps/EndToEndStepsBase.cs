using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;
using PriceEpisode = SFA.DAS.Payments.Model.Core.PriceEpisode;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public abstract class EndToEndStepsBase: StepsBase
    {
        protected DcHelper DcHelper => Get<DcHelper>();
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
            var period = collectionPeriod.ToDate().ToCalendarPeriod();
            Console.WriteLine($"Current collection period is: {period.Name}.");
            CurrentCollectionPeriod = period;
            CollectionPeriod = CurrentCollectionPeriod.Period;
            CollectionYear = CurrentCollectionPeriod.Name.Split('-').FirstOrDefault();
        }

        protected List<PaymentModel> CreatePayments(ProviderPayment providerPayment, Training learnerTraining, long jobId, DateTime submissionTime)
        {
            return new List<PaymentModel>
            {
                new PaymentModel
                {
                    CollectionPeriod = providerPayment.CollectionPeriod.ToDate().ToCalendarPeriod(),
                    DeliveryPeriod = providerPayment.DeliveryPeriod.ToDate().ToCalendarPeriod(),
                    Ukprn = TestSession.Ukprn,
                    JobId = jobId,
                    SfaContributionPercentage = learnerTraining.SfaContributionPercentage,
                    TransactionType = providerPayment.TransactionType,
                    ContractType = learnerTraining.ContractType,
                    PriceEpisodeIdentifier = "pe-1",
                    FundingSource = FundingSourceType.CoInvestedSfa,
                    LearningAimPathwayCode = TestSession.Learner.Course.PathwayCode,
                    LearnerReferenceNumber = TestSession.Learner.LearnRefNumber,
                    LearningAimReference = learnerTraining.AimReference,
                    LearningAimStandardCode = TestSession.Learner.Course.StandardCode,
                    IlrSubmissionDateTime = submissionTime,
                    ExternalId = Guid.NewGuid(),
                    Amount = providerPayment.SfaCoFundedPayments,
                    LearningAimFundingLineType = learnerTraining.FundingLineType,
                    LearnerUln = TestSession.Learner.Uln,
                    LearningAimFrameworkCode = TestSession.Learner.Course.FrameworkCode,
                    LearningAimProgrammeType = learnerTraining.ProgrammeType
                },
                new PaymentModel
                {
                    CollectionPeriod = providerPayment.CollectionPeriod.ToDate().ToCalendarPeriod(),
                    DeliveryPeriod = providerPayment.DeliveryPeriod.ToDate().ToCalendarPeriod(),
                    Ukprn = TestSession.Ukprn,
                    JobId = jobId,
                    SfaContributionPercentage = learnerTraining.SfaContributionPercentage,
                    TransactionType = providerPayment.TransactionType,
                    ContractType = learnerTraining.ContractType,
                    PriceEpisodeIdentifier = "pe-1",
                    FundingSource = FundingSourceType.CoInvestedEmployer,
                    LearningAimPathwayCode = TestSession.Learner.Course.PathwayCode,
                    LearnerReferenceNumber = TestSession.Learner.LearnRefNumber,
                    LearningAimReference = learnerTraining.AimReference,
                    LearningAimStandardCode = TestSession.Learner.Course.StandardCode,
                    IlrSubmissionDateTime = submissionTime,
                    ExternalId = Guid.NewGuid(),
                    Amount = providerPayment.EmployerCoFundedPayments,
                    LearningAimFundingLineType = learnerTraining.FundingLineType,
                    LearnerUln = TestSession.Learner.Uln,
                    LearningAimFrameworkCode = TestSession.Learner.Course.FrameworkCode,
                    LearningAimProgrammeType = learnerTraining.ProgrammeType
                }
            };
        }

        protected void PopulateLearner(FM36Learner learner, Training learnerEarnings)
        {
            learner.LearnRefNumber = TestSession.Learner.LearnRefNumber;
            var priceEpisode = new ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode
            {
                PriceEpisodeIdentifier = "pe-1",
                PriceEpisodeValues = new PriceEpisodeValues
                {
                    EpisodeStartDate = learnerEarnings.StartDate.ToDate(),
                    PriceEpisodeCompletionElement = learnerEarnings.CompletionAmount,
                    PriceEpisodeCompleted = learnerEarnings.CompletionStatus.Equals("completed", StringComparison.OrdinalIgnoreCase),
                    TNP1 = learnerEarnings.TotalTrainingPrice,
                    TNP2 = learnerEarnings.TotalAssessmentPrice,
                    PriceEpisodeInstalmentValue = learnerEarnings.InstallmentAmount,
                    PriceEpisodePlannedInstalments = learnerEarnings.NumberOfInstallments,
                    PriceEpisodeActualInstalments = learnerEarnings.ActualInstallments,
                    PriceEpisodeBalancePayment = learnerEarnings.BalancingPayment,
                    PriceEpisodeFundLineType = learnerEarnings.FundingLineType,
                    PriceEpisodeBalanceValue = learnerEarnings.BalancingPayment,
                    PriceEpisodeCompletionPayment = learnerEarnings.CompletionAmount,
                    PriceEpisodeContractType = learnerEarnings.ContractType.ToString("G"),
                    PriceEpisodeOnProgPayment = learnerEarnings.InstallmentAmount,
                    PriceEpisodePlannedEndDate = learnerEarnings.StartDate.ToDate().AddMonths(learnerEarnings.NumberOfInstallments),
                    PriceEpisodeSFAContribPct = learnerEarnings.SfaContributionPercentage,
                },
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>()
            };

            var firstPeriodNumber = (int)(learnerEarnings.StartDate.ToDate().Subtract("start of academic year".ToDate()).TotalDays / 30) + 1;
            var lastPeriodNumber = firstPeriodNumber + learnerEarnings.NumberOfInstallments - 1;

            var learningValues = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeOnProgPayment",
            };

            PropertyInfo periodProperty;
            for (var i = firstPeriodNumber; i <= lastPeriodNumber; i++)
            {
                periodProperty = learningValues.GetType().GetProperty("Period" + i);
                periodProperty?.SetValue(learningValues, learnerEarnings.InstallmentAmount);
            }

            priceEpisode.PriceEpisodePeriodisedValues.Add(learningValues);

            learner.PriceEpisodes = new List<ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode>(new[] { priceEpisode });


            var completionEarnings = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeCompletionPayment",
            };

            periodProperty = completionEarnings.GetType().GetProperty("Period" + lastPeriodNumber);
            periodProperty?.SetValue(completionEarnings, learnerEarnings.CompletionAmount);
            priceEpisode.PriceEpisodePeriodisedValues.Add(completionEarnings);

            var balancingEarnings = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeBalancePayment",
            };

            periodProperty = balancingEarnings.GetType().GetProperty("Period" + lastPeriodNumber);
            periodProperty?.SetValue(balancingEarnings, learnerEarnings.BalancingPayment);
            priceEpisode.PriceEpisodePeriodisedValues.Add(balancingEarnings);

            learner.LearningDeliveries = new List<LearningDelivery>(new[]
            {
                new LearningDelivery
                {
                    AimSeqNumber = learnerEarnings.AimSequenceNumber,
                    LearningDeliveryValues = new LearningDeliveryValues
                    {
                        LearnAimRef = learnerEarnings.AimReference,
                    }
                }
            });
        }

    }
}