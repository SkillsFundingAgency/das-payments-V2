using System;
using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using DCT.TestDataGenerator.Functor;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class Framework593Learner : FM36Base
    {
        private readonly IEnumerable<LearnerRequest> learnerRequests;
        private readonly IEnumerable<Learner> learners;
        private GenerationOptions options;

        public Framework593Learner(IEnumerable<LearnerRequest> learnerRequests, string featureNumber) : base(featureNumber)
        {
            if (learnerRequests == null || !learnerRequests.Any())
            {
                throw new ArgumentException("At least one learner is required.");
            }

            this.learnerRequests = learnerRequests;
        }

        public Framework593Learner(IEnumerable<Learner> learners, string featureNumber) : base(featureNumber)
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

            if (learnerRequests?.Count() == 2|| learners?.Count() == 2)
            {
                list.Add(new LearnerTypeMutator()
                {
                    LearnerType = LearnerTypeRequired.Apprenticeships,
                    DoMutateLearner = MutateLearner2,
                    DoMutateOptions = MutateLearnerOptions
                });
            }

            return list;
        }

        private void MutateLearnerOptions(GenerationOptions options)
        {
            this.options = options;
            options.LD.IncludeHHS = true;
        }

        private void MutateLearner(MessageLearner learner, bool valid)
        {
            if (learnerRequests != null)
            {
                var trainingRecord = learnerRequests.First();
                MutateCommon(learner, trainingRecord);
                DoSpecificMutate(learner, trainingRecord);
            }
            else
            {
                var trainingRecord = learners.First();
                MutateCommon(learner, trainingRecord);
            }
        }

        private void MutateLearner2(MessageLearner learner, bool valid)
        {
            var trainingRecord = learnerRequests.Skip(1).First();
            MutateCommon(learner, trainingRecord);
            DoSpecificMutate(learner, trainingRecord);
        }

        protected virtual void DoSpecificMutate(MessageLearner learner, LearnerRequest request)
        {
            learner.ULN = request.Uln;
            learner.ULNSpecified = true;
            learner.LearningDelivery[1].LearnAimRef = "00300545";
            MutateHE(learner);
        }

        protected virtual void DoSpecificMutate(MessageLearner learner, Learner request)
        {
            learner.ULN = request.Uln;
            learner.ULNSpecified = true;
            learner.LearningDelivery[1].LearnAimRef = "00300545";
            MutateHE(learner);
        }
    }
}