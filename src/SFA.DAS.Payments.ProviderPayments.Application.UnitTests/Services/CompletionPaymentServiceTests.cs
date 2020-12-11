using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Mapping;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Services
{
    [TestFixture]
    public class CompletionPaymentServiceTests
    {
        private AutoMock mocker;

        private Mock<IProviderPaymentsRepository> providerPaymentsRepository;

        private CompletionPaymentService completionPaymentService;

        private const long Ukprn = 10000;
        private const decimal Amount = 2000.00m;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();

            var payments = new List<PaymentModel>
            {
                new PaymentModel
                {
                    Ukprn = Ukprn,
                    Amount = Amount,
                    CollectionPeriod = new CollectionPeriod
                    {
                        Period = 1, AcademicYear = 1920
                    }
                },
            };

            providerPaymentsRepository = mocker.Mock<IProviderPaymentsRepository>();

            providerPaymentsRepository
                .Setup(o => o.GetMonthEndAct1CompletionPaymentsForProvider(It.IsAny<long>(),It.IsAny<CollectionPeriod>(),
                                                                It.IsAny<CancellationToken>()))
                .ReturnsAsync(payments)
                .Verifiable();

            var config = new MapperConfiguration(c => c.AddProfile(typeof(ProviderPaymentsProfile)));

            var mapper = new Mapper(config);

            var logger = mocker.Mock<IPaymentLogger>();

            completionPaymentService = new CompletionPaymentService(logger.Object, mapper, providerPaymentsRepository.Object);
        }

        [Test]
        public async Task ShouldCallRepoAndMapper()
        {
            var command = new ProcessProviderMonthEndAct1CompletionPaymentCommand
            {
                CollectionPeriod = new CollectionPeriod
                {
                    Period = 1,
                    AcademicYear = 1920
                }
            };

            var act1CompletionPaymentEvents = await completionPaymentService.GetAct1CompletionPaymentEvents(command);

            act1CompletionPaymentEvents.Count.Should().Be(1);
            act1CompletionPaymentEvents.First().Ukprn.Should().Be(Ukprn);
            act1CompletionPaymentEvents.First().AmountDue.Should().Be(Amount);
        }
    }
}