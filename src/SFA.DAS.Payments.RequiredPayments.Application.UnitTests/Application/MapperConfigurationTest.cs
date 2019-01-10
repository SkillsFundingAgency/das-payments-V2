using AutoMapper;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

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
                CollectionPeriod = CollectionPeriod.CreateFromAcademicYearAndPeriod("1819", 11),
                DeliveryPeriod = DeliveryPeriod.CreateFromAcademicYearAndPeriod("1819", 10)
            };

            mapper.Map<PaymentHistoryEntity, Payment>(payment);
        }
    }
}
