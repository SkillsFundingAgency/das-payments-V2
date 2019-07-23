using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Application.Mapping;
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
        private IMapper mapper;

        [SetUp]
        public void SetUp()
        {
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<DataLocksProfile>()));
            repositoryMock = new Mock<IDataLockFailureRepository>(MockBehavior.Strict);

            dataLockStatusServiceMock = new Mock<IDataLockStatusService>(MockBehavior.Strict);
            //dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, null)).Returns(DataLockStatusChange.NoChange);
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, It.Is<List<DataLockFailure>>(f => f == null || f.Count == 0))).Returns(DataLockStatusChange.NoChange);

            processor = new DataLockEventProcessor(repositoryMock.Object, dataLockStatusServiceMock.Object, mapper, new Mock<IPaymentLogger>().Object);
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
                CollectionPeriod = new CollectionPeriod {AcademicYear = 7, Period = 8},
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            new EarningPeriod {Period = 1, Amount = 1, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_03}}},
                            new EarningPeriod {Period = 2, Amount = 1 }
                        })
                    }
                },
                IncentiveEarnings = new List<IncentiveEarning>()
            };

            var dbFailures = new List<DataLockFailureEntity>();

            repositoryMock.Setup(r => r.GetFailures(1, "2", 4, 7, 8, 5, "6", 1819)).ReturnsAsync(dbFailures).Verifiable();
            repositoryMock.Setup(r => r.ReplaceFailures(It.Is<List<long>>(old => old.Count == 0), It.Is<List<DataLockFailureEntity>>(newF => newF.Count == 1), It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.CompletedTask).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[0].DataLockFailures)).Returns(DataLockStatusChange.ChangedToFailed).Verifiable();

            // act
            var statusChangedEvents = await processor.ProcessDataLockFailure(dataLockEvent).ConfigureAwait(false);

            // assert
            statusChangedEvents.Should().NotBeNull();
            statusChangedEvents.Should().HaveCount(1);
            statusChangedEvents[0].Should().BeOfType<DataLockStatusChangedToFailed>();
            statusChangedEvents[0].TransactionTypesAndPeriods.Should().HaveCount(1);
            statusChangedEvents[0].TransactionTypesAndPeriods.First().Key.Should().Be(1);
            statusChangedEvents[0].TransactionTypesAndPeriods.First().Value.Should().HaveCount(1);
            statusChangedEvents[0].TransactionTypesAndPeriods.First().Value[0].Period.Should().Be(1);

        }

        [Test]
        public async Task TestCreatesMultipleEventsForPeriodsAndTypes()
        {
            // arrange



            // change of dlock code
            var oldTT2P5 = new DataLockFailureEntity {Id = 1, DeliveryPeriod = 5, TransactionType = TransactionType.Completion, EarningPeriod = new EarningPeriod { Period = 5, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_06}}}};
            var newTT2p5 = new EarningPeriod {Period = 5, Amount = 1, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_04}}};

            // no new - changed to pass
            var oldTT3p3 = new DataLockFailureEntity {Id = 2, DeliveryPeriod = 3, TransactionType = TransactionType.Balancing, EarningPeriod = new EarningPeriod { Period = 3, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_06}}}};
            var newTT3p3 = new EarningPeriod {Period = 3, Amount = 1, ApprenticeshipId = 4, ApprenticeshipPriceEpisodeId = 4};

            // no change
            var oldTT2p6 = new DataLockFailureEntity {Id = 3, DeliveryPeriod = 6, TransactionType = TransactionType.Completion, EarningPeriod = new EarningPeriod { Period = 6, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_04}}}};
            var newTT2p6 = new EarningPeriod {Period = 6, Amount = 1, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_04}}};

            // change of dlock code
            var oldTT16p5 = new DataLockFailureEntity {Id = 4, DeliveryPeriod = 5, TransactionType = TransactionType.CareLeaverApprenticePayment, EarningPeriod = new EarningPeriod { Period = 5, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_06}}}};
            var newTT16p5 = new EarningPeriod {Period = 5, Amount = 1, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_04}}};

            // no new - change to pass
            var oldTT10p3 = new DataLockFailureEntity {Id = 5, DeliveryPeriod = 3, TransactionType = TransactionType.Balancing16To18FrameworkUplift, EarningPeriod = new EarningPeriod { Period = 3, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_06}}}};
            var newTT10p3 = new EarningPeriod {Period = 3, Amount = 1, ApprenticeshipId = 10, ApprenticeshipPriceEpisodeId = 10, DataLockFailures = new List<DataLockFailure>() };

            // no change
            var oldTT16p6 = new DataLockFailureEntity {Id = 6, DeliveryPeriod = 6, TransactionType = TransactionType.CareLeaverApprenticePayment, EarningPeriod = new EarningPeriod { Period = 6, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_04}}}};
            var newTT16p6 = new EarningPeriod {Period = 6, Amount = 1, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_04}}};

            // no old - change to fail
            var newTT2p1 = new EarningPeriod {Period = 1, Amount = 1, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_03}}};
            var newTT2p2 = new EarningPeriod {Period = 2, Amount = 1};
            var newTT16p1 = new EarningPeriod {Period = 1, Amount = 1, DataLockFailures = new List<DataLockFailure> {new DataLockFailure {DataLockError = DataLockErrorCode.DLOCK_03}}};
            var newTT16p2 = new EarningPeriod {Period = 2, Amount = 1};

            var dataLockEvent = new EarningFailedDataLockMatching()
            {
                Ukprn = 1,
                Learner = new Learner {ReferenceNumber = "2", Uln = 3},
                LearningAim = new LearningAim {FrameworkCode = 4, StandardCode = 5, Reference = "6", PathwayCode = 7, ProgrammeType = 8, FundingLineType = "9"},
                CollectionYear = 1819,
                CollectionPeriod = new CollectionPeriod {AcademicYear = 7, Period = 8},
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Completion, // TT2
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            // changed to fail
                            newTT2p1,
                            // no change
                            newTT2p2,
                            // change of dlock code
                            newTT2p5,
                            // no change
                            newTT2p6,
                            newTT10p3
                        })
                    },
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Balancing,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod> { newTT3p3 })
                    }
                },
                IncentiveEarnings = new List<IncentiveEarning>
                {
                    new IncentiveEarning
                    {
                        Type = IncentiveEarningType.CareLeaverApprenticePayment, // TT16
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            // changed to fail
                            newTT16p1,
                            // no change
                            newTT16p2,
                            // change of dlock code
                            newTT16p5,
                            // no change
                            newTT16p6,
                        })
                    },
                    new IncentiveEarning
                    {
                        Type = IncentiveEarningType.Balancing16To18FrameworkUplift,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod> { newTT10p3 })
                    }
                }
            };

            var oldFailures = new List<DataLockFailureEntity>
            {
                oldTT2P5,
                oldTT3p3,
                oldTT2p6,
                oldTT16p5,
                oldTT10p3,
                oldTT16p6,
            };

            repositoryMock.Setup(r => r.GetFailures(1, "2", 4, 7, 8, 5, "6", 1819)).ReturnsAsync(oldFailures).Verifiable();
            repositoryMock.Setup(r => r.ReplaceFailures(It.Is<List<long>>(old => old.Count == 4), It.Is<List<DataLockFailureEntity>>(newF => newF.Count == 4), It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.CompletedTask).Verifiable();
            // TT2
            // P1
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, newTT2p1.DataLockFailures)).Returns(DataLockStatusChange.ChangedToFailed).Verifiable();
            // P2
            //dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, newTT2p2.DataLockFailures)).Returns(DataLockStatusChange.NoChange).Verifiable();
            // P5
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(oldTT2P5.EarningPeriod.DataLockFailures, newTT2p5.DataLockFailures)).Returns(DataLockStatusChange.FailureChanged).Verifiable();
            // P6
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(oldTT2p6.EarningPeriod.DataLockFailures, newTT2p6.DataLockFailures)).Returns(DataLockStatusChange.NoChange).Verifiable();


            // TT3 P3
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(oldTT3p3.EarningPeriod.DataLockFailures, newTT3p3.DataLockFailures)).Returns(DataLockStatusChange.ChangedToPassed).Verifiable();

            // TT10 P3
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(oldTT10p3.EarningPeriod.DataLockFailures, newTT10p3.DataLockFailures)).Returns(DataLockStatusChange.ChangedToPassed).Verifiable();

            // TT16
            // P1
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, newTT16p1.DataLockFailures)).Returns(DataLockStatusChange.ChangedToFailed).Verifiable();
            // P2
            //dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, newTT16p2.DataLockFailures)).Returns(DataLockStatusChange.NoChange).Verifiable();
            // P5
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(oldTT16p5.EarningPeriod.DataLockFailures, newTT16p5.DataLockFailures)).Returns(DataLockStatusChange.FailureChanged).Verifiable();
            // P6
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(oldTT16p6.EarningPeriod.DataLockFailures, newTT16p6.DataLockFailures)).Returns(DataLockStatusChange.NoChange).Verifiable();

            var statusChangedEvents = await processor.ProcessDataLockFailure(dataLockEvent).ConfigureAwait(false);

            // assert
            statusChangedEvents.Should().NotBeNull();
            statusChangedEvents.Should().HaveCount(3);

            var changedToFail = statusChangedEvents.SingleOrDefault(e => e is DataLockStatusChangedToFailed);
            changedToFail.Should().NotBeNull();

            changedToFail.TransactionTypesAndPeriods.Should().HaveCount(2);
            changedToFail.TransactionTypesAndPeriods.Should().ContainKey(TransactionType.Completion);
            changedToFail.TransactionTypesAndPeriods[TransactionType.Completion].Should().HaveCount(1);
            changedToFail.TransactionTypesAndPeriods[TransactionType.Completion][0].Period.Should().Be(1);

            changedToFail.TransactionTypesAndPeriods.Should().ContainKey(TransactionType.CareLeaverApprenticePayment);
            changedToFail.TransactionTypesAndPeriods[TransactionType.CareLeaverApprenticePayment].Should().HaveCount(1);
            changedToFail.TransactionTypesAndPeriods[TransactionType.CareLeaverApprenticePayment][0].Period.Should().Be(1);


            var changedToPass = statusChangedEvents.SingleOrDefault(e => e is DataLockStatusChangedToPassed);
            changedToPass.Should().NotBeNull();

            changedToPass.TransactionTypesAndPeriods.Should().HaveCount(2);
            changedToPass.TransactionTypesAndPeriods.Should().ContainKey(TransactionType.Balancing);
            changedToPass.TransactionTypesAndPeriods[TransactionType.Balancing].Should().HaveCount(1);
            changedToPass.TransactionTypesAndPeriods[TransactionType.Balancing][0].Period.Should().Be(3);

            changedToPass.TransactionTypesAndPeriods.Should().ContainKey(TransactionType.Balancing16To18FrameworkUplift);
            changedToPass.TransactionTypesAndPeriods[TransactionType.Balancing16To18FrameworkUplift].Should().HaveCount(1);
            changedToPass.TransactionTypesAndPeriods[TransactionType.Balancing16To18FrameworkUplift][0].Period.Should().Be(3);

            var changedCode = statusChangedEvents.SingleOrDefault(e => e is DataLockFailureChanged);
            changedCode.Should().NotBeNull();

            changedCode.TransactionTypesAndPeriods.Should().HaveCount(2);
            changedCode.TransactionTypesAndPeriods.Should().ContainKey(TransactionType.Completion);
            changedCode.TransactionTypesAndPeriods[TransactionType.Completion].Should().HaveCount(1);
            changedCode.TransactionTypesAndPeriods[TransactionType.Completion][0].Period.Should().Be(5);

            changedCode.TransactionTypesAndPeriods.Should().ContainKey(TransactionType.CareLeaverApprenticePayment);
            changedCode.TransactionTypesAndPeriods[TransactionType.CareLeaverApprenticePayment].Should().HaveCount(1);
            changedCode.TransactionTypesAndPeriods[TransactionType.CareLeaverApprenticePayment][0].Period.Should().Be(5);
        }
    }
}