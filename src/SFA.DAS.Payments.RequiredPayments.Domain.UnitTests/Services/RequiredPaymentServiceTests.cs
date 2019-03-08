using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class RequiredPaymentServiceTests
    {
        protected RequiredPaymentService Sut;
        protected Mock<IRefundService> RefundService;
        protected Mock<IPaymentDueProcessor> PaymentsDueService;
        protected List<Payment> PaymentHistory;


        [SetUp]
        public void Setup()
        {
            var automocker = AutoMock.GetStrict();
            PaymentHistory = new List<Payment>();
            PaymentsDueService = automocker.Mock<IPaymentDueProcessor>();
            RefundService = automocker.Mock<IRefundService>();
            Sut = new RequiredPaymentService(PaymentsDueService.Object, RefundService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            PaymentsDueService.Verify();
            RefundService.Verify();
        }

        [TestFixture]
        public class WhenAmountIsLessThanTotalAmountForHistory : RequiredPaymentServiceTests
        {
            [Test]
            public void RequiredPaymentHasCorrectAmount()
            {
                var testEarning = new Earning();
                var exptectedAmount = 50;
                
                PaymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, PaymentHistory)).Returns(exptectedAmount);

                var actual = Sut.GetRequiredPayments(testEarning, PaymentHistory);

                actual.Single().Amount.Should().Be(exptectedAmount);
            }

            [Test]
            public void RequiredPaymentHasSfaContributionPercentageOfInput()
            {

            }

            [Test]
            public void RequiredPaymentHasEarningTypeOfInput()
            {

            }
        }

        [TestFixture]
        public class WhenAmountIsMoreThanTotalAmountForHistory : RequiredPaymentServiceTests
        {
            [TestFixture]
            public class HistoryHasLevyOnly : WhenAmountIsMoreThanTotalAmountForHistory
            {
                [SetUp]
                public new void Setup()
                {
                    PaymentHistory.Add(new Payment {Amount = 1000, FundingSource = FundingSourceType.Levy, SfaContributionPercentage = 0.5m});
                }

                [Test]
                public void RequiredPaymentHasCorrectAmount()
                {

                }

                [Test]
                public void RequiredPaymentHasSfaContributionPercentageOfHistory()
                {

                }

                [Test]
                public void RequiredPaymentHasEarningTypeOfHistory()
                {

                }
            }

            [TestFixture]
            public class HistoryHasCoInvestedOnly : WhenAmountIsMoreThanTotalAmountForHistory
            {
                [SetUp]
                public new void Setup()
                {
                    base.Setup();
                    PaymentHistory.Add(new Payment { Amount = 900, FundingSource = FundingSourceType.CoInvestedSfa, SfaContributionPercentage = 0.9m });
                    PaymentHistory.Add(new Payment { Amount = 100, FundingSource = FundingSourceType.CoInvestedEmployer, SfaContributionPercentage = 0.9m });
                }

                [Test]
                public void RequiredPaymentHasCorrectAmount()
                {

                }

                [Test]
                public void RequiredPaymentHasSfaContributionPercentageOfHistory()
                {

                }

                [Test]
                public void RequiredPaymentHasEarningTypeOfHistory()
                {

                }
            }

            [TestFixture]
            public class HistoryHasIncentiveOnly : WhenAmountIsMoreThanTotalAmountForHistory
            {
                [SetUp]
                public new void Setup()
                {
                    base.Setup();
                    PaymentHistory.Add(new Payment { Amount = 500, FundingSource = FundingSourceType.FullyFundedSfa, SfaContributionPercentage = 0.7m });
                }

                [Test]
                public void RequiredPaymentHasCorrectAmount()
                {

                }

                [Test]
                public void RequiredPaymentHasSfaContributionPercentageOfHistory()
                {

                }

                [Test]
                public void RequiredPaymentHasEarningTypeOfHistory()
                {

                }
            }

            [TestFixture]
            public class HistoryHasMixedFundingSources : WhenAmountIsMoreThanTotalAmountForHistory
            {
                [SetUp]
                public new void Setup()
                {
                    PaymentHistory.Add(new Payment { Amount = 1000, FundingSource = FundingSourceType.Levy, SfaContributionPercentage = 0.5m });
                    PaymentHistory.Add(new Payment { Amount = 900, FundingSource = FundingSourceType.CoInvestedSfa, SfaContributionPercentage = 0.9m });
                    PaymentHistory.Add(new Payment { Amount = 100, FundingSource = FundingSourceType.CoInvestedEmployer, SfaContributionPercentage = 0.9m });
                }

                [Test]
                public void RequiredPaymentHasCorrectAmount()
                {

                }

                [Test]
                public void RequiredPaymentHasSfaContributionPercentageOfHistory()
                {

                }

                [Test]
                public void RequiredPaymentHasEarningTypeOfHistory()
                {

                }
            }
        }
    }
}
