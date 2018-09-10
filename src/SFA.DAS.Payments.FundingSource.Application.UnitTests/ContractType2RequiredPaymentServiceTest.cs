using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Collections.Generic;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests
{
    [TestFixture]
    public class ContractType2RequiredPaymentServiceTest
    {
        [Test]
        public void GetFundedPaymentsShouldCallAllPaymentProcessors()
        {
            // Arrange
            var message = new ApprenticeshipContractType2RequiredPaymentEvent();
            var requiredCoInvestedPayment = new RequiredCoInvestedPayment();
            var fundingSourcePayment = new CoInvestedPayment();

            var sfaPaymentProcessor = new Mock<ICoInvestedPaymentProcessor>(MockBehavior.Strict);
            var employerPaymentProcessor = new Mock<ICoInvestedPaymentProcessor>(MockBehavior.Strict);

            var mapper = new Mock<ICoInvestedFundingSourcePaymentEventMapper>(MockBehavior.Strict);
            mapper.Setup(o => o.MapToRequiredCoInvestedPayment(message)).Returns(requiredCoInvestedPayment);
            mapper.Setup(o => o.MapToCoInvestedPaymentEvent(message, fundingSourcePayment));

            var paymentProcessors = new List<ICoInvestedPaymentProcessor>
            {
               sfaPaymentProcessor.Object,
               employerPaymentProcessor.Object
            };

            sfaPaymentProcessor
                .Setup(o => o.Process(requiredCoInvestedPayment)).Returns(fundingSourcePayment)
                .Verifiable();

            employerPaymentProcessor
                .Setup(o => o.Process(requiredCoInvestedPayment)).Returns(fundingSourcePayment)
                 .Verifiable();

            // Act
            var handler = new ContractType2RequiredPaymentService(paymentProcessors, mapper.Object);
            handler.GetFundedPayments(message);

            //Assert
            sfaPaymentProcessor.Verify();
            employerPaymentProcessor.Verify();
        }
    }
}