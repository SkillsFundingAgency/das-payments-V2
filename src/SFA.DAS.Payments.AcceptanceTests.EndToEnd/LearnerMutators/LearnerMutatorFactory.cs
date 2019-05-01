using System;
using System.Collections.Generic;
using DCT.TestDataGenerator.Functor;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class LearnerMutatorFactory
    {
        public static ILearnerMultiMutator Create(string featureNumber, IEnumerable<LearnerRequest> learnerRequests)
        {
            switch (featureNumber)
            {
                case "615":
                    return new FM36_615(learnerRequests);
                default:
                    throw new ArgumentException("A valid feature number is required.",nameof(featureNumber));
            }
        }
    }
}
