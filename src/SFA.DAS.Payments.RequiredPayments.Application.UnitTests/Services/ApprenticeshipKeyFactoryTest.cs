using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.RequiredPayments.Application.Services;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipKeyFactoryTest
    {
        private IApprenticeshipKeyFactory factory;
        private Mock<IApprenticeshipKeyService> apprenticeshipKeyServiceMock;

        [SetUp]
        public void SetUp()
        {
            apprenticeshipKeyServiceMock = new Mock<IApprenticeshipKeyService>(MockBehavior.Strict);
            factory = new ApprenticeshipKeyFactory(apprenticeshipKeyServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            apprenticeshipKeyServiceMock.Verify();
        }

        [Test]
        public void TestExceptionIsThrownWhenNotInitialised()
        {
            try
            {
                factory.GetCurrentKey();
            }
            catch (ApplicationException ex)
            {
                ex.Message.Should().Contain("Apprenticeship Key is not set");
                return;
            }

            Assert.Fail("No exception thrown");
        }

        [Test]
        public void TestKeyIsReturnedIfInitialised()
        {
            // arrange
            var key = new ApprenticeshipKey();
            apprenticeshipKeyServiceMock.Setup(s => s.ParseApprenticeshipKey("1")).Returns(key).Verifiable();
            ((ApprenticeshipKeyFactory)factory).SetCurrentKey("1");

            // act
            var result = factory.GetCurrentKey();

            // assert
            result.Should().BeSameAs(key);
        }
    }
}
