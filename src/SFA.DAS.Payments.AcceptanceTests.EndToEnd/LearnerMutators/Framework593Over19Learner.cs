namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    using System.Collections.Generic;
    using System.Linq;
    using DCT.TestDataGenerator;
    using DCT.TestDataGenerator.Functor;
    using ESFA.DC.ILR.Model.Loose;

    public class Framework593Over19Learner : FM36Base
    {
        private readonly IEnumerable<LearnerRequest> _learnerRequests;
        private TDG.GenerationOptions _options;

        public Framework593Over19Learner(IEnumerable<LearnerRequest> learnerRequests, string featureNumber) : base(featureNumber)
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

        private void MutateLearnerOptions(TDG.GenerationOptions options)
        {
            _options = options;
            options.LD.IncludeHHS = true;
        }

        private void MutateLearner(MessageLearner learner, bool valid)
        {
            var trainingRecord = _learnerRequests.First();
            TDG.Helpers.MutateDOB(learner, valid, TDG.Helpers.AgeRequired.Exact19, TDG.Helpers.BasedOn.LearnDelStart, TDG.Helpers.MakeOlderOrYoungerWhenInvalid.NoChange);
            MutateCommon(learner, trainingRecord);
            DoSpecificMutate(learner, trainingRecord);
        }

        protected void DoSpecificMutate(MessageLearner learner, LearnerRequest request)
        {
            learner.ULN = request.Uln;
            learner.ULNSpecified = true;
            learner.LearningDelivery[1].LearnAimRef = "00300545";
            MutateHE(learner);
        }
    }
}