using System;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Tests.Core;
using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using DCT.TestDataGenerator.Functor;
using ESFA.DC.ILR.Model.Loose;
using MoreLinq;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public abstract class Fm36Base : ILearnerMultiMutator
    {
        private const int StandardProgrammeType = 25;
        private const int StandardProgrammeEpaDuration = 8;
        private readonly string featureNumber;
        protected ILearnerCreatorDataCache dataCache;

        private readonly IEnumerable<Learner> learners;

        protected Fm36Base(IEnumerable<Learner> learners, string featureNumber)
        {
            var learnersEnumerable = learners as Learner[] ?? learners.ToArray();
            if (learners == null || !learnersEnumerable.Any())
            {
                throw new ArgumentException("At least one learner is required.");
            }

            this.learners = learnersEnumerable;
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

        private IEnumerable<LearnerTypeMutator> CreateLearnerTypeMutators()
        {
            var list = new List<LearnerTypeMutator>();

            foreach (var learner in learners)
            {
                list.Add(new LearnerTypeMutator()
                {
                    LearnerType = LearnerTypeRequired.Apprenticeships,
                    DoMutateLearner = (messageLearner, valid) => { 
                        MutateCommon(messageLearner, learner);
                        DoSpecificMutate(messageLearner, learner);
                    },
                    DoMutateOptions = options =>
                    {
                        options.LD.IncludeHHS = true;
                    }
                });
            }

            return list;
        }

        public string RuleName()
        {
            return "FM36_E2E";
        }

        public string LearnerReferenceNumberStub()
        {
            return $"fm36{featureNumber}";
        }

        protected abstract void DoSpecificMutate(MessageLearner messageLearner, Learner learner);

        protected void MutateCommon(MessageLearner messageLearner, Learner learner)
        {
            messageLearner.DateOfBirth =
                learner.Aims.First().StartDate.ToDate()
                    .AddYears(-learner.Aims.First().FundingLineType.ToLearnerAge());

            if (!string.IsNullOrWhiteSpace(learner.PostcodePrior))
            {
                messageLearner.Postcode = learner.PostcodePrior;
            }

            var learnerLearningDeliveries = messageLearner.LearningDelivery.ToList();

            foreach (var aim in learner.Aims)
            {
                MutateAimForLearner(aim, learnerLearningDeliveries, learner.Aims.Count(a => a.IsMainAim));
            }

            messageLearner.LearningDelivery = learnerLearningDeliveries.ToArray();

            messageLearner.LearnerEmploymentStatus[0].DateEmpStatApp =
                messageLearner.LearningDelivery[0].LearnStartDate.AddMonths(-6);

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

            messageLearner.ULN = learner.Uln;
            messageLearner.ULNSpecified = true;

            MutateHigherEducation(messageLearner);

            SetComponentAimLearnAimReference(messageLearner);

            CorrectComponentAimStartAndEndDates(messageLearner);
        }

        protected void SetupLearningDeliveryActFam(MessageLearnerLearningDelivery delivery)
        {
            var learningDeliveryFam = delivery.LearningDeliveryFAM.Single(ldf => ldf.LearnDelFAMType == LearnDelFAMType.ACT.ToString());

            learningDeliveryFam.LearnDelFAMDateTo = delivery.LearnActEndDate;
            learningDeliveryFam.LearnDelFAMDateToSpecified = true;
        }

        protected void ProcessMessageLearnerForLearnerRequestOriginatingFromTrainingRecord(MessageLearnerLearningDelivery functionalSkillsLearningDelivery, Aim aim)
        {

            if (aim.ActualDurationAsTimespan.HasValue)
            {
                functionalSkillsLearningDelivery.LearnActEndDate =
                    aim.StartDate.ToDate().Add(aim.ActualDurationAsTimespan.Value);

                functionalSkillsLearningDelivery.LearnActEndDateSpecified = true;
            }

            MutateCompletionStatusForLearner(functionalSkillsLearningDelivery, (int)aim.CompletionStatus, functionalSkillsLearningDelivery.LearnActEndDate);
      }

        protected void RemovePmrRecord(MessageLearner learner)
        {
            var deliveries = learner.LearningDelivery.ToList().Where(ld => ld.AimType == 1);
            deliveries.ForEach(d=>d.AppFinRecord = d.AppFinRecord.ToList().Where(af => af.AFinType != LearnDelAppFinType.PMR.ToString()).ToArray());
        }

        protected void AddNewTnpAppFinRecordForTrainingPrice(List<MessageLearnerLearningDeliveryAppFinRecord> appFinRecords, Price priceEpisode)
        {
            var tnp = appFinRecords.SingleOrDefault(a => a.AFinType == LearnDelAppFinType.TNP.ToString() && a.AFinCode == (int)LearnDelAppFinCode.TotalTrainingPrice);

            if (tnp == null)
            {
                tnp = new MessageLearnerLearningDeliveryAppFinRecord()
                {
                    AFinType = LearnDelAppFinType.TNP.ToString(),
                    AFinCode = (int)LearnDelAppFinCode.TotalTrainingPrice,
                    AFinCodeSpecified = true
                };
                appFinRecords.Add(tnp);
            }

            tnp.AFinAmount = Convert.ToInt32(priceEpisode.TotalTrainingPrice);
            tnp.AFinAmountSpecified = true;

            if (!string.IsNullOrWhiteSpace(priceEpisode.TotalTrainingPriceEffectiveDate))
            {
                tnp.AFinDate = priceEpisode.TotalTrainingPriceEffectiveDate.ToDate();
                tnp.AFinDateSpecified = true;
            }
        }

        private void SetComponentAimLearnAimReference(MessageLearner messageLearner)
        {
            var mainAims = messageLearner.LearningDelivery.ToList().Where(aim => aim.AimType == 1);
            var componentAims = messageLearner.LearningDelivery.ToList().Where(aim => aim.AimType == 3);

            // case for 205 only - this logic needs to be improved!
            if (mainAims.Count() == 2 && componentAims.Count() == 1 && (mainAims.Any(a=>a.StdCode == 52) || mainAims.Any(a=>a.StdCode == 53)))
            {
                componentAims.ForEach(ca=>ca.LearnAimRef = "60005105");
            }
            else if (mainAims.First().StdCodeSpecified)
            {
                componentAims.ForEach(ca => ca.LearnAimRef = "00300545");
            }
        }

        private void CorrectComponentAimStartAndEndDates(MessageLearner messageLearner)
        {
            var mainAims = messageLearner.LearningDelivery.ToList().Where(aim => aim.AimType == 1);
            var componentAims = messageLearner.LearningDelivery.ToList().Where(aim => aim.AimType == 3);

            // case for 205 only - this logic needs to be improved!
            if (mainAims.Count() == 2 && componentAims.Count() == 1 && mainAims.First().StdCode == 51)
            {
                componentAims.ForEach(ca =>
                {
                    ca.LearnStartDate = mainAims.First().LearnStartDate;
                    ca.LearnPlanEndDate = mainAims.Last().LearnPlanEndDate;
                }); 
            }
        }

        private void MutateAimForLearner(Aim aim, List<MessageLearnerLearningDelivery> learnerLearningDeliveries, int numberOfAimsForLearner)
        {
            var delivery = learnerLearningDeliveries.SingleOrDefault(learnDelivery =>
                  learnDelivery.AimSeqNumber == aim.AimSequenceNumber && 
                  learnDelivery.LearnAimRef == aim.AimReference);

            if (delivery == null)
            {
                delivery = learnerLearningDeliveries.First(ld => ld.LearnAimRef == "ZPROG001").DeepClone();
                delivery.AimSeqNumber = aim.AimSequenceNumber;
                learnerLearningDeliveries.Add(delivery);
            }

            delivery.LearnStartDate = aim.StartDate.ToDate();
            delivery.LearnStartDateSpecified = true;

            if (aim.PlannedDurationAsTimespan.HasValue)
            {
                if (aim.ProgrammeType == StandardProgrammeType)
                {
                    delivery.LearnPlanEndDate = delivery.LearnStartDate.Add(aim.PlannedDurationAsTimespan.Value).AddDays(StandardProgrammeEpaDuration);
                    delivery.LearnPlanEndDateSpecified = true;
                }
                else
                {
                    delivery.LearnPlanEndDate = delivery.LearnStartDate.Add(aim.PlannedDurationAsTimespan.Value);
                    delivery.LearnPlanEndDateSpecified = true;
                }
            }

            DateTime? actualEndDate = null;
            if (aim.ActualDurationAsTimespan.HasValue)
            {
                actualEndDate = aim.ProgrammeType == StandardProgrammeType
                                    ? delivery.LearnStartDate.Add(aim.ActualDurationAsTimespan.Value).AddDays(StandardProgrammeEpaDuration)
                                    : delivery.LearnStartDate.Add(aim.ActualDurationAsTimespan.Value);
            }

            MutateCompletionStatusForLearner(delivery, (int)aim.CompletionStatus, actualEndDate);

            MutateLearningDeliveryFamsForLearner(delivery, aim);

            delivery.ProgType = aim.ProgrammeType;
            delivery.ProgTypeSpecified = true;

            if (aim.ProgrammeType == StandardProgrammeType)
            {
                delivery.StdCode = aim.StandardCode;
                delivery.StdCodeSpecified = true;
                delivery.FworkCodeSpecified = false;
                delivery.PwayCodeSpecified = false;
            }
            else
            {
                delivery.FworkCode = aim.FrameworkCode;
                delivery.FworkCodeSpecified = true;

                delivery.PwayCode = aim.PathwayCode;
                delivery.PwayCodeSpecified = true;
            }

            if (numberOfAimsForLearner == 1) // assume that this aim was created through the old style of Training records - in which case we need to setup the functionalskillsdelivery as well.
            {
                MutateAimType3ForLearnerFromTrainingRecord(learnerLearningDeliveries, delivery);
            }
        }

        private void MutateCompletionStatusForLearner(MessageLearnerLearningDelivery delivery, int completionStatus, DateTime? actualEndDate)
        {
            delivery.CompStatus = completionStatus;
            delivery.CompStatusSpecified = true;

            if (actualEndDate.HasValue)
            {
                delivery.LearnActEndDate = actualEndDate.Value;
                delivery.LearnActEndDateSpecified = true;
            }

            switch (completionStatus)
            {
                case (int)CompletionStatus.Continuing:
                    delivery.OutcomeSpecified = false;
                    delivery.WithdrawReasonSpecified = false;
                    delivery.LearnActEndDateSpecified = false;
                    break;

                case (int)CompletionStatus.Completed:
                    delivery.Outcome = (int)Outcome.Achieved;
                    delivery.OutcomeSpecified = true;
                    delivery.WithdrawReasonSpecified = false;
                    break;

                case (int)CompletionStatus.Withdrawn:
                    delivery.Outcome = (int) Outcome.NoAchievement;
                    delivery.OutcomeSpecified = true;
                    delivery.WithdrawReason = (int) WithDrawalReason.NotKnown;
                    delivery.WithdrawReasonSpecified = true;
                    break;
            }

        }

        private void MutateLearningDeliveryFamsForLearner(MessageLearnerLearningDelivery delivery, Aim aim)
        {
            List<MessageLearnerLearningDeliveryLearningDeliveryFAM> listOfLearningDeliveryFams;

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
                MutateDeliveryAppFinRecordToPmr(delivery, priceEpisode);
            }

            if (aim.IsMainAim)
            {
                MutateMainAim(delivery, aim, priceEpisode);
            }
        }
        private void MutateDeliveryAppFinRecordToPmr(MessageLearnerLearningDelivery delivery, Price priceEpisode)
        {
            var appFinRecords = new List<MessageLearnerLearningDeliveryAppFinRecord>
            {
                new MessageLearnerLearningDeliveryAppFinRecord()
                {
                    AFinAmount = CalculateEmployerContribution(priceEpisode.SfaContributionPercentage,
                    priceEpisode.TotalTrainingPrice),
                    AFinAmountSpecified = true,
                    AFinType = LearnDelAppFinType.PMR.ToString(),
                    AFinCode = (int)LearnDelAppFinCode.TrainingPayment,
                    AFinCodeSpecified = true,
                    AFinDate = delivery.LearnActEndDate.AddMonths(-1),
                    AFinDateSpecified = true
                }
            };

            delivery.AppFinRecord = appFinRecords.ToArray();
        }

        private void MutateMainAim(MessageLearnerLearningDelivery delivery, Aim aim, Price priceEpisode)
        {
            var appFinRecords = delivery.AppFinRecord?.ToList() ?? new List<MessageLearnerLearningDeliveryAppFinRecord>();

            AddNewTnpAppFinRecordForTrainingPrice(appFinRecords, priceEpisode);

            if (aim.ProgrammeType == StandardProgrammeType)
            {
                if (aim.CompletionStatus == CompletionStatus.Completed)
                {
                    AddNewPmrAppFinRecord(appFinRecords, priceEpisode);
                }

                AddTnpAppFinRecordForAssessmentPrice(appFinRecords, priceEpisode);
                delivery.EPAOrgID = "EPA0022";
            }

            delivery.AppFinRecord = appFinRecords.ToArray();
        }

        private void AddNewPmrAppFinRecord(List<MessageLearnerLearningDeliveryAppFinRecord> appFinRecords, Price priceEpisode)
        {
            var pmr = appFinRecords.SingleOrDefault(a => a.AFinType == LearnDelAppFinType.PMR.ToString());
            if (pmr == null)
            {
                pmr = new MessageLearnerLearningDeliveryAppFinRecord()
                {
                    AFinType = LearnDelAppFinType.PMR.ToString()
                };
                appFinRecords.Add(pmr);
            }

            pmr.AFinCode = (int) LearnDelAppFinCode.TotalAssessmentPrice;
            pmr.AFinCodeSpecified = true;
            pmr.AFinAmount = Convert.ToInt32(priceEpisode.TotalAssessmentPrice);
            pmr.AFinAmountSpecified = true;
            pmr.AFinDate = priceEpisode.TotalAssessmentPriceEffectiveDate.ToDate();
            pmr.AFinDateSpecified = true;
        }

        private void AddTnpAppFinRecordForAssessmentPrice(List<MessageLearnerLearningDeliveryAppFinRecord> appFinRecords, Price priceEpisode)
        {
            var tnp = appFinRecords.SingleOrDefault(a =>
                                                        a.AFinType == LearnDelAppFinType.TNP.ToString() &&
                                                        a.AFinCode == (int) LearnDelAppFinCode.TotalAssessmentPrice);
            if (tnp == null)
            {
                tnp = new MessageLearnerLearningDeliveryAppFinRecord()
                      {
                          AFinType = LearnDelAppFinType.TNP.ToString(),
                          AFinCode = (int) LearnDelAppFinCode.TotalAssessmentPrice,
                          AFinCodeSpecified = true
                      };
                appFinRecords.Add(tnp);
            }

            tnp.AFinAmount = Convert.ToInt32(priceEpisode.TotalAssessmentPrice);
            tnp.AFinAmountSpecified = true;
            tnp.AFinDate = priceEpisode.TotalAssessmentPriceEffectiveDate.ToDate();
            tnp.AFinDateSpecified = true;
        }

        private void MutateAimType3ForLearnerFromTrainingRecord(List<MessageLearnerLearningDelivery> learnerLearningDeliveries, MessageLearnerLearningDelivery delivery)
        {
            var functionalSkillLearningDelivery = learnerLearningDeliveries.Single(learnDelivery =>
                learnDelivery.AimType == 3);
            functionalSkillLearningDelivery.LearnStartDate = delivery.LearnStartDate;
            functionalSkillLearningDelivery.LearnPlanEndDate = delivery.LearnPlanEndDate;

            MutateCompletionStatusForLearner(functionalSkillLearningDelivery, (int) delivery.CompStatus, delivery.LearnActEndDate);
        }

        private long CalculateEmployerContribution(string sfaContributionPercentage, decimal totalTrainingPrice)
        {
            var percentage = decimal.Parse((100 - sfaContributionPercentage.AsPercentage()).ToString());
            var employerContribution = totalTrainingPrice * (percentage / 100);

            return decimal.ToInt64(employerContribution);
        }

        private void MutateHigherEducation(MessageLearner learner)
        {
            var learningDeliveryHes = new List<MessageLearnerLearningDeliveryLearningDeliveryHE>
            {
                new MessageLearnerLearningDeliveryLearningDeliveryHE()
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
                }
            };

            var functionalSkillsLearningDeliveries = learner.LearningDelivery.ToList().Where(ld => ld.AimType == 3);
            functionalSkillsLearningDeliveries.ForEach(ld=>ld.LearningDeliveryHE = learningDeliveryHes.ToArray());
        }

    }
}
