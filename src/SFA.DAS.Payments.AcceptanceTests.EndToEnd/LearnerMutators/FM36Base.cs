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
    public abstract class FM36Base : ILearnerMultiMutator
    {
        private const int StandardProgrammeType = 25;
        private const int StandardProgrammeEpaDuration = 8;
        private const string ProgrammeAim = "ZPROG001";
        private readonly string featureNumber;
        protected ILearnerCreatorDataCache dataCache;

        private readonly IEnumerable<Learner> learners;


        protected FM36Base(IEnumerable<Learner> learners, string featureNumber)
        {
            if (learners == null || !learners.Any())
            {
                throw new ArgumentException("At least one learner is required.");
            }

            this.learners = learners;
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

            MutateAimForLearner(learnerRequest.Aims, learnerLearningDeliveries, learnerRequest.Aims.Count());

            learner.LearningDelivery = learnerLearningDeliveries.ToArray();

            learner.LearnerEmploymentStatus[0].DateEmpStatApp =
                learner.LearningDelivery[0].LearnStartDate.AddMonths(-6);

            switch (learnerRequest.SmallEmployer)
            {
                case "SEM1":
                    {
                        var les1 = learner.LearnerEmploymentStatus[0];
                        var lesm1 = les1.EmploymentStatusMonitoring.ToList();
                        lesm1.Add(new MessageLearnerLearnerEmploymentStatusEmploymentStatusMonitoring()
                        {
                            ESMType = EmploymentStatusMonitoringType.SEM.ToString(),
                            ESMCode = (int)EmploymentStatusMonitoringCode.SmallEmployer,
                            ESMCodeSpecified = true
                        });
                        learner.LearnerEmploymentStatus[0].EmploymentStatusMonitoring = lesm1.ToArray();
                        break;
                    }
            }

            learner.ULN = learnerRequest.Uln;
            learner.ULNSpecified = true;

            MutateHigherEducation(learner);

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

        protected void ProcessMessageLearnerForLearnerRequestOriginatingFromTrainingRecord(MessageLearnerLearningDelivery functionalSkillsLearningDelivery, Aim aim)
        {
            SetDeliveryAsWithdrawn(functionalSkillsLearningDelivery, aim);
        }

        protected void RemovePmrRecord(MessageLearner learner)
        {
            var deliveries = learner.LearningDelivery.ToList().Where(ld => ld.AimType == 1);
            deliveries.ForEach(d => d.AppFinRecord = d.AppFinRecord.ToList().Where(af => af.AFinType != LearnDelAppFinType.PMR.ToString()).ToArray());
        }

        private void MutateAimForLearner(List<Aim> aims, List<MessageLearnerLearningDelivery> learnerLearningDeliveries, int numberOfAimsForLearner)
        {

            MutateMainAimForLearner(aims.Single(a => a.IsMainAim),
                learnerLearningDeliveries.Single(ld => ld.LearnAimRef == ProgrammeAim));

            MutateOtherAimsForLearner(aims.Where(a => !a.IsMainAim),
                learnerLearningDeliveries.Where(ld => ld.LearnAimRef != ProgrammeAim));

            if (numberOfAimsForLearner == 1) // assume that this aim was created through the old style of Training records - in which case we need to setup the functionalskillsdelivery as well.
            {
                MutateAimType3ForLearnerFromTrainingRecord(learnerLearningDeliveries,
                    learnerLearningDeliveries.Single(ld => ld.LearnAimRef == ProgrammeAim));
            }
        }

        private void MutateOtherAimsForLearner(IEnumerable<Aim> otherAims, IEnumerable<MessageLearnerLearningDelivery> otherLearningDeliveries)
        {
            otherAims.OrderBy(x => x.AimSequenceNumber);
            otherLearningDeliveries.OrderBy(x => x.AimSeqNumber);
            for (var i = 0; i < otherAims.Count(); i++)
            {
                var otherLearningDelivery = otherLearningDeliveries.Skip(i).Take(1).Single();
                var otherAim = otherAims.Skip(i).Take(1).Single();
                otherLearningDelivery.LearnAimRef = otherAim.AimReference;

                var actualEndDate = MutateDates(otherAim, otherLearningDelivery);

                MutateCompletionStatusForLearner(otherLearningDelivery, (int)otherAim.CompletionStatus, actualEndDate);

                MutateLearningDeliveryFamsForLearner(otherLearningDelivery, otherAim);

                MutateProgrammeCodes(otherAim, otherLearningDelivery);
            }
        }

        private void MutateMainAimForLearner(Aim aim, MessageLearnerLearningDelivery learningDelivery)
        {
            learningDelivery.LearnAimRef = aim.AimReference;

            var actualEndDate = MutateDates(aim, learningDelivery);

            MutateCompletionStatusForLearner(learningDelivery, (int)aim.CompletionStatus, actualEndDate);

            MutateLearningDeliveryFamsForLearner(learningDelivery, aim);

            MutateProgrammeCodes(aim, learningDelivery);
        }

        private DateTime? MutateDates(Aim aim, MessageLearnerLearningDelivery learningDelivery)
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

        private void MutateProgrammeCodes(Aim aim, MessageLearnerLearningDelivery learningDelivery)
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
                case (int) CompletionStatus.Completed:
                    delivery.Outcome = (int) Outcome.Achieved;
                    delivery.OutcomeSpecified = true;
                    break;
                case (int) CompletionStatus.Withdrawn:
                    delivery.Outcome = (int) Outcome.NoAchievement;
                    delivery.OutcomeSpecified = true;
                    delivery.WithdrawReason = (int) WithDrawalReason.FinancialReasons;
                    delivery.WithdrawReasonSpecified = true;
                    break;
                default:
                {
                    break;
                }
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
            if (aim.CompletionStatus == CompletionStatus.Completed)
            {
                MutateDeliveryAppFinRecordToPMR(delivery, priceEpisode);
            }

            var appFinRecords = delivery.AppFinRecord.ToList();

            AddNewTnpAppFinRecordForTrainingPrice(appFinRecords, priceEpisode);

            if (aim.ProgrammeType == StandardProgrammeType)
            {
                AddNewPmrAppFinRecord(appFinRecords, priceEpisode);
                AddTnpAppFinRecordForAssessmentPrice(appFinRecords, priceEpisode);
                delivery.EPAOrgID = "EPA0022";
            }

            delivery.AppFinRecord = appFinRecords.ToArray();
        }

        private void AddNewTnpAppFinRecordForTrainingPrice(List<MessageLearnerLearningDeliveryAppFinRecord> appFinRecords, Price priceEpisode)
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

            var functionalSkillsLearningDeliveries = learner.LearningDelivery.ToList().Where(ld => ld.AimType == 3);
            functionalSkillsLearningDeliveries.ForEach(ld => ld.LearningDeliveryHE = learningDeliveryHes.ToArray());
        }

    }
}
