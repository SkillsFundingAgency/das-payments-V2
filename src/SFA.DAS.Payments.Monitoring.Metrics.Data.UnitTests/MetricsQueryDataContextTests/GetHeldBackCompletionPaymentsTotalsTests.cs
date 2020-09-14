using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data.UnitTests.MetricsQueryDataContextTests
{
    [TestFixture]
    public class GetHeldBackCompletionPaymentsTotalsTests
    {
        [Test]
        public async Task WhenThereAreMultipleProvidersWithDifferentTotals_ThenTheResultTotalsAreDifferent()
        {
            using (var sut = new InMemoryMetricsQueryDataContext())
            {
                await sut.LatestSuccessfulJobs.AddAsync(new LatestSuccessfulJobModel
                {
                    AcademicYear = 2021,
                    CollectionPeriod = 1,
                    DcJobId = 1,
                    JobId = 1,
                    Ukprn = 0,
                });

                await sut.RequiredPaymentEvents.AddAsync(new RequiredPaymentEventModel
                {
                    Ukprn = 10000,
                    ContractType = ContractType.Act1,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                    Amount = 100,
                    JobId = 1,
                    NonPaymentReason = NonPaymentReason.InsufficientEmployerContribution,
                });
                await sut.RequiredPaymentEvents.AddAsync(new RequiredPaymentEventModel
                {
                    Ukprn = 10000,
                    ContractType = ContractType.Act1,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                    Amount = 50,
                    JobId = 1,
                    NonPaymentReason = NonPaymentReason.InsufficientEmployerContribution,
                });
                await sut.RequiredPaymentEvents.AddAsync(new RequiredPaymentEventModel
                {
                    Ukprn = 10000,
                    ContractType = ContractType.Act2,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                    Amount = 200,
                    JobId = 1,
                    NonPaymentReason = NonPaymentReason.InsufficientEmployerContribution,
                });
                await sut.RequiredPaymentEvents.AddAsync(new RequiredPaymentEventModel
                {
                    Ukprn = 30000,
                    ContractType = ContractType.Act1,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                    Amount = 300,
                    JobId = 1,
                    NonPaymentReason = NonPaymentReason.InsufficientEmployerContribution,
                });
                await sut.RequiredPaymentEvents.AddAsync(new RequiredPaymentEventModel
                {
                    Ukprn = 30000,
                    ContractType = ContractType.Act2,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                    Amount = 600,
                    JobId = 1,
                    NonPaymentReason = NonPaymentReason.InsufficientEmployerContribution,
                });

                await sut.RequiredPaymentEvents.AddAsync(new RequiredPaymentEventModel
                {
                    Ukprn = 50000,
                    ContractType = ContractType.Act1,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                    Amount = 100,
                    JobId = 1,
                    NonPaymentReason = NonPaymentReason.InsufficientEmployerContribution,
                });
                await sut.RequiredPaymentEvents.AddAsync(new RequiredPaymentEventModel
                {
                    Ukprn = 50000,
                    ContractType = ContractType.Act1,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                    Amount = 100,
                    JobId = 1,
                    NonPaymentReason = NonPaymentReason.InsufficientEmployerContribution,
                });
                await sut.SaveChangesAsync();


                var actual = await sut.GetHeldBackCompletionPaymentTotals(2021, 1, CancellationToken.None);

                actual.Where(x => x.Ukprn == 10000).Should().HaveCount(1);
                actual.Where(x => x.Ukprn == 30000).Should().HaveCount(1);

                var metricForProviderA = actual.First(x => x.Ukprn == 10000);
                var metricForProviderB = actual.First(x => x.Ukprn == 30000);

                metricForProviderA.ContractType1.Should().NotBe(metricForProviderB.ContractType1);
                metricForProviderA.ContractType2.Should().NotBe(metricForProviderB.ContractType2);

                metricForProviderA.ContractType1.Should().Be(150);
                metricForProviderA.ContractType2.Should().Be(200);
            }
        }
    }
}
