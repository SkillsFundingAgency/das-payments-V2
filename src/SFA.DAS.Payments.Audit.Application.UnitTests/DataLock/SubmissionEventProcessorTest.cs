using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.DataLock
{
    [TestFixture]
    public class SubmissionEventProcessorTest
    {
        private ISubmissionEventProcessor processor;
        private Mock<IDataLockEventRepository> repositoryMock;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IDataLockEventRepository>(MockBehavior.Strict);
            processor = new SubmissionEventProcessor(repositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            repositoryMock.Verify();
        }

        [Test]
        public async Task TestEventsDeletedOnFailure()
        {
            var submissionFailedEvent = new SubmissionFailedEvent
            {
                AcademicYear = 1,
                CollectionPeriod = 2,
                JobId = 3,
                Ukprn = 4,
                IlrSubmissionDateTime = DateTime.Now
            };

            repositoryMock.Setup(r => r.DeleteEventsOfSubmission(4, 1, 2, submissionFailedEvent.IlrSubmissionDateTime))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await processor.ProcessSubmissionFailedEvent(submissionFailedEvent).ConfigureAwait(false);
        }

        [Test]
        public async Task TestEventsDeletedOnSuccess()
        {
            var submissionSucceededEvent = new SubmissionSucceededEvent
            {
                AcademicYear = 1,
                CollectionPeriod = 2,
                JobId = 3,
                Ukprn = 4,
                IlrSubmissionDateTime = DateTime.Now
            };

            repositoryMock.Setup(r => r.DeleteEventsPriorToSubmission(4, 1, 2, submissionSucceededEvent.IlrSubmissionDateTime))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await processor.ProcessSubmissionSucceededEvent(submissionSucceededEvent).ConfigureAwait(false);
        }
    }
}
