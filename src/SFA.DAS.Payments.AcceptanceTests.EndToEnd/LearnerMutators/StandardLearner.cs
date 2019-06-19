using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class StandardLearner : Fm36Base
    {
        public StandardLearner(IEnumerable<Learner> learners, string featureNumber) : base(learners, featureNumber)
        {
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            var firstOnProgrammeDelivery = messageLearner.LearningDelivery.First(ld => ld.AimType == 1);
            var componentAims = messageLearner.LearningDelivery.Where(aim => aim.AimType == 3).ToList();
            componentAims.ForEach(componentAim =>
                                  {
                                      componentAim.FundModel = firstOnProgrammeDelivery.FundModel;
                                      componentAim.ProgType = firstOnProgrammeDelivery.ProgType;
                                      componentAim.StdCode = firstOnProgrammeDelivery.StdCode;
                                      componentAim.StdCodeSpecified = true;
                                      componentAim.FworkCodeSpecified = false;
                                      componentAim.PwayCodeSpecified = false;
                                      componentAim.LearnStartDate = firstOnProgrammeDelivery.LearnStartDate;
                                      componentAim.LearnPlanEndDate = firstOnProgrammeDelivery.LearnPlanEndDate;
                                      componentAim.LearnAimRef = "60005105";
                                  });
        }
    }
}
