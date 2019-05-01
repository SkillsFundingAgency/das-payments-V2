using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using DCT.TestDataGenerator.Functor;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.Levy
{
    public abstract class SingleLevyLearner : FM36Base
    {
        private readonly IEnumerable<LearnerRequest> _learnerRequests;
        private GenerationOptions _options;

        protected SingleLevyLearner(IEnumerable<LearnerRequest> learnerRequests, string featureNumber) : base(featureNumber)
        {
            _learnerRequests = learnerRequests;
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
            _options = options;
            options.LD.IncludeHHS = false;
        }

        private void MutateLearner(MessageLearner learner, bool valid)
        {
            var trainingRecord = _learnerRequests.First();
            Helpers.MutateDOB(learner, valid, Helpers.AgeRequired.Exact19, Helpers.BasedOn.LearnDelStart, Helpers.MakeOlderOrYoungerWhenInvalid.NoChange);
            MutateCommon(learner, trainingRecord);
            DoSpecificMutate(learner, trainingRecord);
        }

        protected virtual void DoSpecificMutate(MessageLearner learner, LearnerRequest request)
        {
            // override in sub class if required
        }
    }
}
