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
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    public class PriceEpisodesReceivedServiceTest
    {
        [Theory, AutoDomainData]
        public async Task When_job_succeeded_builds_approval_event_for_removed_price_episode(
            [Frozen]Mock<IApprenticeshipRepository> apprenticeshipRepo,
            ICurrentPriceEpisodeForJobStore currentContext,
            IReceivedDataLockEventStore receivedContext,
            TestCaseData testCaseData,
            PriceEpisodesReceivedService sut,
            CurrentPriceEpisode priceEpisode)
        {
            testCaseData.CommonSetup();
            await testCaseData.AddDataLockEventToContext(receivedContext);

            priceEpisode.Uln = testCaseData.earning.Learner.Uln;
            priceEpisode.Ukprn = testCaseData.earning.Ukprn;
            priceEpisode.JobId = testCaseData.earning.JobId + 1;

            await currentContext.Add(priceEpisode);

            var changeMessages = await sut.JobSucceeded(testCaseData.earning.JobId, testCaseData.earning.Ukprn);

            changeMessages.Should().ContainEquivalentOf(
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
            IReceivedDataLockEventStore receivedContext,
            TestCaseData testCaseData,
            PriceEpisodesReceivedService sut)
        {
            testCaseData.CommonSetup();

            await testCaseData.AddDataLockEventToContext(receivedContext);
            var changeMessages = await sut.JobSucceeded(testCaseData.earning.JobId, testCaseData.earning.Ukprn);

            changeMessages.Should().ContainEquivalentOf(
                new
                {
                    DataLock = new
                    {
                        PriceEpisodeIdentifier = testCaseData.earning.PriceEpisodes.First().Identifier,
                        Status = PriceEpisodeStatus.New,
                    },
                });
        }

        [Theory, AutoDomainData]
        public async Task When_job_succeeded_builds_approval_event_for_new_and_updated_price_episodes(
            ICurrentPriceEpisodeForJobStore currentContext,
            IReceivedDataLockEventStore receivedContext,
            TestCaseData testCaseData,
            PriceEpisode newPriceEpisode,
            EarningPeriod newPeriod,
            PriceEpisodesReceivedService sut)
        {

            var allPeriods = testCaseData.earning.OnProgrammeEarnings[0].Periods.ToList();

            newPeriod.ApprenticeshipId = testCaseData.apprenticeship.Id;
            newPeriod.AccountId = testCaseData.apprenticeship.AccountId;
            newPeriod.PriceEpisodeIdentifier = newPriceEpisode.Identifier;
            allPeriods.Add(newPeriod);
            testCaseData.earning.OnProgrammeEarnings[0].Periods = allPeriods.AsReadOnly();
            testCaseData.earning.PriceEpisodes.Add(newPriceEpisode);
            
            await currentContext.Add(new CurrentPriceEpisode
            {
                JobId = testCaseData.earning.JobId,
                Ukprn = testCaseData.earning.Ukprn,
                Uln = testCaseData.earning.Learner.Uln,
                PriceEpisodeIdentifier = testCaseData.earning.PriceEpisodes.First().Identifier,
                AgreedPrice = testCaseData.earning.PriceEpisodes.First().AgreedPrice + 1,
            });

            await testCaseData.AddDataLockEventToContext(receivedContext);
            var changeMessages = await sut.JobSucceeded(testCaseData.earning.JobId, testCaseData.earning.Ukprn);

            changeMessages.Should().ContainEquivalentOf(
                new
                {
                    DataLock = new
                    {
                        PriceEpisodeIdentifier = testCaseData.earning.PriceEpisodes[0].Identifier,
                        Status = PriceEpisodeStatus.Updated,
                    },
                });
            changeMessages.Should().ContainEquivalentOf(
                new
                {
                    DataLock = new
                    {
                        PriceEpisodeIdentifier = newPriceEpisode.Identifier,
                        Status = PriceEpisodeStatus.New,
                    },
                });
        }

        [Theory, AutoDomainData]

        public async Task When_job_succeeded_builds_approval_event_for_new_and_removed_price_episodes(
            ICurrentPriceEpisodeForJobStore currentContext,
            IReceivedDataLockEventStore receivedContext,
            PriceEpisodesReceivedService sut,
            TestCaseData testCaseData,
            CurrentPriceEpisode removed)
        {
            testCaseData.CommonSetup();

            await testCaseData.AddDataLockEventToContext(receivedContext);

            removed.Uln = testCaseData.earning.Learner.Uln;
            removed.Ukprn = testCaseData.earning.Ukprn;
            removed.JobId = testCaseData.earning.JobId + 1;

            await currentContext.Add(removed);

            var changeMessages = await sut.JobSucceeded(testCaseData.earning.JobId, testCaseData.earning.Ukprn);

            changeMessages.Should().ContainEquivalentOf(
                new
                {
                    DataLock = new
                    {
                        PriceEpisodeIdentifier = testCaseData.earning.PriceEpisodes[0].Identifier,
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
        public async Task When_job_succeeded_removes_received_dataLock_events(
            IReceivedDataLockEventStore receivedContext,
            PriceEpisodesReceivedService sut,
            TestCaseData testCaseData)
        {
            testCaseData.CommonSetup();

            await testCaseData.AddDataLockEventToContext(receivedContext);

            await sut.JobSucceeded(testCaseData.earning.JobId, testCaseData. earning.Ukprn);

            (await receivedContext.GetDataLocks(testCaseData.earning.JobId, testCaseData.earning.Ukprn))
                .Should().BeEmpty();
        }

        [Theory, AutoDomainData]
        public async Task When_job_succeeded_replaces_current_price_episodes(
            [Frozen]Mock<IApprenticeshipRepository> apprenticeshipRepo,
            ICurrentPriceEpisodeForJobStore currentContext,
            IReceivedDataLockEventStore receivedContext,
            TestCaseData testCaseData,
            PriceEpisodesReceivedService sut)
        {

            testCaseData.CommonSetup();

            apprenticeshipRepo
                .Setup(x => x.Get(It.IsAny<List<long>>(), CancellationToken.None))
                .Returns(Task.FromResult(new List<ApprenticeshipModel> { testCaseData.apprenticeship }));

            await currentContext.Add(new CurrentPriceEpisode
            {
                JobId = testCaseData.earning.JobId,
                Ukprn = testCaseData.earning.Ukprn,
                Uln = testCaseData.earning.Learner.Uln,
                PriceEpisodeIdentifier = testCaseData.earning.PriceEpisodes.First().Identifier,
                AgreedPrice = testCaseData.earning.PriceEpisodes.First().AgreedPrice + 1,
            });
            await testCaseData.AddDataLockEventToContext(receivedContext);

            await sut.JobSucceeded(testCaseData.earning.JobId, testCaseData.earning.Ukprn);

            var expected = testCaseData.earning.PriceEpisodes.Select(x => new CurrentPriceEpisode
            {
                JobId = testCaseData.earning.JobId,
                Ukprn = testCaseData.earning.Ukprn,
                Uln = testCaseData.earning.Learner.Uln,
                PriceEpisodeIdentifier = x.Identifier,
                AgreedPrice = x.AgreedPrice,
                MessageType = typeof(List<PriceEpisodeStatusChange>).AssemblyQualifiedName
            });

            var results = (await currentContext
                .GetCurrentPriceEpisodes( testCaseData.earning.Ukprn)).ToList();

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


        public class TestCaseData
        {
            public OnProgrammeEarning onProgrammeEarning { get; set; }
            public PriceEpisode priceEpisode { get; set; }
            public EarningPeriod period { get; set; }
            public ApprenticeshipModel apprenticeship { get; set; }
            public EarningFailedDataLockMatching earning { get; set; }

            public void CommonSetup()
            {
                period.DataLockFailures = new List<DataLockFailure>
                {
                    new DataLockFailure
                    {
                        ApprenticeshipId = apprenticeship.Id,
                        DataLockError = DataLockErrorCode.DLOCK_03,
                        ApprenticeshipPriceEpisodeIds = apprenticeship.ApprenticeshipPriceEpisodes.Select(x => x.Id).ToList()
                    }
                };
                period.PriceEpisodeIdentifier = priceEpisode.Identifier;
                var periods = new List<EarningPeriod> { period };

                earning.Learner.Uln = apprenticeship.Uln;
                earning.OnProgrammeEarnings = new List<OnProgrammeEarning> { onProgrammeEarning };
                earning.PriceEpisodes = new List<PriceEpisode> { priceEpisode };
                onProgrammeEarning.Periods = periods.AsReadOnly();
            }

            public async Task  AddDataLockEventToContext(IReceivedDataLockEventStore receivedContext)
            {
                await receivedContext.Add(new ReceivedDataLockEvent
                {
                    JobId = earning.JobId,
                    Ukprn = earning.Ukprn,
                    MessageType = earning.GetType().AssemblyQualifiedName,
                    Message = JsonConvert.SerializeObject(earning),
                });

            }
        }


    }
}
