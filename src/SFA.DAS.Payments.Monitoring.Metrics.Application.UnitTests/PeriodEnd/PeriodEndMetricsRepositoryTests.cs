using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.PeriodEnd
{
    [TestFixture]
    public class PeriodEndMetricsRepositoryTests
    {
        [Test]
        public async Task WhenCallingGetYearToDatePayments_WithMultipleProviders_TheYtdPaymentsAreCorrect()
        {
            var mocker = AutoMock.GetLoose();

            var payments = new List<PaymentModel>
            {
                new PaymentModel
                {
                    Ukprn = 1,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(2021, 1),
                    ContractType = ContractType.Act1,
                    Amount = 1000,
                },
                new PaymentModel
                {
                    Ukprn = 1,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(2021, 1),
                    ContractType = ContractType.Act2,
                    Amount = 2000,
                },

                new PaymentModel
                {
                    Ukprn = 2,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(2021, 1),
                    ContractType = ContractType.Act1,
                    Amount = 10000,
                },
                new PaymentModel
                {
                    Ukprn = 2,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(2021, 1),
                    ContractType = ContractType.Act2,
                    Amount = 20000,
                },
            };



            using (var context = new InMemoryMetricsQueryDataContext())
            {
                var factory = mocker.Mock<IMetricsQueryDataContextFactory>();
                factory.Setup(x => x.Create())
                    .Returns(context);
                var sut = mocker.Create<PeriodEndMetricsRepository>();

                foreach (var payment in payments)
                {
                    await context.AddAsync(payment);
                }

                await context.SaveChangesAsync();

                var actual = await sut.GetYearToDatePayments(2021, 2, CancellationToken.None);

                actual.Where(x => x.Ukprn == 1).Should().HaveCount(1);
                actual.Where(x => x.Ukprn == 2).Should().HaveCount(1);
                actual.Where(x => x.Ukprn == 3).Should().BeEmpty();

                actual.Single(x => x.Ukprn == 1).Total.Should().Be(3000);
                actual.Single(x => x.Ukprn == 2).Total.Should().Be(30000);
            }
        }

        [Test]
        public async Task WhenCallingGetPeriodEndProviderDataLockTypeCounts_ThenMetricsQueryDataContextCalled()
        {
            short academicYear = 2021;
            byte collectionPeriod = 3;

            var persistenceContextMock = new Mock<IMetricsPersistenceDataContext>();
            var metricsQueryDataContextFactoryMock = new Mock<IMetricsQueryDataContextFactory>();
            var metricsQueryDataContextMock = new Mock<IMetricsQueryDataContext>();
            var loggerMock = new Mock<IPaymentLogger>();

            metricsQueryDataContextFactoryMock
                .Setup(x => x.Create())
                .Returns(metricsQueryDataContextMock.Object);

            var sut = new PeriodEndMetricsRepository(persistenceContextMock.Object, metricsQueryDataContextFactoryMock.Object, loggerMock.Object);

            await sut.GetPeriodEndProviderDataLockTypeCounts(academicYear, collectionPeriod, It.IsAny<CancellationToken>());

            metricsQueryDataContextMock.Verify(x => x.GetPeriodEndProviderDataLockCounts(academicYear, collectionPeriod, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

