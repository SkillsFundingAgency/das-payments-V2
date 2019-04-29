using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators;

namespace DCT.TestDataGenerator.Functor
{
    public class FM36_334 : FM36Base
    {
        private readonly IEnumerable<LearnerRequest> _learnerRequests;
        private GenerationOptions _options;

        public FM36_334(IEnumerable<LearnerRequest> learnerRequests) : base("334")
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
            options.LD.IncludeHHS = true;
        }

        private void MutateLearner(MessageLearner learner, bool valid)
        {
            var trainingRecord = _learnerRequests.First();
            Helpers.MutateDOB(learner, valid, Helpers.AgeRequired.Exact19, Helpers.BasedOn.LearnDelStart, Helpers.MakeOlderOrYoungerWhenInvalid.NoChange);
            MutateCommon(learner, trainingRecord);
        }
    }
}
