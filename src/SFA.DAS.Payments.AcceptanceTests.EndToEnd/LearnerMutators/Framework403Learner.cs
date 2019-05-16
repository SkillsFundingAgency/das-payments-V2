namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DCT.TestDataGenerator;
    using DCT.TestDataGenerator.Functor;
    using ESFA.DC.ILR.Model.Loose;

    public class Framework403Learner : FM36Base
    {
        private readonly IEnumerable<LearnerRequest> _learnerRequests;
        private GenerationOptions _options;

        public Framework403Learner(IEnumerable<LearnerRequest> learnerRequests, string featureNumber) : base(featureNumber)
        {
            if (learnerRequests == null || !learnerRequests.Any())
            {
                throw new ArgumentException("At least one learner is required.");
            }

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

            if (_learnerRequests.Count() == 2)
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
            var trainingRecord = _learnerRequests.First();
            MutateCommon(learner, trainingRecord);
            DoSpecificMutate(learner, trainingRecord);
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
            learner.LearningDelivery[1].LearnAimRef = "60005105";

            switch (request.SmallEmployer)
            {
                case "SEM1":
                {
                    var les1 = learner.LearnerEmploymentStatus[0];
                    var lesm1 = les1.EmploymentStatusMonitoring.ToList();
                    lesm1.Add(new MessageLearnerLearnerEmploymentStatusEmploymentStatusMonitoring()
                    {
                        ESMType = EmploymentStatusMonitoringType.SEM.ToString(),
                        ESMCode = (int) EmploymentStatusMonitoringCode.SmallEmployer,
                        ESMCodeSpecified = true
                    });
                    learner.LearnerEmploymentStatus[0].EmploymentStatusMonitoring = lesm1.ToArray();
                    break;
                }
            }

            MutateHE(learner);
        }
    }
}