﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpisodeChanges;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    public class PriceEpisodesReceivedServiceTest
    {
        [Theory, AutoDomainData]
        public async Task When_job_succeeded_builds_approval_event_for_removed_price_episode(
            ICurrentPriceEpisodeForJobStore context,
            IPaymentsDataContext paymentsDataContext,
            PriceEpisodesReceivedService sut,
            CurrentPriceEpisode priceEpisode)
        {
            await context.Add(priceEpisode);

            var changeMessages = await sut.JobSucceeded(priceEpisode.JobId, priceEpisode.Ukprn);

            changeMessages.Should().BeEquivalentTo(
                new
                {
                    DataLock = new
                    {
                        priceEpisode.PriceEpisodeIdentifier,
                        Status = PriceEpisodeStatus.Removed,
                    },
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

            changeMessages.Should().ContainEquivalentOf(
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

            changeMessages.Should().ContainEquivalentOf(
                new
                {
                    DataLock = new
                    {
                        PriceEpisodeIdentifier = earning.PriceEpisodes[0].Identifier,
                        Status = PriceEpisodeStatus.Updated,
                    },
                });
            changeMessages.Should().ContainEquivalentOf(
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
            removed.JobId = earning.JobId;
            removed.Ukprn = earning.Ukprn;
            await currentContext.Add(removed);

            await receivedContext.Add(new ReceivedDataLockEvent
            {
                JobId = earning.JobId,
                Ukprn = earning.Ukprn,
                MessageType = earning.GetType().ToString(),
                Message = JsonConvert.SerializeObject(earning),
            });

            var changeMessages = await sut.JobSucceeded(earning.JobId, earning.Ukprn);

            changeMessages.Should().ContainEquivalentOf(
                new
                {
                    DataLock = new
                    {
                        PriceEpisodeIdentifier = earning.PriceEpisodes[0].Identifier,
                        Status = PriceEpisodeStatus.New,
                    },
                });
            changeMessages.Should().ContainEquivalentOf(
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
            [Frozen]Mock<IApprenticeshipRepository> apprenticeshipRepo,
            ICurrentPriceEpisodeForJobStore currentContext,
            IReceivedDataLockEventStore receivedContext,
            OnProgrammeEarning onProgrammeEarning,
            PriceEpisode priceEpisode,
            PriceEpisodesReceivedService sut,
            EarningPeriod period,
            ApprenticeshipModel apprenticeship,
            EarningFailedDataLockMatching earning)
        {

            period.DataLockFailures = new List<DataLockFailure>
            {
                new DataLockFailure
                {
                    ApprenticeshipId = apprenticeship.Id,
                    DataLockError =DataLockErrorCode.DLOCK_03,
                    ApprenticeshipPriceEpisodeIds = apprenticeship .ApprenticeshipPriceEpisodes.Select(x => x.Id).ToList()
                }
            };

            period.AccountId = default(long?);
            period.PriceEpisodeIdentifier = priceEpisode.Identifier;
            var periods = new List<EarningPeriod> { period };

            earning.Learner.Uln = apprenticeship.Uln;
            earning.OnProgrammeEarnings = new List<OnProgrammeEarning> { onProgrammeEarning };
            earning.PriceEpisodes = new List<PriceEpisode> { priceEpisode };

            onProgrammeEarning.Type = OnProgrammeEarningType.Learning;
            onProgrammeEarning.Periods = periods.AsReadOnly();

            apprenticeshipRepo
                .Setup(x => x.Get(It.IsAny<List<long>>(), CancellationToken.None))
                .Returns(Task.FromResult(new List<ApprenticeshipModel> { apprenticeship }));

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
                MessageType = earning.GetType().AssemblyQualifiedName,
                Message = JsonConvert.SerializeObject(earning),
            });

            await sut.JobSucceeded(earning.JobId, earning.Ukprn);

            var expected = earning.PriceEpisodes.Select(x => new CurrentPriceEpisode
            {
                JobId = earning.JobId,
                Ukprn = earning.Ukprn,
                Uln = earning.Learner.Uln,
                PriceEpisodeIdentifier = x.Identifier,
                AgreedPrice = x.AgreedPrice,
                MessageType = typeof(List<PriceEpisodeStatusChange>).AssemblyQualifiedName
            });

            var results = (await currentContext
                .GetCurrentPriceEpisodes(earning.JobId, earning.Ukprn, earning.Learner.Uln)).ToList();

            results.Should().BeEquivalentTo(expected, c =>
                {
                    c.Excluding(info => info.Id);
                    c.Excluding(info => info.Message);
                    return c;
                });

            results[0].Message.Should().NotBeNull();
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

                fixture.Register<IPaymentsDataContext>(() =>
                    new PaymentsDataContext(
                        new DbContextOptionsBuilder<PaymentsDataContext>()
                            .UseInMemoryDatabase(databaseName: dbName)
                            .Options));


                fixture.Register<IReceivedDataLockEventStore>(() => fixture.Create<ReceivedDataLockEventStore>());
                fixture.Register<ICurrentPriceEpisodeForJobStore>(() => fixture.Create<CurrentPriceEpisodeForJobStore>());
                fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
                return fixture;
            }
        }
    }
}
