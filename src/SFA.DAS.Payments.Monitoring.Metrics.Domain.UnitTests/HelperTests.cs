using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests
{
    [TestFixture]
    public class HelperTests
    {

        [Test]
        public void GetPaymentsSummary_Calculates_Correct_Values()
        {
            IPeriodEndSummaryModel summaryModel = new PeriodEndSummaryModel()
            {
                YearToDatePayments = new ContractTypeAmounts(){ ContractType1 = 500m, ContractType2 = 1000m},
                AdjustedDataLockedEarnings = 1200m,
                Payments =  new ContractTypeAmounts(){ContractType1 = 300m, ContractType2 = 100m},
                HeldBackCompletionPayments =  new ContractTypeAmounts(){ContractType1 = 300m, ContractType2 = 100m},
                DcEarnings = new ContractTypeAmounts(){ ContractType2 = 11000, ContractType1 = 6000}
            };

            var paymentMetrics = Helpers.CreatePaymentMetrics(summaryModel);

            paymentMetrics.ContractType1.Should().Be(summaryModel.YearToDatePayments.ContractType1 +
                                                               summaryModel.Payments.ContractType1 +
                                                               summaryModel.AdjustedDataLockedEarnings +
                                                               summaryModel.HeldBackCompletionPayments.ContractType1);
            paymentMetrics.ContractType2.Should().Be(summaryModel.YearToDatePayments.ContractType2 +
                                                     summaryModel.Payments.ContractType2 +
                                                     summaryModel.HeldBackCompletionPayments.ContractType2);

            paymentMetrics.DifferenceContractType1.Should()
                .Be(paymentMetrics.ContractType1 - summaryModel.DcEarnings.ContractType1);
            paymentMetrics.DifferenceContractType2.Should()
                .Be(paymentMetrics.ContractType2 - summaryModel.DcEarnings.ContractType2);

            paymentMetrics.PercentageContractType1.Should()
                .Be(paymentMetrics.ContractType1 / summaryModel.DcEarnings.ContractType1 * 100);
            paymentMetrics.PercentageContractType2.Should()
                .Be(paymentMetrics.ContractType2 / summaryModel.DcEarnings.ContractType2 * 100);

            paymentMetrics.Percentage.Should().Be(paymentMetrics.Total / summaryModel.DcEarnings.Total * 100);
        }
    }
}