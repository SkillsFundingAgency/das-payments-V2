using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public abstract class EndToEndStepsBase : StepsBase
    {
        protected RequiredPaymentsCacheCleaner RequiredPaymentsCacheCleaner => Container.Resolve<RequiredPaymentsCacheCleaner>();

        private static readonly HashSet<long> AimsProcessedForJob = new HashSet<long>();

        protected DcHelper DcHelper => Get<DcHelper>();

        protected List<Price> CurrentPriceEpisodes
        {
            get => !Context.TryGetValue<List<Price>>(out var currentPriceEpisodes) ? null : currentPriceEpisodes;
            set => Set(value);
        }

        protected List<Training> CurrentIlr
        {
            get => !Context.TryGetValue<List<Training>>(out var currentIlr) ? null : currentIlr;
            set => Set(value);
        }

        protected List<Training> PreviousIlr
        {
            get => Get<List<Training>>("previous_training");
            set => Set(value, "previous_training");
        }

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

        protected void AddNewIlr(Table table)
        {
            var ilr = table.CreateSet<Training>().ToList();
            CurrentIlr = ilr;
            AddTestLearners(CurrentIlr);
        }

        protected void SetCollectionPeriod(string collectionPeriod)
        {
            Console.WriteLine($"Current collection period is: {collectionPeriod}.");
            var period = collectionPeriod.ToCalendarPeriod();
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
                if (ilrLearner.Uln != default(long))
                {
                    learner.Uln = ilrLearner.Uln;
                }
            });
        }

        protected void AddTestLearners(IEnumerable<Learner> learners)
        {
            foreach (var learner in learners)
            {
                var testLearner = TestSession.Learners
                    .FirstOrDefault(l => l.LearnerIdentifier == learner.LearnerIdentifier);
                if (testLearner == null)
                {
                    testLearner = TestSession.GenerateLearner();
                }
                testLearner.LearnerIdentifier = string.IsNullOrEmpty(learner.LearnerIdentifier) ? testLearner.LearnerIdentifier : learner.LearnerIdentifier;
                testLearner.Uln = learner.Uln == 0 ? testLearner.Uln : learner.Uln;
                testLearner.LearnRefNumber = string.IsNullOrEmpty(learner.LearnRefNumber) ? testLearner.LearnRefNumber : learner.LearnRefNumber;
            }
        }

        protected void AddTestAims(IEnumerable<Aim> aims)
        {
            if (TestSession.AtLeastOneScenarioCompleted)
            {
                return;
            }

            foreach (var aim in aims)
            {
                var learner = TestSession.Learners.FirstOrDefault(x => x.LearnerIdentifier == aim.LearnerId);
                if (learner == null)
                {
                    throw new Exception("There is an aim without a matching learner");
                }
                learner.Aims.Add(aim);
            }
        }

        protected List<PaymentModel> CreatePayments(ProviderPayment providerPayment, Training learnerTraining, long jobId, DateTime submissionTime, Earning earning)
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

        private PaymentModel CreatePaymentModel(ProviderPayment providerPayment, Training learnerTraining, long jobId, DateTime submissionTime, Earning earning, decimal amount, FundingSourceType fundingSourceType)
        {
            return new PaymentModel
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
                LearningAimPathwayCode = TestSession.Learner.Course.PathwayCode,
                LearnerReferenceNumber = TestSession.GetLearner(learnerTraining.LearnerId).LearnRefNumber,
                LearningAimReference = learnerTraining.AimReference,
                LearningAimStandardCode = TestSession.Learner.Course.StandardCode,
                IlrSubmissionDateTime = submissionTime,
                ExternalId = Guid.NewGuid(),
                Amount = amount,
                LearningAimFundingLineType = learnerTraining.FundingLineType,
                LearnerUln = providerPayment.Uln,
                LearningAimFrameworkCode = TestSession.Learner.Course.FrameworkCode,
                LearningAimProgrammeType = learnerTraining.ProgrammeType
            };
        }

        protected void PopulateLearner(FM36Learner learner, Learner testLearner, IList<Earning> earnings)
        {
            learner.LearnRefNumber = testLearner.LearnRefNumber;
            learner.ULN = testLearner.Uln;
            learner.PriceEpisodes = new List<PriceEpisode>();
            learner.LearningDeliveries = new List<LearningDelivery>();

            foreach (var aim in testLearner.Aims)
            {
                var aimPeriodisedValues = new List<PriceEpisodePeriodisedValues>();

                foreach (var earning in earnings.Where(e => e.AimSequenceNumber == aim.AimSequenceNumber))
                {
                    var period = earning.DeliveryCalendarPeriod.Period;
                    foreach (var earningValue in earning.Values)
                    {
                        var periodisedValues = aimPeriodisedValues.SingleOrDefault(v => v.AttributeName == earningValue.Key.ToAttributeName());
                        if (periodisedValues == null)
                        {
                            periodisedValues = new PriceEpisodePeriodisedValues { AttributeName = earningValue.Key.ToAttributeName() };
                            aimPeriodisedValues.Add(periodisedValues);
                        }

                        SetPeriodValue(period, periodisedValues, earningValue.Value);
                    }
                }

                var priceEpisodesForAim = new List<PriceEpisode>();

                foreach (var priceEpisode in aim.PriceEpisodes)
                {
                    var episodeStartDate = priceEpisode.TotalTrainingPriceEffectiveDate.ToDate();

                    var newPriceEpisode = new PriceEpisode
                    {
                        PriceEpisodeIdentifier = priceEpisode.PriceEpisodeId,
                        PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>(),
                        PriceEpisodeValues = new PriceEpisodeValues(),
                    };

                    newPriceEpisode.PriceEpisodeValues.PriceEpisodeAimSeqNumber = priceEpisode.AimSequenceNumber;
                    newPriceEpisode.PriceEpisodeValues.EpisodeStartDate = episodeStartDate;
                    newPriceEpisode.PriceEpisodeValues.PriceEpisodeContractType = priceEpisode.ContractType == Model.Core.Entities.ContractType.Act1 ? "Levy Contract" : "Non-Levy Contract";
                    newPriceEpisode.PriceEpisodeValues.PriceEpisodeFundLineType = priceEpisode.FundingLineType ?? aim.FundingLineType;
                    newPriceEpisode.PriceEpisodeValues.TNP1 = priceEpisode.TotalTrainingPrice;
                    newPriceEpisode.PriceEpisodeValues.TNP2 = priceEpisode.TotalAssessmentPrice;
                    newPriceEpisode.PriceEpisodeValues.TNP3 = priceEpisode.ResidualTrainingPrice;
                    newPriceEpisode.PriceEpisodeValues.TNP4 = priceEpisode.ResidualAssessmentPrice;
                    newPriceEpisode.PriceEpisodeValues.PriceEpisodeTotalTNPPrice = newPriceEpisode.PriceEpisodeValues.TNP1 + newPriceEpisode.PriceEpisodeValues.TNP2;
                    newPriceEpisode.PriceEpisodeValues.PriceEpisodeSFAContribPct = priceEpisode.SfaContributionPercentage.ToPercent();

                    priceEpisodesForAim.Add(newPriceEpisode);
                }

                var orderedPriceEpisodes = priceEpisodesForAim
                    .OrderBy(x => x.PriceEpisodeValues.EpisodeStartDate)
                    .ToList();

                for (var i = 0; i < orderedPriceEpisodes.Count; i++)
                {
                    var currentPriceEpisode = priceEpisodesForAim[i];
                    var tnpStartDate = orderedPriceEpisodes
                        .First(x => x.PriceEpisodeValues.PriceEpisodeTotalTNPPrice ==
                                    currentPriceEpisode.PriceEpisodeValues.PriceEpisodeTotalTNPPrice)
                        .PriceEpisodeValues.EpisodeStartDate;
                    currentPriceEpisode.PriceEpisodeValues.EpisodeEffectiveTNPStartDate = tnpStartDate;
                    if (i + 1 < orderedPriceEpisodes.Count &&
                        orderedPriceEpisodes[i + 1].PriceEpisodeValues.EpisodeStartDate.HasValue)
                    {
                        currentPriceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate =
                            orderedPriceEpisodes[i + 1].PriceEpisodeValues.EpisodeStartDate.Value.AddDays(-1);
                    }
                    else if (aim.ActualDurationAsTimespan.HasValue)
                    {
                        currentPriceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate =
                            aim.StartDate.ToDate() + aim.ActualDurationAsTimespan;
                    }

                    var episodeStart = currentPriceEpisode.PriceEpisodeValues.EpisodeStartDate.Value.ToCalendarPeriod();
                    var episodeLastPeriod = currentPriceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate?.ToCalendarPeriod().Period ?? 12;

                    foreach (var currentValues in aimPeriodisedValues)
                    {
                        PriceEpisodePeriodisedValues newValues;

                        // price episodes not covering the whole year are likely to be one of many, copy values only for current episode, set zero for others
                        if ((episodeStart.Period > 1 && episodeStart.GetCollectionYear() == CollectionYear) || episodeLastPeriod < 12)
                        {
                            newValues = new PriceEpisodePeriodisedValues { AttributeName = currentValues.AttributeName };

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

                        currentPriceEpisode.PriceEpisodePeriodisedValues.Add(newValues);
                    }
                }

                learner.PriceEpisodes.AddRange(priceEpisodesForAim);

                var learningDelivery = new LearningDelivery
                {
                    AimSeqNumber = aim.AimSequenceNumber,
                    LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>(),
                    LearningDeliveryValues = new LearningDeliveryValues(),
                };

                learningDelivery.LearningDeliveryValues.LearnDelInitialFundLineType = aim.FundingLineType;
                learningDelivery.LearningDeliveryValues.Completed = aim.CompletionStatus == CompletionStatus.Completed;
                learningDelivery.LearningDeliveryValues.FworkCode = aim.FrameworkCode;
                learningDelivery.LearningDeliveryValues.LearnAimRef = aim.AimReference;
                learningDelivery.LearningDeliveryValues.LearnStartDate = aim.StartDate.ToDate();
                learningDelivery.LearningDeliveryValues.ProgType = aim.ProgrammeType;
                learningDelivery.LearningDeliveryValues.PwayCode = aim.PathwayCode;
                learningDelivery.LearningDeliveryValues.StdCode = aim.StandardCode;

                learner.LearningDeliveries.Add(learningDelivery);
            }
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

            var testLearner = TestSession.Learners.First(x => x.LearnRefNumber == learner.LearnRefNumber);
            learner.ULN = testLearner.Uln;
            var course = testLearner.Course;

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
                        TNP3 = episode.ResidualTrainingPrice,
                        TNP4 = episode.ResidualAssessmentPrice,
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

                    // price episodes not covering the whole year are likely to be one of many, copy values only for current episode, set zero for others
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
                    if (e.DeliveryPeriod != tableRow["Delivery Period"])
                        return false;

                    if (tableRow.TryGetValue("Aim Sequence Number", out var aimSequenceNumber) && long.Parse(aimSequenceNumber) != e.AimSequenceNumber)
                        return false;

                    if ((tableRow.TryGetValue("Learner ID", out var learnerId) || tableRow.TryGetValue("LearnerId", out learnerId)) && learnerId != e.LearnerId)
                        return false;

                    return true;
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

        protected async Task SendProcessLearnerCommand(FM36Learner learner)
        {
            var command = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionPeriod = CurrentCollectionPeriod.Period,
                CollectionYear = CollectionYear,
                Ukprn = TestSession.Ukprn,
                JobId = TestSession.JobId,
                IlrSubmissionDateTime = TestSession.IlrSubmissionTime,
                RequestTime = DateTimeOffset.UtcNow,
                SubmissionDate = TestSession.IlrSubmissionTime, //TODO: ????          
            };

            Console.WriteLine($"Sending process learner command to the earning events service. Command: {command.ToJson()}");
            await MessageSession.Send(command);
        }

    }
}