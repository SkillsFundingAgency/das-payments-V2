using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    public class PriceEpisodesReceivedServiceTest
    {
        [Theory, AutoDomainData]
        public async Task When_job_succeeded_builds_approval_event_for_removed_price_episode(
            ICurrentPriceEpisodeForJobStore context, 
            PriceEpisodesReceivedService sut, 
            CurrentPriceEpisode priceEpisode)
        {
            await context.Add(priceEpisode);

            var changeMessages = await sut.JobSucceeded(priceEpisode.JobId, priceEpisode.Ukprn);

            changeMessages.Should().BeEquivalentTo(
                new
                {
                    PriceEpisodeStatusChanges = new[]
                    {
                        new 
                        {
                            DataLock = new 
                            { 
                                priceEpisode.PriceEpisodeIdentifier,
                                Status = PriceEpisodeStatus.Removed,
                            },
                        }
                    }.ToList(),
                });
        }

        [Theory, AutoDomainData]
        public async Task When_job_succeeded_builds_approval_event_for_new_price_episode(
            IReceivedDataLockEventStore context, 
            PriceEpisodesReceivedService sut, 
            PayableEarningEvent earning)
        {
            await context.Add(new ReceivedDataLockEvent
            {
                JobId = earning.JobId,
                Ukprn = earning.Ukprn,
                MessageType = earning.GetType().ToString(),
                Message = JsonConvert.SerializeObject(earning),
            });

            var changeMessages = await sut.JobSucceeded(earning.JobId, earning.Ukprn);

            changeMessages.PriceEpisodeStatusChanges.Should().ContainEquivalentOf(
                new 
                {
                    DataLock = new 
                    { 
                        PriceEpisodeIdentifier = earning.PriceEpisodes.First().Identifier,
                        Status = PriceEpisodeStatus.New,
                    },
                });
        }

        [Theory, AutoDomainData]
        public async Task When_job_succeeded_builds_approval_event_for_new_and_updated_price_episodes(
            ICurrentPriceEpisodeForJobStore currentContext,
            IReceivedDataLockEventStore receivedContext,
            PriceEpisodesReceivedService sut,
            PayableEarningEvent earning)
        {
            await currentContext.Add(new CurrentPriceEpisode
            { 
                JobId = earning.JobId,
                Ukprn = earning.Ukprn,
                Uln = earning.Learner.Uln,
                PriceEpisodeIdentifier = earning.PriceEpisodes.First().Identifier,
                AgreedPrice = earning.PriceEpisodes.First().AgreedPrice + 1,
            });

            await receivedContext.Add(new ReceivedDataLockEvent
            {
                JobId = earning.JobId,
                Ukprn = earning.Ukprn,
                MessageType = earning.GetType().ToString(),
                Message = JsonConvert.SerializeObject(earning),
            });

            var changeMessages = await sut.JobSucceeded(earning.JobId, earning.Ukprn);

            changeMessages.PriceEpisodeStatusChanges.Should().ContainEquivalentOf(
                new
                {
                    DataLock = new 
                    {
                        PriceEpisodeIdentifier = earning.PriceEpisodes[0].Identifier,
                        Status = PriceEpisodeStatus.Updated,
                    },
                });
            changeMessages.PriceEpisodeStatusChanges.Should().ContainEquivalentOf(
                new
                {
                    DataLock = new 
                    {
                        PriceEpisodeIdentifier = earning.PriceEpisodes[1].Identifier,
                        Status = PriceEpisodeStatus.New,
                    },
                });
        }        
        
        [Theory, AutoDomainData]
        
        public async Task When_job_succeeded_builds_approval_event_for_new_and_removed_price_episodes(
            ICurrentPriceEpisodeForJobStore currentContext,
            IReceivedDataLockEventStore receivedContext,
            PriceEpisodesReceivedService sut,
            PayableEarningEvent earning,
            CurrentPriceEpisode removed)
        {
            await currentContext.Add(removed);

            await receivedContext.Add(new ReceivedDataLockEvent
            {
                JobId = earning.JobId,
                Ukprn = earning.Ukprn,
                MessageType = earning.GetType().ToString(),
                Message = JsonConvert.SerializeObject(earning),
            });

            var changeMessages = await sut.JobSucceeded(earning.JobId, earning.Ukprn);

            changeMessages.PriceEpisodeStatusChanges.Should().ContainEquivalentOf(
                new
                {
                    DataLock = new 
                    { 
                        PriceEpisodeIdentifier = earning.PriceEpisodes[0].Identifier,
                        Status = PriceEpisodeStatus.New,
                    },
                });
            changeMessages.PriceEpisodeStatusChanges.Should().ContainEquivalentOf(
                new
                {
                    DataLock = new
                    {
                        removed.PriceEpisodeIdentifier,
                        Status = PriceEpisodeStatus.Removed,
                    },
                });
        }

        [Theory, AutoDomainData]
        public async Task When_job_succeeded_removes_received_datalock_events(
            IReceivedDataLockEventStore receivedContext,
            PriceEpisodesReceivedService sut,
            PayableEarningEvent earning)
        {
            await receivedContext.Add(new ReceivedDataLockEvent
            {
                JobId = earning.JobId,
                Ukprn = earning.Ukprn,
                MessageType = earning.GetType().ToString(),
                Message = JsonConvert.SerializeObject(earning),
            });

            await sut.JobSucceeded(earning.JobId, earning.Ukprn);

            (await receivedContext.GetDataLocks(earning.JobId, earning.Ukprn))
                .Should().BeEmpty();
        }

        [Theory, AutoDomainData]
        public async Task When_job_succeeded_replaces_current_price_episodes(
            ICurrentPriceEpisodeForJobStore currentContext,
            IReceivedDataLockEventStore receivedContext,
            PriceEpisodesReceivedService sut,
            PayableEarningEvent earning)
        {
            await currentContext.Add(new CurrentPriceEpisode
            {
                JobId = earning.JobId,
                Ukprn = earning.Ukprn,
                Uln = earning.Learner.Uln,
                PriceEpisodeIdentifier = earning.PriceEpisodes.First().Identifier,
                AgreedPrice = earning.PriceEpisodes.First().AgreedPrice + 1,
            });

            await receivedContext.Add(new ReceivedDataLockEvent
            {
                JobId = earning.JobId,
                Ukprn = earning.Ukprn,
                MessageType = earning.GetType().ToString(),
                Message = JsonConvert.SerializeObject(earning),
            });

            var changeMessages = await sut.JobSucceeded(earning.JobId, earning.Ukprn);

            var expected = earning.PriceEpisodes.Select(x => new CurrentPriceEpisode
            {
                JobId = earning.JobId,
                Ukprn = earning.Ukprn,
                Uln = earning.Learner.Uln,
                PriceEpisodeIdentifier = x.Identifier,
                AgreedPrice = x.AgreedPrice,
            });

            (await currentContext.GetCurentPriceEpisodes(earning.JobId, earning.Ukprn))
                .Should().BeEquivalentTo(expected);
        }


        public class AutoDomainDataAttribute : AutoDataAttribute
        {
            public AutoDomainDataAttribute()
                : base(() => Customise())
            {
            }

            private static IFixture Customise()
            {
                var dbName = Guid.NewGuid().ToString();

                var fixture = new Fixture();
                fixture.Register((Func<ICurrentPriceEpisodeForJobStore>)(() =>
                    new CurrentPriceEpisodeContext(
                        new DbContextOptionsBuilder<CurrentPriceEpisodeContext>()
                            .UseInMemoryDatabase(databaseName: dbName)
                            .Options)));
                fixture.Register((Func<IReceivedDataLockEventStore>)(() =>
                    new ReceivedDataLockEventContext(
                        new DbContextOptionsBuilder<ReceivedDataLockEventContext>()
                            .UseInMemoryDatabase(databaseName: dbName)
                            .Options)));
                //fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
                return fixture;
            }
        }
    }
}
