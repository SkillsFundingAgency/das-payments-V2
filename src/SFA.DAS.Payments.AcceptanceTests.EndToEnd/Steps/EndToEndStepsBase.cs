using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public abstract class EndToEndStepsBase : StepsBase
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
                    //LearningAimPathwayCode = TestSession.Learner.Course.PathwayCode,
                    LearnerReferenceNumber = TestSession.Learner.LearnRefNumber,
                    LearningAimReference = learnerTraining.AimReference,
                    //LearningAimStandardCode = TestSession.Learner.Course.StandardCode,
                    IlrSubmissionDateTime = submissionTime,
                    ExternalId = Guid.NewGuid(),
                    Amount = providerPayment.SfaCoFundedPayments,
                    LearningAimFundingLineType = learnerTraining.FundingLineType,
                    LearnerUln = TestSession.Learner.Uln,
                    //LearningAimFrameworkCode = TestSession.Learner.Course.FrameworkCode,
                    //LearningAimProgrammeType = learnerTraining.ProgrammeType
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
                    //LearningAimPathwayCode = TestSession.Learner.Course.PathwayCode,
                    LearnerReferenceNumber = TestSession.Learner.LearnRefNumber,
                    LearningAimReference = learnerTraining.AimReference,
                    //LearningAimStandardCode = TestSession.Learner.Course.StandardCode,
                    IlrSubmissionDateTime = submissionTime,
                    ExternalId = Guid.NewGuid(),
                    Amount = providerPayment.EmployerCoFundedPayments,
                    LearningAimFundingLineType = learnerTraining.FundingLineType,
                    LearnerUln = TestSession.Learner.Uln,
                    //LearningAimFrameworkCode = TestSession.Learner.Course.FrameworkCode,
                    //LearningAimProgrammeType = learnerTraining.ProgrammeType
                }
            };
        }

        protected void PopulateLearner(FM36Learner learner, Training learnerEarnings, List<OnProgrammeEarning> earnings)
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

            learner.PriceEpisodes = new List<ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode>(new[] { priceEpisode });
            priceEpisode.PriceEpisodePeriodisedValues.Add(learningValues);
            priceEpisode.PriceEpisodePeriodisedValues.Add(completionEarnings);
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

        private void SetPeriodValue(int period, PriceEpisodePeriodisedValues periodisedValues, decimal amount)
        {
            var periodProperty = periodisedValues.GetType().GetProperty("Period" + period);
            periodProperty?.SetValue(periodisedValues, amount);
        }
    }
}