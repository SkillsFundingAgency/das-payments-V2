using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class EmployerProviderPriorityStorageServiceTests
    {
        private Mock<IFundingSourceDataContext> dataContext;
        private EmployerChangedProviderPriority providerPriorityEvent;

        private EmployerProviderPriorityStorageService service;

        [SetUp]
        public void SetUp()
        {
            dataContext = new Mock<IFundingSourceDataContext>();

            providerPriorityEvent = new EmployerChangedProviderPriority
            {
                EmployerAccountId = 112,
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.UtcNow,
                OrderedProviders = new List<long>
                {
                    113849,
                    384950,
                    299837
                }
            };

            service = new EmployerProviderPriorityStorageService(dataContext.Object);
        }

        [Test]
        public async Task StoreEmployerProviderPriority_ShouldReplaceEmployerProviderPrioritiesCorrectly()
        {
            await service.StoreEmployerProviderPriority(providerPriorityEvent);

            dataContext.Verify(context => context.ReplaceEmployerProviderPriorities(
                providerPriorityEvent.EmployerAccountId, 
                It.Is<List<EmployerProviderPriorityModel>>(models =>
                    models.Count == 3
                    && models.Any(model =>
                        model.EmployerAccountId == providerPriorityEvent.EmployerAccountId
                        && model.Ukprn == providerPriorityEvent.OrderedProviders[0]
                        && model.Order == 1)
                    && models.Any(model =>
                        model.EmployerAccountId == providerPriorityEvent.EmployerAccountId
                        && model.Ukprn == providerPriorityEvent.OrderedProviders[1]
                        && model.Order == 2)
                    && models.Any(model =>
                        model.EmployerAccountId == providerPriorityEvent.EmployerAccountId
                        && model.Ukprn == providerPriorityEvent.OrderedProviders[2]
                        && model.Order == 3)), 
                It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task StoreEmployerProviderPriority_ShouldSaveChanges()
        {
            await service.StoreEmployerProviderPriority(providerPriorityEvent);

            dataContext.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()));
        }
    }
}
