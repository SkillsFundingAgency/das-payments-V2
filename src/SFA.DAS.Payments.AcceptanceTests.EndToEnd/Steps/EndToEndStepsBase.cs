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
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;
using Payment = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.Payment;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public abstract class EndToEndStepsBase : StepsBase
    {
        public bool NewFeature
        {
            get => Get<bool>("new_feature");
            set => Set(value, "new_feature");
        }
        protected RequiredPaymentsCacheCleaner RequiredPaymentsCacheCleaner => Container.Resolve<RequiredPaymentsCacheCleaner>();

        protected IPaymentsDataContext DataContext => Scope.Resolve<IPaymentsDataContext>();

        protected IMapper Mapper => Scope.Resolve<IMapper>();

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

        public CollectionPeriod CurrentCollectionPeriod
        {
            get => Get<CollectionPeriod>("current_collection_period");
            set => Set(value, "current_collection_period");
        }

        public List<Commitment> Commitments
        {
            get
            {
                if (!Context.TryGetValue<List<Commitment>>("commitments", out var commitments))
                {
                    Set(new List<Commitment>(), "commitments");
                    commitments = Get<List<Commitment>>("commitments");
                }
                return  commitments;
            }
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
            var period = new CollectionPeriodBuilder().WithSpecDate(collectionPeriod).Build();
            Console.WriteLine($"Current collection period name is: {period.ToJson()}.");
            CurrentCollectionPeriod = period;
            CollectionPeriod = CurrentCollectionPeriod.Period;
            AcademicYear = CurrentCollectionPeriod.AcademicYear;
        }

        protected void AddTestLearners(List<Training> training)
        {
            training.ForEach(ilrLearner =>
            {
                var learner = TestSession.GetLearner(ilrLearner.LearnerId);
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
            if (TestSession.AtLeastOneScenarioCompleted || !NewFeature)
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

        protected async Task AddTestCommitments(List<Commitment> commitments)
        {
            commitments.ForEach(x =>
            {
                x.AccountId = TestSession.GetEmployer(x.Employer).AccountId;
                x.Uln = TestSession.GetLearner(x.LearnerId).Uln;
            });
            Commitments.Clear();
            Commitments.AddRange(commitments);
            await SaveTestCommitments();
        }

        protected async Task SaveTestCommitments()
        {
            DataContext.Commitment.AddRange(Mapper.ToModel(Commitments));
            await DataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        protected async Task SaveLevyAccount(Employer employer)
        {
            var existingEmployer = await DataContext.LevyAccount.FirstOrDefaultAsync(o => o.AccountId == employer.AccountId);

            if (existingEmployer == null)
            {
                DataContext.LevyAccount.Add(employer.ToModel());
            }
            else
            {
                existingEmployer.Balance = employer.Balance;
                existingEmployer.IsLevyPayer = employer.IsLevyPayer;
                existingEmployer.TransferAllowance = employer.TransferAllowance;
                DataContext.LevyAccount.Update(existingEmployer);
            }

            await DataContext.SaveChangesAsync();
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
                CollectionPeriod = new CollectionPeriodBuilder().WithSpecDate(providerPayment.CollectionPeriod).Build(),
                DeliveryPeriod = new DeliveryPeriodBuilder().WithSpecDate(providerPayment.DeliveryPeriod).Build(),
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
                EventId = Guid.NewGuid(),
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
                var period = earning.DeliveryCalendarPeriod;
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

                var priceEpisodeStartDateAsDeliveryPeriod = new DeliveryPeriodBuilder()
                    .WithDate(priceEpisode.EpisodeStartDate)
                    .Build();

                var firstEarningForPriceEpisode = earnings
                    .OrderBy(x => x.DeliveryCalendarPeriod)
                    .First(e => e.DeliveryCalendarPeriod >= priceEpisodeStartDateAsDeliveryPeriod);

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
                var episodeStart = new CollectionPeriodBuilder().WithDate(currentPriceEpisode.PriceEpisodeValues.EpisodeStartDate.Value).Build();
                
                foreach (var currentValues in aimPeriodisedValues)
                {
                    PriceEpisodePeriodisedValues newValues;

                    // price episodes not covering the whole year are likely to be one of many, copy values only for current episode, set zero for others
                    if (episodeStart.AcademicYear == AcademicYear && (episodeStart.Period > 1 || episodeLastPeriod < 12))
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
                var contractType = priceEpisode.ContractType;

                if (contractType == 0)
                    contractType = CurrentIlr[0].ContractType;

                return contractType == Model.Core.Entities.ContractType.Act1 ? "Levy Contract" : "Non-Levy Contract";
            }

            byte LastOnProgPeriod(PriceEpisode currentPriceEpisode)
            {
                if (currentPriceEpisode.PriceEpisodeValues
                        .PriceEpisodeActualEndDate == null)
                {
                    return 12;
                }

                return new DeliveryPeriodBuilder().WithDate(currentPriceEpisode.PriceEpisodeValues
                    .PriceEpisodeActualEndDate.Value).BuildLastOnProgPeriod();
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

        protected async Task MatchCalculatedPayments(Table table)
        {
            var expectedPayments = CreatePayments(table);
            var matcher = new RequiredPaymentEventMatcher(TestSession, CurrentCollectionPeriod, expectedPayments, CurrentIlr, CurrentPriceEpisodes);
            await WaitForIt(() => matcher.MatchPayments(), "Required Payment event check failure");
        }

        protected async Task StartMonthEnd()
        {
            var monthEndJobId = TestSession.GenerateId();
            Console.WriteLine($"Month end job id: {monthEndJobId}");
            TestSession.SetJobId(monthEndJobId);
            var processProviderPaymentsAtMonthEndCommand = new ProcessProviderMonthEndCommand
            {
                CollectionPeriod = CurrentCollectionPeriod,
                Ukprn = TestSession.Ukprn,
                JobId = monthEndJobId
            };
            //TODO: remove when DC have implemented the Month End Task
            var dcStartedMonthEndJobCommand = new RecordStartedProcessingMonthEndJob
            {
                JobId = monthEndJobId,
                CollectionPeriod = CollectionPeriod,
                CollectionYear = AcademicYear,
                GeneratedMessages = new List<GeneratedMessage> {new GeneratedMessage
                {
                    StartTime = DateTimeOffset.UtcNow,
                    MessageName = processProviderPaymentsAtMonthEndCommand.GetType().FullName,
                    MessageId = processProviderPaymentsAtMonthEndCommand.CommandId
                }}
            };

            var tasks = new List<Task>();

            foreach (var employer in TestSession.Employers)
            {
                var processLevyFundsAtMonthEndCommand = new ProcessLevyPaymentsOnMonthEndCommand
                {
                    JobId = TestSession.JobId,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = AcademicYear, Period = CollectionPeriod },
                    RequestTime = DateTime.Now,
                    SubmissionDate = TestSession.IlrSubmissionTime,
                    EmployerAccountId = employer.AccountId,
                };

                tasks.Add(MessageSession.Send(processLevyFundsAtMonthEndCommand));
            }

            tasks.Add(MessageSession.Send(dcStartedMonthEndJobCommand));
            tasks.Add(MessageSession.Send(processProviderPaymentsAtMonthEndCommand));
            
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        protected async Task SendProcessLearnerCommand(FM36Learner learner)
        {
            var command = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionPeriod = CurrentCollectionPeriod.Period,
                CollectionYear = AcademicYear,
                Ukprn = TestSession.Ukprn,
                JobId = TestSession.JobId,
                IlrSubmissionDateTime = TestSession.IlrSubmissionTime,
                RequestTime = DateTimeOffset.UtcNow,
                SubmissionDate = TestSession.IlrSubmissionTime, //TODO: ????          
            };

            Console.WriteLine($"Sending process learner command to the earning events service. Command: {command.ToJson()}");
            await MessageSession.Send(command);
        }

        protected async Task MatchOnlyProviderPayments(Table table)
        {
            var expectedPayments = table.CreateSet<ProviderPayment>().ToList();
            var matcher = new ProviderPaymentEventMatcher(CurrentCollectionPeriod, TestSession, expectedPayments);
            await WaitForIt(() => matcher.MatchPayments(), "Provider Payment event check failure");
        }

        private IEnumerable<string> PeriodisedValuesForBalancingAndCompletion()
        {
            yield return TransactionType.BalancingMathsAndEnglish.ToAttributeName();
            yield return TransactionType.Balancing16To18FrameworkUplift.ToAttributeName();
            yield return TransactionType.Completion16To18FrameworkUplift.ToAttributeName();
        }
    }
}