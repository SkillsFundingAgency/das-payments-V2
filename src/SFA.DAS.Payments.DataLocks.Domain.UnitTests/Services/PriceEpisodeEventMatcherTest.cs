using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using System.Collections.Generic;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    public class PriceEpisodeEventMatcherTest
    {
        [Theory, AutoData]
        public void When_price_episode_cannot_be_found_then_return_New(
            PriceEpisode priceEpisode,
            PriceEpisodeStatusCalculator sut)
        {
            var currentPriceEpisodes = new[]
            {
                new CurrentPriceEpisode
                {
                    PriceEpisodeIdentifier = "something else",
                    AgreedPrice = priceEpisode.AgreedPrice,
                }
            };

            var r = sut.Match(currentPriceEpisodes, priceEpisode);

            r.Should().Be(PriceEpisodeStatus.New);
        }

        [Theory, AutoData]
        public void When_price_episode_is_stored_and_has_not_changed_then_return_New(
            PriceEpisode priceEpisode,
            PriceEpisodeStatusCalculator sut)
        {
            var currentPriceEpisodes = new[]
            {
                new CurrentPriceEpisode
                {
                    PriceEpisodeIdentifier = priceEpisode.Identifier,
                    AgreedPrice = priceEpisode.AgreedPrice,
                }
            };

            var r = sut.Match(currentPriceEpisodes, priceEpisode);

            r.Should().Be(PriceEpisodeStatus.New);
        }

        [Theory, AutoData]
        public void When_price_episode_is_stored_and_changes_case_then_return_New(
            PriceEpisode priceEpisode,
            PriceEpisodeStatusCalculator sut)
        {
            priceEpisode.Identifier = priceEpisode.Identifier.ToLower();
            
            var currentPriceEpisodes = new[]
            {
                new CurrentPriceEpisode
                {
                    PriceEpisodeIdentifier = priceEpisode.Identifier.ToUpper(),
                    AgreedPrice = priceEpisode.AgreedPrice,
                }
            };

            var r = sut.Match(currentPriceEpisodes, priceEpisode);

            r.Should().Be(PriceEpisodeStatus.New);
        }

        [Theory, AutoData]
        public void When_price_episode_price_changes_then_return_Updated(
            PriceEpisode priceEpisode,
            PriceEpisodeStatusCalculator sut)
        {
            var currentPriceEpisodes = new[]
            {
                new CurrentPriceEpisode
                {
                    PriceEpisodeIdentifier = priceEpisode.Identifier,
                    AgreedPrice = 999,
                }
            };

            var r = sut.Match(currentPriceEpisodes, priceEpisode);

            r.Should().Be(PriceEpisodeStatus.Updated);
        }

        [Theory, AutoData]
        public void Match_unchanged_updated_missing_and_removed_price_episodes(
            List<PriceEpisode> priceEpisodes,
            string leftOverPriceEpisodeId,
            PriceEpisodeStatusCalculator sut)
        {
            var currentPriceEpisodes = new[]
            {
                new CurrentPriceEpisode //unchanged
                {
                    PriceEpisodeIdentifier = priceEpisodes[1].Identifier,
                    AgreedPrice = priceEpisodes[1].AgreedPrice,
                },
                new CurrentPriceEpisode //updated
                {
                    PriceEpisodeIdentifier = priceEpisodes[2].Identifier,
                    AgreedPrice = priceEpisodes[2].AgreedPrice + 1,
                },
                new CurrentPriceEpisode //removed
                {
                    PriceEpisodeIdentifier = leftOverPriceEpisodeId,
                },
            };

            var r = sut.Calculate(currentPriceEpisodes, priceEpisodes);

            r.Should().BeEquivalentTo(
                (priceEpisodes[0].Identifier, PriceEpisodeStatus.New),
                (priceEpisodes[1].Identifier, PriceEpisodeStatus.New),
                (priceEpisodes[2].Identifier, PriceEpisodeStatus.Updated),
                (leftOverPriceEpisodeId, PriceEpisodeStatus.Removed)
                );
        }
    }
}
