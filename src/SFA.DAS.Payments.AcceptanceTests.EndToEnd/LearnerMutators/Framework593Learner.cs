using System;
using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using DCT.TestDataGenerator.Functor;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class Framework593Learner : FM36Base
    {
        private readonly IEnumerable<Learner> learners;
        private GenerationOptions options;

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

            // Need a better way of doing this.
            if (learners?.Count() == 2)
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
                MutateCommon(learner, learners.First());
                DoSpecificMutate(learner, learners.First());
        }

        private void MutateLearner2(MessageLearner learner, bool valid)
        {
            MutateCommon(learner, learners.Skip(1).First());
            DoSpecificMutate(learner, learners.Skip(1).First());
        }

        protected virtual void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            messageLearner.ULN = learner.Uln;
            messageLearner.ULNSpecified = true;

            var functionalSkillsLearningDelivery = messageLearner.LearningDelivery.Single(ld => ld.AimType == 3);
            functionalSkillsLearningDelivery.LearnAimRef = "00300545";
            functionalSkillsLearningDelivery.FundModel = 36;
            functionalSkillsLearningDelivery.ProgType = 20;
            functionalSkillsLearningDelivery.FworkCode = 593;
            functionalSkillsLearningDelivery.LearnStartDate =
                messageLearner.LearningDelivery.Single(ld => ld.AimType == 1).LearnStartDate;

            MutateHigherEducation(messageLearner);
        }

        protected virtual void SetDeliveryAsWithdrawn(MessageLearnerLearningDelivery delivery, Aim learnerRequestAim)
        {
            delivery.CompStatus = (int)CompletionStatus.Withdrawn;
            delivery.CompStatusSpecified = true;
            delivery.Outcome = (int)Outcome.NoAchievement;
            delivery.OutcomeSpecified = true;
            delivery.WithdrawReason = (int)WithDrawalReason.FinancialReasons;
            delivery.WithdrawReasonSpecified = true;
            if (learnerRequestAim.ActualDurationAsTimespan.HasValue)
            {
                delivery.LearnActEndDate =
                    learnerRequestAim.StartDate.ToDate().Add(learnerRequestAim.ActualDurationAsTimespan.Value);

                delivery.LearnActEndDateSpecified = true;
            }
        }

        protected virtual void SetupLearningDeliveryFam(MessageLearnerLearningDelivery delivery, Aim learnerRequestAim)
        {
            var learningDeliveryFam = delivery.LearningDeliveryFAM.Single(ldf => ldf.LearnDelFAMType == LearnDelFAMType.ACT.ToString());

            learningDeliveryFam.LearnDelFAMDateTo = delivery.LearnActEndDate;
            learningDeliveryFam.LearnDelFAMDateToSpecified = true;
        }

        protected virtual void SetupAppFinRecord(MessageLearner messageLearner, MessageLearnerLearningDelivery delivery, Aim learnerRequestAim)
        {
            var appFinRecord =
                delivery.AppFinRecord.SingleOrDefault(afr => afr.AFinType == LearnDelAppFinType.TNP.ToString());

            if (appFinRecord == null)
            {
                DCT.TestDataGenerator.Helpers.AddAfninRecord(messageLearner, LearnDelAppFinType.TNP.ToString(), (int)LearnDelAppFinCode.TotalTrainingPrice, 15000);

                appFinRecord =
                    delivery.AppFinRecord.SingleOrDefault(afr => afr.AFinType == LearnDelAppFinType.TNP.ToString());
            }

            appFinRecord.AFinDate = delivery.LearnStartDate;
            appFinRecord.AFinDateSpecified = true;
        }

        protected virtual void ProcessMessageLearnerForLearnerRequestOriginatingFromTrainingRecord(MessageLearnerLearningDelivery functionalSkillsLearningDelivery, Aim aim)
        {
            SetDeliveryAsWithdrawn(functionalSkillsLearningDelivery, aim);
        }
    }
}