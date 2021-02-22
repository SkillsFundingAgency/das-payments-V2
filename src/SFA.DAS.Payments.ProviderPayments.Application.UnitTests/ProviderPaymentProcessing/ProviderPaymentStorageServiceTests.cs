using System.Collections.Generic;
using System.Threading;
using Autofac.Extras.Moq;
using NUnit.Framework;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.ProviderPaymentProcessing
{
    [TestFixture]
    public class ProviderPaymentStorageServiceTests
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
        }
        
        [Test]
        public async Task StoresBatchOfProviderPayments()
        {
            var payments = new List<PaymentModel>();
            var service = mocker.Create<ProviderPaymentStorageService>();
            await service.StoreProviderPayments(payments, CancellationToken.None);
            mocker.Mock<IProviderPaymentsRepository>()
                .Verify(repo => repo.SavePayments(It.Is<List<PaymentModel>>(lst => lst == payments), It.IsAny<CancellationToken>()));
        }
    }
}