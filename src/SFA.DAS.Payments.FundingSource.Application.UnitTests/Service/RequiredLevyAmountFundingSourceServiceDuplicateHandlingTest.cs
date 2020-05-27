using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class RequiredLevyAmountFundingSourceServiceDuplicateHandlingTest
    {
        private static IMapper mapper;

        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
        }

        [Test]
        public async Task Does_Not_Add_Duplicate_Events_To_Payments_Store()
        {
            mocker.Mock<IDuplicatePeriodisedPaymentEventService>()
                .Setup(x => x.IsDuplicate(It.IsAny<IPeriodisedRequiredPaymentEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {
                Ukprn = 999,
                StartDate = DateTime.Today,
                EventId = Guid.NewGuid(),
                Priority = 1,
                Learner = new Learner
                {
                    Uln = 999
                },
                AccountId = 1,
            };

            var service = mocker.Create<RequiredLevyAmountFundingSourceService>();
            await service.AddRequiredPayment(requiredPaymentEvent).ConfigureAwait(false);
            mocker.Mock<IDataCache<CalculatedRequiredLevyAmount>>()
                .Verify(c => c.AddOrReplace(It.IsAny<string>(), It.IsAny<CalculatedRequiredLevyAmount>(), CancellationToken.None), Times.Never);
        }
    }
}