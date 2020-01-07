using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    public class ManageReceivedDataLockEventTest
    {

        [Test, AutoDomainData]
        public async Task StoreOnprogPayableDataLockEvents(
            IReceivedDataLockEventStore receivedDataLockEventStore,
            PayableEarningEvent payableEarningEvent,
            List<EarningPeriod> periods,
            ApprenticeshipModel apprenticeship,
            ManageReceivedDataLockEvent manageReceivedDataLockEvent)
        {
            CommonTestSetup(payableEarningEvent, periods, apprenticeship);
            await manageReceivedDataLockEvent.ProcessDataLockEvent(payableEarningEvent);
            var result = (await receivedDataLockEventStore
                .GetDataLocks(payableEarningEvent.JobId, payableEarningEvent.Ukprn)).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].MessageType.Should().Be(typeof(PayableEarningEvent).AssemblyQualifiedName);
            result[0].Message.Should().Be(payableEarningEvent.ToJson());

        }

        [Test, AutoDomainData]
        public async Task StoreValidOnprogNonPayableDataLockEvents(
            IReceivedDataLockEventStore receivedDataLockEventStore,
            EarningFailedDataLockMatching nonPayableEarningEvent,
            List<EarningPeriod> periods,
            ApprenticeshipModel apprenticeship,
            List<DataLockFailure> dataLockFailures,
            ManageReceivedDataLockEvent manageReceivedDataLockEvent)
        {
            CommonTestSetup(nonPayableEarningEvent, periods, apprenticeship, dataLockFailures);
            await manageReceivedDataLockEvent.ProcessDataLockEvent(nonPayableEarningEvent);
            var result = (await receivedDataLockEventStore
                .GetDataLocks(nonPayableEarningEvent.JobId, nonPayableEarningEvent.Ukprn)).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].MessageType.Should().Be(nonPayableEarningEvent.GetType().AssemblyQualifiedName);
            result[0].Message.Should().Be(nonPayableEarningEvent.ToJson());
        }

        [Test, AutoDomainData]
        public async Task OnprogNonPayableDataLockEventWithNoApprenticeshipIdShouldNotBeStored(
            IReceivedDataLockEventStore receivedDataLockEventStore,
            EarningFailedDataLockMatching nonPayableEarningEvent,
            EarningPeriod period,
            ApprenticeshipModel apprenticeship,
            List<DataLockFailure> dataLockFailures,
            ManageReceivedDataLockEvent manageReceivedDataLockEvent)
        {
            period.ApprenticeshipId = default(long?);
            period.AccountId = default(long?);
            period.DataLockFailures = new List<DataLockFailure>
            {
                new DataLockFailure
                {
                    DataLockError = DataLockErrorCode.DLOCK_01
                }
            };

            nonPayableEarningEvent.OnProgrammeEarnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {
                    Periods = (new List<EarningPeriod>
                    {
                        period
                    }).AsReadOnly(),
                    Type = OnProgrammeEarningType.Learning,
                    CensusDate = DateTime.UtcNow
                }
            };

            await manageReceivedDataLockEvent.ProcessDataLockEvent(nonPayableEarningEvent);
            var result = (await receivedDataLockEventStore
                .GetDataLocks(nonPayableEarningEvent.JobId, nonPayableEarningEvent.Ukprn)).ToList();

            result.Should().BeEmpty();
          
        }

        [Test, AutoDomainData]
        public async Task OnprogPayableDataLockEventWithNoApprenticeshipIdShouldNotBeStored(
            IReceivedDataLockEventStore receivedDataLockEventStore,
            PayableEarningEvent payableEarningEvent,
            EarningPeriod period,
            ApprenticeshipModel apprenticeship,
            List<DataLockFailure> dataLockFailures,
            ManageReceivedDataLockEvent manageReceivedDataLockEvent)
        {
            period.ApprenticeshipId = default(long?);
            period.AccountId = default(long?);
            payableEarningEvent.OnProgrammeEarnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {
                    Periods = (new List<EarningPeriod>
                    {
                        period
                    }).AsReadOnly(),
                    Type = OnProgrammeEarningType.Learning,
                    CensusDate = DateTime.UtcNow
                }
            };

            await manageReceivedDataLockEvent.ProcessDataLockEvent(payableEarningEvent);
            var result = (await receivedDataLockEventStore
                .GetDataLocks(payableEarningEvent.JobId, payableEarningEvent.Ukprn)).ToList();

            result.Should().BeEmpty();

        }
        
        [Test, AutoDomainData]
        public async Task DataLockFunctionalSkillEventsShouldNotBeStored(
            IReceivedDataLockEventStore receivedDataLockEventStore,
        ManageReceivedDataLockEvent manageReceivedDataLockEvent,
            long jobId,
            long ukprn)
        {
            await manageReceivedDataLockEvent.ProcessDataLockEvent(new FunctionalSkillEarningFailedDataLockMatching
            {
                JobId = jobId,
                Ukprn = ukprn
            });
            await manageReceivedDataLockEvent.ProcessDataLockEvent(new PayableFunctionalSkillEarningEvent
            {
                JobId = jobId,
                Ukprn = ukprn
            });

            var result = await receivedDataLockEventStore.GetDataLocks(jobId, ukprn);
            result.Should().BeEmpty();

        }

        private static void CommonTestSetup(DataLockEvent dataLock,
            List<EarningPeriod> periods,
            ApprenticeshipModel apprenticeship,
            List<DataLockFailure> dataLockFailures = null
        )
        {
            apprenticeship.ApprenticeshipPriceEpisodes.ForEach(x => x.ApprenticeshipId = apprenticeship.Id);
            periods[0].ApprenticeshipId = apprenticeship.Id;
            periods[0].ApprenticeshipPriceEpisodeId = apprenticeship.ApprenticeshipPriceEpisodes[0].Id;
            periods[0].DataLockFailures = dataLock is PayableEarningEvent ? null : dataLockFailures;
            periods[0].PriceEpisodeIdentifier = dataLock.PriceEpisodes[0].Identifier;
            dataLock.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Learning;
            dataLock.OnProgrammeEarnings[0].Periods = periods.AsReadOnly();
            if (dataLock is EarningFailedDataLockMatching)
            {
                dataLockFailures.ForEach(x =>
                {
                    x.ApprenticeshipId = apprenticeship.Id;
                    x.ApprenticeshipPriceEpisodeIds = apprenticeship.ApprenticeshipPriceEpisodes.Select(o => o.Id).ToList();
                });
            }
        }

        public class AutoDomainDataAttribute : AutoDataAttribute
        {
            public AutoDomainDataAttribute()
                : base(Customise)
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
                fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
                return fixture;
            }

        }

    }
}
