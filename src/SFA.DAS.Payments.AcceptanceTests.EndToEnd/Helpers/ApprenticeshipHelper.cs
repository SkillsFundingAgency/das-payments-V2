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
    public static class ApprenticeshipHelper
    {
        public static async Task AddApprenticeship(ApprenticeshipModel apprenticeship, IPaymentsDataContext dataContext)
        {
            await dataContext.Apprenticeship.AddAsync(apprenticeship).ConfigureAwait(false);
            await dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public static async Task UpdateApprenticeship(long apprenticeshipId, ApprenticeshipStatus status, List<ApprenticeshipPriceEpisodeModel> priceEpisodes, IPaymentsDataContext dataContext)
        {
            var apprenticeship = await dataContext.Apprenticeship
                                     .Include(a => a.ApprenticeshipPriceEpisodes)
                                     .FirstOrDefaultAsync(a => a.Id == apprenticeshipId) 
                                 ?? throw new InvalidOperationException($"Apprenticeship not found: {apprenticeshipId}");

            apprenticeship.Status = status;
            apprenticeship.ApprenticeshipPriceEpisodes.ForEach(priceEpisode => priceEpisode.Removed = true);
            apprenticeship.ApprenticeshipPriceEpisodes.AddRange(priceEpisodes);
            await dataContext.SaveChangesAsync().ConfigureAwait(false);
        }
        
        public static ApprenticeshipModel CreateApprenticeshipModel(Apprenticeship apprenticeshipSpec, TestSession testSession)
        {
            if (apprenticeshipSpec.ApprenticeshipId == default(long)) apprenticeshipSpec.ApprenticeshipId = testSession.GenerateId();

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

            if (apprenticeshipSpec.SenderAccountId == default(long))
                apprenticeshipSpec.SenderAccountId = testSession.GetEmployer(apprenticeshipSpec.SendingEmployer).AccountId;
            if (apprenticeshipSpec.Uln == default(long))
            {
                var learnerId = string.IsNullOrWhiteSpace(apprenticeshipSpec.Identifier)
                    ? testSession.Learner.LearnerIdentifier
                    : apprenticeshipSpec.LearnerId;

                apprenticeshipSpec.Uln = testSession.GetLearner(apprenticeshipSpec.Ukprn, learnerId).Uln;
            }


            var apprenticeshipModel = new ApprenticeshipModel
            {
                Id = apprenticeshipSpec.ApprenticeshipId,
                Ukprn = apprenticeshipSpec.Ukprn,
                AccountId = apprenticeshipSpec.AccountId,
                TransferSendingEmployerAccountId = apprenticeshipSpec.SenderAccountId,
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

        public static ApprenticeshipPriceEpisodeModel CreateApprenticeshipPriceEpisode(Apprenticeship apprenticeshipSpec)
        {
            var startDate = string.IsNullOrWhiteSpace(apprenticeshipSpec.EffectiveFrom)
                ? apprenticeshipSpec.StartDate.ToDate()
                : apprenticeshipSpec.EffectiveFrom.ToDate();

            var endDate = string.IsNullOrWhiteSpace(apprenticeshipSpec.EffectiveTo)
                ? default(DateTime?)
                : apprenticeshipSpec.EffectiveTo.ToDate();

            return new ApprenticeshipPriceEpisodeModel
            {
                ApprenticeshipId = apprenticeshipSpec.ApprenticeshipId,
                Cost = apprenticeshipSpec.AgreedPrice,
                StartDate = startDate,
                EndDate = endDate
            };

        }
    }
}
