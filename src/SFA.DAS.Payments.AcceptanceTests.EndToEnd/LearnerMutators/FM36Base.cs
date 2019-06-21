using System;
using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using DCT.TestDataGenerator.Functor;
using ESFA.DC.ILR.Model.Loose;
using MoreLinq;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public abstract class Fm36Base : ILearnerMultiMutator
    {
        private const int StandardProgrammeType = 25;
        private const int StandardProgrammeEpaDuration = 8;
        private const string ProgrammeAim = "ZPROG001";
        private const string EpaOrgId = "EPA0022";
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
                    DoMutateLearner = (messageLearner, valid) =>
                    {
                        MutateCommon(messageLearner, learner);
                        DoSpecificMutate(messageLearner, learner);
                    },
                    DoMutateOptions = options => DoMutateOptions(options, learner)
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

        protected void DoMutateOptions(GenerationOptions options, Learner learner)
        {
            options.LD.IncludeHHS = true;
            options.LD.GenerateMultipleLDs = Math.Max(1, learner.Aims.Count(a => !a.IsMainAim));
        }

        protected void MutateCommon(MessageLearner messageLearner, Learner learner)
        {
            messageLearner.DateOfBirth =
                learner.Aims.First().StartDate.ToDate()
                    .AddYears(-learner.Aims.First().FundingLineType.ToLearnerAge());

            if (!string.IsNullOrWhiteSpace(learner.PostcodePrior))
            {
                messageLearner.PostcodePrior = learner.PostcodePrior;
            }

            var learnerLearningDeliveries = messageLearner.LearningDelivery.ToList();

            MutateAimsForLearner(learner.Aims, learnerLearningDeliveries);

            messageLearner.LearningDelivery = learnerLearningDeliveries.ToArray();

            messageLearner.LearnerEmploymentStatus[0].DateEmpStatApp =
                messageLearner.LearningDelivery[0].LearnStartDate.AddMonths(-6);

            AddSmallEmployerInfo(learner, learnerRequest);

            messageLearner.ULN = learner.Uln;
            messageLearner.ULNSpecified = true;

            MutateHigherEducation(learner);
        }

        private static void AddSmallEmployerInfo(MessageLearner learner, Learner learnerRequest)
        {
            if (learnerRequest.SmallEmployer != "SEM1") 
                return;

            var learnerEmploymentStatus = learner.LearnerEmploymentStatus[0];
            var statusMonitorings = learnerEmploymentStatus.EmploymentStatusMonitoring.ToList();

            statusMonitorings.Add(new MessageLearnerLearnerEmploymentStatusEmploymentStatusMonitoring
            {
                ESMType = EmploymentStatusMonitoringType.SEM.ToString(),
                ESMCode = (int) EmploymentStatusMonitoringCode.SmallEmployer,
                ESMCodeSpecified = true
            });

            learner.LearnerEmploymentStatus[0].EmploymentStatusMonitoring = statusMonitorings.ToArray();

            var eefCode = (LearnDelFAMCode) learnerRequest.EefCode.GetValueOrDefault((int) LearnDelFAMCode.EEF_Apprenticeship_19);
            DCT.TestDataGenerator.Helpers.AddLearningDeliveryFAM(learner, LearnDelFAMType.EEF, eefCode);
        }

        private void MutateAimsForLearner(List<Aim> aims, List<MessageLearnerLearningDelivery> learnerLearningDeliveries)
        {
            MutateMainAimForLearner(aims.Where(a => a.IsMainAim), learnerLearningDeliveries);

            MutateOtherAimsForLearner(aims.Where(a => !a.IsMainAim),
                learnerLearningDeliveries.Where(ld => ld.LearnAimRef != ProgrammeAim));

            if (aims.All(x => x.IsMainAim)) // assume that this aim was created through the old style of Training records - in which case we need to setup the functionalskillsdelivery as well.
            {
                MutateAimType3ForLearnerFromTrainingRecord(learnerLearningDeliveries,
                                                           learnerLearningDeliveries.Last(ld => ld.LearnAimRef == ProgrammeAim));
            }
        }

        private void MutateMainAimForLearner(IEnumerable<Aim> mainAims, List<MessageLearnerLearningDelivery> learningDeliveries)
        {
            var orderedAims = mainAims.OrderBy(x => x.AimSequenceNumber).ToList();
            var mainLearningDeliveries = learningDeliveries.Where(ld => ld.LearnAimRef == ProgrammeAim).ToList();

            for (var i = 0; i < orderedAims.Count; i++)
            {
                var aim = orderedAims[i];
                var delivery = mainLearningDeliveries.Skip(i).Take(1).SingleOrDefault();

                if (delivery == null)
                {
                    delivery = mainLearningDeliveries.First(ld => ld.LearnAimRef == ProgrammeAim).DeepClone();
                    delivery.AimSeqNumber = aim.AimSequenceNumber;
                    learningDeliveries.Add(delivery);
                }

                delivery.LearnAimRef = aim.AimReference;

                var actualEndDate = SetLearningDeliveryDates(aim, delivery);

                MutateCompletionStatusForLearner(delivery, (int)aim.CompletionStatus, actualEndDate);

                MutateLearningDeliveryFamsForLearner(delivery, aim);

                SetCourseCodes(aim, delivery);
            }
        }
        protected void SetFrameworkComponentAimDetails(MessageLearner learner, string learnAimRef)
        {
            var programmeAim = learner.LearningDelivery.Single(ld => ld.AimType == 1);
            var componentAim = learner.LearningDelivery.Single(ld => ld.AimType == 3);
            componentAim.LearnAimRef = learnAimRef;
            componentAim.FundModel = programmeAim.FundModel;
            componentAim.ProgType = programmeAim.ProgType;
            componentAim.FworkCode = programmeAim.FworkCode;
            componentAim.LearnStartDate = programmeAim.LearnStartDate;
        }

        private void MutateOtherAimsForLearner(IEnumerable<Aim> otherAims, IEnumerable<MessageLearnerLearningDelivery> otherLearningDeliveries)
        {
            var orderedAims = otherAims.OrderBy(x => x.AimSequenceNumber).ToList();
            var orderedLearningDeliveries = otherLearningDeliveries.OrderBy(x => x.AimSeqNumber).ToList();
            for (var i = 0; i < orderedAims.Count; i++)
            {
                var otherLearningDelivery = orderedLearningDeliveries[i];
                var otherAim = orderedAims[i];
                otherLearningDelivery.LearnAimRef = otherAim.AimReference;

                var actualEndDate = SetLearningDeliveryDates(otherAim, otherLearningDelivery);

                MutateCompletionStatusForLearner(otherLearningDelivery, (int)otherAim.CompletionStatus, actualEndDate);

                MutateLearningDeliveryFamsForLearner(otherLearningDelivery, otherAim);

                SetCourseCodes(otherAim, otherLearningDelivery);
            }
        }

        private DateTime? SetLearningDeliveryDates(Aim aim, MessageLearnerLearningDelivery learningDelivery)
        {
            learningDelivery.LearnStartDate = aim.StartDate.ToDate();
            learningDelivery.LearnStartDateSpecified = true;

            if (aim.PlannedDurationAsTimespan.HasValue)
            {
                if (aim.ProgrammeType == StandardProgrammeType)
                {
                    learningDelivery.LearnPlanEndDate =
                        learningDelivery.LearnStartDate.Add(aim.PlannedDurationAsTimespan.Value).AddDays(StandardProgrammeEpaDuration);
                    learningDelivery.LearnPlanEndDateSpecified = true;
                }
                else
                {
                    learningDelivery.LearnPlanEndDate = learningDelivery.LearnStartDate.Add(aim.PlannedDurationAsTimespan.Value);
                    learningDelivery.LearnPlanEndDateSpecified = true;
                }
            }

            DateTime? actualEndDate = null;
            if (aim.ActualDurationAsTimespan.HasValue)
            {
                actualEndDate = aim.ProgrammeType == StandardProgrammeType
                                    ? learningDelivery.LearnStartDate.Add(aim.ActualDurationAsTimespan.Value)
                                                      .AddDays(StandardProgrammeEpaDuration)
                                    : learningDelivery.LearnStartDate.Add(aim.ActualDurationAsTimespan.Value);
            }


            return actualEndDate;
        }

        private void MutateCompletionStatusForLearner(MessageLearnerLearningDelivery delivery, int completionStatus, DateTime? actualEndDate)
        {
            delivery.CompStatus = completionStatus;
            delivery.CompStatusSpecified = true;

            if (actualEndDate.HasValue && actualEndDate.Value != DateTime.MinValue)
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
                    delivery.Outcome = (int)Outcome.NoAchievement;
                    delivery.OutcomeSpecified = true;
                    delivery.WithdrawReason = (int)WithDrawalReason.NotKnown;
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

        private void SetCourseCodes(Aim aim, MessageLearnerLearningDelivery learningDelivery)
        {
            learningDelivery.ProgType = aim.ProgrammeType;
            learningDelivery.ProgTypeSpecified = true;

            if (aim.ProgrammeType == StandardProgrammeType)
            {
                learningDelivery.StdCode = aim.StandardCode;
                learningDelivery.StdCodeSpecified = true;
                learningDelivery.FworkCodeSpecified = false;
                learningDelivery.PwayCodeSpecified = false;
            }
            else
            {
                learningDelivery.FworkCode = aim.FrameworkCode;
                learningDelivery.FworkCodeSpecified = true;

                learningDelivery.PwayCode = aim.PathwayCode;
                learningDelivery.PwayCodeSpecified = true;
            }
        }

        protected void SetDeliveryAsWithdrawn(MessageLearnerLearningDelivery delivery, Aim learnerRequestAim)
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
            var deliveries = learner.LearningDelivery.Where(ld => ld.AimType == 1).ToList();
            deliveries.ForEach(d => d.AppFinRecord = d.AppFinRecord.Where(af => af.AFinType != LearnDelAppFinType.PMR.ToString()).ToArray());
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

        protected void SetupTnpAppFinRecord(MessageLearner messageLearner, MessageLearnerLearningDelivery delivery)
        {
            var appFinRecord =
                delivery.AppFinRecord.SingleOrDefault(afr => afr.AFinType == LearnDelAppFinType.TNP.ToString());

            if (appFinRecord == null)
            {
                // generate dummy record
                DCT.TestDataGenerator.Helpers.AddAfninRecord(messageLearner, LearnDelAppFinType.TNP.ToString(), (int)LearnDelAppFinCode.TotalTrainingPrice, 15000);

                appFinRecord =
                    delivery.AppFinRecord.SingleOrDefault(afr => afr.AFinType == LearnDelAppFinType.TNP.ToString());
            }

            appFinRecord.AFinDate = delivery.LearnStartDate;
            appFinRecord.AFinDateSpecified = true;
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
            if (aim.CompletionStatus == CompletionStatus.Completed)
            {
                MutateDeliveryAppFinRecordToPmr(delivery, priceEpisode);
            }

            var appFinRecords = delivery.AppFinRecord?.ToList() ?? new List<MessageLearnerLearningDeliveryAppFinRecord>();

            AddNewTnpAppFinRecordForTrainingPrice(appFinRecords, priceEpisode);

            if (aim.ProgrammeType == StandardProgrammeType)
            {
                if (aim.CompletionStatus == CompletionStatus.Completed)
                {
                    AddNewPmrAppFinRecord(appFinRecords, priceEpisode);
                }

                AddTnpAppFinRecordForAssessmentPrice(appFinRecords, priceEpisode);
                delivery.EPAOrgID = EpaOrgId;
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

            pmr.AFinCode = (int)LearnDelAppFinCode.TotalAssessmentPrice;
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
                                                        a.AFinCode == (int)LearnDelAppFinCode.TotalAssessmentPrice);
            if (tnp == null)
            {
                tnp = new MessageLearnerLearningDeliveryAppFinRecord()
                {
                    AFinType = LearnDelAppFinType.TNP.ToString(),
                    AFinCode = (int)LearnDelAppFinCode.TotalAssessmentPrice,
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

            MutateCompletionStatusForLearner(functionalSkillLearningDelivery, (int)delivery.CompStatus, delivery.LearnActEndDate);
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
            functionalSkillsLearningDeliveries.ForEach(ld => ld.LearningDeliveryHE = learningDeliveryHes.ToArray());
        }

    }
}
