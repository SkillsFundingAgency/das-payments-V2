using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.RequiredPayments.Application.Mapping;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Mapping
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

        [Test, AutoData]
        public void MapsPaymentHistoryEntity_ApprenticeshipId_ToPayment(PaymentHistoryEntity testPaymentHistoryEntity, Payment payment)
        {
            mapper.Map(testPaymentHistoryEntity, payment);

            payment.ApprenticeshipId.Should().Be(testPaymentHistoryEntity.ApprenticeshipId);
        }

        [Test, AutoData]
        public void MapsPaymentHistoryEntity_ApprenticeshipPriceEpisodeId_ToPayment(PaymentHistoryEntity testPaymentHistoryEntity, Payment payment)
        {
            mapper.Map(testPaymentHistoryEntity, payment);

            payment.ApprenticeshipPriceEpisodeId.Should().Be(testPaymentHistoryEntity.ApprenticeshipPriceEpisodeId);
        }

        [Test, AutoData]
        public void MapsPaymentHistoryEntity_ApprenticeshipEmployerType_ToPayment(PaymentHistoryEntity testPaymentHistoryEntity, Payment payment)
        {
            mapper.Map(testPaymentHistoryEntity, payment);

            payment.ApprenticeshipEmployerType.Should().Be(testPaymentHistoryEntity.ApprenticeshipEmployerType);
        }

        [Test]
        public void MapperDoesNotChangeEventId()
        {
            var payment = new ApprenticeshipContractType1EarningEvent();
            var requiredPayment = new CalculatedRequiredLevyAmount();

            var expected = requiredPayment.EventId;

            mapper.Map(payment, requiredPayment);

            requiredPayment.EventId.Should().Be(expected);
        }
    }
}
