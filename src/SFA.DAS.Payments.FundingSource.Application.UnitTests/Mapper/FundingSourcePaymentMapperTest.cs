using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper
{
    [TestFixture]
    public class FundingSourcePaymentMapperTest
    {
        private IMapper autoMapper;
        private MapperConfiguration mapperConfiguration;

        [SetUp]
        public void Setup()
        {
            mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            autoMapper = mapperConfiguration.CreateMapper();
        }

        [TestCaseSource("GetFundingSourcePayments")]
        public void MapperMapsStandardCode(FundingSourcePayment payment)
        {
            var result = autoMapper.Map<FundingSourcePaymentEvent>(payment);
            result.StandardCode.Should().Be(payment.StandardCode);
        }

        private static IEnumerable<FundingSourcePayment> GetFundingSourcePayments()
        {
            yield return new EmployerCoInvestedPayment{StandardCode = 123};
            yield return new LevyPayment { StandardCode = 123 };
            yield return new SfaCoInvestedPayment { StandardCode = 123 };
        }
    }
}