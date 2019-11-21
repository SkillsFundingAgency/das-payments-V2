using Autofac.Extras.Moq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Repositories
{
    [TestFixture]
    public class LegacyPaymentRepositoryTests
    {
        [Test]
        [TestCase(1, 8, 2019)]
        [TestCase(2, 9, 2019)]
        [TestCase(3, 10, 2019)]
        [TestCase(4, 11, 2019)]
        [TestCase(5, 12, 2019)]
        [TestCase(6, 1, 2020)]
        [TestCase(7, 2, 2020)]
        [TestCase(8, 3, 2020)]
        [TestCase(9, 4, 2020)]
        [TestCase(10, 5, 2020)]
        [TestCase(11, 6, 2020)]
        [TestCase(12, 7, 2020)]
        public void CreateTriggerWorksFor1920(byte period, int month, int year)
        {
            var mocker = AutoMock.GetLoose();

            var sut = new LegacyPaymentsRepository(mocker.Mock<IConfigurationHelper>().Object);
            var actual = sut.CreateTrigger(new CollectionPeriod {AcademicYear = 1920, Period = period});

            actual.CalendarYear.Should().Be(year);
            actual.CalendarMonth.Should().Be(month);
        }
    }
}
