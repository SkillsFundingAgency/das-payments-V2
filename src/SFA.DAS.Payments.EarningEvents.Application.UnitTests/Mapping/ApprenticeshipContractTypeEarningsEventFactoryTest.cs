using System;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests.Mapping
{
    [TestFixture]
    public class ApprenticeshipContractTypeEarningsEventFactoryTest
    {
        [Test]
        [TestCase(null, typeof(ApprenticeshipContractTypeNoneEarningEvent))]
        [TestCase("weird string", typeof(ApprenticeshipContractTypeNoneEarningEvent))]
        [TestCase("Non-Levy Contract", typeof(ApprenticeshipContractType2EarningEvent))]
        [TestCase("Contract for services with the ESFA", typeof(ApprenticeshipContractType2EarningEvent))]
        [TestCase("Levy Contract", typeof(ApprenticeshipContractType1EarningEvent))]
        [TestCase("Contract for services with the employer", typeof(ApprenticeshipContractType1EarningEvent))]
        public void TestFactoryCreatesCorrectContractTypeEvents(string contractType, Type expectedType)
        {
            // arrange
            var factory = new ApprenticeshipContractTypeEarningsEventFactory();
            ApprenticeshipContractTypeEarningsEvent earningEvent;

            // act
            try
            {
                earningEvent = factory.Create(contractType);
            }
            catch (InvalidOperationException)
            {
                if (expectedType == null)
                    return;

                Assert.Fail($"Expected object of type {expectedType} created");
                return;
            }

            // assert
            Assert.AreEqual(expectedType, earningEvent.GetType());
        }
    }
}
