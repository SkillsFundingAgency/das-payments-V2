using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.ProviderPayments.Application.Data;
using SFA.DAS.Payments.ProviderPayments.Model.V1;

namespace SFA.DAS.Payments.ProviderPayments.Domain.Services
{
    public interface IPaymentMapper
    {
        (List<LegacyPaymentModel> payments, List<LegacyRequiredPaymentModel> requiredPayments) MapV2Payments(
            List<PaymentModelWithRequiredPaymentId> payments);
    }

    public class PaymentMapper : IPaymentMapper
    {
        public (List<LegacyPaymentModel> payments, List<LegacyRequiredPaymentModel> requiredPayments) MapV2Payments(
            List<PaymentModelWithRequiredPaymentId> payments)
        {
            var legacyPayments = new List<LegacyPaymentModel>();
            var legacyRequiredPayments = new Dictionary<Guid, LegacyRequiredPaymentModel>();

            foreach (var paymentModel in payments)
            {
                LegacyRequiredPaymentModel requiredPayment;

                if (legacyRequiredPayments.ContainsKey(paymentModel.RequiredPaymentId))
                {
                    requiredPayment = legacyRequiredPayments[paymentModel.RequiredPaymentId];
                }
                else
                {
                    requiredPayment = new LegacyRequiredPaymentModel
                    {
                        Id = paymentModel.RequiredPaymentId,
                        AccountId = paymentModel.AccountId,
                        AccountVersionId = string.Empty,
                        AimSeqNumber = paymentModel.LearningAimSequenceNumber,
                        AmountDue = paymentModel.AmountDue,
                        ApprenticeshipContractType = (int)paymentModel.ContractType,
                        CollectionPeriodMonth = MonthFromPeriod(paymentModel.CollectionPeriod.Period),
                        CollectionPeriodName = $"{paymentModel.CollectionPeriod.AcademicYear}-R{paymentModel.CollectionPeriod.Period:D2}",
                        CollectionPeriodYear = YearFromPeriod(paymentModel.CollectionPeriod.AcademicYear, paymentModel.CollectionPeriod.Period),
                        // TODO: Fix this when available
                        CommitmentId = 0,
                        CommitmentVersionId = string.Empty,
                        UseLevyBalance = false,
                        DeliveryMonth = MonthFromPeriod(paymentModel.DeliveryPeriod),
                        DeliveryYear = YearFromPeriod(paymentModel.CollectionPeriod.AcademicYear, paymentModel.DeliveryPeriod),
                        FrameworkCode = paymentModel.LearningAimFrameworkCode,
                        FundingLineType = paymentModel.LearningAimFundingLineType,
                        IlrSubmissionDateTime = paymentModel.IlrSubmissionDateTime,
                        LearnAimRef = paymentModel.LearningAimReference,
                        LearnRefNumber = paymentModel.LearnerReferenceNumber,
                        LearningStartDate = paymentModel.StartDate,
                        PathwayCode = paymentModel.LearningAimPathwayCode,
                        PriceEpisodeIdentifier = paymentModel.PriceEpisodeIdentifier,
                        ProgrammeType = paymentModel.LearningAimProgrammeType,
                        SfaContributionPercentage = paymentModel.SfaContributionPercentage,
                        StandardCode = paymentModel.LearningAimStandardCode,
                        TransactionType = (int)paymentModel.TransactionType,
                        Ukprn = paymentModel.Ukprn,
                        Uln = paymentModel.LearnerUln,
                    };

                    legacyRequiredPayments.Add(requiredPayment.Id, requiredPayment);
                }

                var payment = new LegacyPaymentModel
                {
                    RequiredPaymentId = requiredPayment.Id,
                    CollectionPeriodMonth = requiredPayment.CollectionPeriodMonth,
                    CollectionPeriodYear = requiredPayment.CollectionPeriodYear,
                    TransactionType = requiredPayment.TransactionType??0,
                    DeliveryYear = requiredPayment.DeliveryYear??0,
                    CollectionPeriodName = requiredPayment.CollectionPeriodName,
                    DeliveryMonth = requiredPayment.DeliveryMonth??0,
                    Amount = paymentModel.Amount,
                    FundingSource = (int)paymentModel.FundingSource,
                    PaymentId = Guid.NewGuid(),
                };
                legacyPayments.Add(payment);
            }

            return (legacyPayments, legacyRequiredPayments.Values.ToList());
        }

        private int YearFromPeriod(short academicYear, byte collectionPeriod)
        {
            var ilrStartYear = academicYear / 100 * 100;

            if (collectionPeriod > 5)
            {
                return ilrStartYear + 1;
            }

            return ilrStartYear;
        }

        private int MonthFromPeriod(byte collectionPeriod)
        {
            if (collectionPeriod > 5)
            {
                return collectionPeriod + 7;
            }

            return collectionPeriod - 5;
        }
    }
}
