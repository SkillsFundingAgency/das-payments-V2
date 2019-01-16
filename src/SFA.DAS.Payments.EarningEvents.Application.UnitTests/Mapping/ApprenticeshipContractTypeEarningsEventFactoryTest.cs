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
        [TestCase(null, null)]
        [TestCase("weird string", null)]
        [TestCase("ContractWithSfa", typeof(ApprenticeshipContractType2EarningEvent))]
        [TestCase("ContractWithEmployer", typeof(ApprenticeshipContractType1EarningEvent))]
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
