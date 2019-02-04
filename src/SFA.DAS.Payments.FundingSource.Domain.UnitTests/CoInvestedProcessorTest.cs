using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Domain.Services;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class CoInvestedProcessorTest
    {
        
        [Test]
        public void Test()
        {
            var requiredPayment = new RequiredPayment();
            var employerCoInvestedPaymentProcessorMock = new Mock<IPaymentProcessor>();
            var sfaCoInvestedPaymentProcessorMock = new Mock<IPaymentProcessor>();

            var payment1 = new EmployerCoInvestedPayment();
            var payment2 = new SfaCoInvestedPayment();

            employerCoInvestedPaymentProcessorMock.Setup(p => p.Process(requiredPayment)).Returns(new []{payment1}).Verifiable();
            sfaCoInvestedPaymentProcessorMock.Setup(p => p.Process(requiredPayment)).Returns(new[] {payment2}).Verifiable();

            ICoInvestedPaymentProcessor processor = new CoInvestedPaymentProcessor(employerCoInvestedPaymentProcessorMock.Object, sfaCoInvestedPaymentProcessorMock.Object);
            var actualPayments = processor.Process(requiredPayment);
            
            actualPayments.Should().HaveCount(2);
            actualPayments[0].Should().BeSameAs(payment1);
            actualPayments[1].Should().BeSameAs(payment2);

            employerCoInvestedPaymentProcessorMock.Verify();
            sfaCoInvestedPaymentProcessorMock.Verify();
        }
    }
}
