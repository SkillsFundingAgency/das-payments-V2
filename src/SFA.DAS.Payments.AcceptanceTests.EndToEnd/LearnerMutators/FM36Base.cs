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

        protected void MutateCommon(MessageLearner learner, LearnerRequest learnerRequest)
        {
            if (learnerRequest.StartDate.HasValue)
            {
                learner.DateOfBirth = learnerRequest.StartDate.Value.AddYears(-learnerRequest.FundingLineType.ToLearnerAge());
                learner.LearningDelivery[0].LearnStartDate = learnerRequest.StartDate.Value;
                learner.LearningDelivery[0].LearnStartDateSpecified = true;
            }

            if (learnerRequest.PlannedDurationInMonths.HasValue && learnerRequest.StartDate.HasValue)
            {
                learner.LearningDelivery[0].LearnPlanEndDate =
                    learnerRequest.StartDate.Value.AddMonths(learnerRequest.PlannedDurationInMonths.Value);
                learner.LearningDelivery[0].LearnPlanEndDateSpecified = true;
            }

            if (learnerRequest.ActualDurationInMonths.HasValue && learnerRequest.StartDate.HasValue && learnerRequest.CompletionStatus == CompStatus.Completed)
            {
                learner.LearningDelivery[0].LearnActEndDate =
                    learnerRequest.StartDate.Value.AddMonths(learnerRequest.ActualDurationInMonths.Value);
                learner.LearningDelivery[0].LearnActEndDateSpecified = true;
            }


            var ld = learner.LearningDelivery[0];
            var ldfams = ld.LearningDeliveryFAM.Where(s => s.LearnDelFAMType != LearnDelFAMType.ACT.ToString());
            ld.LearningDeliveryFAM = ldfams.ToArray();
            var lfams = ld.LearningDeliveryFAM.ToList();
            lfams.Add(new MessageLearnerLearningDeliveryLearningDeliveryFAM()
            {
                LearnDelFAMType = LearnDelFAMType.ACT.ToString(),
                LearnDelFAMCode = ((int)learnerRequest.ContractType).ToString(),
                LearnDelFAMDateFrom = ld.LearnStartDate,
                LearnDelFAMDateFromSpecified = true,
                LearnDelFAMDateTo = ld.LearnActEndDate,
                LearnDelFAMDateToSpecified = ld.LearnActEndDateSpecified,
            });

            ld.LearningDeliveryFAM = lfams.ToArray();

            ld.AppFinRecord[0].AFinDateSpecified = true;
            ld.AppFinRecord[0].AFinDate = ld.LearnStartDate;
            learner.LearningDelivery[0].AppFinRecord =
               learner.LearningDelivery[0].AppFinRecord.Where(af => af.AFinType != LearnDelAppFinType.TNP.ToString()).ToArray();

            if (learnerRequest.CompletionStatus == CompStatus.Completed)
            {
                // change AppFin to PMR
                var appfin = new List<MessageLearnerLearningDeliveryAppFinRecord>();
                appfin.Add(new MessageLearnerLearningDeliveryAppFinRecord()
                {
                    AFinAmount = learnerRequest.EmployerContribution,
                    AFinAmountSpecified = true,
                    AFinType = LearnDelAppFinType.PMR.ToString(),
                    AFinCode = (int)LearnDelAppFinCode.TrainingPayment,
                    AFinCodeSpecified = true,
                    AFinDate = learner.LearningDelivery[0].LearnActEndDate.AddMonths(-1),
                    AFinDateSpecified = true
                });

                learner.LearningDelivery[0].AppFinRecord = appfin.ToArray();
            }

            if (learnerRequest.TotalTrainingPrice.HasValue && learnerRequest.TotalTrainingPriceEffectiveDate.HasValue)
            {
                DCT.TestDataGenerator.Helpers.AddAfninRecord(learner, LearnDelAppFinType.TNP.ToString(), (int)LearnDelAppFinCode.TotalTrainingPrice, learnerRequest.TotalTrainingPrice.Value, 1, "PMR", 1, 1, learnerRequest.TotalTrainingPriceEffectiveDate);
            }

            if (learnerRequest.TotalAssessmentPriceEffectiveDate.HasValue && learnerRequest.TotalAssessmentPrice.HasValue && learnerRequest.ProgrammeType.HasValue && learnerRequest.ProgrammeType.Value == StandardProgrammeType)
            {
                DCT.TestDataGenerator.Helpers.AddAfninRecord(learner, LearnDelAppFinType.TNP.ToString(), (int)LearnDelAppFinCode.TotalAssessmentPrice, learnerRequest.TotalAssessmentPrice.Value, 1, "PMR", 1, 1, learnerRequest.TotalAssessmentPriceEffectiveDate);
            }

            foreach (var lds in learner.LearningDelivery)
            {
                if (learnerRequest.ProgrammeType.HasValue)
                {
                    lds.ProgType = learnerRequest.ProgrammeType.Value;
                    lds.ProgTypeSpecified = true;
                }

                if (learnerRequest.FrameworkCode.HasValue)
                {
                    lds.FworkCode = learnerRequest.FrameworkCode.Value;
                    lds.FworkCodeSpecified = true;
                }

                if (learnerRequest.PathwayCode.HasValue)
                {
                    lds.PwayCode = learnerRequest.PathwayCode.Value;
                    lds.PwayCodeSpecified = true;
                }

                if (learnerRequest.CompletionStatus.HasValue)
                {
                    lds.CompStatus = (int)learnerRequest.CompletionStatus.Value;
                    lds.CompStatusSpecified = true;
                }

                if (ld.LearnActEndDateSpecified && learnerRequest.CompletionStatus == CompStatus.Completed)
                {
                    lds.LearnActEndDate = ld.LearnActEndDate;
                    lds.LearnActEndDateSpecified = true;
                }

                if (learnerRequest.CompletionStatus == CompStatus.Completed)
                {
                    lds.Outcome = (int)Outcome.Achieved;
                    lds.OutcomeSpecified = true;
                }
            }

            // This needs to be fixed (for now) but we need to look at a better way to define these!
            // Component Aim Reference

            learner.LearningDelivery[1].LearnStartDate = ld.LearnStartDate;
            learner.LearningDelivery[1].LearnStartDateSpecified = true;

            var lesm = learner.LearnerEmploymentStatus.ToList();
            lesm[0].DateEmpStatApp = learner.LearningDelivery[0].LearnStartDate.AddMonths(-6);
        }

        protected void MutateCommon(MessageLearner learner, Learner learnerRequest)
        {
            learner.DateOfBirth =
                learnerRequest.Aims.First().StartDate.ToDate()
                    .AddYears(-learnerRequest.Aims.First().FundingLineType.ToLearnerAge());

            var learnerLearningDeliveries = learner.LearningDelivery.ToList();

            foreach (var aim in learnerRequest.Aims)
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

                var ldfams =
                    delivery.LearningDeliveryFAM.Where(s => s.LearnDelFAMType != LearnDelFAMType.ACT.ToString());
                delivery.LearningDeliveryFAM = ldfams.ToArray();
                var lfams = delivery.LearningDeliveryFAM.ToList();

                foreach (var priceEpisode in aim.PriceEpisodes)
                {
                    lfams.Add(new MessageLearnerLearningDeliveryLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearnDelFAMType.ACT.ToString(),
                        LearnDelFAMCode = ((int) priceEpisode.ContractType).ToString(),
                        LearnDelFAMDateFrom = delivery.LearnStartDate,
                        LearnDelFAMDateFromSpecified = true,
                        LearnDelFAMDateTo = delivery.LearnActEndDate,
                        LearnDelFAMDateToSpecified = aim.ActualDurationAsTimespan.HasValue,
                    });

                    if (aim.CompletionStatus == CompletionStatus.Completed && aim.AimSequenceNumber == 1)
                    {
                        // change AppFin to PMR
                        var appfin = new List<MessageLearnerLearningDeliveryAppFinRecord>();
                        appfin.Add(new MessageLearnerLearningDeliveryAppFinRecord()
                        {
                            AFinAmount = CalculateEmployerContribution(priceEpisode.SfaContributionPercentage,
                                priceEpisode.TotalTrainingPrice),
                            AFinAmountSpecified = true,
                            AFinType = LearnDelAppFinType.PMR.ToString(),
                            AFinCode = (int) LearnDelAppFinCode.TrainingPayment,
                            AFinCodeSpecified = true,
                            AFinDate = delivery.LearnActEndDate.AddMonths(-1),
                            AFinDateSpecified = true
                        });

                        delivery.AppFinRecord = appfin.ToArray();
                    }

                    if (aim.AimSequenceNumber == 1)
                    {
                        var appFinRecords = delivery.AppFinRecord.ToList();
                        var tnp = appFinRecords.SingleOrDefault(a => a.AFinType == "TNP");
                        if (tnp == null)
                        {
                            tnp = new MessageLearnerLearningDeliveryAppFinRecord()
                            {
                                AFinType = "TNP"
                            };
                            appFinRecords.Add(tnp);
                        }

                        tnp.AFinCode = (int) LearnDelAppFinCode.TotalTrainingPrice;
                        tnp.AFinCodeSpecified = true;
                        tnp.AFinAmount = Convert.ToInt32(priceEpisode.TotalTrainingPrice);
                        tnp.AFinAmountSpecified = true;
                        tnp.AFinDate = priceEpisode.TotalTrainingPriceEffectiveDate.ToDate();
                        tnp.AFinDateSpecified = true;

                        if (aim.ProgrammeType == StandardProgrammeType)
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

                            pmr.AFinCode = (int) LearnDelAppFinCode.TotalAssessmentPrice;
                            pmr.AFinCodeSpecified = true;
                            pmr.AFinAmount = Convert.ToInt32(priceEpisode.TotalAssessmentPrice);
                            pmr.AFinAmountSpecified = true;
                            pmr.AFinDate = priceEpisode.TotalAssessmentPriceEffectiveDate.ToDate();
                            pmr.AFinDateSpecified = true;
                        }

                        delivery.AppFinRecord = appFinRecords.ToArray();
                    }
                }

                delivery.LearningDeliveryFAM = lfams.ToArray();

                delivery.ProgType = aim.ProgrammeType;
                delivery.ProgTypeSpecified = true;

                delivery.FworkCode = aim.FrameworkCode;
                delivery.FworkCodeSpecified = true;

                delivery.PwayCode = aim.PathwayCode;
                delivery.PwayCodeSpecified = true;

                delivery.CompStatus = (int) aim.CompletionStatus;
                delivery.CompStatusSpecified = true;

                if (aim.CompletionStatus == CompletionStatus.Completed)
                {
                    delivery.Outcome = (int) Outcome.Achieved;
                    delivery.OutcomeSpecified = true;
                }

            }

            learner.LearningDelivery = learnerLearningDeliveries.ToArray();

            learner.LearnerEmploymentStatus[0].DateEmpStatApp =
                learner.LearningDelivery[0].LearnStartDate.AddMonths(-6);

        }

        protected void MutateHigherEducation(MessageLearner learner)
        {
            var ldhe = new List<MessageLearnerLearningDeliveryLearningDeliveryHE>();

            ldhe.Add(new MessageLearnerLearningDeliveryLearningDeliveryHE()
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

            learner.LearningDelivery[1].LearningDeliveryHE = ldhe.ToArray();
        }
        private long CalculateEmployerContribution(string sfaContributionPercentage, decimal totalTrainingPrice)
        {
            if (string.IsNullOrWhiteSpace(sfaContributionPercentage) || !sfaContributionPercentage.Contains("%") || !int.TryParse(sfaContributionPercentage.Split('%')[0],
                    out _))
            {
                throw new InvalidCastException("SfaContributionPercentage is not in the format: xx% (e.g. 90%)");
            }

            var percentage = decimal.Parse((100 - int.Parse(sfaContributionPercentage.Split('%')[0])).ToString());

            var employerContribution = totalTrainingPrice * (percentage / 100);

            return decimal.ToInt64(employerContribution);
        }
    }
}
