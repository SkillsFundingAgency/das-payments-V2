using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Collections.Generic;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests
{
    [TestFixture]
    public class ContractType2RequiredPaymentHandlerTest
    {

        [Test]
        public void HandleShouldCallAllPaymentProcessors()
        {
            // Arrange
            var message = new ApprenticeshipContractType2RequiredPaymentEvent();
            var sfaPaymentProcessor = new Mock<ICoInvestedPaymentProcessor>(MockBehavior.Strict);
            var employerPaymentProcessor = new Mock<ICoInvestedPaymentProcessor>(MockBehavior.Strict);
            var paymentProcessors = new List<ICoInvestedPaymentProcessor>
            {
               sfaPaymentProcessor.Object,
               employerPaymentProcessor.Object
            };

            sfaPaymentProcessor
                .Setup(o => o.Process(message)).Returns(new FundingSourcePaymentEvent())
                .Verifiable();

            employerPaymentProcessor
                .Setup(o => o.Process(message)).Returns(new FundingSourcePaymentEvent())
                 .Verifiable();

            // Act
            var handler = new ContractType2RequiredPaymentHandler(paymentProcessors);
            handler.GetFundedPayments(message);

            //Assert
            sfaPaymentProcessor.Verify();
            employerPaymentProcessor.Verify();
        }
    }
}