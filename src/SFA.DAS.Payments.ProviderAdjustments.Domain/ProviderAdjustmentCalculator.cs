using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.ProviderAdjustments.Domain
{
    public interface IProviderAdjustmentsCalculator
    {
        List<ProviderAdjustment> CalculateDelta(
            List<ProviderAdjustment> historicPayments,
            List<ProviderAdjustment> currentPayments);

        void PopulateCollectonPeriodForPayments(List<ProviderAdjustment> payments, int academicYear, int collectionPeriod);
    }

    public class ProviderAdjustmentCalculator : IProviderAdjustmentsCalculator
    {
        public List<ProviderAdjustment> CalculateDelta(
            List<ProviderAdjustment> historicPayments,
            List<ProviderAdjustment> currentPayments)
        {
            var groupedPreviousPayments = historicPayments.ToLookup(x => new ProviderAdjustmentPaymentGrouping(x));

            var groupedEarnings = EarningsGroupsThatHaveNotBeenProcessed(currentPayments);

            var payments = CalculatePayments(groupedEarnings, groupedPreviousPayments);
            var processedEarningsGroups = new HashSet<ProviderAdjustmentPaymentGrouping>(groupedEarnings.Select(x => x.Key));

            var refunds = CalculateRefunds(groupedPreviousPayments, processedEarningsGroups);

            return payments.Union(refunds).ToList();
        }

        public void PopulateCollectonPeriodForPayments(List<ProviderAdjustment> payments, int academicYear, int collectionPeriod)
        {
            var collectionPeriodMonth = collectionPeriod < 6 ? collectionPeriod + 7 : collectionPeriod - 5;
            var collectionPeriodYear = collectionPeriod < 6 ? 2000 + academicYear / 100 : 2000 + academicYear / 100 + 1;
            var collectionPeriodName = $"{academicYear}-R{collectionPeriod:D2}";

            if (collectionPeriod > 12)
                collectionPeriodMonth++;

            foreach (var payment in payments)
            {
                payment.CollectionPeriodMonth = collectionPeriodMonth;
                payment.CollectionPeriodYear = collectionPeriodYear;
                payment.CollectionPeriodName = collectionPeriodName;
                payment.SubmissionAcademicYear = academicYear;
            }
        }

        private static IEnumerable<ProviderAdjustment> CalculatePayments(
            ILookup<ProviderAdjustmentPaymentGrouping, ProviderAdjustment> groupedEarnings,
            ILookup<ProviderAdjustmentPaymentGrouping, ProviderAdjustment> groupedPreviousPayments)
        {
            foreach (var earningGroup in groupedEarnings)
            {
                var paymentAmount = earningGroup.Sum(x => x.Amount) -
                                    groupedPreviousPayments[earningGroup.Key].Sum(x => x.Amount);

                if (paymentAmount != 0)
                {
                    yield return CreatePayment(earningGroup.Key, paymentAmount);
                }
            }
        }

        private static IEnumerable<ProviderAdjustment> CalculateRefunds(
            ILookup<ProviderAdjustmentPaymentGrouping, ProviderAdjustment> groupedPreviousPayments,
            HashSet<ProviderAdjustmentPaymentGrouping> alreadyProcessedGroups)
        {
            foreach (var previousPaymentGroup in groupedPreviousPayments)
            {
                if (alreadyProcessedGroups.Contains(previousPaymentGroup.Key))
                {
                    continue;
                }

                var paymentAmount = -1 * previousPaymentGroup.Sum(x => x.Amount);
                if (paymentAmount != 0)
                {
                    yield return CreatePayment(previousPaymentGroup.Key, paymentAmount);
                }
            }
        }

        private static ProviderAdjustment CreatePayment(ProviderAdjustmentPaymentGrouping source, decimal amount)
        {
            var payment = new ProviderAdjustment
            {
                Amount = amount,
                PaymentType = source.PaymentType,
                Ukprn = source.Ukprn,
                PaymentTypeName = source.PaymentTypeName,
                SubmissionCollectionPeriod = source.Period,
                SubmissionId = source.SubmissionId,
            };
            return payment;
        }

        private static ILookup<ProviderAdjustmentPaymentGrouping, ProviderAdjustment> EarningsGroupsThatHaveNotBeenProcessed(List<ProviderAdjustment> earnings)
        {
            return earnings
                .ToLookup(x => new ProviderAdjustmentPaymentGrouping(x));
        }
    }
}
