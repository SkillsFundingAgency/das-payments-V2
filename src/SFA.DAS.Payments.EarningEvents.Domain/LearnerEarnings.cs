using System;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Core.Validation;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Domain
{
    public class LearnerEarnings
    {
        private readonly ILearnerValidator learnerValidator;
        private readonly IApprenticeshipContractTypeEarningsEventBuilder apprenticeshipContractTypeEarningsEventBuilder;
        private readonly IFunctionalSkillEarningsEventBuilder functionalSkillEarningsEventBuilder;
        public ApprenticeshipContractTypeEarningsEvent ApprenticeshipContractTypeEarningsEvent { get; private set; }
        public FunctionalSkillEarningsEvent FunctionalSkillEarningsEvent { get; private set; }
        public LearnerEarnings(ILearnerValidator learnerValidator, IApprenticeshipContractTypeEarningsEventBuilder apprenticeshipContractTypeEarningsEventBuilder,
            IFunctionalSkillEarningsEventBuilder functionalSkillEarningsEventBuilder)
        {
            this.learnerValidator = learnerValidator ?? throw new ArgumentNullException(nameof(learnerValidator));
            this.apprenticeshipContractTypeEarningsEventBuilder = apprenticeshipContractTypeEarningsEventBuilder ?? throw new ArgumentNullException(nameof(apprenticeshipContractTypeEarningsEventBuilder));
            this.functionalSkillEarningsEventBuilder = functionalSkillEarningsEventBuilder ?? throw new ArgumentNullException(nameof(functionalSkillEarningsEventBuilder));
        }

        public ValidationResult GenerateEarnings(int ukprn, string collectionYear, string jobId, FM36Learner learner)
        {
            var validationResult = learnerValidator.Validate(learner);
            if (validationResult.Failed)
                return validationResult;

            ApprenticeshipContractTypeEarningsEvent =
                apprenticeshipContractTypeEarningsEventBuilder.Build(ukprn, collectionYear, jobId, learner);

            FunctionalSkillEarningsEvent =
                functionalSkillEarningsEventBuilder.Build(ukprn, collectionYear, jobId, learner);

            return validationResult;
        }
    }
}