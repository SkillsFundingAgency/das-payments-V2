using System;
using System.Collections.Generic;
using System.Diagnostics;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Application.Mapping;
using SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Mapping
{
    [TestFixture]
    public class RequiredPaymentMappingTests
    {
        private IMapper mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            mapper = new Mapper(config);
        }

        [Test]
        [TestCase(typeof(CalculatedRequiredCoInvestedAmount))]
        [TestCase(typeof(CalculatedRequiredIncentiveAmount))]
        [TestCase(typeof(CalculatedRequiredLevyAmount))]
        public void AccountIdIsCorrect(Type requiredPaymentEventType)
        {
            var requiredPaymentEvent = Activator.CreateInstance(requiredPaymentEventType) as PeriodisedRequiredPaymentEvent;
            var requiredPayment = new RequiredPayment
            {
                AccountId = 1,
                TransferSenderAccountId = 2
            };

            mapper.Map(requiredPayment, requiredPaymentEvent);
            requiredPaymentEvent.AccountId.Should().Be(requiredPayment.AccountId);
        }

        [Test]
        [TestCase(typeof(CalculatedRequiredCoInvestedAmount))]
        [TestCase(typeof(CalculatedRequiredIncentiveAmount))]
        [TestCase(typeof(CalculatedRequiredLevyAmount))]
        public void TransferSenderAccountIdIsCorrect(Type requiredPaymentEventType)
        {
            var requiredPaymentEvent = Activator.CreateInstance(requiredPaymentEventType) as PeriodisedRequiredPaymentEvent;
            var expectedAmount = 100;

            var requiredPayment = new RequiredPayment
            {
                AccountId = 1,
                TransferSenderAccountId = 2
            };

            mapper.Map(requiredPayment, requiredPaymentEvent);
            requiredPaymentEvent.TransferSenderAccountId.Should().Be(requiredPayment.TransferSenderAccountId);
        }

        [Test]
        public void Maps_AgeAtStartOfLearning_From_PayableEarningEvent_To_CalculatedRequiredOnProgrammeAmount()
        {
            // Arrange
            var payableEarningEvent = new PayableEarningEvent
            {
                AgeAtStartOfLearning = 25,
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                PriceEpisodes = new List<PriceEpisode>
                {
                    new PriceEpisode
                    {
                        LearningAimSequenceNumber = 1,
                        Identifier = "1"
                    }
                }
            };

            var requiredPaymentEvent = Activator.CreateInstance(typeof(CalculatedRequiredCoInvestedAmount)) as CalculatedRequiredOnProgrammeAmount;

            // Act
            mapper.Map(payableEarningEvent, requiredPaymentEvent);

            // Assert
            requiredPaymentEvent.AgeAtStartOfLearning.Should().Be(25);
        }


    }
}