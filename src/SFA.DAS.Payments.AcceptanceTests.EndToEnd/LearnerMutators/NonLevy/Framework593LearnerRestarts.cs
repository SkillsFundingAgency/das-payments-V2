using System.Collections.Generic;
using DCT.TestDataGenerator.Functor;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.ProviderChange
{
    public class Framework593LearnerRestarts : Framework593Learner
    {
        public Framework593LearnerRestarts(IEnumerable<Learner> learnerRequests, string featureNumber) : base(learnerRequests, featureNumber)
        {
        }

        public override FilePreparationDateRequired FilePreparationDate()
        {
            return FilePreparationDateRequired.NextYear;
        }

        protected override void DoSpecificMutate(MessageLearner learner, Learner request)
        {
            base.DoSpecificMutate(learner, request);

            DCT.TestDataGenerator.Helpers.AddLearningDeliveryRestartFAM(learner);
            learner.LearnerEmploymentStatus[0].DateEmpStatApp = request.Aims[0].OriginalStartDate.ToDate().AddMonths(-1);
        }
    }
}