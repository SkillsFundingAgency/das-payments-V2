using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    [TestFixture]
    public class DataLockEventProcessorTest
    {
        private Mock<IDataLockFailureRepository> repositoryMock;
        private Mock<IDataLockStatusService> dataLockStatusServiceMock;
        private IDataLockEventProcessor processor;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IDataLockFailureRepository>(MockBehavior.Strict);
            dataLockStatusServiceMock = new Mock<IDataLockStatusService>(MockBehavior.Strict);
            processor = new DataLockEventProcessor(repositoryMock.Object, dataLockStatusServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            repositoryMock.Verify();
            dataLockStatusServiceMock.Verify();
        }

        [Test]
        public async Task TestCreatesDataLockChangedEventWhenNewFailure()
        {
            // arrange
            var dataLockEvent = new EarningFailedDataLockMatching()
            {
                Ukprn = 1,
                Learner = new Learner {ReferenceNumber = "2", Uln = 3},
                LearningAim = new LearningAim {FrameworkCode = 4, StandardCode = 5, Reference = "6", PathwayCode = 7, ProgrammeType = 8, FundingLineType = "9"},
                CollectionYear = 1819,
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            new EarningPeriod {Period = 1, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_03}}},
                            new EarningPeriod {Period = 2}
                        })
                    }
                },
                IncentiveEarnings = new List<IncentiveEarning>()
            };

            var dbFailures = new List<DataLockFailureModel>();

            repositoryMock.Setup(r => r.GetFailures(1, "2", 4, 7, 8, 5, "6", 1819)).ReturnsAsync(dbFailures).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[0].DataLockFailures)).Returns(DataLockStatusChange.ChangedToFailed).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[1].DataLockFailures)).Returns(DataLockStatusChange.NoChange).Verifiable();

            // act
            var statusChangedEvents = await processor.ProcessDataLockEvent(dataLockEvent).ConfigureAwait(false);

            // assert
            statusChangedEvents.Should().NotBeNull();
            statusChangedEvents.Should().HaveCount(1);
            statusChangedEvents[0].Should().BeOfType<DataLockStatusChangedToFailed>();
            statusChangedEvents[0].TransactionTypesAndPeriods.Should().HaveCount(1);
            statusChangedEvents[0].TransactionTypesAndPeriods.First().Key.Should().Be(1);
            statusChangedEvents[0].TransactionTypesAndPeriods.First().Value.Should().HaveCount(1);
            statusChangedEvents[0].TransactionTypesAndPeriods.First().Value[0].Should().Be(1);

        }

        [Test] public async Task TestCreatesMultipleEventsForPeriodsAndTypes()
        {
            // arrange
            var dataLockEvent = new EarningFailedDataLockMatching()
            {
                Ukprn = 1,
                Learner = new Learner {ReferenceNumber = "2", Uln = 3},
                LearningAim = new LearningAim {FrameworkCode = 4, StandardCode = 5, Reference = "6", PathwayCode = 7, ProgrammeType = 8, FundingLineType = "9"},
                CollectionYear = 1819,
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Completion,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            // changed to fail
                            new EarningPeriod {Period = 1, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_03}}},
                            // no change
                            new EarningPeriod {Period = 2},
                            // change of dlock code
                            new EarningPeriod {Period = 5, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_04}}},
                            // no change
                            new EarningPeriod {Period = 6, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_04}}},
                        })
                    }
                },
                IncentiveEarnings = new List<IncentiveEarning>
                {
                    new IncentiveEarning
                    {
                        Type = IncentiveEarningType.CareLeaverApprenticePayment,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            // changed to fail
                            new EarningPeriod {Period = 1, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_03}}},
                            // no change
                            new EarningPeriod {Period = 2},
                            // change of dlock code
                            new EarningPeriod {Period = 5, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_04}}},
                            // no change
                            new EarningPeriod {Period = 6, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_04}}},
                        })
                    }
                }
            };

            var dbFailures = new List<DataLockFailureModel>
            {
                new DataLockFailureModel // change of dlock code
                {
                    DeliveryPeriod = 5,
                    TransactionType = TransactionType.Completion,
                    Errors = new List<DataLockFailure>{ new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_06} }
                },
                new DataLockFailureModel // changed to pass
                {
                    DeliveryPeriod = 3,
                    TransactionType = TransactionType.Balancing,
                    Errors = new List<DataLockFailure>{ new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_06} }
                },
                new DataLockFailureModel // change of dlock code
                {
                    DeliveryPeriod = 5,
                    TransactionType = TransactionType.CareLeaverApprenticePayment,
                    Errors = new List<DataLockFailure>{ new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_06} }
                },
                new DataLockFailureModel // change to pass
                {
                    DeliveryPeriod = 3,
                    TransactionType = TransactionType.Balancing16To18FrameworkUplift,
                    Errors = new List<DataLockFailure>{ new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_06} }
                }
            };

            repositoryMock.Setup(r => r.GetFailures(1, "2", 4, 7, 8, 5, "6", 1819)).ReturnsAsync(dbFailures).Verifiable();
            // TT1
            // P1
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[0].DataLockFailures)).Returns(DataLockStatusChange.ChangedToFailed).Verifiable();
            // P2
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[1].DataLockFailures)).Returns(DataLockStatusChange.NoChange).Verifiable();
            // P5
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(dbFailures[0].Errors, dataLockEvent.OnProgrammeEarnings[0].Periods[2].DataLockFailures)).Returns(DataLockStatusChange.FailureCodeChanged).Verifiable();
            // P6
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[3].DataLockFailures)).Returns(DataLockStatusChange.NoChange).Verifiable();


            // TT3 P3
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(dbFailures[1].Errors, null)).Returns(DataLockStatusChange.ChangedToPassed).Verifiable();

            // TT10 P3
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(dbFailures[3].Errors, null)).Returns(DataLockStatusChange.ChangedToPassed).Verifiable();

            // TT16
            // P1
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.IncentiveEarnings[0].Periods[0].DataLockFailures)).Returns(DataLockStatusChange.ChangedToFailed).Verifiable();
            // P2
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.IncentiveEarnings[0].Periods[1].DataLockFailures)).Returns(DataLockStatusChange.NoChange).Verifiable();
            // P5
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(dbFailures[2].Errors, dataLockEvent.IncentiveEarnings[0].Periods[2].DataLockFailures)).Returns(DataLockStatusChange.FailureCodeChanged).Verifiable();
            // P6
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.IncentiveEarnings[0].Periods[3].DataLockFailures)).Returns(DataLockStatusChange.NoChange).Verifiable();

            var statusChangedEvents = await processor.ProcessDataLockEvent(dataLockEvent).ConfigureAwait(false);

            // assert
            statusChangedEvents.Should().NotBeNull();
            statusChangedEvents.Should().HaveCount(3);

            var changedToFail = statusChangedEvents.SingleOrDefault(e => e is DataLockStatusChangedToFailed);
            changedToFail.Should().NotBeNull();

            changedToFail.TransactionTypesAndPeriods.Should().HaveCount(2);
            changedToFail.TransactionTypesAndPeriods.Should().ContainKey(2);
            changedToFail.TransactionTypesAndPeriods[2].Should().HaveCount(1);
            changedToFail.TransactionTypesAndPeriods[2][0].Should().Be(1);
            
            changedToFail.TransactionTypesAndPeriods.Should().ContainKey(16);
            changedToFail.TransactionTypesAndPeriods[16].Should().HaveCount(1);
            changedToFail.TransactionTypesAndPeriods[16][0].Should().Be(1);


            var changedToPass = statusChangedEvents.SingleOrDefault(e => e is DataLockStatusChangedToPassed);
            changedToPass.Should().NotBeNull();

            changedToPass.TransactionTypesAndPeriods.Should().HaveCount(2);
            changedToPass.TransactionTypesAndPeriods.Should().ContainKey(3);
            changedToPass.TransactionTypesAndPeriods[3].Should().HaveCount(1);
            changedToPass.TransactionTypesAndPeriods[3][0].Should().Be(3);
            
            changedToPass.TransactionTypesAndPeriods.Should().ContainKey(10);
            changedToPass.TransactionTypesAndPeriods[10].Should().HaveCount(1);
            changedToPass.TransactionTypesAndPeriods[10][0].Should().Be(3);

            var changedCode = statusChangedEvents.SingleOrDefault(e => e is DataLockFailureChanged);
            changedCode.Should().NotBeNull();

            changedCode.TransactionTypesAndPeriods.Should().HaveCount(2);
            changedCode.TransactionTypesAndPeriods.Should().ContainKey(2);
            changedCode.TransactionTypesAndPeriods[2].Should().HaveCount(1);
            changedCode.TransactionTypesAndPeriods[2][0].Should().Be(5);
            
            changedCode.TransactionTypesAndPeriods.Should().ContainKey(16);
            changedCode.TransactionTypesAndPeriods[16].Should().HaveCount(1);
            changedCode.TransactionTypesAndPeriods[16][0].Should().Be(5);
        }
    }
}
