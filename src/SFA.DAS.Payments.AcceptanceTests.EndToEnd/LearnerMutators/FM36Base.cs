using System;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Tests.Core;
using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using DCT.TestDataGenerator.Functor;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public abstract class FM36Base : ILearnerMultiMutator
    {
        private const int StandardProgrammeType = 25;
        private readonly string featureNumber;
        protected ILearnerCreatorDataCache dataCache;

        protected FM36Base(string featureNumber)
        {
            this.featureNumber = featureNumber;
        }

        public FilePreparationDateRequired FilePreparationDate()
        {
            return FilePreparationDateRequired.July;
        }

        public IEnumerable<LearnerTypeMutator> LearnerMutators(ILearnerCreatorDataCache cache)
        {
            dataCache = cache;

            return CreateLearnerTypeMutators();
        }

        protected abstract IEnumerable<LearnerTypeMutator> CreateLearnerTypeMutators();

        public string RuleName()
        {
            return "FM36_E2E";
        }

        public string LearnerReferenceNumberStub()
        {
            return $"fm36{featureNumber}";
        }

        protected void MutateCommon(MessageLearner learner, Learner learnerRequest)
        {
            learner.DateOfBirth =
                learnerRequest.Aims.First().StartDate.ToDate()
                    .AddYears(-learnerRequest.Aims.First().FundingLineType.ToLearnerAge());

            if (!string.IsNullOrWhiteSpace(learnerRequest.PostcodePrior))
            {
                learner.Postcode = learnerRequest.PostcodePrior;
            }

            var learnerLearningDeliveries = learner.LearningDelivery.ToList();

            foreach (var aim in learnerRequest.Aims)
            {
                MutateAimForLearner(aim, learnerLearningDeliveries, learnerRequest.Aims.Count());
            }

            learner.LearningDelivery = learnerLearningDeliveries.ToArray();

            learner.LearnerEmploymentStatus[0].DateEmpStatApp =
                learner.LearningDelivery[0].LearnStartDate.AddMonths(-6);

        }

        private void MutateAimForLearner(Aim aim, List<MessageLearnerLearningDelivery> learnerLearningDeliveries, int numberOfAimsForLearner)
        {
            var delivery = learnerLearningDeliveries.SingleOrDefault(learnDelivery =>
                  learnDelivery.AimSeqNumber == aim.AimSequenceNumber);

            if (delivery == null)
            {
                delivery = new MessageLearnerLearningDelivery();
                learnerLearningDeliveries.Add(delivery);
            }

            delivery.LearnAimRef = aim.AimReference;

            delivery.LearnStartDate = aim.StartDate.ToDate();
            delivery.LearnStartDateSpecified = true;

            if (aim.PlannedDurationAsTimespan.HasValue)
            {
                delivery.LearnPlanEndDate = delivery.LearnStartDate.Add(aim.PlannedDurationAsTimespan.Value);
                delivery.LearnPlanEndDateSpecified = true;
            }

            if (aim.ActualDurationAsTimespan.HasValue)
            {
                delivery.LearnActEndDate = delivery.LearnStartDate.Add(aim.ActualDurationAsTimespan.Value);
                delivery.LearnActEndDateSpecified = true;
            }

            MutateLearningDeliveryFamsForLearner(delivery, aim);

            delivery.ProgType = aim.ProgrammeType;
            delivery.ProgTypeSpecified = true;

            delivery.FworkCode = aim.FrameworkCode;
            delivery.FworkCodeSpecified = true;

            delivery.PwayCode = aim.PathwayCode;
            delivery.PwayCodeSpecified = true;

            delivery.CompStatus = (int)aim.CompletionStatus;
            delivery.CompStatusSpecified = true;

            if (aim.CompletionStatus == CompletionStatus.Completed)
            {
                delivery.Outcome = (int)Outcome.Achieved;
                delivery.OutcomeSpecified = true;
            }

            if (numberOfAimsForLearner == 1) // assume that this aim was created through the old style of Training records - in which case we need to setup the functionalskillsdelivery as well.
            {
                MutateAimType3ForLearnerFromTrainingRecord(learnerLearningDeliveries, delivery);
            }
        }

        private void MutateLearningDeliveryFamsForLearner(MessageLearnerLearningDelivery delivery, Aim aim)
        {
            List<MessageLearnerLearningDeliveryLearningDeliveryFAM> listOfLearningDeliveryFams = null;

            if (delivery.LearningDeliveryFAM != null)
            {
                var learningDeliveryFams =
                    delivery.LearningDeliveryFAM.Where(s => s.LearnDelFAMType != LearnDelFAMType.ACT.ToString());
                delivery.LearningDeliveryFAM = learningDeliveryFams.ToArray();
                listOfLearningDeliveryFams = delivery.LearningDeliveryFAM.ToList();
            }
            else
            {
                listOfLearningDeliveryFams = new List<MessageLearnerLearningDeliveryLearningDeliveryFAM>();
            }

            foreach (var priceEpisode in aim.PriceEpisodes)
            {
                MutatePriceEpisodeForFam(delivery, listOfLearningDeliveryFams, priceEpisode, aim);
            }

            delivery.LearningDeliveryFAM = listOfLearningDeliveryFams.ToArray();
        }

        private void MutatePriceEpisodeForFam(MessageLearnerLearningDelivery delivery, List<MessageLearnerLearningDeliveryLearningDeliveryFAM> listOfLearningDeliveryFams, Price priceEpisode, Aim aim)
        {
            listOfLearningDeliveryFams.Add(new MessageLearnerLearningDeliveryLearningDeliveryFAM()
            {
                LearnDelFAMType = LearnDelFAMType.ACT.ToString(),
                LearnDelFAMCode = ((int)priceEpisode.ContractType).ToString(),
                LearnDelFAMDateFrom = delivery.LearnStartDate,
                LearnDelFAMDateFromSpecified = true,
                LearnDelFAMDateTo = delivery.LearnActEndDate,
                LearnDelFAMDateToSpecified = aim.ActualDurationAsTimespan.HasValue,
            });

            if (aim.CompletionStatus == CompletionStatus.Completed && aim.AimSequenceNumber == 1)
            {
                MutateDeliveryAppFinRecordToPMR(delivery, priceEpisode);
            }

            if (aim.IsMainAim)
            {
                MutateMainAim(delivery, aim, priceEpisode);
            }
        }
        private void MutateDeliveryAppFinRecordToPMR(MessageLearnerLearningDelivery delivery, Price priceEpisode)
        {
            var appFinRecords = new List<MessageLearnerLearningDeliveryAppFinRecord>();
            appFinRecords.Add(new MessageLearnerLearningDeliveryAppFinRecord()
            {
                AFinAmount = CalculateEmployerContribution(priceEpisode.SfaContributionPercentage,
                    priceEpisode.TotalTrainingPrice),
                AFinAmountSpecified = true,
                AFinType = LearnDelAppFinType.PMR.ToString(),
                AFinCode = (int)LearnDelAppFinCode.TrainingPayment,
                AFinCodeSpecified = true,
                AFinDate = delivery.LearnActEndDate.AddMonths(-1),
                AFinDateSpecified = true
            });

            delivery.AppFinRecord = appFinRecords.ToArray();
        }

        private void MutateMainAim(MessageLearnerLearningDelivery delivery, Aim aim, Price priceEpisode)
        {
            var appFinRecords = delivery.AppFinRecord.ToList();

            AddNewTnpAppFinRecord(delivery, appFinRecords, priceEpisode);

            if (aim.ProgrammeType == StandardProgrammeType)
            {
                AddNewPmrAppFinRecord(delivery, appFinRecords, priceEpisode);
            }

            delivery.AppFinRecord = appFinRecords.ToArray();
        }

        private void AddNewTnpAppFinRecord(MessageLearnerLearningDelivery delivery, List<MessageLearnerLearningDeliveryAppFinRecord> appFinRecords, Price priceEpisode)
        {
            var tnp = appFinRecords.SingleOrDefault(a => a.AFinType == "TNP");
            if (tnp == null)
            {
                tnp = new MessageLearnerLearningDeliveryAppFinRecord()
                {
                    AFinType = "TNP"
                };
                appFinRecords.Add(tnp);
            }

            tnp.AFinCode = (int)LearnDelAppFinCode.TotalTrainingPrice;
            tnp.AFinCodeSpecified = true;
            tnp.AFinAmount = Convert.ToInt32(priceEpisode.TotalTrainingPrice);
            tnp.AFinAmountSpecified = true;

            if (!string.IsNullOrWhiteSpace(priceEpisode.TotalTrainingPriceEffectiveDate))
            {
                tnp.AFinDate = priceEpisode.TotalTrainingPriceEffectiveDate.ToDate();
                tnp.AFinDateSpecified = true;
            }
        }
        private void AddNewPmrAppFinRecord(MessageLearnerLearningDelivery delivery, List<MessageLearnerLearningDeliveryAppFinRecord> appFinRecords, Price priceEpisode)
        {
            var pmr = appFinRecords.SingleOrDefault(a => a.AFinType == "PMR");
            if (pmr == null)
            {
                pmr = new MessageLearnerLearningDeliveryAppFinRecord()
                {
                    AFinType = "PMR"
                };
                appFinRecords.Add(pmr);
            }

            pmr.AFinCode = (int)LearnDelAppFinCode.TotalAssessmentPrice;
            pmr.AFinCodeSpecified = true;
            pmr.AFinAmount = Convert.ToInt32(priceEpisode.TotalAssessmentPrice);
            pmr.AFinAmountSpecified = true;
            pmr.AFinDate = priceEpisode.TotalAssessmentPriceEffectiveDate.ToDate();
            pmr.AFinDateSpecified = true;
        }

        private void MutateAimType3ForLearnerFromTrainingRecord(List<MessageLearnerLearningDelivery> learnerLearningDeliveries, MessageLearnerLearningDelivery delivery)
        {
            var functionalSkillLearningDelivery = learnerLearningDeliveries.Single(learnDelivery =>
                learnDelivery.AimType == 3);

            if (delivery.CompStatus == (int)CompletionStatus.Completed)
            {
                functionalSkillLearningDelivery.CompStatus = (int)CompletionStatus.Completed;
                functionalSkillLearningDelivery.CompStatusSpecified = true;
                functionalSkillLearningDelivery.LearnActEndDate = delivery.LearnActEndDate;
                functionalSkillLearningDelivery.LearnActEndDateSpecified = true;
                functionalSkillLearningDelivery.Outcome = (int)Outcome.Achieved;
                functionalSkillLearningDelivery.OutcomeSpecified = true;
            }
        }

        private long CalculateEmployerContribution(string sfaContributionPercentage, decimal totalTrainingPrice)
        {
            var percentage = decimal.Parse((100 - sfaContributionPercentage.AsPercentage()).ToString());
            var employerContribution = totalTrainingPrice * (percentage / 100);

            return decimal.ToInt64(employerContribution);
        }

        protected void MutateHigherEducation(MessageLearner learner)
        {
            var learningDeliveryHes = new List<MessageLearnerLearningDeliveryLearningDeliveryHE>();

            learningDeliveryHes.Add(new MessageLearnerLearningDeliveryLearningDeliveryHE()
            {
                NUMHUS = "2000812012XTT60021",
                QUALENT3 = QualificationOnEntry.X06.ToString(),
                UCASAPPID = "AB89",
                TYPEYR = (int)TypeOfyear.FEYear,
                TYPEYRSpecified = true,
                MODESTUD = (int)ModeOfStudy.NotInPopulation,
                MODESTUDSpecified = true,
                FUNDLEV = (int)FundingLevel.Undergraduate,
                FUNDLEVSpecified = true,
                FUNDCOMP = (int)FundingCompletion.NotYetCompleted,
                FUNDCOMPSpecified = true,
                STULOAD = 10.0M,
                STULOADSpecified = true,
                YEARSTU = 1,
                YEARSTUSpecified = true,
                MSTUFEE = (int)MajorSourceOfTuitionFees.NoAward,
                MSTUFEESpecified = true,
                PCFLDCS = 100,
                PCFLDCSSpecified = true,
                SPECFEE = (int)SpecialFeeIndicator.Other,
                SPECFEESpecified = true,
                NETFEE = 0,
                NETFEESpecified = true,
                GROSSFEE = 1,
                GROSSFEESpecified = true,
                DOMICILE = "ZZ",
                ELQ = (int)EquivalentLowerQualification.NotRequired,
                ELQSpecified = true
            });

            learner.LearningDelivery[1].LearningDeliveryHE = learningDeliveryHes.ToArray();
        }

    }
}
