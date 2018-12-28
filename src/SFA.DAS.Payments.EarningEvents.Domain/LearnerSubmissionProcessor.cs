using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Core.Validation;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Domain
{
    public interface ILearnerSubmissionProcessor
    {
        (ValidationResult Validation, List<EarningEvent> EarningEvents) GenerateEarnings(ProcessLearnerCommand learnerSubmission);
    }

    public class LearnerSubmissionProcessor: ILearnerSubmissionProcessor
    {
        private readonly ILearnerValidator learnerValidator;
        private readonly IApprenticeshipContractTypeEarningsEventBuilder apprenticeshipContractTypeEarningsEventBuilder;
        private readonly IFunctionalSkillEarningsEventBuilder functionalSkillEarningsEventBuilder;

        public LearnerSubmissionProcessor(ILearnerValidator learnerValidator, IApprenticeshipContractTypeEarningsEventBuilder apprenticeshipContractTypeEarningsEventBuilder, IFunctionalSkillEarningsEventBuilder functionalSkillEarningsEventBuilder)
        {
            this.learnerValidator = learnerValidator ?? throw new ArgumentNullException(nameof(learnerValidator));
            this.apprenticeshipContractTypeEarningsEventBuilder = apprenticeshipContractTypeEarningsEventBuilder ?? throw new ArgumentNullException(nameof(apprenticeshipContractTypeEarningsEventBuilder));
            this.functionalSkillEarningsEventBuilder = functionalSkillEarningsEventBuilder ?? throw new ArgumentNullException(nameof(functionalSkillEarningsEventBuilder));
        }

        public (ValidationResult Validation, List<EarningEvent> EarningEvents) GenerateEarnings(ProcessLearnerCommand learnerSubmission)
        {
            var validationResult = learnerValidator.Validate(learnerSubmission.Learner);

            if (validationResult.Failed)
                return (Validation: validationResult, EarningEvents: null);

            var earningsEvent = new List<EarningEvent>();

            earningsEvent.AddRange(apprenticeshipContractTypeEarningsEventBuilder.Build(learnerSubmission));
            earningsEvent.AddRange(functionalSkillEarningsEventBuilder.Build(learnerSubmission));

            return (Validation: validationResult, EarningEvents: earningsEvent);
        }
    }
}