using System;
using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using DCT.TestDataGenerator.Functor;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class Framework403Learner : FM36Base
    {
        private readonly IEnumerable<Learner> learners;
        private GenerationOptions options;

        public Framework403Learner(IEnumerable<Learner> learners, string featureNumber) : base(featureNumber)
        {
            if (learners == null || !learners.Any())
            {
                throw new ArgumentException("At least one learner is required.");
            }

            this.learners = learners;
        }

        protected override IEnumerable<LearnerTypeMutator> CreateLearnerTypeMutators()
        {
            var list = new List<LearnerTypeMutator>();
            list.Add(new LearnerTypeMutator()
            {
                LearnerType = LearnerTypeRequired.Apprenticeships,
                DoMutateLearner = MutateLearner,
                DoMutateOptions = MutateLearnerOptions
            });

            return list;
        }

        private void MutateLearnerOptions(GenerationOptions options)
        {
            this.options = options;
            options.LD.IncludeHHS = true;
        }

        private void MutateLearner(MessageLearner learner, bool valid)
        {
            foreach (var learnerRequest in learners)
            {
                MutateCommon(learner, learnerRequest);
                DoSpecificMutate(learner, learnerRequest);
            }
        }

        protected virtual void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            messageLearner.ULN = learner.Uln;
            messageLearner.ULNSpecified = true;

            switch (learner.SmallEmployer)
            {
                case "SEM1":
                {
                    var les1 = messageLearner.LearnerEmploymentStatus[0];
                    var lesm1 = les1.EmploymentStatusMonitoring.ToList();
                    lesm1.Add(new MessageLearnerLearnerEmploymentStatusEmploymentStatusMonitoring()
                    {
                        ESMType = EmploymentStatusMonitoringType.SEM.ToString(),
                        ESMCode = (int) EmploymentStatusMonitoringCode.SmallEmployer,
                        ESMCodeSpecified = true
                    });
                    messageLearner.LearnerEmploymentStatus[0].EmploymentStatusMonitoring = lesm1.ToArray();
                    break;
                }
            }

            var functionalSkillsLearningDelivery = messageLearner.LearningDelivery.Single(ld => ld.AimType == 3);
            functionalSkillsLearningDelivery.LearnAimRef = "60005105";
            functionalSkillsLearningDelivery.FundModel = 36;
            functionalSkillsLearningDelivery.ProgType = 2;
            functionalSkillsLearningDelivery.FworkCode = 403;
            functionalSkillsLearningDelivery.LearnStartDate =
                messageLearner.LearningDelivery.Single(ld => ld.AimType == 1).LearnStartDate;

            MutateHigherEducation(messageLearner);

            MutateHigherEducation(messageLearner);
        }
    }
}