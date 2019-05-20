using Autofac;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Abstract;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;
using Payment = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.Payment;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers;

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

        protected List<Apprenticeship> Apprenticeships
        {
            get => !Context.TryGetValue<List<Apprenticeship>>(out var apprenticeships) ? null : apprenticeships;
            set => Set(value);
        }

        protected List<Earning> PreviousEarnings
        {
            get => !Context.TryGetValue<List<Earning>>("previous_earnings", out var previousEarnings) ? null : previousEarnings;
            set => Set(value, "previous_earnings");
        }

        public CollectionPeriod CurrentCollectionPeriod
        {
            get => Get<CollectionPeriod>("current_collection_period");
            set => Set(value, "current_collection_period");
        }

        protected HashSet<(byte period, int academicYear, long ukprn)> ProvidersWithCacheCleared
        {
            get => !Context.TryGetValue<HashSet<(byte period, int academicYear, long ukprn)>>("ProvidersWithCacheCleared", out var providersWithCacheCleared) ? null : providersWithCacheCleared;
            set => Set(value, "ProvidersWithCacheCleared");
        }

        protected EndToEndStepsBase(FeatureContext context) : base(context)
        {
        }

        protected void AddNewIlr(Table table, long ukprn)
        {
            var ilr = table.CreateSet<Training>().ToList();
            AddTestLearners(ilr, ukprn);
            if (CurrentIlr == null) CurrentIlr = new List<Training>();
            CurrentIlr.AddRange(ilr);
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

        protected void AddTestLearners(List<Training> training, long ukprn)
        {
            training.ForEach(ilrLearner =>
            {
                ilrLearner.Ukprn = ukprn;
                var learner = TestSession.GetLearner(ukprn, ilrLearner.LearnerId);
                learner.Course.AimSeqNumber = (short)ilrLearner.AimSequenceNumber;
                learner.Course.StandardCode = ilrLearner.StandardCode;
                learner.Course.FundingLineType = ilrLearner.FundingLineType;
                learner.Course.LearnAimRef = ilrLearner.AimReference;
                learner.Course.CompletionStatus = ilrLearner.CompletionStatus;
                learner.Course.ProgrammeType = ilrLearner.ProgrammeType;
                learner.Course.FrameworkCode = ilrLearner.FrameworkCode;
                learner.Course.PathwayCode = ilrLearner.PathwayCode;
                if (ilrLearner.Uln != default(long)) learner.Uln = ilrLearner.Uln;

            });
        }

        protected void AddTestLearners(IEnumerable<Learner> learners, long ukprn)
        {
            foreach (var learner in learners)
            {
                var testLearner = TestSession.Learners
                    .FirstOrDefault(l => l.LearnerIdentifier == learner.LearnerIdentifier);

                if (testLearner == null)
                {
                    testLearner = TestSession.GetLearner(ukprn, learner.LearnerIdentifier);
                }
                testLearner.LearnerIdentifier = string.IsNullOrEmpty(learner.LearnerIdentifier) ? testLearner.LearnerIdentifier : learner.LearnerIdentifier;
                testLearner.Uln = learner.Uln == 0 ? testLearner.Uln : learner.Uln;
                testLearner.LearnRefNumber = string.IsNullOrEmpty(learner.LearnRefNumber) ? testLearner.LearnRefNumber : learner.LearnRefNumber;
            }
        }

        protected void AddTestAims(IList<Aim> aims, long ukprn)
        {
            if (TestSession.AtLeastOneScenarioCompleted || !NewFeature)
            {
                return;
            }

            var allAimsPerLearner = aims.GroupBy(a => a.LearnerId);

            foreach (var learnerAims in allAimsPerLearner)
            {
                var learner = TestSession.Learners.FirstOrDefault(x => x.LearnerIdentifier == learnerAims.Key && x.Ukprn == ukprn);
                if (learner == null)
                {
                    throw new Exception("There is an aim without a matching learner");
                }

                learner.Aims.Clear();

                learner.Aims.AddRange(learnerAims);
            }
        }

        protected async Task AddOrUpdateTestApprenticeships(List<Apprenticeship> apprenticeshipSpecs)
        {
            if (Apprenticeships == null) Apprenticeships = new List<Apprenticeship>();

            var groupedApprenticeships = apprenticeshipSpecs
                .GroupBy(a => a.Identifier)
                .ToList();

            foreach (var group in groupedApprenticeships)
            {
                var specApprenticeship = Apprenticeships.FirstOrDefault(a => a.Identifier == group.Key);
                if (specApprenticeship == null)
                {
                    //use last apprenticeship to make sure it picks up the most recent status
                    specApprenticeship = group.Last();
                    var apprenticeship = ApprenticeshipHelper.CreateApprenticeshipModel(specApprenticeship, TestSession);
                    apprenticeship.ApprenticeshipPriceEpisodes = group.Select(ApprenticeshipHelper.CreateApprenticeshipPriceEpisode).ToList();
                    await ApprenticeshipHelper.AddApprenticeship(apprenticeship, DataContext).ConfigureAwait(false);
                    specApprenticeship.ApprenticeshipId = apprenticeship.Id;
                    Apprenticeships.Add(specApprenticeship);
                }
                else
                {
                    var priceEpisodes = group.Select(ApprenticeshipHelper.CreateApprenticeshipPriceEpisode).ToList();
                    var status = group.Last().Status?.ToApprenticeshipPaymentStatus() ??
                                 throw new InvalidOperationException($"last item not found: {group.Key}");
                    await ApprenticeshipHelper.UpdateApprenticeship(specApprenticeship.ApprenticeshipId,status, priceEpisodes, DataContext);
                }
            }
        }

        protected async Task UpdateApprenticeshipStatus(long apprenticeshipId, ApprenticeshipStatus status, string stoppedDateText)
        {
            var apprenticeship = await DataContext.Apprenticeship.FirstOrDefaultAsync(appr => appr.Id == apprenticeshipId);
            if (apprenticeship==null)
                throw new InvalidOperationException($"Apprenticeship '{apprenticeshipId}' not found.");
            apprenticeship.Status = status;
            if (!string.IsNullOrEmpty(stoppedDateText) && status == ApprenticeshipStatus.Stopped)
            {
                var stoppedDate = stoppedDateText.ToDate();
                apprenticeship.StopDate = stoppedDate;
            }
            await DataContext.SaveChangesAsync();
        }

        protected async Task SaveLevyAccount(Employer employer)
        {
            var existingEmployer = await DataContext.LevyAccount.FirstOrDefaultAsync(o => o.AccountId == employer.AccountId);

            if (existingEmployer == null)
            {
                DataContext.LevyAccount.Add(employer.ToModel());
                Console.WriteLine($"Employer account created. Id:{employer.AccountId}, Balance:{employer.Balance}, {DateTime.Now}");
            }
            else
            {
                existingEmployer.Balance = employer.Balance;
                existingEmployer.IsLevyPayer = employer.IsLevyPayer;
                existingEmployer.TransferAllowance = employer.TransferAllowance;
                DataContext.LevyAccount.Update(existingEmployer);
                Console.WriteLine($"Employer account updated. Id:{employer.AccountId}, Balance:{employer.Balance}, {DateTime.Now}");
            }

            await DataContext.SaveChangesAsync();
        }
        protected List<PaymentModel> CreatePayments(ProviderPayment providerPayment, List<Training> learnerTraining, long jobId, DateTime submissionTime, decimal? sfaContributionPercentage, long ukprn)
        {
            var onProgTraining = learnerTraining.FirstOrDefault(t => t.AimReference == "ZPROG001");
            var otherTraining = learnerTraining.FirstOrDefault(t => t.AimReference != "ZPROG001");
            var list = new List<PaymentModel>();
            if (providerPayment.SfaFullyFundedPayments > 0)
                list.Add(CreatePaymentModel(providerPayment, otherTraining ?? onProgTraining, jobId, submissionTime, 100,
                    providerPayment.SfaFullyFundedPayments, FundingSourceType.FullyFundedSfa, ukprn, providerPayment.AccountId));

            if (providerPayment.EmployerCoFundedPayments > 0)
                list.Add(CreatePaymentModel(providerPayment, onProgTraining, jobId, submissionTime,
                    sfaContributionPercentage, providerPayment.EmployerCoFundedPayments, FundingSourceType.CoInvestedEmployer, ukprn, providerPayment.AccountId));

            if (providerPayment.SfaCoFundedPayments > 0)
                list.Add(CreatePaymentModel(providerPayment, onProgTraining, jobId, submissionTime,
                    sfaContributionPercentage, providerPayment.SfaCoFundedPayments, FundingSourceType.CoInvestedSfa, ukprn, providerPayment.AccountId));

            if (providerPayment.LevyPayments > 0)
            {
                var payment = CreatePaymentModel(providerPayment, onProgTraining, jobId, submissionTime,
                    sfaContributionPercentage, providerPayment.LevyPayments, FundingSourceType.Levy, ukprn, providerPayment.AccountId);
                list.Add(payment);
            }
            return list;
        }

        private PaymentModel CreatePaymentModel(ProviderPayment providerPayment, Training learnerTraining, long jobId,
            DateTime submissionTime, decimal? sfaContributionPercentage, decimal amount,
            FundingSourceType fundingSourceType, long ukprn, long? accountId)
        {
            return new PaymentModel
            {
                CollectionPeriod = new CollectionPeriodBuilder().WithSpecDate(providerPayment.CollectionPeriod).Build(),
                DeliveryPeriod = new DeliveryPeriodBuilder().WithSpecDate(providerPayment.DeliveryPeriod).Build(),
                Ukprn = ukprn,
                JobId = jobId,
                SfaContributionPercentage =
                    sfaContributionPercentage ?? (learnerTraining.SfaContributionPercentage).ToPercent(),
                TransactionType = providerPayment.TransactionType,
                ContractType = learnerTraining.ContractType,
                PriceEpisodeIdentifier = learnerTraining.AimReference == "ZPROG001" ? "pe-1" : string.Empty,
                FundingSource = fundingSourceType,
                LearningAimPathwayCode = learnerTraining.PathwayCode,
                LearnerReferenceNumber = TestSession.GetLearner(ukprn, learnerTraining.LearnerId).LearnRefNumber,
                LearningAimReference = learnerTraining.AimReference,
                LearningAimStandardCode = learnerTraining.StandardCode,
                IlrSubmissionDateTime = submissionTime,
                EventId = Guid.NewGuid(),
                Amount = amount,
                LearningAimFundingLineType = learnerTraining.FundingLineType,
                LearnerUln = providerPayment.Uln,
                LearningAimFrameworkCode = learnerTraining.FrameworkCode,
                LearningAimProgrammeType = learnerTraining.ProgrammeType,
                AccountId = accountId,
                StartDate = DateTime.UtcNow,
                PlannedEndDate = DateTime.UtcNow,
                ActualEndDate = DateTime.UtcNow,
                CompletionStatus = 1,
                CompletionAmount = 100M,
                InstalmentAmount = 200M,
                NumberOfInstalments = 12
            };
        }

        protected void PopulateLearner(FM36Learner learner, Learner testLearner, IList<Earning> earnings)
        {
            learner.LearnRefNumber = testLearner.LearnRefNumber;
            learner.ULN = testLearner.Uln;
            learner.PriceEpisodes = new List<PriceEpisode>();
            learner.LearningDeliveries = new List<LearningDelivery>();

            IEnumerable<Aim> currentAims;

            // AimPeriodMatcher doesn't support completed aims from the past, this is a workaround until imminent refactoring
            if (testLearner.Aims.Count == 1)
            {
                currentAims = testLearner.Aims;
            }
            else
            {
                currentAims = testLearner.Aims.Where(a => AimPeriodMatcher.IsStartDateValidForCollectionPeriod(a.StartDate, CurrentCollectionPeriod,
                    a.PlannedDurationAsTimespan, a.ActualDurationAsTimespan, a.CompletionStatus, a.AimReference, a.PlannedDuration, a.ActualDuration));
            }

            foreach (var aim in currentAims)
            {
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

                if (aim.IsMainAim)
                {
                    learner.PriceEpisodes.AddRange(GeneratePriceEpisodes(aim, earnings));
                }
                else // maths & english & Learning support don't use price episodes
                {
                    learningDelivery.LearningDeliveryPeriodisedValues = SetPeriodisedValues<LearningDeliveryPeriodisedValues>(aim, earnings);
                }
            }
        }

        private List<PriceEpisode> GeneratePriceEpisodes(Aim aim, IList<Earning> earnings)
        {
            //TODO: refactor all of this, way too big, too complicated, local methods!!!
            var aimPeriodisedValues = SetPeriodisedValues<PriceEpisodePeriodisedValues>(aim, earnings);

            var priceEpisodePrefix = (aim.StandardCode != 0)
                ? $"{aim.ProgrammeType}-{aim.StandardCode}"
                : $"{aim.ProgrammeType}-{aim.FrameworkCode}-{aim.PathwayCode}";

            var priceEpisodesForAim = new List<PriceEpisode>();

            foreach (var priceEpisode in aim.PriceEpisodes)
            {
                var id = CalculatePriceEpisodeIdentifier(priceEpisode, priceEpisodePrefix);

                var priceEpisodeStartDateAsDeliveryPeriod = new DeliveryPeriodBuilder()
                    .WithDate(priceEpisode.EpisodeEffectiveStartDate)
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
                newPriceEpisode.PriceEpisodeValues.EpisodeStartDate = aim.StartDate.ToDate();
                newPriceEpisode.PriceEpisodeValues.EpisodeEffectiveTNPStartDate = priceEpisode.EpisodeEffectiveStartDate;
                newPriceEpisode.PriceEpisodeValues.PriceEpisodeContractType = CalculateContractType(priceEpisode);
                newPriceEpisode.PriceEpisodeValues.PriceEpisodeFundLineType = priceEpisode.FundingLineType ?? aim.FundingLineType;
                newPriceEpisode.PriceEpisodeValues.TNP1 = priceEpisode.TotalTrainingPrice;
                newPriceEpisode.PriceEpisodeValues.TNP2 = priceEpisode.TotalAssessmentPrice;
                newPriceEpisode.PriceEpisodeValues.TNP3 = priceEpisode.ResidualTrainingPrice;
                newPriceEpisode.PriceEpisodeValues.TNP4 = priceEpisode.ResidualAssessmentPrice;
                newPriceEpisode.PriceEpisodeValues.PriceEpisodeTotalTNPPrice = priceEpisode.TotalTNPPrice;
                newPriceEpisode.PriceEpisodeValues.PriceEpisodeSFAContribPct = sfaContributionPercent;
                // Default to max value so that not setting employer contribution won't fail tests 
                newPriceEpisode.PriceEpisodeValues.PriceEpisodeCumulativePMRs = priceEpisode.Pmr ?? int.MaxValue;
                newPriceEpisode.PriceEpisodeValues.PriceEpisodeCompExemCode = priceEpisode.CompletionHoldBackExemptionCode;

                priceEpisodesForAim.Add(newPriceEpisode);
            }

            var orderedPriceEpisodes = priceEpisodesForAim
                .OrderBy(x => x.PriceEpisodeValues.EpisodeEffectiveTNPStartDate)
                .ToList();
            for (var i = 0; i < orderedPriceEpisodes.Count; i++)
            {
                var currentPriceEpisode = priceEpisodesForAim[i];
                var tnpStartDate = orderedPriceEpisodes
                    .First(x => x.PriceEpisodeValues.PriceEpisodeTotalTNPPrice ==
                                currentPriceEpisode.PriceEpisodeValues.PriceEpisodeTotalTNPPrice)
                    .PriceEpisodeValues.EpisodeEffectiveTNPStartDate;

                currentPriceEpisode.PriceEpisodeValues.EpisodeEffectiveTNPStartDate = tnpStartDate;

                if (aim.ActualDurationAsTimespan.HasValue)
                {
                    currentPriceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate =
                        aim.StartDate.ToDate() + aim.ActualDurationAsTimespan;
                }

                if (i + 1 < orderedPriceEpisodes.Count &&
                         orderedPriceEpisodes[i + 1].PriceEpisodeValues.EpisodeStartDate.HasValue)
                {
                    var actualEndDate = orderedPriceEpisodes[i + 1].PriceEpisodeValues.EpisodeEffectiveTNPStartDate.Value.AddDays(-1);
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
                var episodeStart = new CollectionPeriodBuilder().WithDate(currentPriceEpisode.PriceEpisodeValues.EpisodeEffectiveTNPStartDate.Value).Build();

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

                    if (newValues.AttributeName == "PriceEpisodeSFAContribPct" && aim.IsMainAim)
                    {
                        currentPriceEpisode.PriceEpisodePeriodisedValues.Add(newValues);
                    }
                    else if ((EnumHelper.IsOnProgType(EnumHelper.ToTransactionTypeFromAttributeName(newValues.AttributeName)) ||
                        EnumHelper.IsIncentiveType(EnumHelper.ToTransactionTypeFromAttributeName(newValues.AttributeName), true)) &&
                        aim.IsMainAim)
                    {
                        currentPriceEpisode.PriceEpisodePeriodisedValues.Add(newValues);
                    }
                    else if (EnumHelper.IsFunctionalSkillType(EnumHelper.ToTransactionTypeFromAttributeName(newValues.AttributeName), false) &&
                             !aim.IsMainAim)
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

        private static List<T> SetPeriodisedValues<T>(Aim aim, IList<Earning> earnings) where T : PeriodisedAttribute, new()
        {
            var aimPeriodisedValues = new List<T>();

            T sfaContributionPeriodisedValue = null;

            if (earnings.Any(x => !string.IsNullOrEmpty(x.SfaContributionPercentage)))
            {
                sfaContributionPeriodisedValue = new T { AttributeName = "PriceEpisodeSFAContribPct", };
                aimPeriodisedValues.Add(sfaContributionPeriodisedValue);
            }

            foreach (var earning in earnings.Where(e => !e.AimSequenceNumber.HasValue ||
                                                        e.AimSequenceNumber == aim.AimSequenceNumber))
            {
                var period = earning.DeliveryCalendarPeriod;
                foreach (var earningValue in earning.Values)
                {
                    var earningKey = earningValue.Key.ToAttributeName();

                    var periodisedValues = aimPeriodisedValues.SingleOrDefault(v => v.AttributeName == earningKey);
                    if (periodisedValues == null)
                    {
                        periodisedValues = new T { AttributeName = earningKey };
                        aimPeriodisedValues.Add(periodisedValues);
                    }

                    SetPeriodValue(period, periodisedValues, earningValue.Value);
                }

                if (sfaContributionPeriodisedValue != null)
                {
                    SetPeriodValue(period, sfaContributionPeriodisedValue, earning.SfaContributionPercentage.ToPercent());
                }
            }

            return aimPeriodisedValues;
        }

        private static string CalculatePriceEpisodeIdentifier(Price priceEpisode, string priceEpisodePrefix)
        {
            var episodeStartDate = priceEpisode.EpisodeEffectiveStartDate;
            return string.IsNullOrEmpty(priceEpisode.PriceEpisodeId)
                ? $"{priceEpisodePrefix}-{episodeStartDate.Day:D2}/{episodeStartDate.Month:D2}/{episodeStartDate.Year}"
                : priceEpisode.PriceEpisodeId;
        }

        private static void SetPeriodValue(int period, PeriodisedAttribute periodisedValues, decimal amount)
        {
            var periodProperty = periodisedValues.GetType().GetProperty("Period" + period);
            periodProperty?.SetValue(periodisedValues, amount);
        }

        protected static List<Earning> CreateEarnings(Table table, long ukprn)
        {
            var earnings = table.CreateSet<Earning>().ToList();

            foreach (var tableRow in table.Rows)
            {
                //TODO: why do we need this?!?
                var earning = earnings.Single(e =>
                {
                    if (e.DeliveryPeriod != tableRow["Delivery Period"])
                        return false;

                    if (tableRow.TryGetValue("Learning Aim Reference", out var aimRef) && aimRef != e.LearningAimReference)
                        return false;

                    if (tableRow.TryGetValue("Aim Sequence Number", out var aimSequenceNumber) && long.Parse(aimSequenceNumber) != e.AimSequenceNumber)
                        return false;

                    if ((tableRow.TryGetValue("Learner ID", out var learnerId) || tableRow.TryGetValue("LearnerId", out learnerId)) && learnerId != e.LearnerId)
                        return false;

                    if ((tableRow.TryGetValue("Price Episode Identifier", out var priceEpisodeId)) && priceEpisodeId != e.PriceEpisodeIdentifier)
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

                earning.Ukprn = ukprn;
            }

            return earnings;
        }

        protected static List<Payment> CreatePayments(Table table, long ukprn)
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

        protected async Task MatchRequiredPayments(Table table, Provider provider)
        {
            var expectedPayments = CreatePayments(table, provider.Ukprn);
            var matcher = new RequiredPaymentEventMatcher(provider, CurrentCollectionPeriod, expectedPayments, CurrentIlr, CurrentPriceEpisodes);
            await WaitForIt(() => matcher.MatchPayments(), "Required Payment event check failure");
        }

        protected async Task StartMonthEnd(Provider provider)
        {
            if (!provider.MonthEndJobIdGenerated) // for ACT1 it could have been generated on Required Payments check step
            {
                var monthEndJobId = TestSession.GenerateId();
                Console.WriteLine($"Month end job id: {monthEndJobId}");
                provider.JobId = monthEndJobId;
            }

            var processProviderPaymentsAtMonthEndCommand = new ProcessProviderMonthEndCommand
            {
                CollectionPeriod = CurrentCollectionPeriod,
                Ukprn = provider.Ukprn,
                JobId = provider.JobId
            };

            //TODO: remove when DC have implemented the Month End Task
            var dcStartedMonthEndJobCommand = new RecordStartedProcessingMonthEndJob
            {
                JobId = provider.JobId,
                CollectionPeriod = CollectionPeriod,
                CollectionYear = AcademicYear,
                GeneratedMessages = new List<GeneratedMessage> {new GeneratedMessage
                {
                    StartTime = DateTimeOffset.UtcNow,
                    MessageName = processProviderPaymentsAtMonthEndCommand.GetType().FullName,
                    MessageId = processProviderPaymentsAtMonthEndCommand.CommandId
                }}
            };

            var tasks = new List<Task>
            {
                MessageSession.Send(dcStartedMonthEndJobCommand),
                MessageSession.Send(processProviderPaymentsAtMonthEndCommand)
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);

            provider.MonthEndJobIdGenerated = true;
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

        protected async Task MatchOnlyProviderPayments(Table table, Provider provider)
        {
            var expectedPayments = table.CreateSet<ProviderPayment>().ToList();
            var ilr = CurrentIlr ?? PreviousIlr;
            ilr = ilr?.Where(o => o.Ukprn == provider.Ukprn).ToList();

            expectedPayments = SetProviderPaymentAccountIds(ilr, expectedPayments);
            var matcher = new ProviderPaymentEventMatcher(provider, CurrentCollectionPeriod, TestSession, expectedPayments);
            await WaitForIt(() => matcher.MatchPayments(), "Provider Payment event check failure");
        }

        private IEnumerable<string> PeriodisedValuesForBalancingAndCompletion()
        {
            yield return TransactionType.BalancingMathsAndEnglish.ToAttributeName();
            yield return TransactionType.Balancing16To18FrameworkUplift.ToAttributeName();
            yield return TransactionType.Completion16To18FrameworkUplift.ToAttributeName();
            yield return TransactionType.Completion.ToAttributeName();
            yield return TransactionType.Balancing.ToAttributeName();
        }

        protected List<Training> CreateTrainingFromLearners(long ukprn)
        {
            var trainings = new List<Training>();

            foreach (var aim in TestSession.Learners.Where(o => o.Ukprn == ukprn).SelectMany(l => l.Aims).ToList())
            {
                var firstPriceEpisode = aim.PriceEpisodes.First();

                var training = new Training
                {
                    AimReference = aim.AimReference,
                    AimSequenceNumber = aim.AimSequenceNumber,
                    ActualDuration = aim.ActualDuration,
                    CompletionStatus = aim.CompletionStatus.ToString(),
                    ContractType = firstPriceEpisode.ContractType,
                    FrameworkCode = aim.FrameworkCode,
                    FundingLineType = aim.FundingLineType,
                    LearnerId = aim.LearnerId,
                    PathwayCode = aim.PathwayCode,
                    PlannedDuration = aim.PlannedDuration,
                    ProgrammeType = aim.ProgrammeType,
                    SfaContributionPercentage = firstPriceEpisode.SfaContributionPercentage,
                    StandardCode = aim.StandardCode,
                    StartDate = aim.StartDate,
                    TotalAssessmentPrice = firstPriceEpisode.TotalAssessmentPrice,
                    TotalTrainingPrice = firstPriceEpisode.TotalTrainingPrice,
                    Uln = TestSession.GetLearner(ukprn, aim.LearnerId).Uln,
                    Ukprn = ukprn,
                    Pmr = firstPriceEpisode.Pmr,
                    CompletionHoldBackExemptionCode = firstPriceEpisode.CompletionHoldBackExemptionCode,
                };

                trainings.Add(training);
            }

            return trainings;
        }

        protected async Task GeneratePreviousPayment(Table table, long ukprn)
        {
            if (TestSession.AtLeastOneScenarioCompleted) return;

            var previousEarnings = PreviousEarnings.Where(s => s.Ukprn == ukprn).ToList();
            var previousIlr = PreviousIlr.Where(s => s.Ukprn == ukprn).ToList();

            var payments = table.CreateSet<ProviderPayment>().ToList();
            foreach (var payment in payments)
            {
                payment.Uln = TestSession.GetLearner(ukprn, payment.LearnerId).Uln;
                SetProviderPaymentAccountId(previousIlr, payment);
            }

            var previousJobId = TestSession.GenerateId();
            var previousSubmissionTime = DateTime.UtcNow.AddHours(-1);

            Console.WriteLine($"Previous job id: {previousJobId}");
            var previousPayments = payments.SelectMany(p =>
            {
                var learnerTraining = previousIlr;
                var learnerEarning = previousEarnings.First(e => e.LearnerId == p.LearnerId && e.DeliveryPeriod == p.DeliveryPeriod);
                decimal? sfaContributionPercentage = string.IsNullOrWhiteSpace(learnerEarning.SfaContributionPercentage) ?
                                                        default(decimal?) :
                                                        learnerEarning.SfaContributionPercentage.ToPercent();

                return CreatePayments(p, learnerTraining, previousJobId, previousSubmissionTime, sfaContributionPercentage, ukprn);
            }).ToList();

            var currentHistory = await DataContext.Payment.Where(p => p.Ukprn == ukprn).ToListAsync();

            previousPayments = previousPayments
                .Where(p => !currentHistory.Any(historicPayment =>
                    historicPayment.LearnerReferenceNumber == p.LearnerReferenceNumber &&
                    historicPayment.TransactionType == p.TransactionType &&
                    historicPayment.DeliveryPeriod == p.DeliveryPeriod))
                .ToList();

            DataContext.Payment.AddRange(previousPayments);
            await DataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        protected async Task ValidateRecordedProviderPayments(Table table, Provider provider)
        {
            var expectedPayments = table.CreateSet<ProviderPayment>()
                .Where(p => p.ParsedCollectionPeriod.Period == CurrentCollectionPeriod.Period &&
                            p.ParsedCollectionPeriod.AcademicYear == CurrentCollectionPeriod.AcademicYear)
                .ToList();

            var providerCurrentIlr = CurrentIlr?.Where(c => c.Ukprn == provider.Ukprn).ToList();

            expectedPayments = SetProviderPaymentAccountIds(providerCurrentIlr, expectedPayments);

            var providerLearners = TestSession.Learners?.Where(c => c.Ukprn == provider.Ukprn).ToList();
            var contractType = providerCurrentIlr == null
                ? providerLearners.First().Aims.First().PriceEpisodes.First().ContractType
                : providerCurrentIlr.First().ContractType;

            var matcher = new ProviderPaymentModelMatcher(provider, DataContext, TestSession, CurrentCollectionPeriod, expectedPayments, contractType);
            await WaitForIt(() => matcher.MatchPayments(), "Recorded payments check failed");
        }

        private List<ProviderPayment> SetProviderPaymentAccountIds(List<Training> ilr, List<ProviderPayment> expectedPayments)
        {
            expectedPayments?.ForEach(p => { SetProviderPaymentAccountId(ilr, p); });
            return expectedPayments;
        }

        private void SetProviderPaymentAccountId(List<Training> ilr, ProviderPayment expectedPayment)
        {
            var contractType = EnumHelper.GetContractType(ilr, CurrentPriceEpisodes);
            if (contractType == Model.Core.Entities.ContractType.Act1)
            {
                expectedPayment.AccountId = string.IsNullOrWhiteSpace(expectedPayment.Employer)
                               ? TestSession.Employer.AccountId
                               : TestSession.GetEmployer(expectedPayment.Employer).AccountId;
            }
        }

        protected async Task GeneratedAndValidateEarnings(Table table, Provider provider)
        {
            var earnings = CreateEarnings(table, provider.Ukprn);
            var learners = new List<FM36Learner>();
            var providerCurrentIlrs = CurrentIlr?.Where(o => o.Ukprn == provider.Ukprn).ToList();

            if (providerCurrentIlrs != null)
            {
                foreach (var training in providerCurrentIlrs)
                {
                    var aim = new Aim(training);
                    AddTestAims(new List<Aim> { aim }, provider.Ukprn);

                    if (CurrentPriceEpisodes == null)
                    {
                        aim.PriceEpisodes.Add(new Price
                        {
                            AimSequenceNumber = training.AimSequenceNumber,
                            TotalAssessmentPrice = training.TotalAssessmentPrice,
                            TotalTrainingPrice = training.TotalTrainingPrice,
                            TotalTrainingPriceEffectiveDate = training.TotalTrainingPriceEffectiveDate,
                            TotalAssessmentPriceEffectiveDate = training.TotalAssessmentPriceEffectiveDate,
                            SfaContributionPercentage = training.SfaContributionPercentage,
                            CompletionHoldBackExemptionCode = training.CompletionHoldBackExemptionCode,
                            Pmr = training.Pmr,
                        });
                    }
                    else
                    {
                        foreach (var currentPriceEpisode in CurrentPriceEpisodes)
                        {
                            if (currentPriceEpisode.AimSequenceNumber == 0 ||
                                currentPriceEpisode.AimSequenceNumber == aim.AimSequenceNumber)
                            {
                                aim.PriceEpisodes.Add(currentPriceEpisode);
                            }
                        }
                    }

                   

                }
            }

            // Learner -> Aims -> Price Episodes
            foreach (var testSessionLearner in TestSession.Learners.Where(l => l.Ukprn == provider.Ukprn))
            {
                var learner = new FM36Learner { LearnRefNumber = testSessionLearner.LearnRefNumber };
                var learnerEarnings = earnings.Where(e => e.LearnerId == testSessionLearner.LearnerIdentifier)
                    .ToList();

                if (learnerEarnings.Any())
                {
                    PopulateLearner(learner, testSessionLearner, learnerEarnings);
                    learners.Add(learner);
                }
            }
            var dcHelper = Scope.Resolve<DcHelper>();
            await dcHelper.SendLearnerCommands(learners, provider.Ukprn, AcademicYear, CollectionPeriod,
                provider.JobId, provider.IlrSubmissionTime);

            var matcher = new EarningEventMatcher(provider, CurrentPriceEpisodes, providerCurrentIlrs, earnings,
                TestSession, CurrentCollectionPeriod, learners);
            await WaitForIt(() => matcher.MatchPayments(), "Earning event check failure");
        }

        protected async Task HandleIlrReSubmissionForTheLearners(string collectionPeriodText, Provider provider)
        {
            var collectionPeriod = new CollectionPeriodBuilder().WithSpecDate(collectionPeriodText).Build();

            if (ProvidersWithCacheCleared == null)
            {
                ProvidersWithCacheCleared = new HashSet<(byte period, int academicYear, long)>();

                TestSession.Providers.ForEach(p =>
                {
                    ProvidersWithCacheCleared.Add((collectionPeriod.Period, collectionPeriod.AcademicYear, p.Ukprn));
                });
            }

            if (!ProvidersWithCacheCleared.Contains((collectionPeriod.Period, collectionPeriod.AcademicYear, provider.Ukprn)))
            {
                await RequiredPaymentsCacheCleaner.ClearCaches(provider, TestSession).ConfigureAwait(false);
                
                ProvidersWithCacheCleared.Add((collectionPeriod.Period, collectionPeriod.AcademicYear, provider.Ukprn));
            }

            SetCollectionPeriod(collectionPeriodText);
        }

        protected async Task ValidateRequiredPaymentsAtMonthEnd(Table table, Provider provider)
        {
            await MatchRequiredPayments(table, provider);
            await SendLevyMonthEnd();
        }

        protected async Task SendLevyMonthEnd()
        {
            var monthEndJobId = TestSession.GenerateId();
            var submissionDate = DateTime.UtcNow;
            Console.WriteLine($"Month end job id: {monthEndJobId}");

            foreach (var employer in TestSession.Employers)
            {
                var processLevyFundsAtMonthEndCommand = new ProcessLevyPaymentsOnMonthEndCommand
                {
                    JobId = monthEndJobId,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = AcademicYear, Period = CollectionPeriod },
                    RequestTime = DateTime.Now,
                    SubmissionDate = submissionDate,
                    AccountId = employer.AccountId,
                };

                await MessageSession.Send(processLevyFundsAtMonthEndCommand).ConfigureAwait(false);
            }

            TestSession.Providers.ForEach(p =>
            {
                p.JobId = monthEndJobId;
                p.MonthEndJobIdGenerated = true;
            });
        }
    }
}