using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers
{
    public class ApprenticeshipHelper
    {
        public static async Task AddApprenticeships(Apprenticeship apprenticeshipSpec, List<Apprenticeship> testSessionApprenticeships, IPaymentsDataContext dataContext, TestSession testSession)
        {
            var apprenticeshipModel = CreateApprenticeshipModels(apprenticeshipSpec, testSession);

            apprenticeshipModel.ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
            {
                CreateApprenticeshipPriceEpisodes(apprenticeshipSpec)
            };
            await dataContext.Apprenticeship.AddAsync(apprenticeshipModel).ConfigureAwait(false);
            testSessionApprenticeships.Add(apprenticeshipSpec);

            await dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public static async Task UpdateApprenticeships(long currentApprenticeshipId, Apprenticeship apprenticeshipSpec, IPaymentsDataContext dataContext)
        {
            var savedApprenticeship = await dataContext.Apprenticeship.SingleAsync(x => x.Id == currentApprenticeshipId).ConfigureAwait(false);
            savedApprenticeship.Status = apprenticeshipSpec.Status.ToApprenticeshipPaymentStatus();

            var currentEpisodes = await dataContext.ApprenticeshipPriceEpisode
                .Where(x => x.ApprenticeshipId == currentApprenticeshipId)
                .ToListAsync().ConfigureAwait(false);

            currentEpisodes.ForEach(x => x.Removed = true);

            apprenticeshipSpec.CommitmentId = currentApprenticeshipId;
            var newPriceEpisodes = CreateApprenticeshipPriceEpisodes(apprenticeshipSpec);

            await dataContext.ApprenticeshipPriceEpisode.AddAsync(newPriceEpisodes).ConfigureAwait(false);
            await dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private static ApprenticeshipModel CreateApprenticeshipModels(Apprenticeship apprenticeshipSpec, TestSession testSession)
        {
            if (apprenticeshipSpec.CommitmentId == default(long)) apprenticeshipSpec.CommitmentId = testSession.GenerateId();

            if (apprenticeshipSpec.Ukprn == default(long))
            {
                if (string.IsNullOrWhiteSpace(apprenticeshipSpec.Provider))
                {
                    apprenticeshipSpec.Ukprn = testSession.Ukprn;
                }
                else
                {
                    apprenticeshipSpec.Ukprn = testSession.GetProviderByIdentifier(apprenticeshipSpec.Provider).Ukprn;
                }
            }

            if (apprenticeshipSpec.AccountId == default(long))
                apprenticeshipSpec.AccountId = testSession.GetEmployer(apprenticeshipSpec.Employer).AccountId;

            if (apprenticeshipSpec.Uln == default(long))
            {
                var learnerId = string.IsNullOrWhiteSpace(apprenticeshipSpec.Identifier)
                    ? testSession.Learner.LearnerIdentifier
                    : apprenticeshipSpec.LearnerId;

                apprenticeshipSpec.Uln = testSession.GetLearner(apprenticeshipSpec.Ukprn, learnerId).Uln;
            }


            var apprenticeshipModel = new ApprenticeshipModel
            {
                Id = apprenticeshipSpec.CommitmentId,
                Ukprn = apprenticeshipSpec.Ukprn,
                AccountId = apprenticeshipSpec.AccountId,
                Uln = apprenticeshipSpec.Uln,
                FrameworkCode = apprenticeshipSpec.FrameworkCode,
                ProgrammeType = apprenticeshipSpec.ProgrammeType,
                PathwayCode = apprenticeshipSpec.PathwayCode,
                Priority = apprenticeshipSpec.Priority,
                Status = apprenticeshipSpec.Status.ToApprenticeshipPaymentStatus(),
                LegalEntityName = "Test SFA",
                EstimatedStartDate = apprenticeshipSpec.StartDate.ToDate(),
                EstimatedEndDate = apprenticeshipSpec.EndDate.ToDate(),
                AgreedOnDate = DateTime.UtcNow,
                StandardCode = apprenticeshipSpec.StandardCode
            };

            return apprenticeshipModel;
        }

        private static ApprenticeshipPriceEpisodeModel CreateApprenticeshipPriceEpisodes(Apprenticeship apprenticeshipSpec)
        {
            var startDate = string.IsNullOrWhiteSpace(apprenticeshipSpec.EffectiveFrom)
                ? apprenticeshipSpec.StartDate.ToDate()
                : apprenticeshipSpec.EffectiveFrom.ToDate();

            var endDate = string.IsNullOrWhiteSpace(apprenticeshipSpec.EffectiveTo)
                ? default(DateTime?)
                : apprenticeshipSpec.EffectiveTo.ToDate();

            return new ApprenticeshipPriceEpisodeModel
            {
                ApprenticeshipId = apprenticeshipSpec.CommitmentId,
                Cost = apprenticeshipSpec.AgreedPrice,
                StartDate = startDate,
                EndDate = endDate
            };

        }
    }
}
