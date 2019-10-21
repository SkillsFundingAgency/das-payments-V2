using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.EarningEvent
{
    public abstract class EarningEventMappingTests<TSource>: PaymentEventMappingTests<TSource, EarningEventModel>
        where TSource: EarningEvents.Messages.Events.EarningEvent
    {
        protected override void AddProfile(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<EarningEventProfile>();
        }

        protected override void PopulateCommonProperties(TSource paymentEvent)
        {
            base.PopulateCommonProperties(paymentEvent);
            paymentEvent.PriceEpisodes = new List<PriceEpisode>
            {
                new PriceEpisode
                {
                    Identifier = "pe-1",
                    TotalNegotiatedPrice1 = 10,
                    TotalNegotiatedPrice2 = 11,
                    TotalNegotiatedPrice3 = 12,
                    TotalNegotiatedPrice4 = 13,
                    CompletionAmount = 100,
                    InstalmentAmount = 10,
                    Completed = true,
                    NumberOfInstalments = 10,
                    PlannedEndDate = DateTime.Today,
                    EffectiveTotalNegotiatedPriceStartDate = DateTime.Today.AddMonths(-1)
                }
            };
        }
        
        [Test]
        public void Maps_PriceEpisodes()
        {
            var model = Mapper.Map<EarningEventModel>(PaymentEvent);
            model.PriceEpisodes.Count.Should().Be(PaymentEvent.PriceEpisodes.Count());
        }

        [Test]
        public void Maps_AimSeqNumber()
        {
            PaymentEvent.LearningAim.SequenceNumber = 101;
            var model = Mapper.Map<EarningEventModel>(PaymentEvent);
            model.LearningAimSequenceNumber.Should().Be(101);
        }
    }
}