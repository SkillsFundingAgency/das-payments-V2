namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DCT.TestDataGenerator;
    using DCT.TestDataGenerator.Functor;
    using ESFA.DC.ILR.Model.Loose;
    using SFA.DAS.Payments.AcceptanceTests.Core.Data;

    public class Framework593Learner : FM36Base
    {
        private readonly IEnumerable<LearnerRequest> _learnerRequests;
        private readonly IEnumerable<Learner> _learners;
        private GenerationOptions _options;

        public Framework593Learner(IEnumerable<LearnerRequest> learnerRequests, string featureNumber) : base(featureNumber)
        {
            if (learnerRequests == null || !learnerRequests.Any())
            {
                throw new ArgumentException("At least one learner is required.");
            }

            _learnerRequests = learnerRequests;
        }

        public Framework593Learner(IEnumerable<Learner> learners, string featureNumber) : base(featureNumber)
        {
            if (learners == null || !learners.Any())
            {
                throw new ArgumentException("At least one learner is required.");
            }

            _learners = learners;
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

            if (_learnerRequests?.Count() == 2|| _learners?.Count() == 2)
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
            _options = options;
            options.LD.IncludeHHS = true;
        }

        private void MutateLearner(MessageLearner learner, bool valid)
        {
            if (_learnerRequests != null)
            {
                var trainingRecord = _learnerRequests.First();
                MutateCommon(learner, trainingRecord);
                DoSpecificMutate(learner, trainingRecord);
            }
            else
            {
                var trainingRecord = _learners.First();
                MutateCommon(learner, trainingRecord);
            }
        }

        private void MutateLearner2(MessageLearner learner, bool valid)
        {
            var trainingRecord = _learnerRequests.Skip(1).First();
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