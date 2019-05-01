using System;
using System.Collections.Generic;
using DCT.TestDataGenerator.Functor;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.Levy.BasicDay;
using FM36_334 = SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay.FM36_334;
using FM36_615 = SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay.FM36_615;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class LearnerMutatorFactory
    {
        public static ILearnerMultiMutator Create(string featureNumber, IEnumerable<LearnerRequest> learnerRequests)
        {
            switch (featureNumber)
            {
                case "261":
                    return new FM36_261(learnerRequests);
                case "334":
                    return new FM36_334(learnerRequests);
                case "615":
                    return new FM36_615(learnerRequests);
                default:
                    throw new ArgumentException("A valid feature number is required.",nameof(featureNumber));
            }
        }
    }
}
