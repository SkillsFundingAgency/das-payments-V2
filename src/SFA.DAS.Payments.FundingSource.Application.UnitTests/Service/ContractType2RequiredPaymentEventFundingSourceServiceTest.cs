using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class ContractType2RequiredPaymentEventFundingSourceServiceTest
    {
        [Test]
        public void GetFundedPaymentsShouldCallAllPaymentProcessors()
        {
            // Arrange
            var message = new ApprenticeshipContractType2RequiredPaymentEvent();
            var requiredCoInvestedPayment = new RequiredCoInvestedPayment();
            var fundingSourcePayment = new FundingSourcePayment();

            var sfaPaymentProcessor = new Mock<ICoInvestedPaymentProcessor>(MockBehavior.Strict);
            var employerPaymentProcessor = new Mock<ICoInvestedPaymentProcessor>(MockBehavior.Strict);

            var sfaPaymentEvent = new SfaCoInvestedFundingSourcePaymentEvent();

            var mapper = new Mock<ICoInvestedFundingSourcePaymentEventMapper>(MockBehavior.Strict);
            mapper.Setup(o => o.MapToRequiredCoInvestedPayment(message)).Returns(requiredCoInvestedPayment);
            mapper.Setup(o => o.MapToCoInvestedPaymentEvent(message, fundingSourcePayment)).Returns(sfaPaymentEvent);

            sfaPaymentProcessor
                .Setup(o => o.Process(requiredCoInvestedPayment)).Returns(fundingSourcePayment)
                .Verifiable();

            employerPaymentProcessor
                .Setup(o => o.Process(requiredCoInvestedPayment)).Returns(fundingSourcePayment)
                .Verifiable();

            var paymentProcessors = new List<ICoInvestedPaymentProcessor>
            {
               sfaPaymentProcessor.Object,
               employerPaymentProcessor.Object
            };

            // Act
            var handler = new ContractType2RequiredPaymentEventFundingSourceService(paymentProcessors, mapper.Object);
            handler.GetFundedPayments(message);

            //Assert
            sfaPaymentProcessor.Verify();
            employerPaymentProcessor.Verify();
        }
    }
}