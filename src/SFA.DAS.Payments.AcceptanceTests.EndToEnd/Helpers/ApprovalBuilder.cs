using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers
{
    public class ApprovalBuilder
    {
        private ApprenticeshipModel approval;

        //todo this need to be refactored to take in an FM36Learner and match all the values in there
        public ApprovalBuilder BuildSimpleApproval(TestSession session, LearningDeliveryValues learningDeliveryValues)
        {
            if (approval == null) approval = new ApprenticeshipModel();
            approval.Id = session.GenerateId();
            approval.Ukprn = session.Provider.Ukprn;
            approval.AccountId = session.Employer.AccountId;
            approval.Uln = session.Learner.Uln;
            approval.StandardCode = learningDeliveryValues.StdCode ?? 0;
            approval.ProgrammeType = learningDeliveryValues.ProgType;
            approval.Status = ApprenticeshipStatus.Active;
            approval.LegalEntityName = session.Employer.AccountName;
            approval.EstimatedStartDate = new DateTime(2018, 08, 01);
            approval.EstimatedEndDate = new DateTime(2019, 08, 06);
            approval.AgreedOnDate = DateTime.UtcNow;
            approval.FrameworkCode = learningDeliveryValues.FworkCode;
            approval.PathwayCode = learningDeliveryValues.PwayCode;

            return this;
        }

        public ApprovalBuilder WithALevyPayingEmployer()
        {
            approval.IsLevyPayer = true;
            approval.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy;

            return this;
        }

        public ApprovalBuilder WithApprenticeshipPriceEpisode(PriceEpisodeValues fm36PriceEpisodeValues)
        {
            if (approval == null) approval = new ApprenticeshipModel();
            if (approval.ApprenticeshipPriceEpisodes == null) approval.ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>();

            approval.ApprenticeshipPriceEpisodes.Add(new ApprenticeshipPriceEpisodeModel
            {
                ApprenticeshipId = approval.Id,
                Cost = fm36PriceEpisodeValues.PriceEpisodeTotalTNPPrice.GetValueOrDefault(),
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