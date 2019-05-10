namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    using System;
    using System.Collections.Generic;
    using DCT.TestDataGenerator.Functor;
    using NonLevy.BasicDay;

    public class LearnerMutatorFactory
    {
        public static ILearnerMultiMutator Create(string featureNumber, IEnumerable<LearnerRequest> learnerRequests)
        {
            switch (featureNumber)
            {
                case "199":
                case "261":
                case "262":
                case "334":
                case "335":
                case "336":
                case "337":
                case "402":
                case "427":
                case "615":
                    return new Framework593Over19Learner(learnerRequests, featureNumber);
                case "277":
                    return new FM36_277(learnerRequests);
                case "278":
                    return new FM36_278(learnerRequests);
                default:
                    throw new ArgumentException("A valid feature number is required.", nameof(featureNumber));
            }
        }
    }
}