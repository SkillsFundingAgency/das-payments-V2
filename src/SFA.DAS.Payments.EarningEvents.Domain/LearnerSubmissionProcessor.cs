using System;
using SFA.DAS.Payments.Core.Validation;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Domain
{
    public class LearnerSubmissionProcessor
    {
        private readonly ILearnerValidator learnerValidator;
        private readonly IApprenticeshipContractTypeEarningsEventBuilder apprenticeshipContractTypeEarningsEventBuilder;
        public LearnerSubmissionProcessor(ILearnerValidator learnerValidator, IApprenticeshipContractTypeEarningsEventBuilder apprenticeshipContractTypeEarningsEventBuilder)
        {
            this.learnerValidator = learnerValidator ?? throw new ArgumentNullException(nameof(learnerValidator));
            this.apprenticeshipContractTypeEarningsEventBuilder = apprenticeshipContractTypeEarningsEventBuilder ?? throw new ArgumentNullException(nameof(apprenticeshipContractTypeEarningsEventBuilder));
        }

        public (ValidationResult Validation, ApprenticeshipContractTypeEarningsEvent EarningsEvent)  GenerateEarnings(IIlrLearnerSubmission learnerSubmission)
        {
            var validationResult = learnerValidator.Validate(learnerSubmission.Learner);
            if (validationResult.Failed)
                return (Validation: validationResult, EarningsEvent:null);

            var earningsEvent =
                apprenticeshipContractTypeEarningsEventBuilder.Build(learnerSubmission);
            return (Validation: validationResult, EarningsEvent: earningsEvent); 
        }
    }
}