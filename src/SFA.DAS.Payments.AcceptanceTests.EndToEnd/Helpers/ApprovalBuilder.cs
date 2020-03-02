using System;
using System.Collections.Generic;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers
{
    public class ApprovalBuilder
    {
        private ApprenticeshipModel approval;

        public ApprovalBuilder BuildSimpleLevyApproval(TestSession session, int? ukprn = null, long? uln = null)
        {
            if (approval == null) approval = new ApprenticeshipModel();

            approval.Id = session.GenerateId();
            approval.Ukprn = ukprn != null ? session.GetProviderByUkprn(ukprn.Value).Ukprn : session.GetProviderByIdentifier(Guid.NewGuid().ToString()).Ukprn;
            approval.AccountId = session.Employer.AccountId;
            approval.Uln = uln ?? session.GenerateId();
            approval.StandardCode = session.GenerateId();
            approval.ProgrammeType = 25;
            approval.Status = ApprenticeshipStatus.Active;
            approval.LegalEntityName = "Test SFA";
            approval.EstimatedStartDate = new DateTime(2018, 08, 01);
            approval.EstimatedEndDate = new DateTime(2019, 08, 06);
            approval.AgreedOnDate = DateTime.UtcNow;
            approval.IsLevyPayer = true;
            approval.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy;

            return this;
        }

        public ApprovalBuilder WithSimplePriceEpisode(TestSession session, long? id = null)
        {
            if (approval == null) approval = new ApprenticeshipModel();
            if (approval.ApprenticeshipPriceEpisodes == null) approval.ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>();

            approval.ApprenticeshipPriceEpisodes.Add(new ApprenticeshipPriceEpisodeModel
            {
                ApprenticeshipId = approval.Id,
                Id = id ?? session.GenerateId(),
                Cost = 15000,
                StartDate = new DateTime(2018, 08, 01),
                EndDate = new DateTime(2019, 07, 31)
            });

            return this;
        }

        public ApprovalBuilder WithExplicitId(long id)
        {
            if (approval == null) approval = new ApprenticeshipModel();

            approval.Id = id;

            if (approval.ApprenticeshipPriceEpisodes == null) return this;
            foreach (var episode in approval.ApprenticeshipPriceEpisodes)
            {
                episode.ApprenticeshipId = id;
            }

            return this;
        }

        public ApprovalBuilder WithExplicitStartDate(DateTime startDate)
        {
            approval.EstimatedStartDate = startDate;
            foreach (var episode in approval.ApprenticeshipPriceEpisodes)
            {
                episode.StartDate = startDate;
            }
            return this;
        }

        public ApprenticeshipModel ToApprenticeshipModel()
        {
            return approval;
        }
    }
}