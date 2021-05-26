using AutoFixture;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.PeriodEnd.Model;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client.UnitTests
{
    internal class PeriodEndJobClientTests
    {
        private Mock<IMessageSession> messageSessionMock;
        private Mock<IMonitoringMessageSessionFactory> messageSessionFactoryMock;
        private Mock<IPaymentLogger> loggerMock;
        private Mock<IConfigurationHelper> configMock;

        private Fixture fixture;
        private RecordPeriodEndJob periodEndJob;
        private PeriodEndJobClient sut;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();

            messageSessionMock = new Mock<IMessageSession>();
            messageSessionFactoryMock = new Mock<IMonitoringMessageSessionFactory>();
            loggerMock = new Mock<IPaymentLogger>();
            configMock = new Mock<IConfigurationHelper>();

            messageSessionFactoryMock
                .Setup(x => x.Create())
                .Returns(messageSessionMock.Object);

            periodEndJob = fixture.Create<RecordPeriodEndStartJob>();
            sut = new PeriodEndJobClient(messageSessionFactoryMock.Object, loggerMock.Object, configMock.Object);
        }

        [TestCase(PeriodEndTaskType.PeriodEndReports)]
        [TestCase(PeriodEndTaskType.PeriodEndRun)]
        [TestCase(PeriodEndTaskType.PeriodEndStart)]
        [TestCase(PeriodEndTaskType.PeriodEndStop)]
        [TestCase(PeriodEndTaskType.PeriodEndSubmissionWindowValidation)]
        public async Task
            WhenStartingPeriodEndJob_ThenCorrectlyLogsJobType_AndJobId_AndCollectionYear_AndCollectionPeriod(PeriodEndTaskType taskType)
        {
            //Arrange
            SetRecordPeriodEndJobType(taskType);
            var expectedName = periodEndJob.GetType().Name;

            //Act
            await sut.StartPeriodEndJob(periodEndJob);

            //Assert
            loggerMock.Verify(x =>
                x.LogDebug(
                    It.IsRegex($".*{expectedName}.*{periodEndJob.JobId}.*{periodEndJob.CollectionYear}.*{periodEndJob.CollectionPeriod}"),
                    It.IsAny<object[]>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
        }

        [TestCase(PeriodEndTaskType.PeriodEndReports)]
        [TestCase(PeriodEndTaskType.PeriodEndRun)]
        [TestCase(PeriodEndTaskType.PeriodEndStart)]
        [TestCase(PeriodEndTaskType.PeriodEndStop)]
        [TestCase(PeriodEndTaskType.PeriodEndSubmissionWindowValidation)]
        public async Task WhenStartingPeriodEndJob_ThenSendsTheCorrectMessage(PeriodEndTaskType taskType)
        {
            //Arrange
            SetRecordPeriodEndJobType(taskType);

            //Act
            await sut.StartPeriodEndJob(periodEndJob);

            //Assert
            messageSessionMock.Verify(x => x.Send(It.Is<RecordPeriodEndJob>(y => y.GetType().Name != nameof(RecordPeriodEndJob)), It.IsAny<SendOptions>()), Times.Once());
        }

        private void SetRecordPeriodEndJobType(PeriodEndTaskType periodEndTask)
        {
            switch (periodEndTask)
            {
                case PeriodEndTaskType.PeriodEndStart:
                    periodEndJob = fixture.Create<RecordPeriodEndStartJob>();
                    break;

                case PeriodEndTaskType.PeriodEndRun:
                    periodEndJob = fixture.Create<RecordPeriodEndRunJob>();
                    break;

                case PeriodEndTaskType.PeriodEndStop:
                    periodEndJob = fixture.Create<RecordPeriodEndStopJob>();
                    break;

                case PeriodEndTaskType.PeriodEndSubmissionWindowValidation:
                    periodEndJob = fixture.Create<RecordPeriodEndSubmissionWindowValidationJob>();
                    break;

                case PeriodEndTaskType.PeriodEndReports:
                    periodEndJob = fixture.Create<RecordPeriodEndRequestReportsJob>();
                    break;

                default:
                    Assert.Fail();
                    break;
            }
        }
    }
}