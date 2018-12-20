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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<RequiredPaymentsProfile>();
            });
            Mapper.AssertConfigurationIsValid();

            //Mapper.Initialize(cfg => AutoMapperConfigurationFactory.CreateMappingConfig());
            //Mapper.AssertConfigurationIsValid();
        }

        [Test]
        public void ValidateMap()
        {
            var payment = new PaymentHistoryEntity
            {
                CollectionPeriod = "1819-R11",
                DeliveryPeriod = "1819-R10"
            };
            Mapper.Instance.Map<PaymentHistoryEntity, Payment>(payment);
        }
    }
}
