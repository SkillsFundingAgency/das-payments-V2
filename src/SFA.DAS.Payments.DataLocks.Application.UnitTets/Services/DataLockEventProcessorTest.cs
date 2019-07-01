using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            processor = new DataLockEventProcessor();
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
                            new EarningPeriod{Period = 1}
                        })
                    }
                }
            };

            var dbFailures = new List<DataLockFailureModel>();

            repositoryMock.Setup(r => r.GetFailures(1, "2", 4, 7, 8, 5, "6", 1819, ContractType.Act1, 1)).ReturnsAsync(dbFailures).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[0])).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[1])).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[2])).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[3])).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[4])).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[5])).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[6])).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[7])).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[8])).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[9])).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[10])).Verifiable();
            dataLockStatusServiceMock.Setup(s => s.GetStatusChange(null, dataLockEvent.OnProgrammeEarnings[0].Periods[11])).Verifiable();

            // act
            var statusChangedEvent = await processor.ProcessDataLockEvent(dataLockEvent).ConfigureAwait(false);

            // assert
            statusChangedEvent.Should().NotBeNull();
            statusChangedEvent.Should().BeOfType<DataLockStatusChangedToFailed>();
            statusChangedEvent.Periods.Should().HaveCount(1);
            statusChangedEvent.Periods[0].Should().Be(1);

        }
    }
}
