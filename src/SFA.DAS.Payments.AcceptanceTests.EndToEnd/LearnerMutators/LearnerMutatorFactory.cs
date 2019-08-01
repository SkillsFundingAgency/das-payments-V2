using System;
using System.Collections.Generic;
using DCT.TestDataGenerator.Functor;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.ProviderChange;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.SmallEmployer;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class LearnerMutatorFactory
    {
        public static ILearnerMultiMutator Create(string featureNumber, IEnumerable<Learner> learners)
        {
            switch (featureNumber)
            {
                case "199":
                case "209":
                case "210":
                case "251":
                case "255":
                case "258":
                case "261":
                case "262":
                case "268":
                case "266":
                case "274":
                case "282":
                case "283":
                case "334":
                case "335":
                case "336":
                case "337":
                case "379":
                case "381":
                case "382":
                case "387":
                case "389":
                case "390":
                case "393":
                case "394":
                case "402":
                case "427":
                case "429":
                case "435":
                case "436":
                case "437":
                case "438":
                case "463":
                case "464":
                case "465":
                case "466":
                case "485":
                case "488":
                case "499":
                case "501":
                case "509":
                case "514":
                case "516":
                case "518":
                case "520":
                case "522":
                case "528":
                case "529":
                case "609":
                case "615":
                case "893":
                    return new Framework593Learner(learners, featureNumber);
                case "326":
                case "329":
                case "330":
                case "331":
                case "351":
                case "352":
                case "487":
                case "851":
                case "852":
                    return new Framework403Learner(learners, featureNumber);
                case "205":
                case "207":
                case "443":
                case "444":
                case "445":
                case "446":
                case "489":
                case "526":
                case "897":
                case "925":
                    return new StandardLearner(learners, featureNumber);
                case "324":
                case "325":
                    return new Framework593LearnerRestarts(learners, featureNumber);

                case "277":
                    return new Framework593LearnerWithdrawsAfterQualifyingPeriod(learners);
                case "278":
                    return new Framework593LearnerWithdrawsAfterPlannedEndDate(learners);
                case "328":
                    return new Framework403Learner19PlusWithEHCPlanAndSmallEmployer(learners, featureNumber);
                
                default:
                    throw new ArgumentException("A valid feature number is required.", nameof(featureNumber));
            }
        }
    }
}