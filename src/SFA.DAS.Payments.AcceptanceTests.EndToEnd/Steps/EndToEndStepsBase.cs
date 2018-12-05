using System;
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
using TechTalk.SpecFlow.Assist;
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

        //protected List<OnProgrammeEarning> CurrentEarnings
        //{
        //    get => Get<List<OnProgrammeEarning>>("current_earnings");
        //    set => Set(value, "current_earnings");
        //}

        protected List<Earning> PreviousEarnings
        {
            get => Get<List<Earning>>("previous_earnings");
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

        protected List<PaymentModel> CreatePayments(ProviderPayment providerPayment, Training learnerTraining, long jobId, DateTime submissionTime, Earning earning)
        {
            return new List<PaymentModel>
            {
                new PaymentModel
                {
                    CollectionPeriod = providerPayment.CollectionPeriod.ToDate().ToCalendarPeriod(),
                    DeliveryPeriod = providerPayment.DeliveryPeriod.ToDate().ToCalendarPeriod(),
                    Ukprn = TestSession.Ukprn,
                    JobId = jobId,
                    SfaContributionPercentage = (earning.SfaContributionPercentage ?? learnerTraining.SfaContributionPercentage).ToPercent(),
                    TransactionType = providerPayment.TransactionType,
                    ContractType = learnerTraining.ContractType,
                    PriceEpisodeIdentifier = "pe-1",
                    FundingSource = FundingSourceType.CoInvestedSfa,
                    LearningAimPathwayCode = TestSession.Learner.Course.PathwayCode,
                    LearnerReferenceNumber = TestSession.GetLearner(learnerTraining.LearnerId).LearnRefNumber,
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
                    SfaContributionPercentage = (earning.SfaContributionPercentage ?? learnerTraining.SfaContributionPercentage).ToPercent(),
                    TransactionType = providerPayment.TransactionType,
                    ContractType = learnerTraining.ContractType,
                    PriceEpisodeIdentifier = "pe-1",
                    FundingSource = FundingSourceType.CoInvestedEmployer,
                    LearningAimPathwayCode = TestSession.Learner.Course.PathwayCode,
                    LearnerReferenceNumber = TestSession.GetLearner(learnerTraining.LearnerId).LearnRefNumber,
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

        protected void PopulateLearner(FM36Learner learner, Training training, List<Earning> earnings)
        {
            var values = new List<PriceEpisodePeriodisedValues>();

            foreach (var earning in earnings)
            {
                var period = earning.DeliveryCalendarPeriod.Period;
                foreach (var earningValue in earning.Values)
                {
                    var periodisedValues = values.SingleOrDefault(v => v.AttributeName == earningValue.Key.ToAttributeName());
                    if (periodisedValues == null)
                    {
                        periodisedValues = new PriceEpisodePeriodisedValues {AttributeName = earningValue.Key.ToAttributeName()};
                        values.Add(periodisedValues);
                    }

                    SetPeriodValue(period, periodisedValues, earningValue.Value);
                }
            }

            learner.PriceEpisodes = GetPriceEpisodes(training, values, CurrentPriceEpisodes, earnings, CollectionYear);

            var course = TestSession.Learners.First(x => x.LearnRefNumber == learner.LearnRefNumber).Course;

            learner.LearningDeliveries = new List<LearningDelivery>(new[]
            {
                new LearningDelivery
                {
                    AimSeqNumber = training.AimSequenceNumber,
                    LearningDeliveryValues = new LearningDeliveryValues
                    {
                        LearnAimRef = training.AimReference,
                        LearnDelInitialFundLineType = training.FundingLineType,
                        StdCode = course.StandardCode,
                        FworkCode = course.FrameworkCode,
                        ProgType = course.ProgrammeType,
                        PwayCode = course.PathwayCode,
                    }
                }
            });
        }

        private static List<PriceEpisode> GetPriceEpisodes(
            Training learnerEarnings, 
            List<PriceEpisodePeriodisedValues> periodisedValues,
            List<Price> priceEpisodes,
            List<Earning> earnings, 
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
                        PriceEpisodeFundLineType = learnerEarnings.FundingLineType,
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


                foreach (var currentValues in periodisedValues)
                {
                    PriceEpisodePeriodisedValues newValues;

                    // price eipsodes not covering the whole year are likely to be one of many, copy values only for current episode, set zero for others
                    if ((episodeStart.Period > 1 && episodeStart.GetCollectionYear() == collectionYear) || episodeLastPeriod < 12)
                    {
                        newValues = new PriceEpisodePeriodisedValues {AttributeName = currentValues.AttributeName};

                        for (var p = 1; p < 13; p++)
                        {
                            var amount = p >= episodeStart.Period && p <= episodeLastPeriod ? currentValues.GetValue(p) : 0;
                            newValues.SetValue(p, amount);
                        }
                    }
                    else // put everything as is for previous years
                    {
                        newValues = currentValues;
                    }

                    priceEpisode.PriceEpisodePeriodisedValues.Add(newValues);
                }

                result.Add(priceEpisode);
            }

            return result;
        }

        private static void SetPeriodValue(int period, PriceEpisodePeriodisedValues periodisedValues, decimal amount)
        {
            var periodProperty = periodisedValues.GetType().GetProperty("Period" + period);
            periodProperty?.SetValue(periodisedValues, amount);
        }

        
        protected static List<Earning> CreateEarnings(Table table)
        {
            var earnings = table.CreateSet<Earning>().ToList();
            foreach (var tableRow in table.Rows)
            {
                var earning = earnings.Single(e =>
                {
                    var result = e.DeliveryPeriod == tableRow["Delivery Period"];

                    if (tableRow.TryGetValue("Learner ID", out var learnerId) || tableRow.TryGetValue("LearnerId", out learnerId))
                        result |= e.LearnerId == learnerId;

                    return result;
                });
                foreach (var headerCell in table.Header)
                {
                    var name = headerCell == "On-Programme" ? "Learning" : headerCell.Replace(" ", null).Replace("-", null);

                    if (!Enum.TryParse<TransactionType>(name, true, out var transactionType))
                        continue;

                    if (!decimal.TryParse(tableRow[headerCell], out var amount))
                        continue;

                    earning.Values.Add(transactionType, amount);
                }
            }

            return earnings;
        }
    }
}