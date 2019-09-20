using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.DataLock
{
    public abstract class DataLockEventMappingTests<TSource> : PaymentEventMappingTests<TSource, DataLockEventModel>
        where TSource : DataLockEvent, new()
    {
        protected override void AddProfile(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<DataLockProfile>();
        }

        protected override void PopulateCommonProperties(TSource paymentEvent)
        {
            base.PopulateCommonProperties(paymentEvent);
            paymentEvent.EarningEventId = Guid.NewGuid();
            paymentEvent.PriceEpisodes = new List<PriceEpisode>(){new PriceEpisode()};
        }

        [Test]
        public void Maps_EarningEventId()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).EarningEventId.Should().Be(PaymentEvent.EarningEventId);
        }

        [Test]
        public void Maps_PriceEpisodes()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).PriceEpisodes.Count.Should().Be(1);
        }
    }
}

