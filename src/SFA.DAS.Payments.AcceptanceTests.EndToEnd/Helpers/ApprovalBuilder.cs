using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers
{
    public class ApprovalBuilder
    {
        private ApprenticeshipModel _approval;
        private FM36Learner _learner;
        private int _aimSeqNumber;

        //todo this need to be refactored to take in an FM36Learner and match all the values in there
        public ApprovalBuilder BuildSimpleApproval(TestSession session, FM36Learner learner, int aimSeqNumber)
        {
            if (_approval == null) _approval = new ApprenticeshipModel();
            _learner = learner;
            _aimSeqNumber = aimSeqNumber;

            _approval.Id = session.GenerateId();
            _approval.Ukprn = session.Provider.Ukprn;
            _approval.AccountId = session.Employer.AccountId;
            _approval.Uln = session.Learner.Uln;
            _approval.StandardCode = _learner.LearningDeliveries.Single(x => x.AimSeqNumber == _aimSeqNumber).LearningDeliveryValues.StdCode ?? 0;
            _approval.ProgrammeType = _learner.LearningDeliveries.Single(x => x.AimSeqNumber == _aimSeqNumber).LearningDeliveryValues.ProgType;
            _approval.Status = ApprenticeshipStatus.Active;
            _approval.LegalEntityName = session.Employer.AccountName;
            _approval.EstimatedStartDate = new DateTime(2018, 08, 01);
            _approval.EstimatedEndDate = new DateTime(2019, 08, 06);
            _approval.AgreedOnDate = DateTime.UtcNow;
            _approval.FrameworkCode = _learner.LearningDeliveries.Single(x => x.AimSeqNumber == _aimSeqNumber).LearningDeliveryValues.FworkCode;
            _approval.PathwayCode = _learner.LearningDeliveries.Single(x => x.AimSeqNumber == _aimSeqNumber).LearningDeliveryValues.PwayCode;

            return this;
        }

        public ApprovalBuilder WithALevyPayingEmployer()
        {
            _approval.IsLevyPayer = true;
            _approval.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy;

            return this;
        }

        public ApprovalBuilder WithApprenticeshipPriceEpisode()
        {
            if (_approval == null) _approval = new ApprenticeshipModel();
            if (_approval.ApprenticeshipPriceEpisodes == null) _approval.ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>();

            _approval.ApprenticeshipPriceEpisodes.Add(new ApprenticeshipPriceEpisodeModel
            {
                ApprenticeshipId = _approval.Id,
                //todo de-hardcode
                Cost = _learner.PriceEpisodes.Single(x => x.PriceEpisodeValues.PriceEpisodeAimSeqNumber == _aimSeqNumber && x.PriceEpisodeIdentifier == "PE-1").PriceEpisodeValues.PriceEpisodeTotalTNPPrice.GetValueOrDefault(),
                StartDate = new DateTime(2018, 08, 01),
                EndDate = new DateTime(2019, 07, 31)
            });

            return this;
        }

        public ApprovalBuilder WithExplicitId(long id)
        {
            if (_approval == null) _approval = new ApprenticeshipModel();

            _approval.Id = id;

            if (_approval.ApprenticeshipPriceEpisodes == null) return this;
            foreach (var episode in _approval.ApprenticeshipPriceEpisodes)
            {
                episode.ApprenticeshipId = id;
            }

            return this;
        }

        public ApprovalBuilder WithExplicitStartDate(DateTime startDate)
        {
            _approval.EstimatedStartDate = startDate;
            foreach (var episode in _approval.ApprenticeshipPriceEpisodes)
            {
                episode.StartDate = startDate;
            }
            return this;
        }

        public ApprenticeshipModel ToApprenticeshipModel()
        {
            return _approval;
        }
    }
}