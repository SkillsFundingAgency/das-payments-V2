using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd
{
    public interface IPeriodEndSummary
    {
       PeriodEndSummaryModel GetMetrics();
       void AddProviderSummaries(List<ProviderPeriodEndSummaryModel> providerSummaries);
    }
    public class PeriodEndSummary :IPeriodEndSummary
    {
        private readonly long jobId;
        private readonly byte collectionPeriod;
        private readonly short academicYear;
        private List<ProviderPeriodEndSummaryModel> allSummaries;
        private ContractTypeAmounts dcEarnings;
        private ContractTypeAmounts payments;
        private ContractTypeAmounts yearToDatePayments;
        private ContractTypeAmounts heldBackCompletionPayments;
        private decimal dataLockedEarnings;
        private decimal dataLockedAlreadyPaidTotal;

        public PeriodEndSummary(long jobId, byte collectionPeriod, short academicYear)
        {
            this.jobId = jobId;
            this.collectionPeriod = collectionPeriod;
            this.academicYear = academicYear;
            allSummaries = new List<ProviderPeriodEndSummaryModel>();
        }

        public PeriodEndSummaryModel GetMetrics()
        {
            CalculateTotals();

            var result =  new PeriodEndSummaryModel()
            {
                CollectionPeriod = collectionPeriod,
                AcademicYear = academicYear,
                JobId = jobId,
                DcEarnings = dcEarnings,
                HeldBackCompletionPayments = heldBackCompletionPayments,
                PaymentMetrics = new ContractTypeAmountsVerbose(),
                Payments = payments,
                YearToDatePayments = yearToDatePayments,
                AlreadyPaidDataLockedEarnings = dataLockedAlreadyPaidTotal,
                AdjustedDataLockedEarnings = dataLockedEarnings - dataLockedAlreadyPaidTotal,
                TotalDataLockedEarnings = dataLockedEarnings
            };
            result.PaymentMetrics = Helpers.CreatePaymentMetrics(result);
            result.Percentage = result.PaymentMetrics.Percentage;
            return result;
        }

        private void CalculateTotals()
        {
             var allEarnings = allSummaries.Select(x => x.DcEarnings).ToList();
             dcEarnings = new ContractTypeAmounts()
             {
                 ContractType1 = allEarnings.Sum(x => x.ContractType1),
                 ContractType2 = allEarnings.Sum(x => x.ContractType2)
             };

             var allProviderPayments = allSummaries.Select(x => x.Payments).ToList();
             payments = new ContractTypeAmounts()
             {
                 ContractType1 = allProviderPayments.Sum(x => x.ContractType1),
                 ContractType2 = allProviderPayments.Sum(x => x.ContractType2)
             };

             var allYearToDatePayments = allSummaries.Select(x => x.YearToDatePayments).ToList();
             yearToDatePayments = new ContractTypeAmounts()
             {
                 ContractType1 = allYearToDatePayments.Sum(x => x.ContractType1),
                 ContractType2 = allYearToDatePayments.Sum(x => x.ContractType2)
             };

             var allHeldBackCompletionPayments = allSummaries.Select(x => x.HeldBackCompletionPayments).ToList();
             heldBackCompletionPayments = new ContractTypeAmounts()
             {
                 ContractType1 = allHeldBackCompletionPayments.Sum(x=>x.ContractType1),
                 ContractType2 = allHeldBackCompletionPayments.Sum(x=>x.ContractType2),
             };

             dataLockedEarnings = allSummaries.Select(x => x.TotalDataLockedEarnings).Sum();
             dataLockedAlreadyPaidTotal = allSummaries.Select(x => x.AlreadyPaidDataLockedEarnings).Sum();
        }

        public void AddProviderSummaries(List<ProviderPeriodEndSummaryModel> providerSummaries)
        {
            allSummaries = providerSummaries;
        }
    }
}