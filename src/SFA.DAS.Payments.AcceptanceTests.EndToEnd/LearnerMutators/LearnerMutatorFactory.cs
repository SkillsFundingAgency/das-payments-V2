using System;
using System.Collections.Generic;
using DCT.TestDataGenerator.Functor;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.Levy.BasicDay;
using FM36_334 = SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay.FM36_334;
using FM36_615 = SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay.FM36_615;
using FM36_277 = SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay.FM36_277;
using FM36_278 = SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay.FM36_278;

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
                case "277":
                    return new FM36_277(learnerRequests);
                case "278":
                    return new FM36_278(learnerRequests);
                default:
                    throw new ArgumentException("A valid feature number is required.",nameof(featureNumber));
            }
        }
    }
}
