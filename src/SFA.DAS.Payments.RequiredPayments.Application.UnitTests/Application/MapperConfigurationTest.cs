using AutoMapper;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;
using FluentAssertions;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application
{
    [TestFixture]
    public class MapperConfigurationTest
    {
        private IMapper mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            mapper = new Mapper(config);
        }

        [Test]
        public void ValidateMap()
        {
            var payment = new PaymentHistoryEntity
            {
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 11),
                DeliveryPeriod = 10
            };

            mapper.Map<PaymentHistoryEntity, Payment>(payment);
        }

        [Test]
        public void MapperDoesNotChangeEventId()
        {
            var payment = new ApprenticeshipContractType1EarningEvent();
            var requiredPayment = new ApprenticeshipContractType1RequiredPaymentEvent();

            var expected = requiredPayment.EventId;

            mapper.Map(payment, requiredPayment);

            requiredPayment.EventId.Should().Be(expected);
        }
    }
}
