using System;
using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator.Model;
using ESFA.DC.ILR.Model.Loose;

namespace DCT.TestDataGenerator.Functor
{
    /// <summary>
    /// The FM35 test data generator helps create a range of high quality test data specifically to test the FM35 funding model
    /// It starts with simple learners but becomes more and more complex in what the earning calculation has to do and areas where in the past there have been specific
    /// issues and bugs
    /// </summary>
    public class FM36_261
        : ILearnerMultiMutator
    {
        private ILearnerCreatorDataCache _dataCache;
        private GenerationOptions _options;
        private NonLevyLearnerRequest _learnerRequest;

        public FilePreparationDateRequired FilePreparationDate()
        {
            return FilePreparationDateRequired.July;
        }

        public IEnumerable<LearnerTypeMutator> LearnerMutators(ILearnerCreatorDataCache cache)
        {
            _dataCache = cache;
            _learnerRequest = (NonLevyLearnerRequest)_dataCache.LearnerRequest();
            if (_learnerRequest == null)
            {
                SetupLearnerRequest();
            }

            return new List<LearnerTypeMutator>()
            {
                new LearnerTypeMutator() { LearnerType = LearnerTypeRequired.Apprenticeships, DoMutateLearner = Mutate19, DoMutateOptions = MutateGenerationOptionsOlderApprenticeship }
            };
        }

        public string RuleName()
        {
            return "FM36";
        }

        public string LearnerReferenceNumberStub()
        {
            return $"fm36{_learnerRequest.FeatureNumber}{_learnerRequest.LearnerId}";
        }

        private void SetupLearnerRequest()
        {
            _learnerRequest = new NonLevyLearnerRequest()
            {
                StartDate = new DateTime(2018, 08, 03),
                PlannedDurationInMonths = 12,
                TotalTrainingPrice = 15000,
                ProgrammeType = 20,
                FrameworkCode = 593,
                PathwayCode = 1,
                AimReferenceNumber = "ZPROG001"
            };
        }

        private void Mutate19(MessageLearner learner, bool valid)
        {
            Helpers.MutateDOB(learner, valid, Helpers.AgeRequired.Exact19, Helpers.BasedOn.LearnDelStart, Helpers.MakeOlderOrYoungerWhenInvalid.NoChange);
            MutateCommon(learner, valid);
        }

        private void MutateCommon(MessageLearner learner, bool valid)
        {
            if (_learnerRequest.StartDate.HasValue)
            {
                learner.DateOfBirth = _learnerRequest.StartDate.Value.AddYears(-21);
            }

            learner.LearningDelivery[0].LearnPlanEndDateSpecified = _learnerRequest.PlannedDurationInMonths.HasValue;

            if (_learnerRequest.PlannedDurationInMonths.HasValue && _learnerRequest.StartDate.HasValue)
            {
                learner.LearningDelivery[0].LearnPlanEndDate =
                    _learnerRequest.StartDate.Value.AddMonths(_learnerRequest.PlannedDurationInMonths.Value);
                learner.LearningDelivery[0].LearnPlanEndDateSpecified = true;
            }

            if (_learnerRequest.ActualDurationInMonths.HasValue && _learnerRequest.StartDate.HasValue && _learnerRequest.CompletionStatus == CompStatus.Completed)
            {
                learner.LearningDelivery[0].LearnActEndDate =
                    _learnerRequest.StartDate.Value.AddMonths(_learnerRequest.ActualDurationInMonths.Value);
                learner.LearningDelivery[0].LearnActEndDateSpecified = true;
            }

            learner.LearningDelivery[0].LearnStartDateSpecified = _learnerRequest.StartDate.HasValue;

            if (_learnerRequest.StartDate.HasValue)
            {
                learner.LearningDelivery[0].LearnStartDate = _learnerRequest.StartDate.Value;
                learner.LearningDelivery[0].LearnStartDateSpecified = true;
            }

            var ld = learner.LearningDelivery[0];
            var ldfams = ld.LearningDeliveryFAM.Where(s => s.LearnDelFAMType != LearnDelFAMType.ACT.ToString());
            ld.LearningDeliveryFAM = ldfams.ToArray();
            var lfams = ld.LearningDeliveryFAM.ToList();
            lfams.Add(new MessageLearnerLearningDeliveryLearningDeliveryFAM()
            {
                LearnDelFAMType = LearnDelFAMType.ACT.ToString(),
                LearnDelFAMCode = ((int)LearnDelFAMCode.ACT_ContractESFA).ToString(),
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

            if (_learnerRequest.CompletionStatus == CompStatus.Completed)
            {
                // change AppFin to PMR
                var appfin = new List<MessageLearnerLearningDeliveryAppFinRecord>();
                appfin.Add(new MessageLearnerLearningDeliveryAppFinRecord()
                {
                    AFinAmount = _learnerRequest.TotalTrainingPrice.Value,
                    AFinAmountSpecified = true,
                    AFinType = LearnDelAppFinType.PMR.ToString(),
                    AFinCode = (int)LearnDelAppFinCode.TrainingPayment,
                    AFinCodeSpecified = true,
                    AFinDate = learner.LearningDelivery[0].LearnActEndDate.AddMonths(-1),
                    AFinDateSpecified = true
                });

                learner.LearningDelivery[0].AppFinRecord = appfin.ToArray();
            }

            if (_learnerRequest.TotalTrainingPrice.HasValue && _learnerRequest.TotalTrainingPriceEffectiveDate.HasValue)
            {
                Helpers.AddAfninRecord(learner, LearnDelAppFinType.TNP.ToString(), (int)LearnDelAppFinCode.TotalTrainingPrice, _learnerRequest.TotalTrainingPrice.Value, 1, "PMR", 1, 1, _learnerRequest.TotalTrainingPriceEffectiveDate);
            }

            if (_learnerRequest.TotalAssessmentPriceEffectiveDate.HasValue && _learnerRequest.TotalAssessmentPrice.HasValue)
            {
                Helpers.AddAfninRecord(learner, LearnDelAppFinType.TNP.ToString(), (int)LearnDelAppFinCode.TotalAssessmentPrice, _learnerRequest.TotalAssessmentPrice.Value, 1, "PMR", 1, 1, _learnerRequest.TotalAssessmentPriceEffectiveDate);
            }

            foreach (var lds in learner.LearningDelivery)
            {
                if (_learnerRequest.ProgrammeType.HasValue)
                {
                    lds.ProgType = _learnerRequest.ProgrammeType.Value;
                    lds.ProgTypeSpecified = true;
                }

                if (_learnerRequest.FrameworkCode.HasValue)
                {
                    lds.FworkCode = _learnerRequest.FrameworkCode.Value;
                    lds.FworkCodeSpecified = true;
                }

                if (_learnerRequest.PathwayCode.HasValue)
                {
                    lds.PwayCode = _learnerRequest.PathwayCode.Value;
                    lds.PwayCodeSpecified = true;
                }

                if (_learnerRequest.CompletionStatus.HasValue)
                {
                    lds.CompStatus = (int)_learnerRequest.CompletionStatus.Value;
                    lds.CompStatusSpecified = true;
                }

                if (ld.LearnPlanEndDateSpecified && _learnerRequest.CompletionStatus == CompStatus.Completed)
                {
                    lds.LearnActEndDate = ld.LearnPlanEndDate;
                    lds.LearnActEndDateSpecified = true;
                }

                if (_learnerRequest.CompletionStatus == CompStatus.Completed)
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

            if (_learnerRequest.CompletionStatus != CompStatus.Completed)
            {
                learner.LearningDelivery[1].LearnAimRef = "6030571x";
            }
            else
            {
                learner.LearningDelivery[1].LearnAimRef = "00300545";
                MutateHE(learner, valid);
            }
        }

        private void MutateHE(MessageLearner learner, bool valid)
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

        private void MutateGenerationOptionsOlderApprenticeship(GenerationOptions options)
        {
            _options = options;
            options.LD.IncludeHHS = true;
        }
    }
}
