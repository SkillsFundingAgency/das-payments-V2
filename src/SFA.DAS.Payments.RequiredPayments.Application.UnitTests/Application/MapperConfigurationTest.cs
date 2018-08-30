using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;

namespace SFA.DAS.Payments.RequiredPayments.UnitTests.Application
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
