using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.PaymentsDue
{
    public abstract class PaymentsDueMappingTests<TSource> : PeriodisedEventMappingTests<TSource, PaymentsDueEventModel>
        where TSource : PaymentDueEvent
    {
        protected override void AddProfile(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<PaymentsDueProfile>();
        }


        [Test]
        public void Maps_EarningEventId()
        {
            Mapper.Map<PaymentsDueEventModel>(PaymentEvent).EarningEventId.Should().Be(PaymentEvent.EarningEventId);
        }

        protected override void PopulateCommonProperties(TSource paymentEvent)
        {
            base.PopulateCommonProperties(paymentEvent);
            paymentEvent.EarningEventId = Guid.NewGuid();
        }
    }
}