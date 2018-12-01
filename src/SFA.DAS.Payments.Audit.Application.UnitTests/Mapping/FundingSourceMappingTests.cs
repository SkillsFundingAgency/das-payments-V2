using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping;
using SFA.DAS.Payments.Audit.Application.Mapping.FundingSource;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping
{
    public abstract class FundingSourceMappingTests
    {
        protected FundingSourcePaymentEvent PaymentEvent { get; private set; }

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AuditProfile>();
                AddProfile(cfg);
            });
            Mapper.AssertConfigurationIsValid();
        }

        protected abstract void AddProfile(IMapperConfigurationExpression cfg);
        protected abstract FundingSourcePaymentEvent CreateEvent();

        [SetUp]
        public void SetUp()
        {
            PaymentEvent = CreateEvent();
        }

        [Test]
        public void Maps_EventId()
        {
            var model = Mapper.Map<FundingSourceEventModel>(PaymentEvent);
            model.EventId.Should().Be(PaymentEvent.EventId);
        }
    }

    [TestFixture]
    public class SfaCoInvestedFundingSourcePaymentEventTests : FundingSourceMappingTests
    {
        protected override void AddProfile(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<FundingSourceProfile>();
        }

        protected override FundingSourcePaymentEvent CreateEvent()
        {
            return new SfaCoInvestedFundingSourcePaymentEvent
            {
                
            };
        }

        [Test]
        public void Test() { }
    }
}