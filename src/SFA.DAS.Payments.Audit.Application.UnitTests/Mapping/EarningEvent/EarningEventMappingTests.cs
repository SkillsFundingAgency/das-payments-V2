using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.EarningEvent
{
    [TestFixture]
    public abstract class EarningEventMappingTests<TSource>: PaymentEventMappingTests<TSource, EarningEventModel>
        where TSource: EarningEvents.Messages.Events.EarningEvent
    {
        private readonly  PriceEpisode priceEpisode = new PriceEpisode
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
            StartDate = DateTime.Today.AddMonths(-1),
            EffectiveTotalNegotiatedPriceStartDate = DateTime.Today.AddMonths(-2),
            EmployerContribution = 99.0005M,
            CompletionHoldBackExemptionCode = 1
        };

        protected override void AddProfile(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<EarningEventProfile>();
        }

        protected override void PopulateCommonProperties(TSource paymentEvent)
        {
            base.PopulateCommonProperties(paymentEvent);
            paymentEvent.PriceEpisodes = new List<PriceEpisode>
            {
                priceEpisode
            };
        }


        [Test]
        public void Maps_PriceEpisodes()
        {
            var model = Mapper.Map<EarningEventModel>(PaymentEvent);
            model.PriceEpisodes.Count.Should().Be(PaymentEvent.PriceEpisodes.Count());
            model.PriceEpisodes.First().PriceEpisodeIdentifier.Should().Be(priceEpisode.Identifier);
            model.PriceEpisodes.First().TotalNegotiatedPrice1.Should().Be(priceEpisode.TotalNegotiatedPrice1);
            model.PriceEpisodes.First().TotalNegotiatedPrice2.Should().Be(priceEpisode.TotalNegotiatedPrice2);
            model.PriceEpisodes.First().TotalNegotiatedPrice3.Should().Be(priceEpisode.TotalNegotiatedPrice3);
            model.PriceEpisodes.First().TotalNegotiatedPrice4.Should().Be(priceEpisode.TotalNegotiatedPrice4);
            model.PriceEpisodes.First().StartDate.Should().Be(priceEpisode.StartDate);
            model.PriceEpisodes.First().EffectiveTotalNegotiatedPriceStartDate.Should().Be(priceEpisode.EffectiveTotalNegotiatedPriceStartDate);
            model.PriceEpisodes.First().InstalmentAmount.Should().Be(priceEpisode.InstalmentAmount);
            model.PriceEpisodes.First().NumberOfInstalments.Should().Be(priceEpisode.NumberOfInstalments);
            model.PriceEpisodes.First().CompletionAmount.Should().Be(priceEpisode.CompletionAmount);
            model.PriceEpisodes.First().Completed.Should().Be(priceEpisode.Completed);
            model.PriceEpisodes.First().PlannedEndDate.Should().Be(priceEpisode.PlannedEndDate);
            model.PriceEpisodes.First().ActualEndDate.Should().Be(priceEpisode.ActualEndDate);
            model.PriceEpisodes.First().CompletionHoldBackExemptionCode.Should().Be(priceEpisode.CompletionHoldBackExemptionCode);
            model.PriceEpisodes.First().EmployerContribution.Should().Be(priceEpisode.EmployerContribution);
        }
    }
}