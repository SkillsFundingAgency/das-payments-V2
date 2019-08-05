using System.Collections.Generic;
using DCT.TestDataGenerator.Functor;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy
{
    public class Framework593LearnerWithNewFilePreparationDate : Framework593Learner
    {
        public Framework593LearnerWithNewFilePreparationDate(IEnumerable<Learner> learnerRequests, string featureNumber) : base(learnerRequests, featureNumber)
        {
        }

        public override FilePreparationDateRequired FilePreparationDate()
        {
            return FilePreparationDateRequired.NextYear;
        }
    }
}