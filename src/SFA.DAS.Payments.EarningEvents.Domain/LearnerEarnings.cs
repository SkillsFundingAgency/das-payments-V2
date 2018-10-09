using System;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner;

namespace SFA.DAS.Payments.EarningEvents.Domain
{
    public class LearnerEarnings
    {
        private readonly ILearnerValidator learnerValidator;

        public LearnerEarnings(ILearnerValidator learnerValidator)
        {
            this.learnerValidator = learnerValidator ?? throw new ArgumentNullException(nameof(learnerValidator));
        }

        public void GenerateEarnings(FM36Learner learner)
        {
            //validate the learner, if learner is invalid then throw an exception with the validation failure details
            //map the fm36 learner to a Learner
            throw new NotImplementedException();
        }
    }
}