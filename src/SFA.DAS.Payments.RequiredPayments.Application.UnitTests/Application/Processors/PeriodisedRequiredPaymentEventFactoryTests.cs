using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class PeriodisedRequiredPaymentEventFactoryTests
    {
        private IPeriodisedRequiredPaymentEventFactory factory;
        private Mock<IPaymentLogger> logger;

        [SetUp]
        public void Setup()
        {
            logger = new Mock<IPaymentLogger>();
            logger.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<object[]>(),
                It.IsAny<long>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Verifiable();
            factory = new PeriodisedRequiredPaymentEventFactory(logger.Object);
        }
        [Test]
        [TestCaseSource(nameof(GetValidRequiredPaymentEventConfiguration))]
        public void CoInvestedCreatedCorrectly((EarningType earningType, int transactionType, Type eventType) config)
        {
            var result = factory.Create(config.earningType, config.transactionType);

            Assert.IsInstanceOf(config.eventType, result);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidRequiredPaymentEventConfiguration))]
        public void IncentiveTransactionForCoInvestedReturnsNull((EarningType earningType, int transactionType) config)
        {
            factory.Create(config.earningType, config.transactionType).Should().BeNull();

            logger.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<object[]>(),
                It.IsAny<long>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        private static IEnumerable<(EarningType earningType, int transactionType, Type eventType)> GetValidRequiredPaymentEventConfiguration()
        {
            var coInvestedType = typeof(CalculatedRequiredCoInvestedAmount);
            yield return (EarningType.CoInvested, (int) TransactionType.Learning, coInvestedType);
            yield return (EarningType.CoInvested, (int) TransactionType.Balancing, coInvestedType);
            yield return (EarningType.CoInvested, (int) TransactionType.Completion, coInvestedType);

            var incentiveType = typeof(CalculatedRequiredIncentiveAmount);
            yield return (EarningType.Incentive, (int) TransactionType.First16To18EmployerIncentive, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.First16To18ProviderIncentive, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.Second16To18EmployerIncentive, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.Second16To18ProviderIncentive, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.OnProgramme16To18FrameworkUplift, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.Completion16To18FrameworkUplift, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.Balancing16To18FrameworkUplift, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.FirstDisadvantagePayment, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.SecondDisadvantagePayment, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.OnProgrammeMathsAndEnglish, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.BalancingMathsAndEnglish, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.LearningSupport, incentiveType);
            yield return (EarningType.Incentive, (int) TransactionType.CareLeaverApprenticePayment, incentiveType);

            var levyType = typeof(CalculatedRequiredLevyAmount);
            yield return (EarningType.Levy, (int)TransactionType.Learning, levyType);
            yield return (EarningType.Levy, (int)TransactionType.Balancing, levyType);
            yield return (EarningType.Levy, (int)TransactionType.Completion, levyType);

        }

        private static IEnumerable<(EarningType earningType, int transactionType)> GetInvalidRequiredPaymentEventConfiguration()
        {
            yield return (EarningType.CoInvested, (int)TransactionType.First16To18EmployerIncentive);
            yield return (EarningType.CoInvested, (int)TransactionType.First16To18ProviderIncentive);
            yield return (EarningType.CoInvested, (int)TransactionType.Second16To18EmployerIncentive);
            yield return (EarningType.CoInvested, (int) TransactionType.Second16To18ProviderIncentive);
            yield return (EarningType.CoInvested, (int) TransactionType.OnProgramme16To18FrameworkUplift);
            yield return (EarningType.CoInvested, (int) TransactionType.Completion16To18FrameworkUplift);
            yield return (EarningType.CoInvested, (int) TransactionType.Balancing16To18FrameworkUplift);
            yield return (EarningType.CoInvested, (int) TransactionType.FirstDisadvantagePayment);
            yield return (EarningType.CoInvested, (int) TransactionType.SecondDisadvantagePayment);
            yield return (EarningType.CoInvested, (int) TransactionType.OnProgrammeMathsAndEnglish);
            yield return (EarningType.CoInvested, (int) TransactionType.BalancingMathsAndEnglish);
            yield return (EarningType.CoInvested, (int) TransactionType.LearningSupport);
            yield return (EarningType.CoInvested, (int) TransactionType.CareLeaverApprenticePayment);

            yield return (EarningType.Incentive, (int)TransactionType.Learning);
            yield return (EarningType.Incentive, (int)TransactionType.Balancing);
            yield return (EarningType.Incentive, (int)TransactionType.Completion);

            yield return (EarningType.Levy, (int)TransactionType.First16To18EmployerIncentive);
            yield return (EarningType.Levy, (int)TransactionType.First16To18ProviderIncentive);
            yield return (EarningType.Levy, (int)TransactionType.Second16To18EmployerIncentive);
            yield return (EarningType.Levy, (int)TransactionType.Second16To18ProviderIncentive);
            yield return (EarningType.Levy, (int)TransactionType.OnProgramme16To18FrameworkUplift);
            yield return (EarningType.Levy, (int)TransactionType.Completion16To18FrameworkUplift);
            yield return (EarningType.Levy, (int)TransactionType.Balancing16To18FrameworkUplift);
            yield return (EarningType.Levy, (int)TransactionType.FirstDisadvantagePayment);
            yield return (EarningType.Levy, (int)TransactionType.SecondDisadvantagePayment);
            yield return (EarningType.Levy, (int)TransactionType.OnProgrammeMathsAndEnglish);
            yield return (EarningType.Levy, (int)TransactionType.BalancingMathsAndEnglish);
            yield return (EarningType.Levy, (int)TransactionType.LearningSupport);
            yield return (EarningType.Levy, (int)TransactionType.CareLeaverApprenticePayment);


        }

    }
}