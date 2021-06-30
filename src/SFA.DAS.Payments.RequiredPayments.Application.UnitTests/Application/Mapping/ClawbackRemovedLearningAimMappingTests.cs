using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using AutoMapper;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Application.Mapping;
using SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Mapping
{
    [TestFixture]
    internal class ClawbackRemovedLearningAimMappingTests
    {

        private IMapper sut;
        private PaymentModel payment;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var fixture = new Fixture();

            var config = new MapperConfiguration(x => x.AddProfile<RequiredPaymentsProfile>());
            
            config.AssertConfigurationIsValid();

            payment = fixture.Create<PaymentModel>();
            sut = new Mapper(config);
        }

        [Test]
        public void ThenMaps_PaymentModel_To_CalculatedRequiredIncentiveAmount()
        {
            //Act
            var result = sut.Map<CalculatedRequiredIncentiveAmount>(payment);
            
            //Assert
            result.AssertPeriodisedRequiredPaymentEventWasCorrectlyMappedFrom(payment);
        }

        [Test]
        public void ThenMaps_PaymentModel_To_CalculatedRequiredLevyAmount()
        {
            //Act
            var result = sut.Map<CalculatedRequiredLevyAmount>(payment);
            
            //Assert
            result.AssertCalculatedRequiredLevyAmountWasCorrectlyMappedFrom(payment);
        }

        [Test]
        public void ThenMaps_PaymentModel_To_CalculatedRequiredCoInvestedAmount()
        {
            //Act
            var result = sut.Map<CalculatedRequiredCoInvestedAmount>(payment);
            
            //Assert
            result.AssertCalculatedRequiredCoInvestedAmountWasCorrectlyMappedFrom(payment);
        }
    }
}
