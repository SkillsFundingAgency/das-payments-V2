using System;
using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers
{
    public class ApprenticeshipBuilder
    {
        private ApprenticeshipModel apprenticeship;

        public ApprenticeshipBuilder BuildSimpleApprenticeship(TestSession session, LearningDeliveryValues learningDeliveryValues, long id)
        {
            if (apprenticeship == null) apprenticeship = new ApprenticeshipModel();
            apprenticeship.Id = id;
            apprenticeship.Ukprn = session.Provider.Ukprn;
            apprenticeship.AccountId = session.Employer.AccountId;
            apprenticeship.Uln = session.Learner.Uln;
            apprenticeship.StandardCode = learningDeliveryValues.StdCode ?? 0;
            apprenticeship.ProgrammeType = learningDeliveryValues.ProgType;
            apprenticeship.Status = ApprenticeshipStatus.Active;
            apprenticeship.LegalEntityName = session.Employer.AccountName;
            apprenticeship.EstimatedStartDate = new DateTime(2019, 08, 01);
            apprenticeship.EstimatedEndDate = new DateTime(2020, 08, 06);
            apprenticeship.AgreedOnDate = DateTime.UtcNow;
            apprenticeship.FrameworkCode = learningDeliveryValues.FworkCode ?? 0;
            apprenticeship.PathwayCode = learningDeliveryValues.PwayCode ?? 0;

            return this;
        }

        public ApprenticeshipBuilder WithALevyPayingEmployer()
        {
            apprenticeship.IsLevyPayer = true;
            apprenticeship.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy;

            return this;
        }

        public ApprenticeshipBuilder WithApprenticeshipPriceEpisode(PriceEpisodeValues fm36PriceEpisodeValues)
        {
            if (apprenticeship == null) apprenticeship = new ApprenticeshipModel();
            if (apprenticeship.ApprenticeshipPriceEpisodes == null) apprenticeship.ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>();

            apprenticeship.ApprenticeshipPriceEpisodes.Add(new ApprenticeshipPriceEpisodeModel
            {
                ApprenticeshipId = apprenticeship.Id,
                Cost = fm36PriceEpisodeValues.PriceEpisodeTotalTNPPrice.GetValueOrDefault(),
                StartDate = new DateTime(2019, 08, 01),
                EndDate = new DateTime(2020, 07, 31)
            });

            return this;
        }

        public ApprenticeshipModel ToApprenticeshipModel()
        {
            return apprenticeship;
        }
    }
}