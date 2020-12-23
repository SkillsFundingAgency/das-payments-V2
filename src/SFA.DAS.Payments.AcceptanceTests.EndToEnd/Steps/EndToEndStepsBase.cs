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
using SFA.DAS.Payments.AcceptanceTests.Core.Services;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;
using Payment = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.Payment;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Client;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public abstract class EndToEndStepsBase : StepsBase
    {
        public bool NewFeature
        {
            get => Get<bool>("new_feature");
            set => Set(value, "new_feature");
        }

        protected IPaymentsDataContext DataContext => Scope.Resolve<IPaymentsDataContext>();

        protected IMapper Mapper => Scope.Resolve<IMapper>();

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
        protected List<EmployerProviderPriorityModel> EmployerProviderPriorities
        {
            get => !Context.TryGetValue<List<EmployerProviderPriorityModel>>(out var employerProviderPriorities) ? null : employerProviderPriorities;
            set => Set(value);
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
            if (!TestSession.AtLeastOneScenarioCompleted)
            {
                var ilr = table.CreateSet<Training>().ToList();
                AddTestLearners(ilr, ukprn);

                if (CurrentIlr == null)
                    CurrentIlr = new List<Training>();

                if (Config.ValidateDcAndDasServices)
                    CurrentIlr.Clear();
                CurrentIlr.AddRange(ilr);
            }
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
                learner.Course.AimSeqNumber = (short) ilrLearner.AimSequenceNumber;
                learner.Course.StandardCode = ilrLearner.StandardCode;
                learner.Course.FundingLineType = ilrLearner.FundingLineType;
                learner.Course.LearnAimRef = ilrLearner.AimReference;
                learner.Course.CompletionStatus = ilrLearner.CompletionStatus;
                learner.Course.ProgrammeType = ilrLearner.ProgrammeType;
                learner.Course.FrameworkCode = ilrLearner.FrameworkCode;
                learner.Course.PathwayCode = ilrLearner.PathwayCode;
                learner.SmallEmployer = ilrLearner.SmallEmployer;
                learner.PostcodePrior = ilrLearner.PostcodePrior;

                if (ilrLearner.Uln != default(long))
                {
                    learner.Uln = ilrLearner.Uln;
                }
                else
                {
                    ilrLearner.Uln = learner.Uln;
                }
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

                if (learner.Aims.All(x => x.CompletionStatus != CompletionStatus.PlannedBreak))
                {
                    learner.Aims.Clear();
                }

                learner.Aims.AddRange(learnerAims);
            }
        }

        protected async Task AddOrUpdateTestApprenticeships(List<Apprenticeship> apprenticeshipSpecs)

        {
            if (Apprenticeships == null) Apprenticeships = new List<Apprenticeship>();

            var groupedApprenticeships = apprenticeshipSpecs
                .GroupBy(a => a.Identifier)
                .ToList();

            var dataContext = Scope.Resolve<TestPaymentsDataContext>();

            foreach (var group in groupedApprenticeships)
            {
                var specApprenticeship = Apprenticeships.FirstOrDefault(a => a.Identifier == group.Key);
                if (specApprenticeship == null)
                {
                    //use last apprenticeship to make sure it picks up the most recent status
                    specApprenticeship = group.Last();
                    var isNew = specApprenticeship.ApprenticeshipId == default(long);

                    var apprenticeship = ApprenticeshipHelper.CreateApprenticeshipModel(specApprenticeship, TestSession);
                    apprenticeship.ApprenticeshipPriceEpisodes = group.Select(ApprenticeshipHelper.CreateApprenticeshipPriceEpisode).ToList();

                    if (isNew)
                        await dataContext.ClearApprenticeshipData(apprenticeship.Id, apprenticeship.Uln).ConfigureAwait(false);

                    await ApprenticeshipHelper.AddApprenticeship(apprenticeship, dataContext).ConfigureAwait(false);
                    specApprenticeship.ApprenticeshipId = apprenticeship.Id;
                    Apprenticeships.Add(specApprenticeship);
                }
                else
                {
                    var priceEpisodes = group.Select(ApprenticeshipHelper.CreateApprenticeshipPriceEpisode).ToList();
                    var status = group.Last().Status?.ToApprenticeshipPaymentStatus() ??
                                 throw new InvalidOperationException($"last item not found: {group.Key}");

                    await ApprenticeshipHelper.UpdateApprenticeship(specApprenticeship.ApprenticeshipId, status, priceEpisodes, dataContext)
                        .ConfigureAwait(false);
                }
            }

            await dataContext.SaveChangesAsync().ConfigureAwait(false);

            //check for duplicate apprenticeships
            await HandleDuplicateApprenticeships(dataContext).ConfigureAwait(false);
        }

        private async Task HandleDuplicateApprenticeships(IPaymentsDataContext dataContext)
        {
            foreach (var apprenticeship in Apprenticeships)
            {
                var duplicates = Apprenticeships
                    .Where(x => x.Uln == apprenticeship.Uln &&
                                x.ApprenticeshipId != apprenticeship.ApprenticeshipId &&
                                x.Ukprn != apprenticeship.Ukprn)
                    .ToList();

                foreach (var duplicate in duplicates)
                {
                    var duplicateApprenticeshipModel =
                        ApprenticeshipHelper.CreateApprenticeshipDuplicateModel(apprenticeship.Ukprn, duplicate);
                    await ApprenticeshipHelper.AddApprenticeshipDuplicate(duplicateApprenticeshipModel, dataContext)
                        .ConfigureAwait(false);
                }
            }
        }

        protected async Task UpdateApprenticeshipStatus(long apprenticeshipId, ApprenticeshipStatus status, string stoppedDateText)
        {
            var apprenticeship = await DataContext.Apprenticeship.FirstOrDefaultAsync(appr => appr.Id == apprenticeshipId);
            if (apprenticeship == null)
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
                await DataContext.LevyAccount.AddAsync(employer.ToModel());
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
            var onProgTraining = learnerTraining.FirstOrDefault(t => t.AimReference == "ZPROG001" && (string.IsNullOrWhiteSpace(t.LearnerId) || t.LearnerId.Equals(providerPayment.LearnerId)));
            var otherTraining = learnerTraining.FirstOrDefault(t => t.AimReference != "ZPROG001" && (string.IsNullOrWhiteSpace(t.LearnerId) || t.LearnerId.Equals(providerPayment.LearnerId)));
            var list = new List<PaymentModel>();
            if (providerPayment.SfaFullyFundedPayments > 0)
                if (providerPayment.TransactionType == TransactionType.LearningSupport)
                {
                    list.Add(CreatePaymentModel(providerPayment, onProgTraining, jobId, submissionTime, 1M,
                        providerPayment.SfaFullyFundedPayments, FundingSourceType.FullyFundedSfa, ukprn, providerPayment.AccountId, providerPayment.SendingAccountId));
                }
                else
                {
                    list.Add(CreatePaymentModel(providerPayment, otherTraining ?? onProgTraining, jobId, submissionTime, 1M,
                        providerPayment.SfaFullyFundedPayments, FundingSourceType.FullyFundedSfa, ukprn, providerPayment.AccountId, providerPayment.SendingAccountId));
                }

            if (providerPayment.EmployerCoFundedPayments > 0)
                list.Add(CreatePaymentModel(providerPayment, onProgTraining, jobId, submissionTime,
                    sfaContributionPercentage, providerPayment.EmployerCoFundedPayments, FundingSourceType.CoInvestedEmployer, ukprn, providerPayment.AccountId, providerPayment.SendingAccountId));

            if (providerPayment.SfaCoFundedPayments > 0)
                list.Add(CreatePaymentModel(providerPayment, onProgTraining, jobId, submissionTime,
                    sfaContributionPercentage, providerPayment.SfaCoFundedPayments, FundingSourceType.CoInvestedSfa, ukprn, providerPayment.AccountId, providerPayment.SendingAccountId));

            if (providerPayment.LevyPayments > 0)
            {
                var payment = CreatePaymentModel(providerPayment, onProgTraining, jobId, submissionTime,
                    sfaContributionPercentage, providerPayment.LevyPayments, FundingSourceType.Levy, ukprn, providerPayment.AccountId, providerPayment.SendingAccountId);
                list.Add(payment);
            }

            if (providerPayment.TransferPayments > 0)
            {
                var payment = CreatePaymentModel(providerPayment, onProgTraining, jobId, submissionTime,
                    sfaContributionPercentage, providerPayment.TransferPayments, FundingSourceType.Transfer, ukprn, providerPayment.AccountId, providerPayment.SendingAccountId);
                list.Add(payment);
            }
            return list;
        }

        private PaymentModel CreatePaymentModel(ProviderPayment providerPayment, Training learnerTraining, long jobId,
            DateTime submissionTime, decimal? sfaContributionPercentage, decimal amount,
            FundingSourceType fundingSourceType, long ukprn, long? accountId, long? senderAccountId)
        {
            var apprenticeshipId = Apprenticeships?.FirstOrDefault(x => x.LearnerId == learnerTraining.LearnerId)?.ApprenticeshipId;
            var priceEpisodeId = CurrentPriceEpisodes
                ?.FirstOrDefault(x => x.LearnerId == learnerTraining.LearnerId)
                ?.PriceEpisodeId.ParseAsNullableLong();

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
                TransferSenderAccountId = senderAccountId,
                StartDate = DateTime.UtcNow,
                PlannedEndDate = DateTime.UtcNow,
                ActualEndDate = DateTime.UtcNow,
                CompletionStatus = 1,
                CompletionAmount = 100M,
                InstalmentAmount = 200M,
                NumberOfInstalments = 12,
                ReportingAimFundingLineType = learnerTraining.FundingLineType,
                ApprenticeshipEmployerType = providerPayment.IsEmployerLevyPayer ? ApprenticeshipEmployerType.Levy : ApprenticeshipEmployerType.NonLevy,
                ApprenticeshipId = apprenticeshipId,
                ApprenticeshipPriceEpisodeId = priceEpisodeId,
                LearningStartDate = learnerTraining.StartDate.ToNullableDate(),
                RequiredPaymentEventId = Guid.Empty
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
                    learningDelivery.LearningDeliveryPeriodisedValues = SetLearningSupportValues(aim, earnings);
                }
                else // maths & english & Learning support don't use price episodes
                {
                    learningDelivery.LearningDeliveryPeriodisedValues = SetPeriodisedValues<LearningDeliveryPeriodisedValues>(aim, earnings);
                }

                learningDelivery.LearningDeliveryPeriodisedTextValues = SetPeriodisedTextValues(aim, earnings);
            }
        }

        private List<LearningDeliveryPeriodisedValues> SetLearningSupportValues(Aim aim, IList<Earning> earnings)
        {
            var aimPeriodisedValues = new List<LearningDeliveryPeriodisedValues>();
            foreach (var earning in earnings.Where(x => x.AimSequenceNumber == aim.AimSequenceNumber))
            {
                var period = earning.DeliveryCalendarPeriod;
                foreach (var earningValue in earning.Values.Where(v => v.Key == TransactionType.LearningSupport))
                {
                    var earningKey = earningValue.Key.ToAttributeName(!aim.IsMainAim);

                    var periodisedValues = aimPeriodisedValues.SingleOrDefault(v => v.AttributeName == earningKey);
                    if (periodisedValues == null)
                    {
                        periodisedValues = new LearningDeliveryPeriodisedValues { AttributeName = earningKey };
                        aimPeriodisedValues.Add(periodisedValues);
                    }

                    SetPeriodValue(period, periodisedValues, earningValue.Value);
                }
            }

            return aimPeriodisedValues;
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
                    .FirstOrDefault(e => e.DeliveryCalendarPeriod >= priceEpisodeStartDateAsDeliveryPeriod && e.PriceEpisodeIdentifier == priceEpisode.PriceEpisodeId);

                var sfaContributionPercent = (firstEarningForPriceEpisode?.SfaContributionPercentage ??
                                  priceEpisode.SfaContributionPercentage).ToPercent();

                var newPriceEpisode = new PriceEpisode
                {
                    PriceEpisodeIdentifier = id,
                    PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>(),
                    PriceEpisodeValues = new PriceEpisodeValues(),
                };

                // price episodes cannot span across academic year boundary
                var episodeStartDate = priceEpisode.EpisodeEffectiveStartDate;
                var academicYearStart = new DateTime(AcademicYear / 100 + 2000, 8, 1);
                if (episodeStartDate < academicYearStart) episodeStartDate = academicYearStart;

                newPriceEpisode.PriceEpisodeValues.PriceEpisodeAimSeqNumber = CalculateAimSequenceNumber(priceEpisode);
                newPriceEpisode.PriceEpisodeValues.EpisodeStartDate = episodeStartDate;
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
                    var aimEarnings = earnings.Where(x => x.AimSequenceNumber == aim.AimSequenceNumber || x.AimSequenceNumber == null).ToList();

                    // price episodes not covering the whole year are likely to be one of many, copy values only for current episode, set zero for others
                    if (episodeStart.AcademicYear == AcademicYear && (episodeStart.Period > 1 || episodeLastPeriod < 12))
                    {
                        newValues = new PriceEpisodePeriodisedValues { AttributeName = currentValues.AttributeName };

                        for (var p = 1; p < 13; p++)
                        {
                            var earningRow = p - 1;

                            var amount = p >= episodeStart.Period && p <= episodeLastPeriod ||
                                         (PeriodisedValuesForBalancingAndCompletion()
                                              .Contains(currentValues.AttributeName) ||
                                          PeriodisedValuesForIncentives().Contains(currentValues.AttributeName)) &&
                                         p > episodeLastPeriod
                                ? currentValues.GetValue(p)
                                : 0;

                            var earningPriceEpisodeIdentifier = aimEarnings[earningRow].PriceEpisodeIdentifier;
                            var currentPriceEpisodeIdentifier = currentPriceEpisode.PriceEpisodeIdentifier;
                            if (!string.IsNullOrWhiteSpace(earningPriceEpisodeIdentifier) &&
                                earningPriceEpisodeIdentifier ==
                                currentPriceEpisodeIdentifier)
                            {
                                newValues.SetValue(p, amount);
                            }
                            else
                            {
                                if (earningRow < aimEarnings.Count &&
                                    !string.IsNullOrWhiteSpace(earningPriceEpisodeIdentifier))
                                {
                                    if (earningPriceEpisodeIdentifier !=
                                        currentPriceEpisodeIdentifier)
                                    {
                                        amount = decimal.Zero;
                                    }
                                }

                                newValues.SetValue(p, amount);
                            }
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

                return GetContractTypeDescription(contractType);
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

        private static string GetContractTypeDescription(ContractType contractType)
        {
            return contractType == ContractType.Act1 ? "Contract for services with the employer" : "Contract for services with the ESFA";
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

            var currentEarnings = earnings.Where(e => !e.AimSequenceNumber.HasValue ||
                                                  e.AimSequenceNumber == aim.AimSequenceNumber).ToList();

            if (currentEarnings.Any())
            {
                foreach (var earning in currentEarnings)
                {
                    var period = earning.DeliveryCalendarPeriod;
                    foreach (var earningValue in earning.Values)
                    {
                        if (!aim.IsMainAim && EnumHelper.IsOnProgType(earningValue.Key))
                        {
                            continue;
                        }

                        var earningKey = earningValue.Key.ToAttributeName(aim.IsMainAim);

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
            }
            else
            {
                var periodisedValues = new T { AttributeName = "MathEngOnProgPayment" };
                aimPeriodisedValues.Add(periodisedValues);
                for (byte i = 1; i < 13; i++)
                {
                    SetPeriodValue(i, periodisedValues, 0);
                }
            }

            return aimPeriodisedValues;
        }

        private static List<LearningDeliveryPeriodisedTextValues> SetPeriodisedTextValues(Aim aim, IList<Earning> earnings)
        {
            var aimPeriodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>();
            const string learningDeliveryContractType = "LearnDelContType";
            const string learningDeliveryFundingLineType = "FundLineType";
            var currentEarnings = earnings.Where(e => !e.AimSequenceNumber.HasValue ||
                                                      e.AimSequenceNumber == aim.AimSequenceNumber).ToList();

            if (currentEarnings.Any())
            {
                foreach (var earning in currentEarnings)
                {
                    var period = earning.DeliveryCalendarPeriod;
                    foreach (var earningValue in earning.Values)
                    {
                        if (MathsAndEnglishTransactionTypes().Contains(earningValue.Key))
                        {
                            AddPeriodisedTextAttributes(aimPeriodisedTextValues, learningDeliveryContractType, period, GetContractTypeDescription(earning.ContractType));
                            AddPeriodisedTextAttributes(aimPeriodisedTextValues, learningDeliveryFundingLineType, period, aim.FundingLineType);
                        }
                    }
                }
            }
            else
            {
                for (byte i = 1; i < 13; i++)
                {
                    AddPeriodisedTextAttributes(aimPeriodisedTextValues, learningDeliveryContractType, i, "None");
                    AddPeriodisedTextAttributes(aimPeriodisedTextValues, learningDeliveryFundingLineType, i, "None");
                }
            }

            return aimPeriodisedTextValues;
        }

        private static void AddPeriodisedTextAttributes(List<LearningDeliveryPeriodisedTextValues> aimPeriodisedTextValues,
            string attributeName,
            byte period,
            string valueToSet)
        {
            var periodisedTextValues = aimPeriodisedTextValues.SingleOrDefault(v => v.AttributeName == attributeName);

            if (periodisedTextValues == null)
            {
                periodisedTextValues = new LearningDeliveryPeriodisedTextValues
                {
                    AttributeName = attributeName
                };
                aimPeriodisedTextValues.Add(periodisedTextValues);
            }

            SetPeriodTextValue(period, periodisedTextValues, valueToSet);
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

        private static void SetPeriodTextValue(int period, LearningDeliveryPeriodisedTextValues periodisedValues, string value)
        {
            var periodProperty = periodisedValues.GetType().GetProperty("Period" + period);
            periodProperty?.SetValue(periodisedValues, value);
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

                foreach (var headerCell in table.Header)
                {
                    var name = headerCell.Replace(" ", null).Replace("-", null);

                    if (!Enum.TryParse<NonPaymentReason>(name, true, out var nonPaymentReason))
                        continue;

                    if (!decimal.TryParse(tableRow[headerCell], out var amount))
                        continue;

                    payment.NonPaymentReasons.Add(nonPaymentReason, amount);
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

        protected async Task MatchHeldBackRequiredPayments(Table table, Provider provider)
        {
            var expectedPayments = CreatePayments(table, provider.Ukprn);
            var matcher = new CompletionPaymentHeldBackEventMatcher(provider, CurrentCollectionPeriod, expectedPayments);
            await WaitForIt(() => matcher.MatchPayments(), "Held Back Required Payment event check failure");
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

            var providerLearners = TestSession.Learners?.Where(c => c.Ukprn == provider.Ukprn).ToList();

            var contractType = GetContractType(expectedPayments, CurrentCollectionPeriod, ilr,
                providerLearners);

            if (contractType != ContractType.Act2)
            {
                expectedPayments = SetProviderPaymentAccountIds(ilr, expectedPayments);
            }

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

        private IEnumerable<string> PeriodisedValuesForIncentives()
        {
            yield return TransactionType.First16To18EmployerIncentive.ToAttributeName();
            yield return TransactionType.First16To18ProviderIncentive.ToAttributeName();
            yield return TransactionType.Second16To18EmployerIncentive.ToAttributeName();
            yield return TransactionType.Second16To18ProviderIncentive.ToAttributeName();
            yield return TransactionType.Second16To18ProviderIncentive.ToAttributeName();
            yield return TransactionType.OnProgramme16To18FrameworkUplift.ToAttributeName();
            yield return TransactionType.FirstDisadvantagePayment.ToAttributeName();
            yield return TransactionType.SecondDisadvantagePayment.ToAttributeName();
            yield return TransactionType.OnProgrammeMathsAndEnglish.ToAttributeName();
            yield return TransactionType.LearningSupport.ToAttributeName();
            yield return TransactionType.CareLeaverApprenticePayment.ToAttributeName();
        }

        private static IEnumerable<TransactionType> MathsAndEnglishTransactionTypes()
        {
            yield return TransactionType.OnProgrammeMathsAndEnglish;
            yield return TransactionType.BalancingMathsAndEnglish;
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

            await DataContext.Payment.AddRangeAsync(previousPayments);
            await DataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        protected async Task ValidateRecordedProviderPayments(Table table, Provider provider)
        {
            var expectedPayments = table.CreateSet<ProviderPayment>()
                .Where(p => p.ParsedCollectionPeriod.Period == CurrentCollectionPeriod.Period &&
                            p.ParsedCollectionPeriod.AcademicYear == CurrentCollectionPeriod.AcademicYear)
                .ToList();

            var providerCurrentIlr = CurrentIlr?.Where(c => c.Ukprn == provider.Ukprn).ToList();

            var providerLearners = TestSession.Learners?.Where(c => c.Ukprn == provider.Ukprn).ToList();
            var contractType = GetContractType(expectedPayments, CurrentCollectionPeriod, providerCurrentIlr,
                providerLearners);

            if (contractType != ContractType.Act2)
            {
                expectedPayments = SetProviderPaymentAccountIds(providerCurrentIlr, expectedPayments);
            }

            var matcher = new ProviderPaymentModelMatcher(provider, DataContext, TestSession, CurrentCollectionPeriod, expectedPayments, contractType);
            await WaitForIt(() => matcher.MatchPayments(), "Recorded payments check failed");
        }

        private ContractType GetContractType(List<ProviderPayment> expectedPayments, CollectionPeriod currentCollectionPeriod, List<Training> providerCurrentIlr, List<Learner> providerLearners)
        {
            if (expectedPayments.Any())
            {
                var priceEpisodeIdentifier = expectedPayments.First().PriceEpisodeIdentifier;

                if (!string.IsNullOrWhiteSpace(priceEpisodeIdentifier))
                {
                    var matchingPriceEpisode = CurrentPriceEpisodes.FirstOrDefault(p => p.PriceEpisodeId == priceEpisodeIdentifier);

                    if (matchingPriceEpisode != null)
                    {
                        return matchingPriceEpisode.ContractType;
                    }
                }

            }

            return providerCurrentIlr == null
                ? providerLearners.First().Aims.First().PriceEpisodes.First().ContractType
                : providerCurrentIlr.First().ContractType;
        }

        private List<ProviderPayment> SetProviderPaymentAccountIds(List<Training> ilr, List<ProviderPayment> expectedPayments)
        {
            expectedPayments?.ForEach(p => { SetProviderPaymentAccountId(ilr, p); });
            return expectedPayments;
        }

        private void SetProviderPaymentAccountId(List<Training> ilr, ProviderPayment expectedPayment)
        {
            var contractTypes = EnumHelper.GetContractTypes(ilr, CurrentPriceEpisodes);
            if (contractTypes != null && contractTypes.Any(x => x == ContractType.Act1))
            {
                expectedPayment.AccountId = string.IsNullOrWhiteSpace(expectedPayment.Employer)
                               ? TestSession.Employer.AccountId
                               : expectedPayment.Employer.ToLowerInvariant() == "no employer"
                                   ? default(long?)
                                   : TestSession.GetEmployer(expectedPayment.Employer).AccountId;
                expectedPayment.IsEmployerLevyPayer = string.IsNullOrWhiteSpace(expectedPayment.Employer)
                    ? TestSession.Employer.IsLevyPayer : TestSession.GetEmployer(expectedPayment.Employer).IsLevyPayer;

                expectedPayment.SendingAccountId = string.IsNullOrWhiteSpace(expectedPayment.SendingEmployer)
                    ? TestSession.Employer.AccountId
                    : TestSession.GetEmployer(expectedPayment.SendingEmployer).AccountId;
            }
        }

        protected async Task GeneratedAndValidateEarnings(Table table, Provider provider)
        {
            var earnings = CreateEarnings(table, provider.Ukprn);
            var learners = new List<FM36Learner>();
            var providerCurrentIlrs = CurrentIlr?.Where(o => o.Ukprn == provider.Ukprn).ToList();

            await GenerateEarnings(table, provider, earnings, learners, providerCurrentIlrs).ConfigureAwait(false);

            var matcher = new EarningEventMatcher(provider, CurrentPriceEpisodes, providerCurrentIlrs, earnings,
                TestSession, CurrentCollectionPeriod, learners);
            await WaitForIt(() => matcher.MatchPayments(), "Earning event check failure");
        }

        protected async Task GeneratedAndValidateEarningsOnRestart(Table table, Provider provider)
        {
            var earnings = CreateEarnings(table, provider.Ukprn);
            var learners = new List<FM36Learner>();
            var providerCurrentIlrs = CurrentIlr?.Where(o => o.Ukprn == provider.Ukprn).ToList();

            await GenerateEarnings(table, provider, earnings, learners, providerCurrentIlrs).ConfigureAwait(false);
        }

        protected async Task GenerateEarnings(Table table, Provider provider, List<Earning> earnings, List<FM36Learner> learners, List<Training> providerCurrentIlrs)
        {
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
                        foreach (var currentPriceEpisode in CurrentPriceEpisodes.Where(p => (string.IsNullOrWhiteSpace(p.Provider) || TestSession.GetProviderByIdentifier(p.Provider).Ukprn == provider.Ukprn)
                                                                                            && (string.IsNullOrWhiteSpace(p.LearnerId) || p.LearnerId == aim.LearnerId)))
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
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(learners, provider.Ukprn, AcademicYear, CollectionPeriod,
                provider.JobId);

        }

        protected async Task GenerateEarnings(Provider provider)
        {
            var table = new Table("Delivery Period", "On-Programme", "Completion", "Balancing", "Price Episode Identifier");
            table.AddRow("Aug/Current Academic Year", "1000", "0", "0", "pe-1");
            table.AddRow("Sep/Current Academic Year", "1000", "0", "0", "pe-1");
            table.AddRow("Oct/Current Academic Year", "1000", "0", "0", "pe-1");
            table.AddRow("Nov/Current Academic Year", "1000", "0", "0", "pe-1");
            table.AddRow("Dec/Current Academic Year", "1000", "0", "0", "pe-1");
            table.AddRow("Jan/Current Academic Year", "1000", "0", "0", "pe-1");
            table.AddRow("Feb/Current Academic Year", "1000", "0", "0", "pe-1");
            table.AddRow("Mar/Current Academic Year", "1000", "0", "0", "pe-1");
            table.AddRow("Apr/Current Academic Year", "1000", "0", "0", "pe-1");
            table.AddRow("May/Current Academic Year", "1000", "0", "0", "pe-1");
            table.AddRow("Jun/Current Academic Year", "1000", "0", "0", "pe-1");
            table.AddRow("Jul/Current Academic Year", "1000", "0", "0", "pe-1");
            await GenerateEarnings(table, TestSession.Provider).ConfigureAwait(false);
        }

        protected async Task GenerateEarnings(Table table, Provider provider)
        {
            var earnings = CreateEarnings(table, provider.Ukprn);
            var learners = new List<FM36Learner>();
            var providerCurrentIlrs = CurrentIlr?.Where(o => o.Ukprn == provider.Ukprn).ToList();
            await GenerateEarnings(table, provider, earnings, learners, providerCurrentIlrs).ConfigureAwait(false);
        }

        protected void AddTestLearners(Table table)
        {
            PreviousIlr = table.CreateSet<Training>().ToList();
            AddTestLearners(PreviousIlr, TestSession.Provider.Ukprn);
        }


        protected void CreatePreviousEarningsAndTraining(Table table)
        {
            PreviousEarnings = CreateEarnings(table, TestSession.Provider.Ukprn);
            // for new style specs where no ILR specified
            if (PreviousIlr == null)
            {
                PreviousIlr = CreateTrainingFromLearners(TestSession.Provider.Ukprn);
            }
        }

        protected Task HandleIlrReSubmissionForTheLearners(string collectionPeriodText, Provider provider)
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

            //TODO: Is this still required?
            if (!ProvidersWithCacheCleared.Contains((collectionPeriod.Period, collectionPeriod.AcademicYear, provider.Ukprn)))
            {

                ProvidersWithCacheCleared.Add((collectionPeriod.Period, collectionPeriod.AcademicYear, provider.Ukprn));
            }

            SetCollectionPeriod(collectionPeriodText);

            return Task.CompletedTask;
        }

        protected void AddPriceDetails(Table table)
        {
            if (TestSession.AtLeastOneScenarioCompleted)
            {
                return;
            }

            var newPriceEpisodes = table.CreateSet<Price>().ToList();
            CurrentPriceEpisodes = newPriceEpisodes;

            if (TestSession.Learners.Any(x => x.Aims.Count > 0))
            {
                foreach (var newPriceEpisode in newPriceEpisodes)
                {
                    Aim aim;
                    try
                    {
                        aim = TestSession.Learners.SelectMany(x => x.Aims)
                            .SingleOrDefault(x => x.AimSequenceNumber == newPriceEpisode.AimSequenceNumber);
                    }
                    catch (Exception)
                    {
                        throw new Exception("There are too many aims with the same sequence number");
                    }

                    if (aim == null)
                    {
                        throw new Exception("There is a price episode without a matching aim");
                    }

                    aim.PriceEpisodes.Add(newPriceEpisode);
                }
            }
        }

        protected async Task ValidateRequiredPaymentsAtMonthEnd(Table table, Provider provider)
        {
            await MatchRequiredPayments(table, provider);
            await Task.Delay(TimeSpan.FromSeconds(1));
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

        protected async Task AddLevyAccountPriorities(Table table, TestSession testSession, CollectionPeriod currentCollectionPeriod, IPaymentsDataContext dataContext)
        {
            if (EmployerProviderPriorities == null) EmployerProviderPriorities = new List<EmployerProviderPriorityModel>();

            var priorities = table.CreateSet<ProviderPriority>()
                .Select(x =>
                {
                    var period = new CollectionPeriodBuilder()
                        .WithSpecDate(x.SpecCollectionPeriod)
                        .Build();
                    return new
                    {
                        AcademicYear = period.AcademicYear,
                        CollectionPeriod = period.Period,
                        Priority = x.Priority,
                        Ukprn = testSession.GetProviderByIdentifier(x.ProviderIdentifier).Ukprn,
                    };
                })
                .Where(x => x.AcademicYear == currentCollectionPeriod.AcademicYear &&
                            x.CollectionPeriod == currentCollectionPeriod.Period)
                .Select(x => new EmployerProviderPriorityModel
                {
                    EmployerAccountId = testSession.GetEmployer(null).AccountId,
                    Order = x.Priority,
                    Ukprn = x.Ukprn,
                })
                .ToList();

            if (priorities.Any())
            {
                var existingRecords = EmployerProviderPriorities.Any(x => x.EmployerAccountId == priorities.First().EmployerAccountId);
                if (existingRecords)
                {
                    foreach (var employerProviderPriorityModel in priorities)
                    {
                        await MessageSession.Send(new EmployerChangedProviderPriority
                        {
                            EmployerAccountId = priorities.First().EmployerAccountId,
                            OrderedProviders = priorities.OrderBy(x => x.Order).Select(p => p.Ukprn).ToList()
                        }).ConfigureAwait(false);
                    }

                    await Task.Delay(2000);
                }
                else
                {
                    dataContext.EmployerProviderPriority.AddRange(priorities);
                    dataContext.SaveChanges();

                    EmployerProviderPriorities.AddRange(priorities);
                }
            }
        }

        protected async Task SubmitIlrInPeriod(string collectionPeriodText, FeatureNumber featureNumber)
        {
            Task ClearCache() => HandleIlrReSubmissionForTheLearners(collectionPeriodText, TestSession.Provider);
            await Scope.Resolve<IIlrService>().PublishLearnerRequest(CurrentIlr, TestSession.Learners, collectionPeriodText, featureNumber.Extract(), ClearCache);
        }

    }
}