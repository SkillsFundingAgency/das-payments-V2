using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.Payments.PaymentsDue.Application.Infrastructure.Configuration;

namespace SFA.DAS.Payments.PaymentsDue.UnitTests.Application
{
    [TestFixture]
    public class MapperConfigurationTest
    {
        [Test]
        public void ValidateMap()
        {
            var config = AutoMapperConfigurationFactory.CreateMappingConfig();
            config.AssertConfigurationIsValid();
        }
    }
}
