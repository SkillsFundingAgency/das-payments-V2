﻿using System;
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
using SFA.DAS.Payments.Model.Core.Incentives;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;
using Payment = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.Payment;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public abstract class EndToEndStepsBase : StepsBase
    {
        protected RequiredPaymentsCacheCleaner RequiredPaymentsCacheCleaner => Container.Resolve<RequiredPaymentsCacheCleaner>();

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
            get => !Context.TryGetValue<List<Training>>("previous_training", out var previousIlr) ? null : previousIlr;
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
            CollectionYear = CurrentCollectionPeriod.AcademicYear;
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

        protected void AddTestAims(IList<Aim> aims)
        {
            if (TestSession.AtLeastOneScenarioCompleted)
            {
                return;
            }

            var allAimsPerLearner = aims.GroupBy(a => a.LearnerId);

            foreach (var learnerAims in allAimsPerLearner)
            {
                var learner = TestSession.Learners.FirstOrDefault(x => x.LearnerIdentifier == learnerAims.Key);
                if (learner == null)
                {
                    throw new Exception("There is an aim without a matching learner");
                }

                learner.Aims.Clear();

                learner.Aims.AddRange(learnerAims);
            }
        }

        protected List<PaymentModel> CreatePayments(ProviderPayment providerPayment, List<Training> learnerTraining, long jobId, DateTime submissionTime, Earning earning)
        {
            var onProgTraining = learnerTraining.FirstOrDefault(t => t.AimReference == "ZPROG001");
            var otherTraining = learnerTraining.FirstOrDefault(t => t.AimReference != "ZPROG001");
            var list = new List<PaymentModel>();
            if (providerPayment.SfaFullyFundedPayments > 0)
                list.Add(CreatePaymentModel(providerPayment, otherTraining?? onProgTraining, jobId, submissionTime, earning, providerPayment.SfaFullyFundedPayments, FundingSourceType.FullyFundedSfa));

            if (providerPayment.EmployerCoFundedPayments > 0)
                list.Add(CreatePaymentModel(providerPayment, onProgTraining, jobId, submissionTime, earning, providerPayment.EmployerCoFundedPayments, FundingSourceType.CoInvestedEmployer));

            if (providerPayment.SfaCoFundedPayments > 0)
                list.Add(CreatePaymentModel(providerPayment, onProgTraining, jobId, submissionTime, earning, providerPayment.SfaCoFundedPayments, FundingSourceType.CoInvestedSfa));

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
                LearningAimPathwayCode = learnerTraining.PathwayCode,
                LearnerReferenceNumber = TestSession.GetLearner(learnerTraining.LearnerId).LearnRefNumber,
                LearningAimReference = learnerTraining.AimReference,
                LearningAimStandardCode = learnerTraining.StandardCode,
                IlrSubmissionDateTime = submissionTime,
                ExternalId = Guid.NewGuid(),
                Amount = amount,
                LearningAimFundingLineType = learnerTraining.FundingLineType,
                LearnerUln = providerPayment.Uln,
                LearningAimFrameworkCode = learnerTraining.FrameworkCode,
                LearningAimProgrammeType = learnerTraining.ProgrammeType
            };
        }

        protected void PopulateLearner(FM36Learner learner, Learner testLearner, IList<Earning> earnings)
        {
            learner.LearnRefNumber = testLearner.LearnRefNumber;
            learner.ULN = testLearner.Uln;
            learner.PriceEpisodes = new List<PriceEpisode>();
            learner.LearningDeliveries = new List<LearningDelivery>();

            foreach (var aim in testLearner.Aims.Where(a => AimPeriodMatcher.IsStartDateValidForCollectionPeriod(a.StartDate, CurrentCollectionPeriod,
                a.PlannedDurationAsTimespan, a.ActualDurationAsTimespan, a.CompletionStatus, a.AimReference)))
            {
                learner.PriceEpisodes.AddRange(GeneratePriceEpisodes(aim, earnings));

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

        private List<PriceEpisode> GeneratePriceEpisodes(Aim aim, IList<Earning> earnings)
        {
            var aimPeriodisedValues = new List<PriceEpisodePeriodisedValues>();

            PriceEpisodePeriodisedValues sfaContributionPeriodisedValue = null;

            if (earnings.Any(x => !string.IsNullOrEmpty(x.SfaContributionPercentage)))
            {
                sfaContributionPeriodisedValue = new PriceEpisodePeriodisedValues { AttributeName = "PriceEpisodeSFAContribPct", };
                aimPeriodisedValues.Add(sfaContributionPeriodisedValue);
            }
            
            foreach (var earning in earnings.Where(e => !e.AimSequenceNumber.HasValue ||
                                                        e.AimSequenceNumber == aim.AimSequenceNumber))
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

                if (sfaContributionPeriodisedValue != null)
                {
                    SetPeriodValue(period, sfaContributionPeriodisedValue, earning.SfaContributionPercentage.ToPercent());
                }
            }
            
            var priceEpisodePrefix = (aim.StandardCode != 0)
                ? $"{aim.ProgrammeType}-{aim.StandardCode}"
                : $"{aim.ProgrammeType}-{aim.FrameworkCode}-{aim.PathwayCode}";

            var priceEpisodesForAim = new List<PriceEpisode>();

            foreach (var priceEpisode in aim.PriceEpisodes)
            {
                var id = CalculatePriceEpisodeIdentifier(priceEpisode, priceEpisodePrefix);

                var firstEarningForPriceEpisode = earnings.First(e =>
                {
                    var deliveryPeriod = e.DeliveryCalendarPeriod;
                    var earningAcademicYear = int.Parse(deliveryPeriod.Name.Substring(0, 4));
                    return earningAcademicYear * 100 + deliveryPeriod.Period >=
                           int.Parse(priceEpisode.EpisodeStartDate.ToCalendarPeriod().Name
                               .Substring(0, 4)) * 100 + priceEpisode
                               .EpisodeStartDate.ToCalendarPeriod().Period;
                });
                var sfaContributionPercent = (firstEarningForPriceEpisode.SfaContributionPercentage ??
                                  priceEpisode.SfaContributionPercentage).ToPercent();

                var newPriceEpisode = new PriceEpisode
                {
                    PriceEpisodeIdentifier = id,
                    PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>(),
                    PriceEpisodeValues = new PriceEpisodeValues(),
                };

                newPriceEpisode.PriceEpisodeValues.PriceEpisodeAimSeqNumber = CalculateAimSequenceNumber(priceEpisode);
                newPriceEpisode.PriceEpisodeValues.EpisodeStartDate = priceEpisode.EpisodeStartDate;
                newPriceEpisode.PriceEpisodeValues.PriceEpisodeContractType = CalculateContractType(priceEpisode);
                newPriceEpisode.PriceEpisodeValues.PriceEpisodeFundLineType = priceEpisode.FundingLineType ?? aim.FundingLineType;
                newPriceEpisode.PriceEpisodeValues.TNP1 = priceEpisode.TotalTrainingPrice;
                newPriceEpisode.PriceEpisodeValues.TNP2 = priceEpisode.TotalAssessmentPrice;
                newPriceEpisode.PriceEpisodeValues.TNP3 = priceEpisode.ResidualTrainingPrice;
                newPriceEpisode.PriceEpisodeValues.TNP4 = priceEpisode.ResidualAssessmentPrice;
                newPriceEpisode.PriceEpisodeValues.PriceEpisodeTotalTNPPrice = priceEpisode.TotalTNPPrice;
                newPriceEpisode.PriceEpisodeValues.PriceEpisodeSFAContribPct = sfaContributionPercent;

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

                if (aim.ActualDurationAsTimespan.HasValue)
                {
                    currentPriceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate =
                        aim.StartDate.ToDate() + aim.ActualDurationAsTimespan;
                }

                if (i + 1 < orderedPriceEpisodes.Count &&
                         orderedPriceEpisodes[i + 1].PriceEpisodeValues.EpisodeStartDate.HasValue)
                {
                    var actualEndDate = orderedPriceEpisodes[i + 1].PriceEpisodeValues.EpisodeStartDate.Value.AddDays(-1);
                    if (currentPriceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate.HasValue)
                    {
                        if (actualEndDate < currentPriceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate)
                        {
                            currentPriceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate = actualEndDate;
                        }
                    }
                    else
                    {
                        currentPriceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate = actualEndDate;
                    }
                }

                var episodeLastPeriod = LastOnProgPeriod(currentPriceEpisode);
                var episodeStart = currentPriceEpisode.PriceEpisodeValues.EpisodeStartDate.Value.ToCalendarPeriod();

                foreach (var currentValues in aimPeriodisedValues)
                {
                    PriceEpisodePeriodisedValues newValues;

                    // price episodes not covering the whole year are likely to be one of many, copy values only for current episode, set zero for others
                    if (episodeStart.GetCollectionYear() == CollectionYear && (episodeStart.Period > 1 || episodeLastPeriod < 12))
                    {
                        newValues = new PriceEpisodePeriodisedValues { AttributeName = currentValues.AttributeName };

                        for (var p = 1; p < 13; p++)
                        {
                            var amount = p >= episodeStart.Period && p <= episodeLastPeriod || 
                                         (PeriodisedValuesForBalancingAndCompletion().Contains(currentValues.AttributeName) && p > episodeLastPeriod)
                                ? currentValues.GetValue(p) 
                                : 0;
                            newValues.SetValue(p, amount);
                        }
                    }
                    else // put everything as is for previous years
                    {
                        newValues = currentValues;
                    }

                    if (newValues.AttributeName == "PriceEpisodeSFAContribPct" && aim.AimReference == "ZPROG001")
                    {
                        currentPriceEpisode.PriceEpisodePeriodisedValues.Add(newValues);
                    }
                    else if ((EnumHelper.IsOnProgType(EnumHelper.ToTransactionTypeFromAttributeName(newValues.AttributeName)) ||
                        EnumHelper.IsIncentiveType(EnumHelper.ToTransactionTypeFromAttributeName(newValues.AttributeName))) &&
                        aim.AimReference == "ZPROG001")
                    {
                        currentPriceEpisode.PriceEpisodePeriodisedValues.Add(newValues);
                    }
                    else if (EnumHelper.IsFunctionalSkillType(EnumHelper.ToTransactionTypeFromAttributeName(newValues.AttributeName)) &&
                             aim.AimReference != "ZPROG001")
                    {
                        currentPriceEpisode.PriceEpisodePeriodisedValues.Add(newValues);
                    }
                }
            }

            return priceEpisodesForAim;

            // Local methods
            int CalculateAimSequenceNumber(Price priceEpisode)
            {
                return (priceEpisode.AimSequenceNumber == 0)
                    ? aim.AimSequenceNumber
                    : priceEpisode.AimSequenceNumber;
            }

            string CalculateContractType(Price priceEpisode)
            {
                return priceEpisode.ContractType ==
                       Model.Core.Entities.ContractType.Act1 ? "Levy Contract" : "Non-Levy Contract";
            }

            byte LastOnProgPeriod(PriceEpisode currentPriceEpisode)
            {
                return currentPriceEpisode.PriceEpisodeValues
                           .PriceEpisodeActualEndDate?.ToLastOnProgPeriod().Period ?? 12;
            }
        }

        private static string CalculatePriceEpisodeIdentifier(Price priceEpisode, string priceEpisodePrefix)
        {
            var episodeStartDate = priceEpisode.EpisodeStartDate;
            return string.IsNullOrEmpty(priceEpisode.PriceEpisodeId)
                ? $"{priceEpisodePrefix}-{episodeStartDate.Day:D2}/{episodeStartDate.Month:D2}/{episodeStartDate.Year}"
                : priceEpisode.PriceEpisodeId;
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

        protected static List<Payment> CreatePayments(Table table)
        {
            var payments = table.CreateSet<Payment>().ToList();

            for (var i = 0; i < table.RowCount; i++)
            {
                var tableRow = table.Rows[i];
                var payment = payments[i];

                foreach (var headerCell in table.Header)
                {
                    var name = headerCell.Replace(" ", null).Replace("-", null);

                    if (!Enum.TryParse<IncentivePaymentType>(name, true, out var transactionType))
                        continue;

                    if (!decimal.TryParse(tableRow[headerCell], out var amount))
                        continue;

                    payment.IncentiveValues.Add(transactionType, amount);
                }
            }

            return payments;
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

        private IEnumerable<string> PeriodisedValuesForBalancingAndCompletion()
        {
            yield return TransactionType.BalancingMathsAndEnglish.ToAttributeName();
            yield return TransactionType.Balancing16To18FrameworkUplift.ToAttributeName();
            yield return TransactionType.Completion16To18FrameworkUplift.ToAttributeName();
        }
    }
}