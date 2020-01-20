using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class RefundRemovedLearningAimServiceTests
    {
        private List<Payment> history;
        private RefundRemovedLearningAimService sut;

        [SetUp]
        public void SetUp()
        {
            sut = new RefundRemovedLearningAimService();
            history = new List<Payment>();
        }

        [Test]
        [TestCase(FundingSourceType.CoInvestedEmployer)]
        [TestCase(FundingSourceType.CoInvestedSfa)]
        [TestCase(FundingSourceType.FullyFundedSfa)]
        [TestCase(FundingSourceType.Levy)]
        [TestCase(FundingSourceType.Transfer)]
        public void Refunds_All_Delivery_Periods(FundingSourceType testFundingSource)
        {
            history.AddRange(Enumerable.Range(1, 12)
                .Select(period => new Payment
                {
                    DeliveryPeriod = (byte)period, 
                    SfaContributionPercentage = .9M, 
                    Amount = period * 10, 
                    PriceEpisodeIdentifier = "pe-1",
                    FundingSource = testFundingSource,
                }));
            var requiredPayments = sut.RefundLearningAim(history);
            
            requiredPayments.Should().HaveCount(12);
            
            for (var i = 1; i <= 12; i++)
            {
                var period = i;
                requiredPayments.Should()
                    .ContainSingle(x => x.deliveryPeriod == period &&
                                        x.payment.Amount == -period * 10);
            }
        }
    }
}