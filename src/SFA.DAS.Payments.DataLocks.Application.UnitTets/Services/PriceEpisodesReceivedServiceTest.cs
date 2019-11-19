using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    public class PriceEpisodesReceivedServiceTest
    {
        //[Theory, AutoDomainData]
        //public void When_job_succeeded_calculate_status_for_all_events_in_that_job(
        //    ICurrentPriceEpisodeForJobStore context,
        //    [Frozen] Mock<ISendSomeEvents> sending, 
        //    CurrentPriceEpisode priceEpisode,
        //    PriceEpisodesReceivedService sut)
        //{
        //    context.Add(priceEpisode);

        //    sut.JobSucceeded(priceEpisode.JobId, priceEpisode.Ukprn);

        //    var c = new[] { (priceEpisode.PriceEpisodeIdentifier, PriceEpisodeStatus.New) }.ToList();
        //    sending.Verify(x => x.Send(c));
        //}

        [Theory, AutoDomainData]
        public void When_job_succeeded_builds_approval_event_for_removed_price_episode(
            ICurrentPriceEpisodeForJobStore context, 
            PriceEpisodesReceivedService sut, 
            CurrentPriceEpisode priceEpisode)
        {
            context.Add(priceEpisode);

            var changeMessages = sut.JobSucceeded(priceEpisode.JobId, priceEpisode.Ukprn);

            changeMessages.Should().BeEquivalentTo(
                new
                {
                    PriceEpisodeStatusChanges = new[]
                    {
                        new 
                        {
                            PriceEpisode = new {Identifier = priceEpisode.PriceEpisodeIdentifier },
                            Status = PriceEpisodeStatus.Removed,
                        }
                    }.ToList(),
                });
        }

        [Theory, AutoDomainData]
        public void When_job_succeeded_builds_approval_event_for_new_price_episode(
            IReceivedDataLockEventStore context, 
            PriceEpisodesReceivedService sut, 
            PayableEarningEvent earning)
        {
            context.Add(new ReceivedDataLockEvent
            {
                JobId = earning.JobId,
                Ukprn = earning.Ukprn,
                MessageType = earning.GetType().ToString(),
                Message = JsonConvert.SerializeObject(earning),
            });

            var changeMessages = sut.JobSucceeded(earning.JobId, earning.Ukprn);

            changeMessages.PriceEpisodeStatusChanges.Should().ContainEquivalentOf(
                new 
                {
                    PriceEpisode = new { earning.PriceEpisodes.First().Identifier },
                    Status = PriceEpisodeStatus.New,
                });
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

    public class CurrentPriceEpisodeContext : DbContext, ICurrentPriceEpisodeForJobStore
    {
        public DbSet<CurrentPriceEpisode> Prices { get; set; }

        public CurrentPriceEpisodeContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrentPriceEpisode>().HasKey(o => new { o.JobId, o.Ukprn });
        }

        public void Add(CurrentPriceEpisode priceEpisode)
        {
            Prices.Add(priceEpisode);
            SaveChanges();
        }

        public IEnumerable<CurrentPriceEpisode> GetCurentPriceEpisodes(long jobId, long ukprn)
        {
            return Prices;
        }

        public void Remove(long jobId, long ukprn)
        {
            Prices.RemoveRange(Prices.Where(x => x.JobId == jobId && x.Ukprn == ukprn).AsNoTracking());
            SaveChanges();
        }
    }

    public class ReceivedDataLockEventContext : DbContext, IReceivedDataLockEventStore
    {
        public DbSet<ReceivedDataLockEvent> DataLockEvents { get; set; }

        public ReceivedDataLockEventContext(DbContextOptions options) : base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ReceivedDataLockEvent>().HasKey(o => o.Id);
        //}

        public new void Add(ReceivedDataLockEvent dataLock)
        {
            DataLockEvents.Add(dataLock);
            SaveChanges();
        }

        public IEnumerable<ReceivedDataLockEvent> GetDataLocks(long jobId, long ukprn)
        {
            return DataLockEvents;
        }
    }
}
