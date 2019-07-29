using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Model;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class GenerateSortedPaymentKeysTest
    {
        private static IGenerateSortedPaymentKeys sut;
        private Mock<IDataCache<List<EmployerProviderPriorityModel>>> employerProviderPrioritiesMock;
        private Mock<IDataCache<List<string>>> refundSortKeysCacheMock;
        private Mock<IDataCache<List<TransferPaymentSortKeyModel>>> transferPaymentSortKeysCacheMock;
        private Mock<IDataCache<List<RequiredPaymentSortKeyModel>>> requiredPaymentSortKeysCacheMock;
        private AutoMock mocker;

        [SetUp]
        public void Setup()
        {
            mocker = AutoMock.GetStrict();
            employerProviderPrioritiesMock = mocker.Mock<IDataCache<List<EmployerProviderPriorityModel>>>();
            refundSortKeysCacheMock = mocker.Mock<IDataCache<List<string>>>();
            transferPaymentSortKeysCacheMock = mocker.Mock<IDataCache<List<TransferPaymentSortKeyModel>>>();
            requiredPaymentSortKeysCacheMock = mocker.Mock<IDataCache<List<RequiredPaymentSortKeyModel>>>();

            sut = mocker.Create<GenerateSortedPaymentKeys>();
        }

        [TearDown]
        public void TearDown()
        {
            employerProviderPrioritiesMock.Verify();
            refundSortKeysCacheMock.Verify();
            transferPaymentSortKeysCacheMock.Verify();
            requiredPaymentSortKeysCacheMock.Verify();
        }
        
        [Test]
        public async Task ShouldSortAllPaymentsCorrectly()
        {
            const string transferPaymentKey = "1";
            const string requiredPaymentKey = "2";
            const string refundKey = "3";

            transferPaymentSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.SenderTransferKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => 
                    new ConditionalValue<List<TransferPaymentSortKeyModel>>(true, 
                    new List<TransferPaymentSortKeyModel>
                    {
                        new TransferPaymentSortKeyModel
                        {
                            Id = transferPaymentKey,
                            Uln = 1,
                            AgreedOnDate = DateTime.Today
                        }
                    })).Verifiable();

            employerProviderPrioritiesMock
                .Setup(c => c.TryGet(CacheKeys.EmployerPaymentPriorities, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<EmployerProviderPriorityModel>>(false, null))
                .Verifiable();

            requiredPaymentSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.RequiredPaymentKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => 
                    new ConditionalValue<List<RequiredPaymentSortKeyModel>>(true, 
                        new List<RequiredPaymentSortKeyModel>
                        {
                            new RequiredPaymentSortKeyModel
                            {
                                Id = requiredPaymentKey,
                                Ukprn =  2,
                                Uln = 2,
                                AgreedOnDate = DateTime.Today
                            }
                        })).Verifiable();

            refundSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.RefundPaymentsKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => 
                    new ConditionalValue<List<string>>(true, new List<string>{ refundKey }))
                .Verifiable();

            var keys = await sut.GeyKeys();
           

            keys[0].Should().Be(refundKey);
            keys[1].Should().Be(transferPaymentKey);
            keys[2].Should().Be(requiredPaymentKey);
        }

        [Test]
        public async Task ShouldSortRequiredPaymentsCorrectly()
        {
            var priorities = new List<EmployerProviderPriorityModel>
            {
                new EmployerProviderPriorityModel
                {
                    EmployerAccountId = 1,
                    Ukprn = 1,
                    Order = 1
                },
                new EmployerProviderPriorityModel
                {
                    EmployerAccountId = 1,
                    Ukprn = 2,
                    Order = 2
                },
                new EmployerProviderPriorityModel
                {
                    EmployerAccountId = 1,
                    Ukprn = 3,
                    Order = 3
                },
                new EmployerProviderPriorityModel
                {
                    EmployerAccountId = 1,
                    Ukprn = 4,
                    Order = 4
                }
            };
            var sortKeyModels = new List<RequiredPaymentSortKeyModel>
            {
                new RequiredPaymentSortKeyModel
                {
                    Id = "1",
                    Ukprn = 4,
                    Uln = 1,
                    AgreedOnDate = DateTime.Today
                },
                new RequiredPaymentSortKeyModel
                {
                    Id = "2",
                    Ukprn = 2,
                    Uln = 2,
                    AgreedOnDate = DateTime.Today
                },
                new RequiredPaymentSortKeyModel
                {
                    Id = "3",
                    Ukprn = 3,
                    Uln = 3,
                    AgreedOnDate = DateTime.Today
                },
                new RequiredPaymentSortKeyModel
                {
                    Id = "4",
                    Ukprn = 1,
                    Uln = 4,
                    AgreedOnDate = DateTime.Today.AddDays(-1)
                },

                new RequiredPaymentSortKeyModel
                {
                    Id = "5",
                    Ukprn = 1,
                    Uln = 6,
                    AgreedOnDate = DateTime.Today
                },
                new RequiredPaymentSortKeyModel
                {
                    Id = "6",
                    Ukprn = 1,
                    Uln = 5,
                    AgreedOnDate = DateTime.Today
                }
            };

            transferPaymentSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.SenderTransferKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<TransferPaymentSortKeyModel>>(false,null))
                .Verifiable();

            refundSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.RefundPaymentsKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(false, null))
                .Verifiable();

            employerProviderPrioritiesMock
                .Setup(c => c.TryGet(CacheKeys.EmployerPaymentPriorities, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<EmployerProviderPriorityModel>>(true, priorities))
                .Verifiable();

            requiredPaymentSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.RequiredPaymentKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<RequiredPaymentSortKeyModel>>(true, sortKeyModels))
                .Verifiable();


            var keys = await sut.GeyKeys();

            keys.Should().HaveCount(6);
            keys[0].Should().Be("4");
            keys[1].Should().Be("6");
            keys[2].Should().Be("5");
            keys[3].Should().Be("2");
            keys[4].Should().Be("3");
            keys[5].Should().Be("1");
        }

        [Test]
        public async Task ShouldSortPaymentsCorrectlyForNonPrioritisedProviders()
        {
            var priorities = new List<EmployerProviderPriorityModel>
            {
                new EmployerProviderPriorityModel
                {
                    EmployerAccountId = 1,
                    Ukprn = 2,
                    Order = 1
                }
            };
            var sortKeyModels = new List<RequiredPaymentSortKeyModel>
            {
                new RequiredPaymentSortKeyModel
                {
                    Id = "2",
                    Ukprn = 2,
                    Uln = 2,
                    AgreedOnDate = DateTime.Today
                },
                new RequiredPaymentSortKeyModel
                {
                    Id = "4",
                    Ukprn = 1,
                    Uln = 4,
                    AgreedOnDate = DateTime.Today.AddDays(-1)
                },

                new RequiredPaymentSortKeyModel
                {
                    Id = "5",
                    Ukprn = 1,
                    Uln = 6,
                    AgreedOnDate = DateTime.Today
                },
                new RequiredPaymentSortKeyModel
                {
                    Id = "6",
                    Ukprn = 1,
                    Uln = 5,
                    AgreedOnDate = DateTime.Today
                }
            };

            transferPaymentSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.SenderTransferKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<TransferPaymentSortKeyModel>>(false, null))
                .Verifiable();

            refundSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.RefundPaymentsKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(false, null))
                .Verifiable();

            employerProviderPrioritiesMock
                .Setup(c => c.TryGet(CacheKeys.EmployerPaymentPriorities, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<EmployerProviderPriorityModel>>(true, priorities))
                .Verifiable();

            requiredPaymentSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.RequiredPaymentKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<RequiredPaymentSortKeyModel>>(true, sortKeyModels))
                .Verifiable();


            var keys = await sut.GeyKeys();

            keys.Should().HaveCount(4);
            keys[0].Should().Be("2");
            keys[1].Should().Be("4");
            keys[2].Should().Be("6");
            keys[3].Should().Be("5");
        }

        [Test]
        public async Task ShouldSortTransferPaymentsCorrectly()
        {
           
            var sortKeyModels = new List<TransferPaymentSortKeyModel>
            {
                new TransferPaymentSortKeyModel
                {
                    Id = "4",
                    Uln = 4,
                    AgreedOnDate = DateTime.Today.AddDays(-1)
                },

                new TransferPaymentSortKeyModel
                {
                    Id = "5",
                    Uln = 6,
                    AgreedOnDate = DateTime.Today
                },
                new TransferPaymentSortKeyModel
                {
                    Id = "6",
                    Uln = 5,
                    AgreedOnDate = DateTime.Today
                }
            };

            transferPaymentSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.SenderTransferKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<TransferPaymentSortKeyModel>>(true, sortKeyModels))
                .Verifiable();

            refundSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.RefundPaymentsKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(false, null))
                .Verifiable();

            employerProviderPrioritiesMock
                .Setup(c => c.TryGet(CacheKeys.EmployerPaymentPriorities, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<EmployerProviderPriorityModel>>(false, null))
                .Verifiable();

            requiredPaymentSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.RequiredPaymentKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<RequiredPaymentSortKeyModel>>(false, null))
                .Verifiable();

            var keys = await sut.GeyKeys();

            keys.Should().HaveCount(3);
            keys[0].Should().Be("4");
            keys[1].Should().Be("6");
            keys[2].Should().Be("5");
        }
    }
}
