﻿using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.RequiredPayments
{
    public abstract class RequiredPaymentsMappingTests<TSource> : PeriodisedEventMappingTests<TSource, RequiredPaymentEventModel>
        where TSource : PeriodisedRequiredPaymentEvent
    {
        protected override void AddProfile(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<RequiredPaymentProfile>();
        }

        [Test]
        public void Maps_EarningEventId()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).EarningEventId.Should().Be(PaymentEvent.EarningEventId);
        }

        protected override void PopulateCommonProperties(TSource paymentEvent)
        {
            base.PopulateCommonProperties(paymentEvent);
            paymentEvent.EarningEventId = Guid.NewGuid();
        }
    }
}